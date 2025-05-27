using Domain.DTOs.Auth;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IAuthService
{
        // Task<Response<string>> ChangeUserRoleAsync(ChangeRoleDto dto);
    Task<Response<string>> RegisterAsync(RegisterDto registerDto);
    Task<Response<TokenDto>> LoginAsync(LoginDto loginDto);
    Task<Response<string>> RequestResetPassword(RequestResetPassword requestResetPassword);
    Task<Response<string>> ResetPassword(ResetPasswordDto resetPasswordDto);
}
