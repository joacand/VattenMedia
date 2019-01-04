using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VattenMedia.Common.Entities;

namespace VattenMedia.Infrastructure.Services
{
    public class YoutubeService : IYoutubeService
    {
        private readonly IRestClient client;
        private readonly IConfigHandler configHandler;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string youtubeApiKey;
        private readonly string channelId;

        private readonly string state;
        private readonly string code_verifier;
        private readonly string code_challenge;

        private static readonly string code_challenge_method = "S256";
        private static readonly string authorizationEndpoint = @"https://accounts.google.com/o/oauth2/v2/auth";
        private static readonly string tokenEndpoint = @"https://www.googleapis.com/oauth2/v4/token";
        private static readonly string youtubeSearchEndpoint = @"https://www.googleapis.com/youtube/v3/search";
        private static readonly string youtubeSubscriptionsEndpoint = @"https://www.googleapis.com/youtube/v3/subscriptions";
        // To be used to get viewer count: private static readonly string youtubeVideosEndpoint = @"https://www.googleapis.com/youtube/v3/videos";
        private static readonly string redirectUrl = "http://localhost";

        public string OAuthUrl { get; }

        public YoutubeService(IConfigHandler configHandler)
        {
            this.configHandler = configHandler;
            var config = configHandler.Config;
            clientId = config.YoutubeClientId;
            clientSecret = config.YoutubeClientSecret;
            youtubeApiKey = config.YoutubeApiKey;
            channelId = config.YoutubeChannelId;

            state = RandomDataBase64url(32);
            code_verifier = RandomDataBase64url(32);
            code_challenge = Base64urlencodeNoPadding(Sha256(code_verifier));

            string youtubeAuthScope = "https://www.googleapis.com/auth/youtube.readonly";
            OAuthUrl = $"{authorizationEndpoint}?response_type=code&scope={youtubeAuthScope}&redirect_uri=" +
                    $"{redirectUrl}&client_id={clientId}&state={state}&code_challenge={code_challenge}" +
                    $"&code_challenge_method={code_challenge_method}";

            var baseUrl = "https://www.googleapis.com/";
            client = new RestClient(baseUrl);
        }

        public async Task<List<LiveChannel>> GetLiveChannels(string oAuthId)
        {
            var channelIds = await GetSubscriptionChannelIds(oAuthId);
            return await GetYoutubeLiveStreams(channelIds);
        }

        private async Task<List<ChannelId>> GetSubscriptionChannelIds(string oAuthId)
        {
            var channelIds = new List<ChannelId>();
            bool pageNotDone = true;
            bool refreshedToken = false;
            string pageToken = null;

            while (pageNotDone)
            {
                string requestUrl = $"{youtubeSubscriptionsEndpoint}?part=id%2Csnippet&maxResults=50&channelId={channelId}&key={youtubeApiKey}";

                if (pageToken != null)
                {
                    requestUrl += $"&pageToken={pageToken}";
                }

                var request = new RestRequest(requestUrl, Method.GET);
                request.AddHeader("Authorization", $"Bearer {oAuthId}");

                var response = await client.ExecuteTaskAsync<YoutubeRootResponse>(request);

                if (!response.IsSuccessful && response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (!refreshedToken)
                    {
                        oAuthId = await RefreshAccessToken();
                        continue;
                    }
                    else
                    {
                        // Something else went wrong - return empty list
                        return channelIds;
                    }
                }


                var data = response?.Data;

                if (data?.items != null)
                {
                    foreach (var item in data.items)
                    {
                        string channelId = item?.snippet?.resourceId?.channelId;
                        if (!string.IsNullOrWhiteSpace(channelId))
                        {
                            channelIds.Add(new ChannelId(channelId));
                        }
                    }
                }
                pageToken = data?.nextPageToken;
                pageNotDone = pageToken != null;
            }
            return channelIds;
        }

        private async Task<List<LiveChannel>> GetYoutubeLiveStreams(List<ChannelId> channelIds)
        {
            ConcurrentBag<LiveChannel> youtubeLiveStreams = new ConcurrentBag<LiveChannel>();

            await Task.Run(() =>
            {
                Parallel.ForEach(channelIds, (channelId) =>
                {
                    List<LiveChannel> channelLiveStreams = GetLiveStreamsFromId(channelId).Result;

                    foreach (var channelLiveStream in channelLiveStreams)
                    {
                        youtubeLiveStreams.Add(channelLiveStream);
                    }
                });
            });

            return youtubeLiveStreams.ToList();
        }

