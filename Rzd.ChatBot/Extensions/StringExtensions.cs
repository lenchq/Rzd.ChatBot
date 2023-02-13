using Telegram.Bot.Types;

namespace Rzd.ChatBot.Extensions;

public static class StringExtensions
{
    public static string Capitalize(this string str)
    {
        if (str.Length == 0)
            //throw new ArgumentException("String length is 0", nameof(str));
            return string.Empty;
        var o = char.ToUpper(str[0]);
        return o + str.ToLower().Substring(1);
    }
}