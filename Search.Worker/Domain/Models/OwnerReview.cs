namespace Search.Worker.Domain.Models;

public class OwnerReview(string title, int rating)
{
    public string Title { get; set; } = title;
    public int Rating { get; set; } = rating;
}
