using Domain.DTOs.User;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IUserService
{
    Task<Response<GetUserDto>> CreateUserAsync(CreateUserDto request);
    Task<Response<List<GetUserDto>>> GetAllUsersAsync(UserFilter filter);
    Task<Response<GetUserDto>> GetUserByIdAsync(string id);
    Task<Response<GetUserDto>> UpdateUserAsync(string id, UpdateUserDto request);
    Task<Response<string>> DeleteUserAsync(string id);
}
