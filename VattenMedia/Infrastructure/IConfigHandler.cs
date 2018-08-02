using VattenMedia.Entities;

namespace VattenMedia.Infrastructure
{
    public interface IConfigHandler
    {
        bool HasAccessToken { get; }
        Config Config { get; }
        void SetAccessToken(string accessToken);
        bool ReadFromFile();
        void SaveToFile();
    }
}
