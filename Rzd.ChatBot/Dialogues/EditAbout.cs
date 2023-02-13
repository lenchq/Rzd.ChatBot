using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public sealed class EditAboutDialogue : OptionOrInputDialogue
{
    public override State State => State.AboutInput;

    public EditAboutDialogue()
        : base("editAbout")
    {
        Options = new BotAction[] {ContinueWithoutDescription};
    }

    [OptionIndex(0)]
    private ValueTask<State> ContinueWithoutDescription(Context ctx)
    {
        ctx.UserForm.About = string.Empty;
        return new ValueTask<State>(State.PhotoInput);
    }

    public override ValueTask<State> ProceedInput(Context ctx)
    {
        ctx.UserForm.About = ctx.Message.Text?.Trim();

        return ValueTask.FromResult(State.PhotoInput);
    }

    public override bool Validate(Message msg)
    {
        return msg.Text?.Length < 800;
    }
}