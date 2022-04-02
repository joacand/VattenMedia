using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VattenMedia.Core.Entities;
using VattenMedia.Core.Entities.Twitch;
using VattenMedia.Core.Interfaces;
using Video = VattenMedia.Core.Entities.Video;

namespace VattenMedia.Infrastructure.Services
{
    internal class TwitchService : ITwitchService
    {
        private readonly IRestClient client;
        private readonly string baseUrl;
        private readonly string clientId;
        private readonly string userId;

        public string OAuthUrl { get; }

        public TwitchService(IConfigHandler configHandler, AppConfiguration appConfiguration)
        {
            var config = configHandler.Config;
            clientId = config.TwitchClientId;
            userId = config.TwitchUserId;
            baseUrl = appConfiguration.TwitchApiUrl;

            OAuthUrl = appConfiguration.TwitchAuthApiUrl + $@"oauth2/authorize?client_id={clientId}&redirect_uri=http://localhost&response_type=token+id_token&scope=user:read:follows chat:read chat:edit openid";

            client = new RestClient(baseUrl);
        }

        public async Task<IEnumerable<LiveChannel>> GetLiveChannels(string oAuthId)
        {
            var requestUrl = baseUrl + $"helix/streams/followed?stream_type=live&user_id={userId}";
            var request = new RestRequest(requestUrl, Method.GET);
            request.AddHeader("Accept", "application/vnd.twitchtv.v5+json");
            request.AddHeader("Client-ID", clientId);
            request.AddHeader("Authorization", $"Bearer {oAuthId}");

            var response = await client.ExecuteAsync<TwitchStreamsRootResponse>(request);

            return CreateChannels(response.Data);
        }

        public async Task<IEnumerable<Video>> GetVideos(string oAuthId, string channelId)
        {
            var requestUrl = baseUrl + $"helix/channels/{channelId}/videos";
            var request = new RestRequest(requestUrl, Method.GET);
            request.AddHeader("Accept", "application/vnd.twitchtv.v5+json");
            request.AddHeader("Client-ID", clientId);
            request.AddHeader("Authorization", $"Bearer {oAuthId}");

            var response = await client.ExecuteAsync<TwitchVideosRootResponse>(request);

            return CreateVideos(response.Data);
        }

        public Task<AuthDetails> GetAuthIdFromUrl(string url)
        {
            var result = new AuthDetails();
            if (url.Contains("access_token="))
            {
                result.AccessToken = url.Split(new string[] { "access_token=" }, StringSplitOptions.None)[1].Split(new string[] { "&" }, StringSplitOptions.None)[0];
            }
            if (url.Contains("id_token="))
            {
                result.IdToken = url.Split(new string[] { "id_token=" }, StringSplitOptions.None)[1].Split(new string[] { "&" }, StringSplitOptions.None)[0];
            }
            return Task.FromResult(result);
        }

        public async Task<string> GetChannelId(string oAuthId, string channelName)
        {
            var requestUrl = baseUrl + $"helix/users?login={channelName}";
            var request = new RestRequest(requestUrl, Method.GET);
            request.AddHeader("Accept", "application/vnd.twitchtv.v5+json");
            request.AddHeader("Client-ID", clientId);
            request.AddHeader("Authorization", $"Bearer {oAuthId}");

            var response = await client.ExecuteAsync<TwitchChannelRootResponse>(request);

            return response.Data.users.Count > 0
                ? response.Data.users.First()._id
                : null;
        }

        private IEnumerable<LiveChannel> CreateChannels(TwitchStreamsRootResponse inChannels)
        {
            if (inChannels?.data != null)
            {
                foreach (var inChannel in inChannels.data)
                {
                    yield return CreateLiveChannel(inChannel);
                }
            }
        }

        private IEnumerable<Video> CreateVideos(TwitchVideosRootResponse inVideos)
        {
            if (inVideos?.videos != null)
            {
                foreach (var inVideo in inVideos.videos)
                {
                    yield return CreateVideo(inVideo);
                }
            }
        }

        private LiveChannel CreateLiveChannel(Stream channel)
        {
            var runtime = DateTime.Now.ToUniversalTime() - channel.started_at;

            if (runtime.Ticks < DateTime.MinValue.Ticks || runtime.Ticks > DateTime.MaxValue.Ticks)
            {
                runtime = new TimeSpan(0);
            }

            return new LiveChannel(
                channel.user_name,
                channel.title,
                channel.game_name,
                channel.viewer_count,
                new DateTime(runtime.Ticks).ToString("H\\h mm\\m"),
                channel.thumbnail_url,
                $"https://www.twitch.tv/{channel.user_name}",
                channel.user_id);
        }

        private Video CreateVideo(Core.Entities.Twitch.Video inVideo)
        {
            return new Video(
                inVideo.channel.name,
                inVideo.title,
                inVideo.game,
                inVideo.length,
                inVideo.preview.large,
                new Uri(inVideo.url),
                inVideo.published_at);
        }
    }
}
