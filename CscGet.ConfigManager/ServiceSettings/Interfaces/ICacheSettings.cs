namespace CscGet.ConfigManager.ServiceSettings.Interfaces
{
    public interface ICacheSettings
    {
        string RedisCacheConnectionString { get; }
    }
}
