using System.Reflection;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public sealed class BotDialogues
{
    public Dialogue[] Dialogues { get; }
    public Dictionary<State, Dialogue> StateDialogues { get; private set; }

    private readonly IServiceProvider _provider;
    
    public BotDialogues(IServiceProvider provider)
    {
        _provider = provider;
        Dialogues = GetTypesInNamespace(
                Assembly.GetExecutingAssembly(),
                "Rzd.ChatBot.Dialogues"
            )
            // Exclude this class and its nested types
            .Where(_ => !_.AssemblyQualifiedName!.Contains("BotDialogues"))
            // Instantiate each of Dialogue classes in ChatBot.Dialogues namespace
            .Select(dialogue => (Dialogue) Activator.CreateInstance(dialogue)!)
            .ToArray();
        
        StateDialogues = Dialogues
            .ToDictionary(_ => _.State, _ => _);
        
        InitializeDependencies();
    }

    public Dialogue GetDialogueByState(State state)
    {
        // foreach (var dialogue in Dialogues)
        // {
        //     if (dialogue.State == state)
        //     {
        //         return dialogue;
        //     }
        // }
        var success = StateDialogues.TryGetValue(state, out var dialogue);
        if (!success)
            throw new KeyNotFoundException($"Dialogue with state {state} not found");
        return dialogue!;
    }

    private void InitializeDependencies()
    {
        foreach (var dialogue in Dialogues)
        {
            dialogue.DependencyInjection(_provider);
        }
    }
    
    private Type[] GetTypesInNamespace(Assembly assembly, string @namespace)
    {
        return 
            assembly.GetTypes()
                .Where(t => String.Equals(t.Namespace, @namespace, StringComparison.Ordinal))
                .Where(t => ( t.Attributes & TypeAttributes.NestedPrivate ) != TypeAttributes.NestedPrivate)
                .ToArray();
    }
}