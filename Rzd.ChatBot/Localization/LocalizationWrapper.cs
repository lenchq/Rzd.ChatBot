using SmartFormat;

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
        _flattenOptions = FlattenOptions();
        _flattenColors = FlattenColors();
    }

    public string? GetInnerPic(string key) => _localization.GetPicPath(key);

    private string[]? FlattenColors()
    {
        var values = new List<string>();
        var keys = _localization.GetChildren($"{_prefix}:vk_colors");
        
        foreach (var _ in keys.Select(int.Parse))
        {
            values.AddRange(_localization.GetValues($"{_prefix}:vk_colors"));
        }

        return values.ToArray();
    }

    public string? this[int index] => Option(index);
    
    public string RawFormattedText(string key = "caption", params object?[] args)
    {
        var text = _localization[key];
        return Smart.Format(text, args);
    }

    public string FormattedText(string key = "caption", params object?[] args)
    {
        var text = _localization[$"{_prefix}:{key}"];
        return Smart.Format(text, args);
    }

    public string Text(string key = "caption", bool throwIfNull = true)
    {
        var text = _localization[$"{_prefix}:{key}"];
        if (throwIfNull && text is null)
            throw new KeyNotFoundException($"No locale string with path \"{_prefix}:{key}\" found");
        return text!;
    }

    //TODO: rework with GetChildren
    private string[] FlattenOptions()
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

    public string? Raw(string key)
    {
        return _localization[key];
    }

    public string[][] Options()
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
    
    public string[][] Options(int[] indexes)
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

        // from values select only these that have indexes in
        // "indexes" argument
        var indexesValues = values
            .SelectMany(x => x)
            .Select((x, i) => (i, x))
            .Where(x => indexes.Contains(x.i))
            .Select(x => x.x);


        return values
            .Select(x => x.Intersect(indexesValues).ToArray())
            .ToArray();

        // return values
        //     // Select only values from indexesValues
        //     .Where(x => x.Intersect(indexesValues).Any())
        //     // convert to string[][]
        //     .Select(_ => _.ToArray())
        //     .ToArray();
    }

    public string[][] Colors()
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

    public string Option(int index)
    {
        if (_flattenOptions.Length <= index)
            throw new Exception($" \"{_prefix}\" locale have less options than requested ({index + 1})");
        return _flattenOptions[index];
        //return _localization[$"{_prefix}:options:{index}"];
    }

    public IEnumerable<IEnumerable<(string Key, string Text)>>? GetInlines()
    {
        var itemsCount = _localization.GetChildren($"{_prefix}:inlines").Length;
        if (itemsCount == 0) return null;
        var buttons = Enumerable.Range(0, itemsCount)
            .Select(index =>
            {
                var key = _localization.GetChildren($"match:inlines:{index}")[0];
                return (key, _localization[$"match:inlines:{index}:{key}"]);
            });

        return new[]
        {
            buttons,
        };
    }
}