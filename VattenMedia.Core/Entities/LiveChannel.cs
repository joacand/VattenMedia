﻿using System;

namespace VattenMedia.Core.Entities
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
        public string ChannelId { get; }

        public LiveChannel(string name, string title, string game, int viewers, string runTime, string bitmapUrl, string url, string channelId)
        {
            Name = name;
            Title = title;
            Game = game;
            Viewers = viewers;
            RunTime = runTime;
            BitmapUrl = string.IsNullOrWhiteSpace(bitmapUrl) ? null : new Uri(bitmapUrl);
            Url = new Uri(url);
            ChannelId = channelId;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
