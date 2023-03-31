using EntityFrameworkCore.Triggered;
using Rzd.ChatBot.Model;

namespace Rzd.ChatBot.Data;

public class MatchTrigger : IAfterSaveTrigger<UserLike>
{
    public Task AfterSave(ITriggerContext<UserLike> context, CancellationToken cancellationToken)
    {
        

        return Task.CompletedTask;
    }
}