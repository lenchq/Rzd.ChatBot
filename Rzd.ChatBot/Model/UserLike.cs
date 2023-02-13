using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rzd.ChatBot.Model;

public class UserLike
{
    [Key,
     DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    [ForeignKey(nameof(From))]
    public long FromId { get; set; }
    
    [ForeignKey(nameof(To))]
    public long ToId { get; set; }
    
    public UserForm From { get; set; } 
    public UserForm To { get; set; }
    
    public bool Like { get; set; }
}