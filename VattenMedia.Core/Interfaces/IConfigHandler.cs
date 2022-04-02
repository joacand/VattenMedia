using VattenMedia.Core.Entities;

namespace VattenMedia.Core.Interfaces
{
    public interface IConfigHandler
    {
        bool HasTwitchAccessToken { get; }
        bool HasYoutubeAccessToken { get; }
        bool HasYoutubeRefreshToken { get; }
        Config Config { get; }
        void SetTwitchAuthDetails(AuthDetails authDetails);
        void SetYoutubeAccessToken(string accessToken);
        void SetYoutubeRefreshToken(string refreshToken);
        bool ReadFromFile();
        void SaveToFile();
        ViewType GetViewType();
        void SetViewType(ViewType viewType);
        bool ToggleFavorited(string stream);
        bool IsFavorited(string stream);
    }
}
