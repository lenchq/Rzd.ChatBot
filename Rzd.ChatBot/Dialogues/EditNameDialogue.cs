using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;


namespace Rzd.ChatBot.Dialogues;

public sealed class EditNameDialogue : InputDialogue
{ 
    public override State State => State.EditName;

    public EditNameDialogue()
        : base("editName")
    {

    }

    public override State ProceedInput(Context ctx)
    {
        ctx.UserForm.Name = ctx.Message.Text?.Trim();

        return State.EditAge;
    }


    public override bool Validate(Message msg)
    {
        return msg.Text?.Length < 32;
    }
}