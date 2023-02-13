using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public sealed class EditAboutDialogue : OptionOrInputDialogue
{
    public override State State => State.EditAbout;

    public EditAboutDialogue()
        : base("editAbout")
    {
        Options = new BotAction[] {ContinueWithoutDescription};
    }

    private State ContinueWithoutDescription(Context ctx)
    {
        ctx.UserForm.About = string.Empty;
        return State.EditPhoto;
    }

    public override State ProceedInput(Context ctx)
    {
        ctx.UserForm.About = ctx.Message.Text?.Trim();

        return State.EditPhoto;
    }


    public override bool Validate(Message msg)
    {
        return msg.Text?.Length < 800;
    }
}