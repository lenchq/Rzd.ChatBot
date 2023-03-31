using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public class FormStartingMessage : MessageDialogue
{
    public override State State => State.FormStarting;
    public override State NextState(Context ctx) => State.AgeInput;
    public FormStartingMessage() : base("formStarting")
    {
    }
}