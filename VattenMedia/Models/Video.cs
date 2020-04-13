using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows.Media.Imaging;

namespace VattenMedia.Models
{
    public class Video
    {
        public string Name { get; }
        public string Title { get; }
        public string Game { get; }
        public string Length { get; }
        public BitmapImage Image { get; private set; }
        public Uri BitmapUrl { get; }
        public Uri Url { get; }
        public DateTime PublishedAt { get; }

        public Video(string name, string title, string game, string length, Uri bitmapUrl, Uri url, DateTime publishedAt)
        {
            Name = name;
            Title = title;
            Game = game;
            Length = length;
            BitmapUrl = bitmapUrl;
            Url = url;
            PublishedAt = publishedAt;
            Image = GetImage();
        }

        private BitmapImage GetImage()
        {
            var myRequest = (HttpWebRequest)WebRequest.Create(BitmapUrl);
            myRequest.Method = "GET";
            Bitmap bitmap = null;
            using (var myResponse = (HttpWebResponse)myRequest.GetResponse())
            {
                bitmap = new Bitmap(myResponse.GetResponseStream());
            }

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                bitmap.Dispose();
                memory.Position = 0;
                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }
    }
}
