using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Rzd.ChatBot.Data.Interfaces;
using Rzd.ChatBot.Types.Options;
using StackExchange.Redis;

namespace Rzd.ChatBot.Data;

public class RedisCache<TData> : IMemoryCache<TData>
{
    private readonly IDatabase _db;
    private readonly ILogger<RedisCache<TData>> _logger;

    public RedisCache(ILogger<RedisCache<TData>> logger,IOptions<RedisOptions> options)
    {
        ConnectionMultiplexer connection;
        _logger = logger;
        var opts = options.Value;
        try
        {
            connection = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = new EndPointCollection { opts.Host, opts.Port.ToString() },
                User = opts.User,
                Password = opts.Password,
                ReconnectRetryPolicy = new LinearRetry(60_000),
            });
        }
        catch (Exception ex)
        {
            throw new RedisConnectionException(ConnectionFailureType.UnableToResolvePhysicalConnection,
                "Cannot connect to Redis server", ex);
        }
        _db = connection.GetDatabase();
    }

    [Obsolete]
    public TData Get(string key)
    {
        var json = _db.StringGet(key);
        if (json.IsNullOrEmpty)
            throw new ArgumentException("No such key exists", nameof(key));
        var obj = JsonParse<TData>(json!);
        return obj;
    }

    public async Task<TData> GetAsync(string key)
    {
        var json = await _db.StringGetAsync(key);
        if (json.IsNullOrEmpty)
            throw new ArgumentException("No such key exists", nameof(key));
        var obj = JsonParse<TData>(json!);
        return obj;
    }

    [Obsolete]
    public void Set(string key, TData value, TimeSpan? expiry = null)
    {
        var stringified = JsonStringify(value);
        var result = _db.StringSet(key, stringified, expiry);
        if (!result)
        {
            throw new Exception($"Something went wrong when setting {key} to {stringified}");
        }
    }
    public async Task SetAsync(string key, TData value, TimeSpan? expiry = null)
    {
        var stringified = JsonStringify(value);
        var result = await _db.StringSetAsync(key, stringified, expiry);
        if (!result)
        {
            throw new Exception($"Something went wrong when setting {key} to {stringified}");
        }
    }

    [Obsolete]
    public void Delete(string key)
    {
        _db.KeyDelete(key);
    }
    
    public async Task DeleteAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }


    private static string JsonStringify(TData value)
        => JsonConvert.SerializeObject(value);
    private static T JsonParse<T>(string jsonText)
        => JsonConvert.DeserializeObject<T>(jsonText)!;

    public void Dispose()
    {
        // _connection.GetServers()
        //     .ToList()
        //     .ForEach(x => x.Shutdown());
    }
}