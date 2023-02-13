using System.Reflection;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Attributes;
using Rzd.ChatBot.Types.Enums;
using Rzd.ChatBot.Types.Interfaces;

namespace Rzd.ChatBot.Types;

public abstract class ActionDialogue : Dialogue, IActionDialogue
{
    public override InputType InputType => InputType.Option;
    public virtual bool FormattedOptions { get; } = false;

    public virtual Dictionary<string, BotAction> Actions
    {
        get
        {
            return new Dictionary<string, BotAction>(
                Options.Select(
                    (action, i) =>
                    {
                        // var getOption = FormattedOptions
                        //     ? LocalizationWrapper.Option
                        //     : new Func<int, string?>(index => LocalizationWrapper.FormattedText(LocalizationWrapper.Option(index), 
                        //         GetOptionFormattingArguments(index)));
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

    public  virtual IEnumerable<BotAction> Options { get; set; }
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
        // Actions = new Dictionary<string, BotAction>(
        //     Options.Select(
        //         (action, i) =>
        //         {
        //             if (
        //                 action.Method.GetCustomAttributes(typeof(OptionIndexAttribute))
        //                     .SingleOrDefault() is OptionIndexAttribute attr)
        //             {
        //                 return new KeyValuePair<string, BotAction>(LocalizationWrapper.Option(attr.Index), action);
        //             }
        //
        //             return new KeyValuePair<string, BotAction>(LocalizationWrapper.Option(i), action);
        //         }
        //     )
        // );
    }

    public virtual void WrongAnswer(Context ctx)
    {
        
    }

    public virtual string WrongAnswerText(Context ctx)
        => LocalizationWrapper.Text("wrongOption", false) ?? LocalizationWrapper.Raw("wrongOption")!;
    
    public OptionsProvider GetOptions()
    {
        // return new OptionsProvider
        // {
        //     Options = LocalizationWrapper.Options(),
        //     Colors = LocalizationWrapper.Colors(),
        // };
        if (Options is null)
            throw new ArgumentNullException(nameof(Options),
                "Options must be initialized in class constructor");
        
        var includedIndexes = new List<int>();
        var methods = Options
            .Select(botAction =>
                (
                    Method: botAction,
                    MethodInfo: botAction.Method,
                    Attributes: botAction.Method.GetCustomAttributes(typeof(OptionIndexAttribute), false)
                )
            );

        foreach (var action in methods)
        {
            if (Options.Any(x => x == action.Method) &&
                action.Attributes
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