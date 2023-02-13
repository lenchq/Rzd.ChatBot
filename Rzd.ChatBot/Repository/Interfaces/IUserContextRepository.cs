using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Repository.Interfaces;

public interface IUserContextRepository
{
    Task<UserContext> GetContextAsync(long id);
    UserContext GetContext(long id);
    void DeleteContext(long id);
    void Initialize(string? botPrefix);
    bool TryGetContext(long id, out UserContext result);
    void SetContext(UserContext ctx);
    Task SetContextAsync(UserContext ctx);
    void SetState(long chatId, State state);
}