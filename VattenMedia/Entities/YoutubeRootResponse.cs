﻿using System;
using System.Collections.Generic;

namespace VattenMedia.Entities
{
    public class PageInfo
    {
        public int totalResults { get; set; }
        public int resultsPerPage { get; set; }
    }

    public class ResourceId
    {
        public string kind { get; set; }
        public string channelId { get; set; }
    }

    public class Default
    {
        public string url { get; set; }
    }

    public class Medium
    {
        public string url { get; set; }
    }

    public class High
    {
        public string url { get; set; }
    }

    public class Thumbnails
    {
        public Default @default { get; set; }
        public Medium medium { get; set; }
        public High high { get; set; }
    }

    public class Snippet
    {
        public DateTime publishedAt { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public ResourceId resourceId { get; set; }
        public string channelId { get; set; }
        public Thumbnails thumbnails { get; set; }
    }

    public class Item
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public Snippet snippet { get; set; }
    }

    public class YoutubeRootResponse
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string nextPageToken { get; set; }
        public PageInfo pageInfo { get; set; }
        public List<Item> items { get; set; }
    }
}
