using VattenMedia.Core.Entities;

namespace VattenMedia.Core.Interfaces
{
    public interface IConfigHandler
    {
        bool HasTwitchAccessToken { get; }
        bool HasYoutubeAccessToken { get; }
        bool HasYoutubeRefreshToken { get; }
        Config Config { get; }
        void SetTwitchAccessToken(string accessToken);
        void SetYoutubeAccessToken(string accessToken);
        void SetYoutubeRefreshToken(string refreshToken);
        bool ReadFromFile();
        void SaveToFile();
    }
}
