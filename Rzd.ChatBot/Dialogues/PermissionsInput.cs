using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class PermissionsInput : ActionDialogue
{
    public override State State => State.PermissionsInput;
    
    public PermissionsInput() : base("permissions")
    {
        Options = new BotAction[] {OnlyTelegram, OnlySeat, Both};
    }

    
    [OptionIndex(0)]
    private ValueTask<State> OnlyTelegram(Context ctx)
    {
        ctx.UserForm.ShowContact = true;
        ctx.UserForm.ShowCoupe = false;
        return new ValueTask<State>(State.BeforeFormMessage);
    }

    [OptionIndex(1)]
    private ValueTask<State> OnlySeat(Context ctx)
    {
        ctx.UserForm.ShowContact = false;
        ctx.UserForm.ShowCoupe = true;
        return new ValueTask<State>(State.BeforeFormMessage);
    }

    [OptionIndex(2)]
    private ValueTask<State> Both(Context ctx)
    {
        ctx.UserForm.ShowContact = true;
        ctx.UserForm.ShowCoupe = true;
        return new ValueTask<State>(State.BeforeFormMessage);
    }
}