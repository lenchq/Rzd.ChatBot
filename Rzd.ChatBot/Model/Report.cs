using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rzd.ChatBot.Model;

public class Report
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    [Required]
    public long From { get; set; }
    [Required]
    public long Target { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime Created { get; set; }
    public ReportType Type { get; set; }
    public string? Reason { get; set; }
}

public enum ReportType
{
    Pornography,
    Reseller,
    WontAnswer,
    Other,
}