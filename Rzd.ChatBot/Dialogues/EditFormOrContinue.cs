using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class EditFormOrContinueDialogue : ActionDialogue
{
    public EditFormOrContinueDialogue() : base("editFormOrContinue")
    {
        Options = new BotAction[] { Edit, Continue };
    }
    
    [OptionIndex(0)]
    private ValueTask<State> Continue(Context ctx)
    {
        ctx.UserForm.Fulfilled = true;
        ctx.UserForm.Disabled = false;
        return new ValueTask<State>(State.Menu);
    }

    [OptionIndex(1)]
    private ValueTask<State> Edit(Context ctx)
        => new ValueTask<State>(State.EditFormMenu);
    

    public override State State => State.EditFormOrContinue;
}