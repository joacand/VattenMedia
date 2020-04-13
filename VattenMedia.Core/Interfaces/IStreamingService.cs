using System.Collections.Generic;
using System.Threading.Tasks;
using VattenMedia.Core.Entities;

namespace VattenMedia.Core.Interfaces
{
    public interface IStreamingService
    {
        string OAuthUrl { get; }
        Task<IEnumerable<LiveChannel>> GetLiveChannels(string oAuthId);
        Task<string> GetAuthIdFromUrl(string url);
    }
}
