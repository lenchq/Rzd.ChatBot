using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;


namespace Rzd.ChatBot.Dialogues;

public sealed class EditCoupeNumberDialogue : InputDialogue
{
    public override State State => State.AddCoupe;

    public EditCoupeNumberDialogue()
        : base("editCoupe")
    {

    }

    public override ValueTask<State> ProceedInput(Context ctx)
    {
        //TODO  
        var val = int.Parse(ctx.Message.Text);
        ctx.UserForm.Seat = val;

        return ValueTask.FromResult(State.PermissionsInput);
    }


    public override bool Validate(Message msg)
    {
        //TODO check coupe number
        return int.TryParse(msg.Text, out _);
    }
}