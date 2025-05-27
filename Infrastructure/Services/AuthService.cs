
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Domain.Contains;
using Domain.DTOs.Auth;
using Domain.DTOs.Email;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;


namespace Infrastructure.Services;

public class AuthService(UserManager<IdentityUser> userManager,
 RoleManager<IdentityRole> roleManager, IMapper mapper, IConfiguration config, IEmailService emailService, DataContext context, ILogger<AuthService> logger) : IAuthService
{
    public async Task<Response<string>> RegisterAsync(RegisterDto registerDto)
    {
        try
        {
            var user = new IdentityUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
            };

            var result = await userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return new Response<string>(HttpStatusCode.InternalServerError, "Failed to create user");
            }

            await userManager.AddToRoleAsync(user, Roles.User);
            return new Response<string>("User created");
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Response<string>> RequestResetPassword(RequestResetPassword requestResetPassword)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(requestResetPassword.Email);
            if (user == null)
            {
                return new Response<string>(HttpStatusCode.NotFound, "User not found");
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var emailDto = new EmailDto()
            {
                To = requestResetPassword.Email,
                Subject = "Reset Password",
                Body = $"Your token is {token}",
            };

            var result = await emailService.SendEmailAsync(emailDto);

            return !result
                ? new Response<string>(HttpStatusCode.InternalServerError, "Failed to send email")
                : new Response<string>("Token sent successfully to email");
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Response<TokenDto>> LoginAsync(LoginDto loginDto)
    {
        var user = await userManager.FindByNameAsync(loginDto.UserName);
        if (user == null || !await userManager.CheckPasswordAsync(user, loginDto.Password))
            return new Response<TokenDto>(HttpStatusCode.BadRequest, "Username or password is incorrect");

        var token = await GenerateJwt(user);
        return new Response<TokenDto>(new TokenDto { Token = token });
    }

    private async Task<string> GenerateJwt(IdentityUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? "")
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    public async Task<Response<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var resetResult =
            await userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

        return resetResult.Succeeded
            ? new Response<string>("Password reset successfully")
            : new Response<string>(HttpStatusCode.InternalServerError, "Failed to reset password");
    }
    // public async Task<Response<string>> RequestResetPassword(RequestResetPassword requestResetPassword)
    // {
    //     var user = await userManager.FindByEmailAsync(requestResetPassword.Email);
    //     if (user == null)
    //     {
    //         return new Response<string>(HttpStatusCode.NotFound, "User not found");
    //     }

    //     var token = await userManager.GeneratePasswordResetTokenAsync(user);

    //     var emailDto = new EmailDto()
    //     {
    //         To = requestResetPassword.Email,
    //         Subject = "Reset Password",
    //         Body = $"Your token is {token}",
    //     };

    //     var result = await emailService.SendEmailAsync(emailDto);

    //     return !result
    //         ? new Response<string>(HttpStatusCode.InternalServerError, "Failed to send email")
    //         : new Response<string>("Token sent successfully to email");
    // }


    // public async Task<Response<string>> ChangeUserRoleAsync(ChangeRoleDto dto)
    // {
    //     var user = await userManager.FindByEmailAsync(dto.Email);
    //     if (user == null)
    //         return new Response<string>(HttpStatusCode.NotFound, "User not found");

    //     var currentRoles = await userManager.GetRolesAsync(user);
    //     var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);

    //     if (!removeResult.Succeeded)
    //         return new Response<string>(HttpStatusCode.InternalServerError, "Failed to remove existing roles");

    //     var addResult = await userManager.AddToRoleAsync(user, dto.NewRole);
    //     if (!addResult.Succeeded)
    //         return new Response<string>(HttpStatusCode.InternalServerError, "Failed to add new role");

    //     return new Response<string>("User role updated successfully");
    // }

}