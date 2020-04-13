using System.Collections.Generic;

namespace VattenMedia.Core.Entities.Twitch
{
    public class TwitchChannelRootResponse
    {
        public int _total { get; set; }
        public List<User> users { get; set; } = new List<User>();
    }
}
