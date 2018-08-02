using System;

namespace VattenMedia.Entities
{
    public class TwitchChannel
    {
        public string Name { get; }
        public Uri Url { get; }

        public TwitchChannel(string name, string url)
        {
            Name = name;
            Url = new Uri(url);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
