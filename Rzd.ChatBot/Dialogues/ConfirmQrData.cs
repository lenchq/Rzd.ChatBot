using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class ConfirmQrData : ActionDialogue
{
    public ConfirmQrData() : base("confirmQr")
    {
        Options = new BotAction[] { Correct, Incorrect };
    }

    //TODO REDIRECT
    [OptionIndex(0)]
    private ValueTask<State> Correct(Context ctx) => new(State.PermissionsInput);

    [OptionIndex(1)]
    private ValueTask<State> Incorrect(Context ctx) => new(State.ScanQr);

    public override State State => State.ConfirmQr;

    public override string GetText(Context ctx)
    {
        return LocalizationWrapper.FormattedText("caption", ctx.UserForm);
    }
}