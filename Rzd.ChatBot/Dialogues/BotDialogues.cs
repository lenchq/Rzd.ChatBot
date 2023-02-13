using System.Reflection;
using Rzd.ChatBot.Types;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Dialogues;

public sealed class BotDialogues
{
    public Dialogue[] Dialogues { get; }
    public Dictionary<State, Dialogue> StateDialogues { get; }

    private readonly IServiceProvider _provider;
    
    public BotDialogues(IServiceProvider provider)
    {
        _provider = provider;
        Dialogues = GetTypesInNamespace(
                Assembly.GetExecutingAssembly(),
                "Rzd.ChatBot.Dialogues"
            )
            // Exclude this class and its nested types
            .Where(type => !type.AssemblyQualifiedName!.Contains("BotDialogues"))
            // Instantiate each of Dialogue classes in ChatBot.Dialogues namespace
            .Select(dialogue => (Dialogue) Activator.CreateInstance(dialogue)!)
            .ToArray();
        
        StateDialogues = Dialogues
            .ToDictionary(key => key.State, val => val);
        
        InitializeDependencies();
    }

    public Dialogue DialogueByState(State state)
    {
        var success = StateDialogues.TryGetValue(state, out var dialogue);
        if (!success)
            throw new KeyNotFoundException($"Dialogue with state {state} not found");
        return dialogue!;
    }
    public Dialogue NewDialogueByState(State state)
    {
        var success = StateDialogues.TryGetValue(state, out var dialogue);
        if (!success)
            throw new KeyNotFoundException($"Dialogue with state {state} not found");
        var type = dialogue!.GetType();
        var newDialogue = (Dialogue) Activator.CreateInstance(type)!;
        newDialogue.DependencyInjection(_provider);
        
        return newDialogue;
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
                .Where(type => string.Equals(type.Namespace, @namespace, StringComparison.Ordinal))
                // Exclude nested types
                .Where(type => !type.Attributes.HasFlag(TypeAttributes.NestedPrivate))
                .ToArray();
    }
}