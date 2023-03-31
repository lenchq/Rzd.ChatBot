using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class FormDisabled : ActionDialogue
{
    public FormDisabled() : base("formDisabled")
    {
        Options = new BotAction[] { EnableForm };
    }

    [OptionIndex(0)]
    ValueTask<State> EnableForm(Context ctx)
    {
        ctx.UserForm.Disabled = false;
        return new(State.Menu);
    }

    public override State State => State.FormDisabled;
}