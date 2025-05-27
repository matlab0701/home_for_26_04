namespace Domain.DTOs.Product;

public class CreateProductDto
{
      public string Name { get; set; } 
    public string Description { get; set; } 
    public decimal Price { get; set; } 
    public int CategoryId { get; set; } 
    public int UserId { get; set; } 
    public bool IsTop { get; set; } 
    public bool IsPremium { get; set; } 
    public DateTime? PremiumOrTopExpiryDate { get; set; } 
    public string ImageUrl { get; set; } 
    public int StockQuantity { get; set; } 
}
