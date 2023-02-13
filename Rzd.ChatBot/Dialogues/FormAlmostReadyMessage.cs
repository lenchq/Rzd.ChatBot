using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class FormAlmostReadyMessage : MessageDialogue
{
    public FormAlmostReadyMessage() : base("formAlmostReady")
    {
    }

    public override State State => State.FormAlmostReady;
    public override State NextState { get; set; } = State.ScanQr;
}