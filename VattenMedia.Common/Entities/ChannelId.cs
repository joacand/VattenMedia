namespace VattenMedia.Common.Entities
{
    public class ChannelId
    {
        string Id { get; }

        public ChannelId(string id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return Id;
        }
    }
}
