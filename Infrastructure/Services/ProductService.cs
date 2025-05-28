using System.Net;
using AutoMapper;
using Domain.DTOs.Product;
using Domain.Entities;
using Domain.Filters;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class ProductService(DataContext context, IBaseRepository<Product, int> repository, ILogger<ProductService> logger, IMapper mapper, IMemoryCacheService memoryCacheService, IRedisCacheService redisCacheService) : IPoductService
{
    public async Task<Response<GetProductDto>> CreateAsync(CreateProductDto request)
    {
        var product = mapper.Map<Product>(request);
        var result = await repository.AddAsync(product);
        if (result == 0)
        {
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Product not added!");
        }
        var data = mapper.Map<GetProductDto>(product);
        await redisCacheService.RemoveData("products");

        return new Response<GetProductDto>(data);
    }

    public async Task<Response<string>> DeleteAsync(int Id)
    {
        var Product = await repository.GetByAsync(Id);
        if (Product == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, $"Product with id {Id} not found");
        }
        var result = await repository.DeleteAsync(Product);
        if (result == 0)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Product not deleted!");
        }
        await redisCacheService.RemoveData("products");

        return new Response<string>("Product deleted successfuly");
    }

    public async Task<Response<List<GetProductDto>>> GetAllAsync(ProductFilter filter)
    {

        const string cacheKey = "products";
        // var product = memoryCacheService.GetData<List<GetProductDto>>(cacheKey);
        var product = redisCacheService.GetData<List<GetProductDto>>(cacheKey);

        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);

        var Product = await repository.GetAll();
        if (filter.UserId.HasValue)
        {
            Product = Product.Where(o => o.UserId == filter.UserId);
        }
        if (filter.CategoryId != null)
        {
            Product = Product.Where(o => o.CategoryId == filter.CategoryId);

        }
        if (filter.From.HasValue)
        {
            Product = Product.Where(o => o.Price <= filter.From.Value);
        }
        if (filter.To.HasValue)
        {
            Product = Product.Where(o => o.Price >= filter.To.Value);
        }
        if (filter.Name != null)
        {
            Product = Product.Where(o => o.Name.ToLower().Contains(filter.Name.ToLower()));
        }

        var mapped = mapper.Map<List<GetProductDto>>(Product);

        var totalRecords = mapped.Count;

        var data = mapped
        .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
        .Take(validFilter.PageSize).ToList();

        return new PagedResponse<List<GetProductDto>>(data, validFilter.PageNumber, validFilter.PageSize, totalRecords);
    }

    public async Task<Response<GetProductDto>> GetByIdAsync(int Id)
    {
        var Product = await repository.GetByAsync(Id);
        if (Product == null)
        {
            return new Response<GetProductDto>(HttpStatusCode.NotFound, $"Product with id {Id} not found");
        }

        var data = mapper.Map<GetProductDto>(Product);
        await redisCacheService.RemoveData("products");
        return new Response<GetProductDto>(data);
    }

    public async Task<Response<GetProductDto>> UpDateAsync(int Id, UpdateProductDto request)
    {
        var Product = await repository.GetByAsync(Id);
        if (Product == null)
        {
            return new Response<GetProductDto>(HttpStatusCode.NotFound, $"Product with id {Id} not found");
        }

        Product.Name = request.Name;
        Product.Description = request.Description;
        Product.Price = request.Price;
        Product.CategoryId = request.CategoryId;
        Product.UserId = request.UserId;
        Product.IsTop = request.IsTop;
        Product.IsPremium = request.IsPremium;
        Product.ImageUrl = request.ImageUrl;
        Product.StockQuantity = request.StockQuantity;
        var result = await repository.UpdateAsync(Product);
        if (result == 0)
        {
            return new Response<GetProductDto>(HttpStatusCode.BadRequest, "Product not deleted!");
        }

        var data = mapper.Map<GetProductDto>(Product);
        await redisCacheService.RemoveData("products");

        return new Response<GetProductDto>(data);

    }


    public async Task PromoteProductAsync(int productId, bool makeTop, bool makePremium)
    {
        var product = await context.Products.FindAsync(productId);
        if (product == null) return;

        if (makeTop) product.IsTop = true;
        if (makePremium) product.IsPremium = true;

        product.PremiumOrTopExpiryDate = DateTime.UtcNow.AddDays(7);
        await redisCacheService.RemoveData("products");

        await context.SaveChangesAsync();
    }


}
