using System.Configuration;
using Unity;
using Unity.Lifetime;
using VattenMedia.Core.Interfaces;
using VattenMedia.Infrastructure;
using VattenMedia.Infrastructure.Services;

namespace VattenMedia
{
    public static class Bootstrapper
    {
        public static void AddRegistrations(this IUnityContainer container)
        {
            container.RegisterInstance(CreateAppConfiguration(), new ContainerControlledLifetimeManager());
            container.RegisterType<IConfigHandler, ConfigHandler>();
            container.RegisterType<IStreamStarterService, StreamStarterService>();
            container.RegisterType<IStatusManager, StatusManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<ITwitchService, TwitchService>();
            container.RegisterType<IYoutubeService, YoutubeService>();
        }

        private static AppConfiguration CreateAppConfiguration()
        {
            return new AppConfiguration
            {
                StreamUtilityPath = ConfigurationManager.AppSettings["StreamUtilityPath"],
                StreamUtilityRcPath = ConfigurationManager.AppSettings["StreamUtilityRcPath"],
                TwitchApiUrl = ConfigurationManager.AppSettings["TwitchApiUrl"],
                TwitchEnabled = bool.Parse(ConfigurationManager.AppSettings["TwitchEnabled"]),
                YoutubeEnabled = bool.Parse(ConfigurationManager.AppSettings["YoutubeEnabled"])
            };
        }
    }
}
