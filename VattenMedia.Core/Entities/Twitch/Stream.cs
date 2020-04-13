using System;
using System.Collections.Generic;

namespace VattenMedia.Core.Entities.Twitch
{
    public class Stream
    {
        public object _id { get; set; }
        public string game { get; set; }
        public string broadcast_platform { get; set; }
        public string community_id { get; set; }
        public List<object> community_ids { get; set; }
        public int viewers { get; set; }
        public int video_height { get; set; }
        public double average_fps { get; set; }
        public int delay { get; set; }
        public DateTime created_at { get; set; }
        public bool is_playlist { get; set; }
        public string stream_type { get; set; }
        public Preview preview { get; set; }
        public Channel channel { get; set; }
    }
}
