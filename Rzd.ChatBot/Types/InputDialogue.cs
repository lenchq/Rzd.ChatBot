using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Types;

public abstract class InputDialogue : Dialogue
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
    public abstract State ProceedInput(Context ctx);
    public abstract bool Validate(Message msg);

    public virtual string GetErrorText(Context ctx)
        => LocalizationWrapper.GetText("validationError");
}