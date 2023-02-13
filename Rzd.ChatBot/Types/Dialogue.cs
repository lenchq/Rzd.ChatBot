using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Types;

public abstract class Dialogue
{
    private readonly string _localizationName;
    protected IServiceProvider ServiceProvider { get; private set; }
    public abstract InputType InputType { get; }
    public abstract State State { get; }
    //contextual buttons
    
    protected LocalizationWrapper LocalizationWrapper;

    protected Dialogue(string localizationName)
    {
        _localizationName = localizationName;
        // LocalizationWrapper = new LocalizationWrapper(localizationName);
    }
    public virtual string GetText(Context ctx)
        => LocalizationWrapper.GetText();

    public void DependencyInjection(IServiceProvider provider)
    {
        ServiceProvider = provider;
        //TODO: throw error ? 
        var localization = provider.GetService<AppLocalization>();
        LocalizationWrapper = new LocalizationWrapper(localization, _localizationName);
        Initialized();
    }

    protected T GetService<T>()
    {
        if (ServiceProvider.GetService<T>() is {} service)
            return service;
        throw new NullReferenceException("No such service");
    }

    protected virtual void Initialized()
    {
        
    }
}