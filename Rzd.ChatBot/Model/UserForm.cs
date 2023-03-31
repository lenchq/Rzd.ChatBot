using System.ComponentModel.DataAnnotations.Schema;
using Rzd.ChatBot.Types.Enums;

namespace Rzd.ChatBot.Model;

public record UserForm
{
    private string? _name;
    private int? _age;
    private string[] _photos = Array.Empty<string>();
    private string? _about;
    private int? _seat;
    private string? _trainNumber;
    private Gender? _gender;
    private bool? _showCoupe;
    private bool? _showContact;
    private bool _disabled = true;
    private bool _fulfilled = false;
    private string? _username;

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
    public string[] Photos 
    {
        get => _photos;
        set => SetField(value, out _photos);
    }
    public string? About 
    {
        get => _about;
        set => SetField(value, out _about);
    }
    public int? Seat 
    {
        get => _seat;
        set => SetField(value, out _seat);
    }
    public string? TrainNumber 
    {
        get => _trainNumber;
        set => SetField(value, out _trainNumber);
    }
    public Gender? Gender 
    {
        get => _gender;
        set => SetField(value, out _gender);
    }
    public bool? ShowCoupe 
    {
        get => _showCoupe;
        set => SetField(value, out _showCoupe);
    }

    public bool? ShowContact
    {
        get => _showContact;
        set => SetField(value, out _showContact);
    }

    public bool Disabled
    {
        get => _disabled;
        set => SetField(value, out _disabled);
    }

    public bool Fulfilled
    {
        get => _fulfilled;
        set => SetField(value, out _fulfilled);
    }

    public string? Username
    {
        get => _username;
        set => SetField(value, out _username);
    }
    
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    // public DateTime LastActive { get; set; }
    
    
    [NotMapped]
    public bool Modified { get; set; }

    // public void UpdateActive()
    // {
    //     LastActive = DateTime.UtcNow;
    // }
    
    private void SetField<T>(T value, out T field)
    {
        Modified = true;
        field = value;
    }
}