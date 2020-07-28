using System.Collections.Generic;
using System.Threading.Tasks;

namespace VattenMedia.Core.Interfaces
{
    public interface ITwitchEmotesService
    {
        Task LoadChannelEmotes(string channelId);
        Dictionary<string, string> CheckForEmotes(string channelId, string message);
    }
}
