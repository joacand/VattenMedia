using VattenMedia.Core.Interfaces;
using VattenMedia.ViewModels;

namespace VattenMedia.Factories
{
    internal class ViewModelFactory : IViewModelFactory
    {
        private readonly ITwitchChatClientFactory twitchChatClientFactory;
        private readonly ITwitchEmotesService twitchEmotesService;

        public ViewModelFactory(
            ITwitchChatClientFactory twitchChatClientFactory,
            ITwitchEmotesService twitchEmotesService)
        {
            this.twitchChatClientFactory = twitchChatClientFactory;
            this.twitchEmotesService = twitchEmotesService;
        }

        public ChatViewModel CreateChatViewModel()
        {
            return new ChatViewModel(twitchChatClientFactory.Create(), twitchEmotesService);
        }
    }
}
