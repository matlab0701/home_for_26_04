using Domain.DTOs.Category;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [HttpPost]
    public async Task<Response<GetCategoryDto>> CreateAsync(CreateCategoryDto request)
    {
        return await categoryService.CraeteAsync(request);
    }
    [HttpDelete("{Id:int}")]
    public async Task<Response<string>> DeleteAsync(int Id)
    {
        return await categoryService.DeleteAsync(Id);
    }

    [HttpGet]
    public async Task<Response<List<GetCategoryDto>>> GetAllAsync([FromQuery] CategoryFilter filter)
    {
        return await categoryService.GetAllAsync(filter);
    }
    [HttpGet("{Id:int}")]
    public async Task<Response<GetCategoryDto>> GetByIdAsync(int Id)
    {
        return await categoryService.GetByIdAsync(Id);

    }
    [HttpPut("{Id:int}")]
    public async Task<Response<GetCategoryDto>> UpdateAsync(int Id, UpdateCategoryDto request)
    {
        return await categoryService.UpdateAsync(Id, request);
    }
}
