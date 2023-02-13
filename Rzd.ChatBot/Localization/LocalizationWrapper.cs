namespace Rzd.ChatBot.Localization;

public class LocalizationWrapper
{
    private readonly string _prefix;
    private readonly AppLocalization _localization;
    private readonly string[] _flattenOptions;
    private readonly string[] _flattenColors;
    

    public LocalizationWrapper(AppLocalization localization, string prefixName = "")
    {
        _prefix = prefixName;
        _localization = localization;
        _flattenOptions = GetFlattenOptions();
        _flattenColors = GetFlattenColors();
    }

    private string[]? GetFlattenColors()
    {
        var values = new List<string>();
        var keys = _localization.GetChildren($"{_prefix}:vk_colors");
        
        foreach (var _ in keys.Select(int.Parse))
        {
            values.AddRange(_localization.GetValues($"{_prefix}:vk_colors"));
        }

        return values.ToArray();
    }

    public string? this[int index] => GetOption(index);

    public string? GetText(string key = "caption", bool throwIfNull = true)
    {
        var text = _localization[$"{_prefix}:{key}"];
        if (throwIfNull && text is null)
            throw new KeyNotFoundException($"No locale string with path \"{_prefix}:{key}\" found");
        return text;
    }

    //TODO: rework with GetChildren
    private string[] GetFlattenOptions()
    {
        var values = new List<string>();
        // because first element is root key
        var keys = _localization.GetKeys($"{_prefix}:options").Select(x => x[5..]).ToArray();
        foreach (var key in keys)
        {
            var value = _localization.GetItem(key);
            if (value is not null)
            {
                values.Insert(0, value);
            }
        }

        return values.ToArray();
    }

    public string? GetRaw(string key)
    {
        return _localization[key];
    }

    public IEnumerable<IEnumerable<string>> GetOptions()
    {
        var values = new List<List<string>>();
        var keys = _localization.GetChildren($"{_prefix}:options");
        
        for (var i = 0; i < keys.Length; i++)
        {
            values.Add(new List<string>());
        }
        foreach (var i in keys.Select(int.Parse))
        {
            values[i].AddRange(_localization.GetValues($"{_prefix}:options:{i}"));
        }
        
        return values.Select(x => x.ToArray()).ToArray();
    }

    public string[][] GetColors()
    {
        var values = new List<List<string>>();
        var keys = _localization.GetChildren($"{_prefix}:vk_colors");

        for (int i = 0; i < keys.Length; i++)
        {
            values.Add(new List<string>());
        }
        
        foreach (var i in keys.Select(int.Parse))
        {
            values[i].AddRange(_localization.GetValues($"{_prefix}:vk_colors:{i}"));
        }

        return values.Select(x => x.ToArray()).ToArray();
    }

    public string? GetOption(int index)
    {
        return _flattenOptions[index];
        //return _localization[$"{_prefix}:options:{index}"];
    }
    
}