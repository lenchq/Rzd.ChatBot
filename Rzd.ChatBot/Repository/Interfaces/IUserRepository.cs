using Rzd.ChatBot.Model;

namespace Rzd.ChatBot.Repository;

public interface IUserRepository
{
    public Task<UserForm> CreateFormAsync(long chatId);
    public Task<UserForm?> GetFormAsync(long chatId);
    public Task<bool> FormExistsAsync(long chatId);
    public Task UpdateForm(UserForm form);
    public Task DeleteForm(long chatId);
}