using System.ComponentModel.DataAnnotations;

namespace Codehaks.Pagination.Sample.Data;

/// <summary>A person row, trimmed to just the fields the sample list renders.</summary>
public class User
{
    [Key]
    public int Number { get; set; }

    [Required]
    [StringLength(20)]
    public string Givenname { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Maidenname { get; set; } = string.Empty;

    public int Age { get; set; }
}
