using Rzd.ChatBot.Localization;
using Rzd.ChatBot.Model;
using Rzd.ChatBot.Types.Enums;
using File = Telegram.Bot.Types.File;

namespace Rzd.ChatBot.Types;

public abstract class Dialogue
{
    private readonly string _localizationName;
    protected ILogger Logger { get; private set; }
    protected IServiceProvider ServiceProvider { get; private set; }
    public abstract InputType InputType { get; }
    public abstract State State { get; }
    //TODO contextual buttons
    
    protected LocalizationWrapper LocalizationWrapper;

    protected Dialogue(string localizationName)
    {
        _localizationName = localizationName;
    }
    public virtual string GetText(Context ctx)
        => LocalizationWrapper.Text();

    public void DependencyInjection(IServiceProvider provider)
    {
        ServiceProvider = provider;
        //TODO: throw error ? 
        var localization = provider.GetService<AppLocalization>();
        LocalizationWrapper = new LocalizationWrapper(localization, _localizationName);

        var logger = provider.GetService<ILogger<Dialogue>>();
        Logger = logger;
        Initialized();
    }

    protected T GetService<T>()
    {
        if (ServiceProvider.GetService<T>() is {} service)
            return service;
        throw new NullReferenceException("No such service");
    }

    protected object GetService(Type t)
    {
        if (ServiceProvider.GetService(t) is { } service)
            return service;
        throw new NullReferenceException("No such service");
    }

    public virtual ValueTask PostInitializeAsync(Context ctx)
    {
        return ValueTask.CompletedTask;
    }

    public virtual Photo[]? GetPhotos()
    {
        // if is null
        if (LocalizationWrapper.GetInnerPic(_localizationName) is not { } picPath)
            return null;
        
        var innerPicPath = picPath
            .Replace('/', Path.DirectorySeparatorChar);

        var fullPath = Path.GetFullPath(innerPicPath);
        if (!System.IO.File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Picture for {_localizationName} not found", innerPicPath);
        }

        var pic = System.IO.File.OpenRead(fullPath);
        return new Photo[]
        {
            new PhotoData
            {
                // заглушка
                FileId = string.Empty,

                FileName = Path.GetFileName(fullPath),
                FileData = pic,
            }
        };
    }

    public virtual InlineButtonsProvider? GetInlineButtons()
    {
        return null;
    }

    protected virtual void Initialized()
    {
    }
}