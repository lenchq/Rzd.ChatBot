using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;


namespace Rzd.ChatBot.Dialogues;

public sealed class EditNameDialogue : InputDialogue
{ 
    public override State State => State.NameInput;

    public EditNameDialogue()
        : base("editName")
    {

    }

    public override ValueTask<State> ProceedInput(Context ctx)
    {
        ctx.UserForm.Name = ctx.Message.Text?.Trim();

        return ValueTask.FromResult(State.AgeInput);
    }


    public override bool Validate(Message msg)
    {
        return msg.Text?.Length < 32;
    }
}