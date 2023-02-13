namespace Rzd.ChatBot.Extensions;

public static class ArrayExtensions
{
    public static T[] SingleArray<T>(this T singleElement) => new T[] { singleElement };
}