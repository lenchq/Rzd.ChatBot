using Microsoft.EntityFrameworkCore;
using Rzd.ChatBot.Data;
using Rzd.ChatBot.Model;

namespace Rzd.ChatBot.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    
    public UserRepository(
        AppDbContext db
        )
    {
        _db = db;
    }
    
    public async Task<UserForm> CreateFormAsync(long chatId)
    {
        if (await FormExistsAsync(chatId))
        {
            throw new ArgumentException("User with this id already created form");
        }
        var form = new UserForm
        {
            Id = chatId,
        };
        await _db.Forms.AddAsync(form);
        await _db.SaveChangesAsync();

        return form;
    }

    public async Task<UserForm?> GetFormAsync(long chatId)
    {
        return await _db.Forms
            .Where(_ => _.Id == chatId)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> FormExistsAsync(long chatId)
    {
        return await _db.Forms
            .AnyAsync(_ => _.Id == chatId);
    }

    public async Task UpdateForm(UserForm form)
    {
        _db.Forms
            .Update(form);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteForm(long chatId)
    {
        await _db.Forms
            .Where(form => form.Id == chatId)
            .ExecuteDeleteAsync();
    }
}