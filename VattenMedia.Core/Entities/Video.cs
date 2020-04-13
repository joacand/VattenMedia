using System;

namespace VattenMedia.Core.Entities
{
    public class Video
    {
        public string Name { get; }
        public string Title { get; }
        public string Game { get; }
        public string Length { get; }
        public Uri BitmapUrl { get; }
        public Uri Url { get; }
        public DateTime PublishedAt { get; }

        public Video(string name, string title, string game, int length, string bitmapUrl, Uri url, DateTime publishedAt)
        {
            Name = name;
            Title = title;
            Game = game;
            Length = new DateTime(TimeSpan.FromSeconds(length).Ticks).ToString("H\\h mm\\m");
            BitmapUrl = string.IsNullOrWhiteSpace(bitmapUrl) ? null : new Uri(bitmapUrl);
            Url = url;
            PublishedAt = publishedAt;
        }
    }
}
