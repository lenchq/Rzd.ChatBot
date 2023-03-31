using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class BeforeBrowsingMessage : MessageDialogue
{
    public BeforeBrowsingMessage() : base("beforeBrowsing")
    {
    }

    public override State State => State.BeforeBrowsing;
    public override State NextState(Context ctx) => State.Browsing;
}