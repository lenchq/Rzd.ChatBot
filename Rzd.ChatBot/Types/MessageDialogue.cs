using Rzd.ChatBot.Types.Enums;
using Rzd.ChatBot.Types.Interfaces;

namespace Rzd.ChatBot.Types;

public abstract class MessageDialogue : Dialogue, IMessageDialogue
{
    public override InputType InputType => InputType.None;
    public abstract State NextState { get; set; }

    protected MessageDialogue(string localizationName) : base(localizationName)
    {
    }

    protected void Redirect(State newState)
    {
        NextState = newState;
    }
}