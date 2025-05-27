using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Order
{
    public int Id { get; set; } 
    public string UserId { get; set; } 
    public int ProductId { get; set; } 
    public DateTime OrderDate { get; set; } 
    public int Quantity { get; set; } 
    public Status Status { get; set; }


    public virtual IdentityUser User { get; set; }
    public virtual Product Product { get; set; }
}
