using Unity;
using Unity.Lifetime;
using VattenMedia.Core.Interfaces;
using VattenMedia.Infrastructure.Factories;
using VattenMedia.Infrastructure.Services;

namespace VattenMedia.Infrastructure.Extensions
{
    public static class ContainerExtensions
    {
        public static void AddInfrastructureRegistrations(this IUnityContainer container)
        {
            container.RegisterType<IConfigHandler, ConfigHandler>();
            container.RegisterType<IStreamStarterService, StreamStarterService>();
            container.RegisterType<IStatusManager, StatusManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITwitchService, TwitchService>();
            container.RegisterType<IYoutubeService, YoutubeService>();
            container.RegisterType<ITwitchChatClientFactory, TwitchChatClientFactory>();
            container.RegisterType<ITwitchEmotesService, TwitchEmotesService>();
        }
    }
}
