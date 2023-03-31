using Rzd.ChatBot.Data.Interfaces;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository.Interfaces;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Repository;

public class UserContextRepository : IUserContextRepository
{
    private IMemoryCache<UserContext> _db;
    private readonly ILogger<UserContextRepository> _logger;
    private bool _initialized = false;
    private string? _botPrefix;
    private const string ContextPrefix = "context:";

    public UserContextRepository(
        ILogger<UserContextRepository> logger,
        IMemoryCache<UserContext> cache
        )
    {
        _logger = logger;
        _db = cache;
    }

    public async Task DeleteContextAsync(long id)
    {
        await _db.DeleteAsync(BuildKey(id));
    }

    public void Initialize(string? botPrefix)
    {
        if (_initialized) return;
        
        _botPrefix = botPrefix;
        _initialized = true;
    }

    private void ThrowIfNotInitialized()
    {
        if (!_initialized)
            throw new Exception("Call Initialize() method before using UserContextRepository");
    }

    private string BuildKey(long id)
    {
        return _botPrefix + ContextPrefix + id;
    }
    
    public UserContext GetContext(long chatId)
    {
        ThrowIfNotInitialized();
        return _db.Get(BuildKey(chatId));
    }
    public async Task<UserContext> GetContextAsync(long chatId)
    {
        ThrowIfNotInitialized();
        return await _db.GetAsync(BuildKey(chatId));
    }


    public void DeleteContext(long id)
    {
        var contextId = BuildKey(id);
        ThrowIfNotInitialized();
        _logger.LogDebug("Deleted context {0}", contextId);
        _db.Delete(contextId);
    }
    
    public bool TryGetContext(long id, out UserContext result)
    {
        ThrowIfNotInitialized();
        try
        {
            result = _db.Get(BuildKey(id));
            return true;
        }
        catch
        {
            result = new UserContext();
        }

        return false;
    }

    public void SetContext(UserContext ctx)
    {
        ThrowIfNotInitialized();

        _db.Set(BuildKey(ctx.Id), ctx);
    }
    public async Task SetContextAsync(UserContext ctx)
    {
        ThrowIfNotInitialized();

        await _db.SetAsync(BuildKey(ctx.Id), ctx);
    }

    public void SetState(long chatId, State state)
    {
        ThrowIfNotInitialized();
        var userCtx = _db.Get(BuildKey(chatId));
        userCtx.State = state;
        _db.Set(ContextPrefix + chatId, userCtx);
    }
}