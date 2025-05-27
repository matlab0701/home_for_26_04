using Domain.Contains;
using Domain.DTOs.User;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{


    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<List<GetUserDto>>> GetAllAsync([FromQuery] UserFilter filter)
    {
        return await userService.GetAllUsersAsync(filter);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetUserDto>> GetUserByIdAsync(string id)
    {
        return await userService.GetUserByIdAsync(id);
    }

    [HttpPost]
    public async Task<Response<GetUserDto>> CreateUserAsync(CreateUserDto request)
    {
        return await userService.CreateUserAsync(request);
    }



}
