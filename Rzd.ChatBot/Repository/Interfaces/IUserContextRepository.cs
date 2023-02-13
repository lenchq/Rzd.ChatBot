using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Repository.Interfaces;

public interface IUserContextRepository
{
    UserContext GetContext(long id);
    void DeleteContext(long id);
    void Initialize(string? botPrefix);
    bool TryGetContext(long id, out UserContext result);
    void SetContext(UserContext ctx);
    void SetState(long chatId, State state);
}