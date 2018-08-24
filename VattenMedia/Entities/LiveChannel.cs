using System;

namespace VattenMedia.Entities
{
    public class LiveChannel
    {
        public string Name { get; }
        public string Title { get; }
        public string Game { get; }
        public int Viewers { get; }
        public string RunTime { get; }
        public Uri BitmapUrl { get; }
        public Uri Url { get; }

        public LiveChannel(string name, string title, string game, int viewers, string runTime, string bitmapUrl, string url)
        {
            Name = name;
            Title = title;
            Game = game;
            Viewers = viewers;
            RunTime = runTime;
            BitmapUrl = string.IsNullOrWhiteSpace(bitmapUrl) ? null : new Uri(bitmapUrl);
            Url = string.IsNullOrWhiteSpace(url) ? null : new Uri(url);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