        private async Task<List<LiveChannel>> GetLiveStreamsFromId(ChannelId channelId)
        {
            var liveChannels = new List<LiveChannel>();
            var requestUrl = $"{youtubeSearchEndpoint}?part=snippet&channelId={channelId}&eventType=live&maxResults=25&type=video&key={youtubeApiKey}";
            var request = new RestRequest(requestUrl, Method.GET);

            var response = await client.ExecuteTaskAsync<Common.Entities.YoutubeSearch.YoutubeSearchRoot>(request);
            var data = response?.Data;

            if (data?.items != null)
            {
                foreach (var item in data.items)
                {
                    string videoid = item?.id?.videoId;
                    if (!string.IsNullOrWhiteSpace(videoid))
                    {
                        string url = $"https://www.youtube.com/watch?v={videoid}";
                        int viewers = await GetViewersFromStream(videoid);
                        var liveChannel = new LiveChannel(item?.snippet?.channelTitle, item?.snippet?.title, "N/A", 0, "", item?.snippet?.thumbnails?.medium?.url, url);
                        liveChannels.Add(liveChannel);
                    }
                }
            }

            return liveChannels;
        }

        private Task<int> GetViewersFromStream(string videoid)
        {
            // TODO:
            return Task.FromResult(0);
            /*
            var requestUrl = $"{youtubeVideosEndpoint}?part=liveStreamingDetails&id={channelId}&key={youtubeApiKey}";

            var request = new RestRequest(requestUrl, Method.GET);
            */
        }

        public async Task<string> GetAuthIdFromUrl(string url)
        {
            string code = "";
            if (url.Contains("code="))
            {
                code = url.Split(new string[] { "code=" }, StringSplitOptions.None)[1].Split(new string[] { "&" }, StringSplitOptions.None)[0];
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                // Something went wrong
                return null;
            }

            return await GetAuthIdFromCode(code, redirectUrl);
        }

        private async Task<string> GetAuthIdFromCode(string code, string redirectUrl)
        {
            string tokenRequestUrl = $"{tokenEndpoint}?code={code}&redirect_uri={redirectUrl}" +
                $"&client_id={clientId}&code_verifier={code_verifier}&client_secret={clientSecret}&scope=&grant_type=authorization_code";

            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestUrl);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] byteVersion = Encoding.ASCII.GetBytes(tokenRequestUrl);
            tokenRequest.ContentLength = byteVersion.Length;
            using (System.IO.Stream stream = tokenRequest.GetRequestStream())
            {
                await stream.WriteAsync(byteVersion, 0, byteVersion.Length);
            }

            WebResponse tokenResponse = await tokenRequest.GetResponseAsync();

            using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
            {
                string responseText = await reader.ReadToEndAsync();
                Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                string accessToken = tokenEndpointDecoded["access_token"];
                string refreshToken = tokenEndpointDecoded["refresh_token"];
                configHandler.SetYoutubeRefreshToken(refreshToken);

                return tokenEndpointDecoded["access_token"];
            }
        }

        private async Task<string> RefreshAccessToken()
        {
            if (!configHandler.HasYoutubeRefreshToken)
            {
                return "";
            }

            string refreshToken = configHandler.Config.YoutubeRefreshToken;
            string tokenRequestUrl = $"{tokenEndpoint}?refresh_token={refreshToken}&client_id={clientId}&client_secret={clientSecret}&grant_type=refresh_token";

            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestUrl);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] byteVersion = Encoding.ASCII.GetBytes(tokenRequestUrl);
            tokenRequest.ContentLength = byteVersion.Length;
            using (System.IO.Stream stream = tokenRequest.GetRequestStream())
            {
                await stream.WriteAsync(byteVersion, 0, byteVersion.Length);
            }

            WebResponse tokenResponse = await tokenRequest.GetResponseAsync();

            using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
            {
                string responseText = await reader.ReadToEndAsync();
                Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                string accessToken = tokenEndpointDecoded["access_token"];
                configHandler.SetYoutubeAccessToken(accessToken);
                return accessToken;
            }
        }

        #region Crypto functions
        private static string RandomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return Base64urlencodeNoPadding(bytes);
        }

        private static byte[] Sha256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        private static string Base64urlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);
            base64 = base64.Replace("+", "-").Replace("/", "_").Replace("/", "_").Replace("=", "");
            return base64;
        }
        #endregion
    }
}
