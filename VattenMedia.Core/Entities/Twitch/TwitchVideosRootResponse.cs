using System.Collections.Generic;

namespace VattenMedia.Core.Entities.Twitch
{
    public class TwitchVideosRootResponse
    {
        public int _total { get; set; }
        public List<Video> videos { get; set; }
    }
}
