using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class _DeletedMessage : MessageDialogue
{
    public _DeletedMessage() : base("_dev_deleted")
    {
    }

    public override State State => State.Undefined;
    public override State NextState(Context ctx) => State.Starting;
}