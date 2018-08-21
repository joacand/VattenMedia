using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VattenMedia.Entities;

namespace VattenMedia.Infrastructure
{
    public class TwitchService : IStreamingService
    {
        private readonly IRestClient client;
        private readonly string baseUrl = Properties.Settings.Default.TwitchApiUrl;
        private readonly string clientId;

        public string OAuthUrl { get; }

        public TwitchService(IConfigHandler configHandler)
        {
            var config = configHandler.Config;
            clientId = config.TwitchClientId;

            OAuthUrl = baseUrl + $@"kraken/oauth2/authorize?client_id={clientId}&redirect_uri=http://localhost&response_type=token+id_token&scope=user_read openid";

            client = new RestClient(baseUrl);
        }

        public async Task<List<LiveChannel>> GetLiveChannels(string oAuthId)
        {
            string requestUrl = baseUrl + @"kraken/streams/followed?stream_type=live";

            var request = new RestRequest(requestUrl, Method.GET);
            request.JsonSerializer.ContentType = "application/json; charset=utf-8";
            request.AddHeader("Accept", "application/vnd.twitchtv.v5+json");
            request.AddHeader("Client-ID", clientId);
            request.AddHeader("Authorization", $"OAuth {oAuthId}");

            var response = await client.ExecuteTaskAsync<TwitchRootResponse>(request);

            return CreateChannels(response.Data);
        }

        private List<LiveChannel> CreateChannels(TwitchRootResponse inChannels)
        {
            var liveChannels = new List<LiveChannel>();

            if (inChannels?.streams == null)
            {
                return liveChannels;
            }

            for (int i = 0; i < inChannels.streams.Count; i++)
            {
                var chan = inChannels.streams[i];
                TimeSpan runtime = DateTime.Now.ToUniversalTime() - chan.created_at;
                var liveChannel = new LiveChannel(chan.channel.name, chan.channel.status, chan.game, chan.viewers, new DateTime(runtime.Ticks).ToString("H\\h mm\\m"), chan.preview.medium, chan.channel.url);
                liveChannels.Add(liveChannel);
            }

            return liveChannels;
        }
    }
}
