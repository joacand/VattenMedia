using System.Collections.Generic;

namespace VattenMedia.Core.Entities
{
    public static class QualityOptions
    {
        public static List<string> HighQuality => new List<string>()
        {
            //"source",
            "1080p60",
            "900p60",
            "720p60",
            "best",
            "1080p30",
            "900p",
            "720p30",
            "540p30",
            "480p30",
            "360p30",
            "240p30",
            "160p30"
        };

        public static List<string> MediumQuality => new List<string>
        {
            "medium",
            "480p30",
            "480p",
            "540p30",
            "540p",
            "worst"
        };

        public static List<string> LowQuality => new List<string>
        {
            "low",
            "360p30",
            "360p",
            "240p30",
            "240p",
            "160p30",
            "160p",
            "worst"
        };
    }
}
