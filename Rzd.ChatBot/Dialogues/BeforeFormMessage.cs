using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class BeforeFormMessage : MessageDialogue
{
    public BeforeFormMessage() : base("beforeForm")
    {
    }

    public override State State => State.BeforeFormMessage;
    public override State NextState(Context _) => State.MyForm;
}