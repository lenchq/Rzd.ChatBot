namespace Rzd.ChatBot.Data.Interfaces;

public interface IMemoryCache<TValue> : IDisposable
{
    TValue Get(string key);
    Task<TValue> GetAsync(string key);
    void Set(string key, TValue value, TimeSpan? expiry = null);
    Task SetAsync(string key, TValue value, TimeSpan? expiry = null);
    void Delete(string key);
    Task DeleteAsync(string key);
}