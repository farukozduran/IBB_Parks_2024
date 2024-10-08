namespace IBB.Nesine.Caching.Interfaces
{
    public interface ICacheProvider
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan expiration);
    }
}
