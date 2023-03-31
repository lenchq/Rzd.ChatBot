using Rzd.ChatBot.Model;
using Rzd.ChatBot.Repository;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class MenuDialogue : ActionDialogue
{
    private IUserRepository _repo;

    public override State State => State.Menu;
    
    public MenuDialogue() : base("menu")
    {
        Options = new BotAction[] { Browse, MyForm, DisableForm };
    }

    protected override void Initialized()
    {
        _repo = GetService<IUserRepository>();
    }

    [OptionIndex(0)]
    private async ValueTask<State> Browse(Context ctx)
    {
        if (! await _repo.CanPickForm(ctx.UserForm.Id))
        {
            return State.EndOfFormsRedir;
        }
        
        return State.Browsing;
    }
    [OptionIndex(1)]
    private ValueTask<State> MyForm(Context ctx)
    {
        return new ValueTask<State>(State.MyFormEdited);
    }
    [OptionIndex(2)]
    private ValueTask<State> DisableForm(Context ctx)
    {
        ctx.UserForm.Disabled = true;
        return new ValueTask<State>(State.FormDisabled);
    }
}