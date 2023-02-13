using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class FormStartingMessage : MessageDialogue
{
    public FormStartingMessage() : base("formStarting")
    {
    }

    public override State State => State.FormStarting;
    public override State NextState { get; init; } = State.EditAge;
}