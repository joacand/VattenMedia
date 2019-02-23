using System.Collections.Generic;
using System.Threading.Tasks;
using VattenMedia.Common.Entities;

namespace VattenMedia.Infrastructure.Services
{
    public interface IStreamingService
    {
        string OAuthUrl { get; }
        Task<IEnumerable<LiveChannel>> GetLiveChannels(string oAuthId);
        Task<string> GetAuthIdFromUrl(string url);
    }
}
