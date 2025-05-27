using Domain.DTOs;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Domain.DTOs.Order;

namespace WebApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    public async Task<Response<GetOrderDto>> CreateAsync(CreateOrderDto request)
    {
        return await orderService.CreateAsync(request);
    }
    [HttpDelete]
    public async Task<Response<string>> DeleteAsync(int Id)
    {
        return await orderService.DeleteAsync(Id);
    }

[HttpGet]
    public async Task<Response<List<GetOrderDto>>> GetAllAsync([FromQuery] OrderFilter filter)
    {
        return await orderService.GetAllAsync(filter);
    }
    [HttpGet("{Id:int}")]
    public async Task<Response<GetOrderDto>> GetByIdAsync(int Id)
    {
        return await orderService.GetByIdAsync(Id);

    }
[HttpPut("{Id:int}")]
    public async Task<Response<GetOrderDto>> UpdateAsync(int Id, UpdateOrderDto request)
    {
        return await orderService.UpDateAsync(Id, request);
    }
}