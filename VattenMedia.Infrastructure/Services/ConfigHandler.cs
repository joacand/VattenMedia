using Newtonsoft.Json;
using System.IO;
using VattenMedia.Core.Entities;
using VattenMedia.Core.Interfaces;

namespace VattenMedia.Infrastructure.Services
{
    internal class ConfigHandler : IConfigHandler
    {
        public bool HasTwitchAccessToken => !string.IsNullOrWhiteSpace(Config.TwitchAccessToken);
        public bool HasYoutubeAccessToken => !string.IsNullOrWhiteSpace(Config.YoutubeToken);
        public bool HasYoutubeRefreshToken => !string.IsNullOrWhiteSpace(Config.YoutubeRefreshToken);
        public Config Config { get; private set; }

        public ConfigHandler()
        {
            if (!ReadFromFile())
            {
                Config = new Config();
                SaveToFile();
            }
        }

        public void SetTwitchAccessToken(string accessToken)
        {
            Config.TwitchAccessToken = accessToken;
            SaveToFile();
        }

        public void SetYoutubeAccessToken(string accessToken)
        {
            Config.YoutubeToken = accessToken;
            SaveToFile();
        }

        public void SetYoutubeRefreshToken(string refreshToken)
        {
            Config.YoutubeRefreshToken = refreshToken;
            SaveToFile();
        }

        public bool ReadFromFile()
        {
            if (!File.Exists(Constants.ConfigPath))
            {
                return false;
            }
            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Constants.ConfigPath));
            return true;
        }

        public void SaveToFile()
        {
            using var writer = new StreamWriter(Constants.ConfigPath);
            var serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented
            };
            serializer.Serialize(writer, Config);
        }

        public ViewType GetViewType()
        {
            return Config.View;
        }

        public void SetViewType(ViewType viewType)
        {
            if (Config.View != viewType)
            {
                Config.View = viewType;
                SaveToFile();
            }
        }

        public bool IsFavorited(string stream)
        {
            return Config.FavoritedStreams.Contains(stream);
        }

        public bool ToggleFavorited(string stream)
        {
            var isFavoriteResult = false;
            if (IsFavorited(stream))
            {
                Config.FavoritedStreams.Remove(stream);
            }
            else
            {
                Config.FavoritedStreams.Add(stream);
                isFavoriteResult = true;
            }
            SaveToFile();
            return isFavoriteResult;
        }
    }
}
