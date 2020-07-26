using VattenMedia.Core.Interfaces;
using VattenMedia.Infrastructure.Services;

namespace VattenMedia.Infrastructure.Factories
{
    internal class TwitchChatClientFactory : ITwitchChatClientFactory
    {
        public ITwitchChatClient Create()
        {
            return new TwitchChatClient();
        }
    }
}
