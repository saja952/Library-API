
using System.ComponentModel.DataAnnotations;

namespace Library.Application.DTOs;

public class CreateAuthorRequest
{
    [Required(ErrorMessage = "Author name is required.")]
    [StringLength(100, ErrorMessage = "Author name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;
}
