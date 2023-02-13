using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public sealed class EditAgeDialogue : InputDialogue
{
    public override State State => State.AgeInput;
    public EditAgeDialogue()
        : base("editAge")
    {
        
    }
    
    public override ValueTask<State> ProceedInput(Context ctx)
    {
        ctx.UserForm.Age = int.Parse(ctx.Message.Text!);

        return ValueTask.FromResult(State.AboutInput);
    }

    public override bool Validate(Message msg)
    {
        return (
            int.TryParse(msg.Text, out var age)
            && age is > 13 and < 99
        );
    }
}