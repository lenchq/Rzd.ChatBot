using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Types;

public abstract class MessageDialogue : Dialogue
{
    public override InputType InputType { get; } = InputType.None;
    public abstract State NextState { get; init; }

    protected MessageDialogue(string localizationName) : base(localizationName)
    {
    }
}