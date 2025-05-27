using Domain.DTOs;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Domain.DTOs.Product;

namespace WebApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ProductController(IPoductService poductService) : ControllerBase
{
    [HttpPost]
    public async Task<Response<GetProductDto>> CreateAsync(CreateProductDto request)
    {
        return await poductService.CreateAsync(request);
    }
    [HttpDelete]
    public async Task<Response<string>> DeleteAsync(int Id)
    {
        return await poductService.DeleteAsync(Id);
    }

[HttpGet]
    public async Task<Response<List<GetProductDto>>> GetAllAsync([FromQuery] ProductFilter filter)
    {
        return await poductService.GetAllAsync(filter);
    }
    [HttpGet("{Id:int}")]
    public async Task<Response<GetProductDto>> GetByIdAsync(int Id)
    {
        return await poductService.GetByIdAsync(Id);

    }
[HttpPut("{Id:int}")]
    public async Task<Response<GetProductDto>> UpdateAsync(int Id, UpdateProductDto request)
    {
        return await poductService.UpDateAsync(Id, request);
    }
}