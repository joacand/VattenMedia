using System.Threading.Tasks;
using VattenMedia.Entities;

namespace VattenMedia.Infrastructure
{
    public interface ITwitchService
    {
        string OAuthUrl { get; }
        string RequestUrl { get; }
        Task<TwitchRootResponse> GetLiveChannels(string oAuthId);
    }
}
