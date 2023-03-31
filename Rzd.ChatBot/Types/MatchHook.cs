using Rzd.ChatBot.Model;

namespace Rzd.ChatBot.Types;

public delegate ValueTask AsyncEventHandler<in TArgs>(TArgs args);

// public record LikeEventArgs(UserContext MatchTarget, UserForm Matcher);
public record LikeEventArgs(long FromId, long FoId);
public record MatchEventArgs(long MatchFrom, long MatchTo);

public class MatchHook
{
    public static readonly MatchHook Instance = new();
    
    public event AsyncEventHandler<LikeEventArgs> Like;
    public event AsyncEventHandler<MatchEventArgs> Match; 

    public async Task OnLike(long fromId, long toId)
    {
        await Like.Invoke(new LikeEventArgs(fromId, toId));
    }

    public async Task OnMatch(long matchFrom, long matchTo)
    {
        await Match.Invoke(new MatchEventArgs(matchFrom, matchTo));
    }
}