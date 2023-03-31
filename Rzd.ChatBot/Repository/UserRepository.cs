using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Rzd.ChatBot.Data;
using Rzd.ChatBot.Model;
using QueryableExtensions = System.Data.Entity.QueryableExtensions;

namespace Rzd.ChatBot.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    
    public UserRepository(AppDbContext db)
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
        var entry = await _db.Forms.AddAsync(form);
        await _db.SaveChangesAsync();

        return entry.Entity;
    }

    public async Task<UserForm> CreateFormAsync(UserForm newForm)
    {
        if (await FormExistsAsync(newForm.Id))
        {
            throw new ArgumentException("User with this id already created form");
        }

        var entry = await _db.Forms.AddAsync(newForm);
        await _db.SaveChangesAsync();

        return entry.Entity;
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
    public async Task<int> FormsCount()
    {
        return await _db.Forms.CountAsync();
    }

    public async Task<UserForm> RandomForm()
    {
        return await _db.Forms.FromSql($"SELECT * FROM \"Forms\" ORDER BY RANDOM() LIMIT 1").SingleAsync();
    }

    public async Task<UserLike> AddLikeAsync(UserLike like)
    {
        var entry = await _db.Likes.AddAsync(like);
        await _db.SaveChangesAsync();
        
        return entry.Entity;
    }

    public async Task<UserLike> AddLikeAsync(long fromId, long toId, bool like)
    {
        var userLike = new UserLike
        {
            FromId = fromId,
            ToId = toId,
            Like = like
        };
        
        var entry = await _db.Likes.AddAsync(userLike);
        await _db.SaveChangesAsync();
        
        return entry.Entity;
    }

    public async Task DeleteForm(long chatId)
    {
        await _db.Forms
            .AsNoTracking()
            .Where(form => form.Id == chatId)
            .ExecuteDeleteAsync();
        _db.DetachForm(chatId);
    }

    public async Task<bool> CanPickForm(long userId)
    {
        var userForm = await GetFormAsync(userId);

        var viewedIds = await _db.Likes
            .Where(like => like.FromId == userId)
            .Select(like => like.ToId)
            .ToListAsync();


        return await _db.Forms
            .Where(form => form.TrainNumber == userForm.TrainNumber && form.Id != userId && !form.Disabled)
            .Where(form => !viewedIds.Contains(form.Id))
            .AnyAsync();
    }

    public async Task<bool> HasLike(long fromId, long toId)
    {
        return await _db.Likes
            .AnyAsync(like => like.FromId == fromId && like.ToId == toId);
    }

    public async Task<UserForm?> PickForm(long userId)
    {
        var userForm = await GetFormAsync(userId);

        // TODO convert to ef core linq
        var f = await _db.Forms.FromSqlInterpolated(
                $"SELECT f.* FROM \"Forms\" AS f WHERE NOT \"Disabled\" AND \"Id\" != {userId} AND \"TrainNumber\" = {userForm!.TrainNumber} AND \"Id\" NOT IN ( SELECT \"ToId\" FROM \"Likes\" WHERE \"FromId\" = {userForm.Id}) LIMIT 1")
            .SingleOrDefaultAsync();
        
        // var f = await _db.Forms
        //     .Where(form => form.TrainNumber == userForm.TrainNumber && !form.Disabled)
        //     .Join(_db.Likes,
        //         form => form.Id,
        //         like => like.FromId)
        //     .Take(1)
        //     .SingleAsync();
        
        // f = f.OrderBy(form => Math.Abs(userForm.Age.Value - form.Age.Value))
        //     .ToList();

        // .GroupJoin(_db.Likes,
            //     form => form.Id,
            //     like => like.ToId,
            //     (f,l) => new
            //     {
            //         
            //     })
            //
            // .GroupJoin(_db.Likes,
            //     form => form.Id,
            //     like => like.ToId,
            //     (f, l) => new
            //     {
            //         f.Id,
            //         f.About,
            //         f.Gender,
            //         f.Name,
            //         f.Photos,
            //         f.Seat,
            //         f.ShowContact,
            //         f.ShowCoupe,
            //     })
            

        // var f = ( from form in _db.Forms
        //     where form.TrainNumber == userForm!.TrainNumber
        //     join l in _db.Likes on form.Id equals l.ToId
        //     group form by form.Id into w
        //     orderby Math.Abs(form.Age.Value - userForm.Age.Value)
        //     select  form ).ToArray();

        return f;
    }
}