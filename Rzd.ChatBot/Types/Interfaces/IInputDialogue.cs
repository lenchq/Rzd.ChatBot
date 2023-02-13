using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Types.Interfaces;

public interface IInputDialogue
{
    public ValueTask<State> ProceedInput(Context ctx);
    public bool Validate(Message msg);
    public string GetErrorText(Context ctx);
}