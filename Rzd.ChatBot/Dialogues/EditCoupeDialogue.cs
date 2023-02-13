using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;


namespace Rzd.ChatBot.Dialogues;

public sealed class EditCoupeNumberDialogue : InputDialogue
{
    public override State State => State.EditCoupe;

    public EditCoupeNumberDialogue()
        : base("editCoupe")
    {

    }

    public override State ProceedInput(Context ctx)
    {
        //TODO  
        var val = int.Parse(ctx.Message.Text);
        ctx.UserForm.Coupe = val;

        return State.FormStarting;
    }


    public override bool Validate(Message msg)
    {
        //TODO check coupe number
        return int.TryParse(msg.Text, out _);
    }
}