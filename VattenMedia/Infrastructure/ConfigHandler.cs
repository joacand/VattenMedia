using System.IO;
using System.Xml;
using System.Xml.Serialization;
using VattenMedia.Entities;

namespace VattenMedia.Infrastructure
{
    public class ConfigHandler : IConfigHandler
    {
        public bool HasAccessToken => !string.IsNullOrWhiteSpace(Config.AccessToken);
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

        public void SetAccessToken(string accessToken)
        {
            Config.AccessToken = accessToken;
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
    }
}
