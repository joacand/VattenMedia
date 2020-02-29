using System.Configuration;
using Unity;
using Unity.Lifetime;
using VattenMedia.Core.Entities;
using VattenMedia.Infrastructure.Extensions;

namespace VattenMedia
{
    public static class Bootstrapper
    {
        public static void AddRegistrations(this IUnityContainer container)
        {
            container.AddInfrastructureRegistrations();
            container.RegisterInstance(CreateAppConfiguration(), new ContainerControlledLifetimeManager());
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
