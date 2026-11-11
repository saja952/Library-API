
using System.ComponentModel.DataAnnotations;

namespace Library.Application.DTOs;

public class CreateBookRequest
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{13}$",
        ErrorMessage = "ISBN must contain exactly 13 digits.")]
    public string Isbn { get; set; } = string.Empty;

    [Range(1450, 9999)]
    public int PublishedYear { get; set; }

    [Range(1, int.MaxValue)]
    public int AuthorId { get; set; }
}
