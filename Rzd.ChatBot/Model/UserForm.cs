using System.ComponentModel.DataAnnotations.Schema;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Model;

public record UserForm
{
    private string? _name;
    private int? _age;
    private string[] _photos = Array.Empty<string>();
    private string? _about;
    private int? _coupe;
    private string? _trainNumber;
    private Gender? _gender;
    private bool? _showCoupe;

    public long Id { get; init; }

    public string? Name
    {
        get => _name;
        set => SetField(value, out _name);
    }

    public int? Age
    {
        get => _age;
        set => SetField(value, out _age);
    }
    public string[] Photos {
        get => _photos;
        set => SetField(value, out _photos);
    }
    public string? About {
        get => _about;
        set => SetField(value, out _about);
    }
    public int? Coupe {
        get => _coupe;
        set => SetField(value, out _coupe);
    }
    public string? TrainNumber {
        get => _trainNumber;
        set => SetField(value, out _trainNumber);
    }
    public Gender? Gender {
        get => _gender;
        set => SetField(value, out _gender);
    }
    public bool? ShowCoupe {
        get => _showCoupe;
        set => SetField(value, out _showCoupe);
    }
    
    
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime LastActive { get; init; }
    
    
    [NotMapped]
    public bool Modified { get; private set; }
    
    private void SetField<T>(T value, out T field)
    {
        Modified = true;
        field = value;
    }
}