using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class FormEditAbout : OptionOrInputDialogue
{
    public override State State => State.FormEditAbout;
    
    public FormEditAbout() : base("formEditAbout")
    {
        Options = new BotAction[] {ContinueWithoutDescription, GoBack};
    }

    [OptionIndex(0)]
    private ValueTask<State> ContinueWithoutDescription(Context ctx)
    {
        ctx.UserForm.About = string.Empty;
        return new(State.MyFormEdited);
    }
    
    [OptionIndex(1)]
    private ValueTask<State> GoBack(Context ctx) => new(State.MyFormEdited);

    
    public override ValueTask<State> ProceedInput(Context ctx)
    {
        ctx.UserForm.About = ctx.Message.Text?.Trim();

        return ValueTask.FromResult(State.MyFormEdited);
    }

    public override bool Validate(Message msg)
    {
        return msg.Text?.Length < 800;
    }
}