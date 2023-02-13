namespace Rzd.ChatBot.Data.Interfaces;

public interface IMemoryCache<TValue> : IDisposable
{
    TValue this[string key] { get; set; }
    TValue Get(string key);
    void Set(string key, TValue value);
    void Delete(string key);
    void Set(string key, TValue value, TimeSpan expiry);
}