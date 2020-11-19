using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace VattenMedia.Core.Entities
{
    public class Config
    {
        public string TwitchAccessToken { get; set; } = string.Empty;
        public string TwitchClientId { get; set; } = string.Empty;
        public string TwitchUsername { get; set; } = string.Empty;

        public string YoutubeChannelId { get; set; } = string.Empty;
        public string YoutubeApiKey { get; set; } = string.Empty;
        public string YoutubeClientId { get; set; } = string.Empty;
        public string YoutubeClientSecret { get; set; } = string.Empty;
        public string YoutubeToken { get; set; } = string.Empty;
        public string YoutubeRefreshToken { get; set; } = string.Empty;

        [JsonConverter(typeof(StringEnumConverter))]
        public ViewType View { get; set; } = ViewType.List;
        public HashSet<string> FavoritedStreams { get; set; } = new HashSet<string>();
    }
}
