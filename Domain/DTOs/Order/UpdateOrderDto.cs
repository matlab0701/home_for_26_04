using Domain.Enums;

namespace Domain.DTOs.Order;

public class UpdateOrderDto
{
      public int Quantity { get; set; } 
    public Status Status { get; set; }
}
