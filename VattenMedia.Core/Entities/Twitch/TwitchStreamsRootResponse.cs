using System.Collections.Generic;

namespace VattenMedia.Core.Entities.Twitch
{
    public class TwitchStreamsRootResponse
    {
        public int _total { get; set; }
        public List<Stream> streams { get; set; }
    }
}
