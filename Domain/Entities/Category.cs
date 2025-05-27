namespace Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    
     public virtual List<Product> Products { get; set; } 
}
