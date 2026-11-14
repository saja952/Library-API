
namespace Library.Application.DTOs;

public class UpdateBookRequest
{
    public string Title { get; set; } = string.Empty;

    public string Isbn { get; set; } = string.Empty;

    public int PublishedYear { get; set; }

    public int AuthorId { get; set; }
}
