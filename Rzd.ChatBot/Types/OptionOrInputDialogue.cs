using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Types;

public abstract class OptionOrInputDialogue : ActionDialogue
{
    public override InputType InputType => InputType.OptionOrText;

    protected OptionOrInputDialogue(string localizationName) : base(localizationName)
    {
    }

    public virtual string GetErrorText(Context ctx)
        => LocalizationWrapper.GetText("validationError");
    
    public abstract State ProceedInput(Context ctx);
    public abstract bool Validate(Message msg);
}