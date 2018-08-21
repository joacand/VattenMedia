using System.Collections.Generic;
using System.Threading.Tasks;
using VattenMedia.Entities;

namespace VattenMedia.Infrastructure
{
    public interface IStreamingService
    {
        string OAuthUrl { get; }
        Task<List<LiveChannel>> GetLiveChannels(string oAuthId);
    }
}
