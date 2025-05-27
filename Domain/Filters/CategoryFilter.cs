namespace Domain.Filters;

public class CategoryFilter
{
    public string? Name { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
