using System.ComponentModel.DataAnnotations.Schema;
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

    private long? _currentForm;
    public long? CurrentForm
    {
        get => _currentForm;
        set => SetField(value, out _currentForm);
    }

    private Queue<long> _likeQueue = new Queue<long>();
    private int? _cachedQueueCount;

    public Queue<long> LikeQueue
    {
        get => _likeQueue;
        set
        {
            if (value is not null)
            {
                _cachedQueueCount ??= value.Count;
                SetField(value, out _likeQueue);
            }
        }
    }

    private string? _startData;

    public string? StartData
    {
        get => _startData;
        set => SetField(value, out _startData);
    }

    private dynamic? _customDataHolder;
    public dynamic? CustomDataHolder
    {
        get => _customDataHolder;
        set => SetField(value, out _customDataHolder);
    }

    private bool _isAdmin;
    public bool IsAdmin
    {
        get => _isAdmin;
        set => SetField(value, out _isAdmin);
    }


    private bool _modified;

    [JsonIgnore]
    [NotMapped]
    public bool Modified
    {
        get => EnsureModified() || _modified;
        set => _modified = value;
    }

    private bool EnsureModified()
    {
        if (LikeQueue is null || _cachedQueueCount is null)
            return false;
        return LikeQueue.Count != _cachedQueueCount;
    }

    private void SetField<T>(T value, out T field)
    {
        Modified = true;
        field = value;
    }
}