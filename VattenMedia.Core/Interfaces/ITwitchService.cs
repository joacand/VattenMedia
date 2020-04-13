using System.Collections.Generic;
using System.Threading.Tasks;
using VattenMedia.Core.Entities;

namespace VattenMedia.Core.Interfaces
{
    public interface ITwitchService : IStreamingService
    {
        Task<string> GetChannelId(string oAuthId, string channelName);
        Task<IEnumerable<Video>> GetVideos(string oAuthId, string channelId);
    }
}
