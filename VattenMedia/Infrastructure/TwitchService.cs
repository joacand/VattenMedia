using RestSharp;
using System.Threading.Tasks;
using VattenMedia.Entities;

namespace VattenMedia.Infrastructure
{
    public class TwitchService : ITwitchService
    {
        private readonly IRestClient client;
        private readonly string baseUrl = Properties.Settings.Default.TwitchApiUrl;
        private readonly string clientId;
        private readonly string clientSecret;

        public string OAuthUrl { get; }
        public string RequestUrl { get; }

        public TwitchService(IConfigHandler configHandler)
        {
            var config = configHandler.Config;
            clientId = config.TwitchClientId;
            clientSecret = config.TwitchClientSecret;

            OAuthUrl = baseUrl + $@"kraken/oauth2/authorize?client_id={clientId}&redirect_uri=http://localhost&response_type=token+id_token&scope=user_read openid";
            RequestUrl = baseUrl +
                $@"oauth2/token?client_id={clientId}&client_secret={clientSecret}&redirect_uri=http://localhost&grant_type=authorization_code&scope=user_read&code=";

            client = new RestClient(baseUrl);
        }

        public async Task<TwitchRootResponse> GetLiveChannels(string oAuthId)
        {
            string requestUrl = baseUrl + @"kraken/streams/followed?stream_type=live";

            var request = new RestRequest(requestUrl, Method.GET);
            request.JsonSerializer.ContentType = "application/json; charset=utf-8";
            request.AddHeader("Accept", "application/vnd.twitchtv.v5+json");
            request.AddHeader("Client-ID", clientId);
            request.AddHeader("Authorization", $"OAuth {oAuthId}");

            var response = await client.ExecuteTaskAsync<TwitchRootResponse>(request);
            return response.Data;
        }
    }
}
