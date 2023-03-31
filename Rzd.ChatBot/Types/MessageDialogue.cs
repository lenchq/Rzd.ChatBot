using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;
using Rzd.ChatBot.Types.Interfaces;

namespace Rzd.ChatBot.Types;

public abstract class MessageDialogue : Dialogue, IMessageDialogue
{
    public override InputType InputType => InputType.None;
    public abstract State NextState(Context ctx);

    protected MessageDialogue(string localizationName) : base(localizationName)
    {
    }
}