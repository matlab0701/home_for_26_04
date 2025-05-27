using System.Net;
using AutoMapper;
using Domain.Contains;
using Domain.DTOs.User;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Service;

public class UserService(UserManager<IdentityUser> userManager,
 RoleManager<IdentityRole> roleManager, IMapper mapper, IConfiguration configuration) : IUserService
{

    public async Task<Response<GetUserDto>> CreateUserAsync(CreateUserDto request)
    {
        var user = new IdentityUser
        {
            Email = request.Email,
            UserName = request.Email,

        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (result == null)
        {
            return new Response<GetUserDto>(HttpStatusCode.BadRequest, "User not Created");
        }
        await userManager.AddToRoleAsync(user, Roles.User);

        var data = mapper.Map<GetUserDto>(user);
        return new Response<GetUserDto>(data);
    }

    public async Task<Response<string>> DeleteUserAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Id in not found");
        }
        var result = await userManager.DeleteAsync(user);

        return result == null ?
        new Response<string>(HttpStatusCode.BadRequest, "User is not delete")
        : new Response<string>("User deleted successed!");
    }

    public async Task<Response<List<GetUserDto>>> GetAllUsersAsync(UserFilter filter)
    {
        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);
        var user = userManager.Users.AsQueryable();
        if (filter.Email != null)
        {
            user = user.Where(u => u.Email.ToLower().Contains(filter.Email.ToLower()));
        }
        var mapped = mapper.Map<List<GetUserDto>>(await user.ToListAsync());
        var totalRecords = mapped.Count;
        var data = mapped
        .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        .Take(validFilter.PageSize)
        .ToList();
        return new PagedResponse<List<GetUserDto>>(data, validFilter.PageNumber, validFilter.PageSize, totalRecords);
    }


    public async Task<Response<GetUserDto>> GetUserByIdAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return new Response<GetUserDto>(HttpStatusCode.NotFound, "Id in not found");
        }
        var data = mapper.Map<GetUserDto>(user);
        return new Response<GetUserDto>(data);
    }
    public async Task<Response<GetUserDto>> UpdateUserAsync(string id, UpdateUserDto request)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            return new Response<GetUserDto>(HttpStatusCode.NotFound, "Id in not found");
        }
        user.Email = request.Email;
        user.UserName = request.Email;
        var result = await userManager.UpdateAsync(user);
        var data = mapper.Map<GetUserDto>(user);
        return result == null ?
        new Response<GetUserDto>(HttpStatusCode.BadRequest, "User not Updated!")
        : new Response<GetUserDto>(data);
    }


}
