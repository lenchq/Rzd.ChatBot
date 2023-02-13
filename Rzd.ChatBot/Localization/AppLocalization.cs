using NetEscapades.Configuration.Yaml;

namespace Rzd.ChatBot.Localization;

public class AppLocalization
{
    private readonly IConfiguration _localization;
    private const string LocalizationPath = "l10n";

    public AppLocalization(IConfiguration configuration)
    {
        _localization = configuration.GetSection(LocalizationPath);
    }

    public string? this[string key]
    {
        get => GetItem(key);
    }

    public string? GetItem(string key)
    {
        return _localization[key];
    }

    //TODO: rework with GetChildren
    public string[] GetKeys(string key)
    {
        return _localization.GetSection(key)
            .AsEnumerable()
            .Select(x => x.Key)
            .ToArray();
    }

    public string[] GetChildren(string section)
    {
        return _localization.GetSection(section)
            .GetChildren()
            .AsEnumerable()
            .Select(x => x.Key)
            .ToArray();
    }

    public string?[] GetValues(string section)
    {
        return _localization.GetSection(section)
            .GetChildren()
            .AsEnumerable()
            .Select(x => x.Value)
            .ToArray();
    }
}