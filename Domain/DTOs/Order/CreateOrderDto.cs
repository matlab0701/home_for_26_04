namespace Domain.DTOs.Order;

public class CreateOrderDto
{
    public string UserId { get; set; } 
    public int ProductId { get; set; } 
    public int Quantity { get; set; } 
}
