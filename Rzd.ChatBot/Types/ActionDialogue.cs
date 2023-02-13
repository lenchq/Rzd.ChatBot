using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Types;

public abstract class ActionDialogue : Dialogue
{
    public override InputType InputType { get;} = InputType.Option;
    public virtual Dictionary<string, BotAction> Actions { get; private set; }
    
    public virtual IEnumerable<BotAction> Options { get; set; }
    protected ActionDialogue(string localizationName) : base(localizationName)
    {
        
    }

    protected override void Initialized()
    {
        if (Options is null)
        {
            throw new Exception("You need to define dialogue options in class constructor");
        }
        // i-th localization string maps to i-th element of Options collection
        Actions = new Dictionary<string, BotAction>(
            Options.Select(
                (x,i) => new KeyValuePair<string, BotAction>(LocalizationWrapper[i], x)
            )
        );
    }

    public virtual void WrongAnswer(Context ctx)
    {
        
    }

    public virtual string WrongAnswerText(Context ctx)
        => LocalizationWrapper.GetText("wrongOption", false) ?? LocalizationWrapper.GetRaw("wrongOption")!;

    // public IEnumerable<IEnumerable<string>> GetOptions()
    // {
    //     return LocalizationWrapper.GetOptions();
    // }
    public OptionsProvider GetOptions()
    {
        return new OptionsProvider
        {
            Options = LocalizationWrapper.GetOptions(),
            Colors = LocalizationWrapper.GetColors(),
        };
    }
}