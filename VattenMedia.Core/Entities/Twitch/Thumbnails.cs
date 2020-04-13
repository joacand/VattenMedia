using System.Collections.Generic;

namespace VattenMedia.Core.Entities.Twitch
{
    public class Thumbnails
    {
        public List<Large> large { get; set; }
        public List<Medium> medium { get; set; }
        public List<Small> small { get; set; }
        public List<Template> template { get; set; }
    }
}
