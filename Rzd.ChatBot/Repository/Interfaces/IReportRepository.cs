using Rzd.ChatBot.Model;

namespace Rzd.ChatBot.Repository.Interfaces;

public interface IReportRepository
{
    public Task CreateReportAsync(Report report);
}