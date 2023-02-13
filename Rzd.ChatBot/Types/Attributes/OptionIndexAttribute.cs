namespace Rzd.ChatBot.Types.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class OptionIndexAttribute : Attribute
{
    public int Index { get; }

    public OptionIndexAttribute(int index) => (Index) = (index);
}