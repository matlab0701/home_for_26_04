using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Product
{
    public int Id { get; set; } 
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

    
    public virtual Category Category { get; set; }
    public virtual IdentityUser User { get; set; }
    public virtual List<Order> Order { get; set; }
}
