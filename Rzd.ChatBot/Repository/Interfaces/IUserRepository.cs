using Rzd.ChatBot.Model;

namespace Rzd.ChatBot.Repository;

public interface IUserRepository
{
    public Task<UserForm> CreateFormAsync(long chatId);
    public Task<UserForm> CreateFormAsync(UserForm newForm);
    public Task<UserForm?> GetFormAsync(long chatId);
    public Task<bool> FormExistsAsync(long chatId);
    public Task UpdateForm(UserForm form);
    public Task DeleteForm(long chatId);
    public Task<int> FormsCount();
    public Task<UserForm> RandomForm();
    public Task<UserLike> AddLikeAsync(UserLike like);
    public Task<UserLike> AddLikeAsync(long fromId, long toId, bool like);
    public Task<UserForm?> PickForm(long userId);
    public Task<bool> CanPickForm(long userId);
    public Task<bool> HasLike(long fromId, long toId);
}