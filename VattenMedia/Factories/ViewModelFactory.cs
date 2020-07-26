using VattenMedia.Core.Interfaces;
using VattenMedia.ViewModels;

namespace VattenMedia.Factories
{
    internal class ViewModelFactory : IViewModelFactory
    {
        private readonly ITwitchChatClientFactory twitchChatClientFactory;

        public ViewModelFactory(ITwitchChatClientFactory twitchChatClientFactory)
        {
            this.twitchChatClientFactory = twitchChatClientFactory;
        }

        public ChatViewModel CreateChatViewModel()
        {
            return new ChatViewModel(twitchChatClientFactory.Create());
        }
    }
}
