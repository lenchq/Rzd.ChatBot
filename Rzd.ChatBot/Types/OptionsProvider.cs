namespace Rzd.ChatBot.Types;


//TODO: rename
public class OptionsProvider
{
    public static OptionsPreserve Preserve = new OptionsPreserve();
    
    public IEnumerable<IEnumerable<string>>? Options { get; set; }
    public string[][] Colors { get; set; } = null!;
}

public class OptionsPreserve : OptionsProvider
{
}