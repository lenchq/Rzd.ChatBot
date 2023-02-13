using Rzd.ChatBot.Model;
using VkMessage = VkNet.Model.Message;
using TelegramMessage = Telegram.Bot.Types.Message;

namespace Rzd.ChatBot.Types;

public static class ContextFactory
{
    public static Context FromVk(VkMessage msg, UserContext ctx)
    {
        return new Context
        {
            Message = Message.FromVk(msg),
            UserContext = ctx,
        };
    }
    public static Context FromTelegramMessage(TelegramMessage msg, UserContext ctx)
    {
        return new Context
        {
            Message = Message.FromTelegramMessage(msg),
            UserContext = ctx,
        };
    }
}