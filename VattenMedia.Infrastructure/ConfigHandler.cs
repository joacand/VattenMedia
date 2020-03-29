using System.IO;
using System.Xml;
using System.Xml.Serialization;
using VattenMedia.Core.Entities;
using VattenMedia.Core.Interfaces;

namespace VattenMedia.Infrastructure
{
    internal class ConfigHandler : IConfigHandler
    {
        public bool HasTwitchAccessToken => !string.IsNullOrWhiteSpace(Config.TwitchAccessToken);
        public bool HasYoutubeAccessToken => !string.IsNullOrWhiteSpace(Config.YoutubeToken);
        public bool HasYoutubeRefreshToken => !string.IsNullOrWhiteSpace(Config.YoutubeRefreshToken);
        public Config Config { get; private set; }

        private static readonly string configPath = "VattenMediaConfig.xml";

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
            if (!File.Exists(configPath))
            {
                return false;
            }
            var serializer = new XmlSerializer(typeof(Config));
            using (var reader = XmlReader.Create(configPath))
            {
                Config = (Config)serializer.Deserialize(reader);
            }
            return true;
        }

        public void SaveToFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Config));

            using (StreamWriter writer = new StreamWriter(configPath))
            {
                serializer.Serialize(writer, Config);
            }
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
    }
}
