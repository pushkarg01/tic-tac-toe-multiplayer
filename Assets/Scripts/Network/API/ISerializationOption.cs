
namespace Network.API
{
    public interface ISerializationOption
    {
        string ContentType { get; }
        string Token { get; set; }
        string Version { get; }
        string DeviceType { get; }

        // Add PlayerIdentifier Class here
        T Deserialize<T>(string text);
    }
}