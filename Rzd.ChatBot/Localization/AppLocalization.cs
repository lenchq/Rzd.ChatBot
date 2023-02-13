using NetEscapades.Configuration.Yaml;

namespace Rzd.ChatBot.Localization;

public class AppLocalization
{
    private readonly IConfiguration _localization;
    private readonly IConfiguration _pics;
    private const string LocalizationPath = "l10n";
    private const string PicsPath = "pics";

    public AppLocalization(IConfiguration configuration)
    {
        _localization = configuration.GetSection(LocalizationPath);
        _pics = configuration.GetSection(PicsPath);
    }

    public string? this[string key] => GetItem(key);


    public string? GetPicPath(string key)
    {
        return _pics[key]; // ?? throw new NullReferenceException($"Picture reference with name {key} not found");
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