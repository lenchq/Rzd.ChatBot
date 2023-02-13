using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;
using Rzd.ChatBot.Types.Interfaces;

namespace Rzd.ChatBot.Types;

public abstract class InputDialogue : Dialogue, IInputDialogue
{
    public override InputType InputType => InputType.Text;

    protected InputDialogue(string localizationName) : base(localizationName)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns>Next state</returns>
    public abstract ValueTask<State> ProceedInput(Context ctx);
    
    // TODO replace bool returning with object ValidationResult containing bool ValidationResult and other data (e.g. type of validation failure) 
    // Same with States in ProceedInput, better to replace them with ExecutionResult containing NextState and other data for extensibility 
    public abstract bool Validate(Message msg);

    public virtual string GetErrorText(Context ctx)
        => LocalizationWrapper.Text("validationError");
}