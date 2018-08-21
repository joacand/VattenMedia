using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VattenMedia.Entities;

namespace VattenMedia.Infrastructure
{
    class YoutubeService : IStreamingService
    {
        public string OAuthUrl => throw new NotImplementedException();

        public Task<List<LiveChannel>> GetLiveChannels(string oAuthId)
        {
            throw new NotImplementedException();
        }
    }
}
