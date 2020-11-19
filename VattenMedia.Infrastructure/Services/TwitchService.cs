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

        public string OAuthUrl { get; }

        public TwitchService(IConfigHandler configHandler, AppConfiguration appConfiguration)
        {
            var config = configHandler.Config;
            clientId = config.TwitchClientId;
            baseUrl = appConfiguration.TwitchApiUrl;

            OAuthUrl = baseUrl + $@"kraken/oauth2/authorize?client_id={clientId}&redirect_uri=http://localhost&response_type=token+id_token&scope=user:read:email+chat:read openid";

            client = new RestClient(baseUrl);
        }

        public async Task<IEnumerable<LiveChannel>> GetLiveChannels(string oAuthId)
        {
            var requestUrl = baseUrl + @"kraken/streams/followed?stream_type=live";
            var request = new RestRequest(requestUrl, Method.GET);
            request.AddHeader("Accept", "application/vnd.twitchtv.v5+json");
            request.AddHeader("Client-ID", clientId);
            request.AddHeader("Authorization", $"OAuth {oAuthId}");

            var response = await client.ExecuteAsync<TwitchStreamsRootResponse>(request);

            return CreateChannels(response.Data);
        }

        public async Task<IEnumerable<Video>> GetVideos(string oAuthId, string channelId)
        {
            var requestUrl = baseUrl + $"kraken/channels/{channelId}/videos";
            var request = new RestRequest(requestUrl, Method.GET);
            request.AddHeader("Accept", "application/vnd.twitchtv.v5+json");
            request.AddHeader("Client-ID", clientId);
            request.AddHeader("Authorization", $"OAuth {oAuthId}");

            var response = await client.ExecuteAsync<TwitchVideosRootResponse>(request);

            return CreateVideos(response.Data);
        }

        public Task<string> GetAuthIdFromUrl(string url)
        {
            if (url.Contains("access_token="))
            {
                return Task.FromResult(url.Split(new string[] { "access_token=" }, StringSplitOptions.None)[1].Split(new string[] { "&" }, StringSplitOptions.None)[0]);
            }
            return null;
        }

        public async Task<string> GetChannelId(string oAuthId, string channelName)
        {
            var requestUrl = baseUrl + $"kraken/users?login={channelName}";
            var request = new RestRequest(requestUrl, Method.GET);
            request.AddHeader("Accept", "application/vnd.twitchtv.v5+json");
            request.AddHeader("Client-ID", clientId);
            request.AddHeader("Authorization", $"OAuth {oAuthId}");

            var response = await client.ExecuteAsync<TwitchChannelRootResponse>(request);

            return response.Data.users.Count > 0
                ? response.Data.users.First()._id
                : null;
        }

        private IEnumerable<LiveChannel> CreateChannels(TwitchStreamsRootResponse inChannels)
        {
            if (inChannels?.streams != null)
            {
                foreach (var inChannel in inChannels.streams)
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
            var runtime = DateTime.Now.ToUniversalTime() - channel.created_at;

            if (runtime.Ticks < DateTime.MinValue.Ticks || runtime.Ticks > DateTime.MaxValue.Ticks)
            {
                runtime = new TimeSpan(0);
            }

            return new LiveChannel(
                channel.channel.name,
                channel.channel.status,
                channel.game,
                channel.viewers,
                new DateTime(runtime.Ticks).ToString("H\\h mm\\m"),
                channel.preview.large,
                channel.channel.url,
                channel.channel._id.ToString());
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
