using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public sealed class EditAgeDialogue : InputDialogue
{
    public override State State => State.EditAge;
    public EditAgeDialogue()
        : base("editAge")
    {
        
    }
    
    public override State ProceedInput(Context ctx)
    {
        ctx.UserForm.Age = int.Parse(ctx.Message.Text!);
        //ctx.UserContext.Age = int.Parse(ctx.Message.Text);
        return State.EditAbout;
    }

    public override bool Validate(Message msg)
    {
        return (
            int.TryParse(msg.Text, out var age)
            && age is > 12 and < 99
        );
    }
}