namespace Rzd.ChatBot.Types.Enums;

[Flags]
public enum InputType
{
    None = 0,
    Text = 1,
    Photo = 1 << 1,
    Option = 1 << 2,
}