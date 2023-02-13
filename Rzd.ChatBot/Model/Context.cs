using Rzd.ChatBot.Types;

namespace Rzd.ChatBot.Model;


public class Context
{
    public UserContext UserContext { get; init; } = null!;
    public Message Message { get; init; } = null!;
    public UserForm UserForm { get; init; } = null!;
}