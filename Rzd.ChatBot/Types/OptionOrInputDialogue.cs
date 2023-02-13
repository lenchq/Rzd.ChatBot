using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;
using Rzd.ChatBot.Types.Interfaces;

namespace Rzd.ChatBot.Types;

public abstract class OptionOrInputDialogue : ActionDialogue, IInputDialogue
{
    public override InputType InputType => InputType.Text
                                           | InputType.Option;

    protected OptionOrInputDialogue(string localizationName) : base(localizationName)
    {
    }

    public virtual string GetErrorText(Context ctx)
        => LocalizationWrapper.Text("validationError");
    
    public abstract ValueTask<State> ProceedInput(Context ctx);
    public abstract bool Validate(Message msg);
}