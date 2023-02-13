using System.Reflection;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;
using Rzd.ChatBot.Types.Interfaces;

namespace Rzd.ChatBot.Types;

public abstract class PhotoOrActionDialogue : PhotoDialogue, IActionDialogue
{
    public override InputType InputType => InputType.Photo
                                           | InputType.Option;

    public PhotoOrActionDialogue(string localizationName) : base(localizationName)
    {
    }

    public abstract IEnumerable<BotAction> Options { get; set; }

    public virtual Dictionary<string, BotAction> Actions
    {
        get
        {
            return new Dictionary<string, BotAction>(
                Options.Select(
                    (action, i) =>
                    {
                        if (
                            action.Method.GetCustomAttributes(typeof(OptionIndexAttribute))
                                .SingleOrDefault() is OptionIndexAttribute attr)
                        {
                            return new KeyValuePair<string, BotAction>(LocalizationWrapper.Option(attr.Index), action);
                        }

                        return new KeyValuePair<string, BotAction>(LocalizationWrapper.Option(i), action);
                    }
                )
            );
        }
    }

    public void WrongAnswer(Context ctx)
    {
    }

    public virtual string WrongAnswerText(Context ctx)
        => LocalizationWrapper.Text("wrongOption", false) ?? LocalizationWrapper.Raw("wrongOption")!;

    public OptionsProvider GetOptions()
    {
        if (Options is null)
            throw new ArgumentNullException(nameof(Options),
                "Options must be initialized in class constructor");
        
        var includedIndexes = new List<int>();
        var methods = Options
            .Select(botAction =>
                (
                    MethodInfo: botAction.Method,
                    Attributes: botAction.Method.GetCustomAttributes(typeof(OptionIndexAttribute), false)
                )
            );

        foreach (var action in methods)
        {
            if (action.Attributes
                    .SingleOrDefault(attr => attr is OptionIndexAttribute)
                is OptionIndexAttribute optionIndexAttr)
            {
                includedIndexes.Add(optionIndexAttr.Index);
            }
            else
            {
                Logger.LogWarning("{Name}.{MethodInfo.Name} Do not has the OptionIndex attribute", 
                    GetType().Name,
                    action.MethodInfo.Name);
            }
        }

        return new OptionsProvider
        {
            //todo kostyl, return only options present in Dialogue.Options
            Options = LocalizationWrapper.Options(includedIndexes.ToArray()),
            Colors = LocalizationWrapper.Colors(),
        };
    }
}