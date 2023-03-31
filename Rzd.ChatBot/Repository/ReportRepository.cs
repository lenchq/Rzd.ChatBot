using Rzd.ChatBot.Data;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository.Interfaces;

namespace Rzd.ChatBot.Repository;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _db;
    
    public ReportRepository(AppDbContext dbContext)
    {
        _db = dbContext;
    }
    
    public async Task CreateReportAsync(Report report)
    {
        await _db.Reports.AddAsync(report);
        await _db.SaveChangesAsync();
    }
}