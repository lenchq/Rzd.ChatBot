using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class FormConfirmQrData : ActionDialogue
{
    public FormConfirmQrData() : base("confirmQr")
    {
        Options = new BotAction[] { Correct, Incorrect };
    }
    
    [OptionIndex(0)]
    private ValueTask<State> Correct(Context ctx) => new(State.MyFormEdited);

    [OptionIndex(1)]
    private ValueTask<State> Incorrect(Context ctx) => new(State.FormRescanQr);

    public override State State => State.FormEditConfirmQr;

    public override string GetText(Context ctx)
    {
        return LocalizationWrapper.FormattedText("caption", ctx.UserForm);
    }
}