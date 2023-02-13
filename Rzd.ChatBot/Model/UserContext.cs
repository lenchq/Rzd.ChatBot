using Newtonsoft.Json;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Model;

public record UserContext
{
    /// <summary>
    /// Represents telegram chat id
    /// </summary>
    public long Id { get; init; }

    private int _photoCount = 0;

    public int PhotoCount
    {
        get => _photoCount;
        set => SetField(value, out _photoCount);
    }
    
    private InputType _inputType = InputType.Option;    
    public InputType InputType
    {
        get => _inputType;
        set => SetField(value, out _inputType);
    }
    
    private State _state;
    public State State
    {
        get => _state;
        set => SetField(value, out _state);
    }

    [JsonIgnore]
    public bool Modified { get; set; } = false;
    
    private void SetField<T>(T value, out T field)
    {
        Modified = true;
        field = value;
    }
}