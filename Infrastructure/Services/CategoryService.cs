using System.Net;
using AutoMapper;
using Domain.DTOs.Category;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class CategoryService(IBaseRepository<Category, int> repository, IMapper mapper, ILogger<CategoryService> logger, IMemoryCacheService memoryCacheService, IRedisCacheService redisCacheService) : ICategoryService
{
    public async Task<Response<GetCategoryDto>> CraeteAsync(CreateCategoryDto request)
    {
        var cat = mapper.Map<Category>(request);
        var result = await repository.AddAsync(cat);
        if (result == 0)
        {
            logger.LogWarning($"can`t to added category{cat} category");
            return new Response<GetCategoryDto>(HttpStatusCode.NotFound, "Id is not found");
        }
        var data = mapper.Map<GetCategoryDto>(cat);
        // await memoryCacheService.DeleteData("categories");
        await redisCacheService.RemoveData("categories");
        return new Response<GetCategoryDto>(data);
    }

    public async Task<Response<string>> DeleteAsync(int Id)
    {
        var category = await repository.GetByAsync(Id);
        if (category == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Id is not Found");
        }
        var result = await repository.DeleteAsync(category);
        if (result == 0)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Category not deleted!");
        }
        // await memoryCacheService.DeleteData("categories");
        await redisCacheService.RemoveData("categories");
        return new Response<string>("Category delete to succes!");

    }

    public async Task<Response<List<GetCategoryDto>>> GetAllAsync(CategoryFilter filter)
    {
        const string cacheKey = "categories";

        // var category = await memoryCacheService.GetData<List<GetCategoryDto>>(cacheKey);
        var category = await redisCacheService.GetData<List<GetCategoryDto>>(cacheKey);

        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);

        if (category == null)
        {
            var categories = await repository.GetAll();
            category = categories.Select(c => new GetCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToList();

            await memoryCacheService.SetData(cacheKey, category, 1);
        }
        if (filter.Name != null)
        {
            category = category.Where(c => c.Name.ToLower().Contains(filter.Name.ToLower())).ToList();
        }
        if (filter.From != null)
        {
            category = category.Where(c => c.CreatedAt >= filter.From).ToList();
        }
        if (filter.To != null)
        {
            category = category.Where(c => c.CreatedAt <= filter.To).ToList();
        }

        var mapped = mapper.Map<List<GetCategoryDto>>(category);
        var totalRecords = mapped.Count;
        var data = mapped
        .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        .Take(validFilter.PageSize)
        .ToList();
        return new PagedResponse<List<GetCategoryDto>>(data, validFilter.PageNumber, validFilter.PageSize, totalRecords);


    }

    public async Task<Response<GetCategoryDto>> GetByIdAsync(int Id)
    {
        var res = await repository.GetByAsync(Id);
        if (res == null)
        {
            return new Response<GetCategoryDto>(HttpStatusCode.NotFound, "Id is not found");
        }
        var data = mapper.Map<GetCategoryDto>(res);
        // await memoryCacheService.DeleteData("categories"); 
        await redisCacheService.RemoveData("categories");

        return new Response<GetCategoryDto>(data);

    }

    public async Task<Response<GetCategoryDto>> UpdateAsync(int Id, UpdateCategoryDto request)
    {
        var category = await repository.GetByAsync(Id);
        if (category == null)
        {
            return new Response<GetCategoryDto>(HttpStatusCode.NotFound, "Id is not Found");
        }


        category.Name = request.Name;
        category.CreatedAt = request.CreatedAt;
        category.Description = request.Description;

        var result = await repository.UpdateAsync(category);
        if (result == 0)
        {
            logger.LogWarning($"Не удалось обновить category: {category}", category);
            return new Response<GetCategoryDto>(HttpStatusCode.BadRequest, "category not updated");
        }
        var data = mapper.Map<GetCategoryDto>(category);
        // await memoryCacheService.DeleteData("categories"); 
        await redisCacheService.RemoveData("categories");
        return new Response<GetCategoryDto>(data);


    }

}
