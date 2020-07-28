using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VattenMedia.Core.Entities.TwitchEmotes;
using VattenMedia.Core.Interfaces;

namespace VattenMedia.Infrastructure.Services
{
    internal class TwitchEmotesService : ITwitchEmotesService
    {
        private readonly IRestClient client;
        private readonly string baseUrl;

        private Dictionary<string, TwitchEmotesResponse> ChannelIdEmotes { get; set; } = new Dictionary<string, TwitchEmotesResponse>();

        public string OAuthUrl { get; }

        public TwitchEmotesService()
        {
            baseUrl = @"https://api.twitchemotes.com/";
            client = new RestClient(baseUrl);
        }

        public async Task LoadChannelEmotes(string channelId)
        {
            if (ChannelIdEmotes.ContainsKey(channelId)) return;

            var requestUrl = baseUrl + $"api/v4/channels/{channelId}";
            var request = new RestRequest(requestUrl, Method.GET);
            request.AddHeader("Accept", "application/json");

            var response = await client.ExecuteTaskAsync<TwitchEmotesResponse>(request);

            ChannelIdEmotes.Add(channelId, response.Data);
        }

        public Dictionary<string, string> CheckForEmotes(string channelId, string message)
        {
            var result = new Dictionary<string, string>();
            if (!ChannelIdEmotes.ContainsKey(channelId)) return result;


            var tokens = message.Split(' ');
            var emotes = ChannelIdEmotes[channelId].Emotes;

            foreach (var token in tokens)
            {
                var match = emotes.FirstOrDefault(x => x.Code.Equals(token));
                if (match != null)
                {
                    result.Add(token, @"https://static-cdn.jtvnw.net/emoticons/v1/" + match.Id + @"/2.0");
                }
            }

            return result;
        }
    }
}
