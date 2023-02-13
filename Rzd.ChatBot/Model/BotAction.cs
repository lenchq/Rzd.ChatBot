using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Model;

public delegate ValueTask<State> BotAction(Context ctx);