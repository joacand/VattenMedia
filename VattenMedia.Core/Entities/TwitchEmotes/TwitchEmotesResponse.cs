using System;
using System.Collections.Generic;

namespace VattenMedia.Core.Entities.TwitchEmotes
{
    public class Emote
    {
        public string Code { get; set; }
        public int Emoticon_set { get; set; }
        public int Id { get; set; }
    }

    public class TwitchEmotesResponse
    {
        public string Channel_name { get; set; }
        public string Channel_id { get; set; }
        public string Broadcaster_type { get; set; }
        public List<Emote> Emotes { get; set; }
        public string Base_set_id { get; set; }
        public string Display_name { get; set; }
        public DateTime Generated_at { get; set; }
    }
}
