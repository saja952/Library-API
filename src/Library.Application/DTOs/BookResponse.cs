namespace Library.Application.DTOs;

public class BookResponse
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Isbn { get; set; } = string.Empty;

    public int PublishedYear { get; set; }

    public int AuthorId { get; set; }

    public string AuthorName { get; set; } = string.Empty;
}
