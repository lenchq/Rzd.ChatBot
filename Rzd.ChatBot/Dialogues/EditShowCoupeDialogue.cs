using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;


namespace Rzd.ChatBot.Dialogues;

public sealed class EditShowCoupeDialogue : ActionDialogue
{
    public override State State => State.EditShowCoupe;

    public EditShowCoupeDialogue()
        : base("editShowCoupe")
    {
        Options = new BotAction[] { Yes, No };
    }

    public State Yes(Context ctx)
    {
        ctx.UserForm.ShowCoupe = true;
        return State.FormStarting;
    }
    public State No(Context ctx)
    {
        ctx.UserForm.ShowCoupe = false;
        return State.FormStarting;
    }
}