namespace Domain.DTOs.User;

public class UpdateUserDto : CreateUserDto
{
    public DateTime UpdatedAt { get; set; }
}
