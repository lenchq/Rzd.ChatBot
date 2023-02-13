using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;


namespace Rzd.ChatBot.Dialogues;

public sealed class FormEditName : OptionOrInputDialogue
{ 
    public override State State => State.FormEditName;

    public FormEditName()
        : base("editName")
    {
        Options = new BotAction[] {Default};
    }

    [OptionIndex(0)]
    private ValueTask<State> Default(Context ctx) => new(State.FormEditAge);

    public override ValueTask<State> ProceedInput(Context ctx)
    {
        ctx.UserForm.Name = ctx.Message.Text?.Trim();

        return ValueTask.FromResult(State.FormEditAge);
    }


    public override bool Validate(Message msg)
    {
        return msg.Text?.Length < 32;
    }
    
    
}