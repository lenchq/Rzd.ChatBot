using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class Browsing : ActionDialogue
{
    private IUserRepository _repo;
    private UserForm currForm;

    public Browsing() : base("browsing")
    {
        // TODO Like with message
        // TODO ReportDialogue
        Options = new BotAction[] {Like, Dislike, Leave};
    }

    protected override void Initialized()
    {
        _repo = GetService<IUserRepository>();
    }

    public override async ValueTask PostInitializeAsync(Context ctx)
    {
        UserForm form;
        if (ctx.UserContext.LikeQueue.Any())
        {
            form = await _repo.GetFormAsync(ctx.UserContext.LikeQueue.Peek());
            ctx.UserContext.CurrentForm = form.Id;
        }
        else if (ctx.UserContext.CurrentForm is not null)
        {
            form = await _repo.GetFormAsync(ctx.UserContext.CurrentForm.Value);
            ctx.UserContext.CurrentForm = null;
        }
        else
        {
            form = await _repo.PickForm(ctx.UserContext.Id);
            ctx.UserContext.CurrentForm = form.Id;
        }

        currForm = form!;
    }

    public override State State => State.Browsing;

    [OptionIndex(0)]
    private async ValueTask<State> Like(Context ctx)
    {
        var bothLike = await _repo.HasLike(currForm.Id, ctx.UserContext.Id);
        await _repo.AddLikeAsync(ctx.UserForm.Id, currForm.Id, true);
        if (bothLike)
        {
            // TODO show data about match if likes is on both sides
            // if (ctx.UserContext.LikeQueue.TryPeek(out var peek) && peek == currForm.Id)
            //     ctx.UserContext.LikeQueue.Dequeue();
            await MatchHook.Instance.OnMatch(ctx.UserContext.Id, currForm.Id);
            return State.MatchRedir;
        }

        // TODO!! for fake generated forms!! remove!
        if (currForm.Id > 0)
            await MatchHook.Instance.OnLike(ctx.UserContext.Id, currForm.Id);
        
        if (ctx.UserContext.LikeQueue.Any())
            ctx.UserContext.LikeQueue.Dequeue();
        
        if (!await _repo.CanPickForm(ctx.UserForm.Id)) {
            return State.EndOfFormsRedir;
        }
        return State.Browsing;
    }

    [OptionIndex(1)]
    private async ValueTask<State> Dislike(Context ctx)
    {
        //TODO dislike
        await _repo.AddLikeAsync(ctx.UserForm.Id, currForm.Id, false);
        if (!await _repo.CanPickForm(ctx.UserForm.Id)) {
            return State.EndOfFormsRedir;
        }
        return State.Browsing;
    }

    [OptionIndex(2)]
    private ValueTask<State> Leave(Context ctx)
    {
        ctx.UserContext.CurrentForm = null;
        return new ValueTask<State>(State.Menu);
    }

    public override Photo[]? GetPhotos()
    {
        if (!currForm.Photos.Any())
            return null;
        
        return currForm.Photos
            .Select(fileId => new Photo(fileId))
            .ToArray();
    }

    public override string GetText(Context ctx)
    {
        return LocalizationWrapper.FormattedText("form", currForm);
    }
}