using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Types.Interfaces;

public interface IMessageDialogue
{
    public State NextState { get; }
}