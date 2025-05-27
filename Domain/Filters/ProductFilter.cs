namespace Domain.Filters;

public class ProductFilter
{
    public string? Name { get; set; }
     public int? From { get; set; }
    public int? To { get; set; }
    public int? CategoryId { get; set; }
    public int? UserId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
