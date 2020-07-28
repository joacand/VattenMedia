using System.Collections.Generic;

namespace VattenMedia.Core.Entities.Twitch
{
    public class Links
    {
        public string Self { get; set; }
    }

    public class Image
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Url { get; set; }
        public int Emoticon_set { get; set; }
    }

    public class Emoticon
    {
        public int Id { get; set; }
        public string Regex { get; set; }
        public List<Image> Images { get; set; }
    }

    public class EmoticonsRoot
    {
        public Links _links { get; set; }
        public List<Emoticon> Emoticons { get; set; }
    }
}
