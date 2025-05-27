using Domain.DTOs.Product;
using Domain.Filters;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IPoductService
{
    Task PromoteProductAsync(int productId, bool makeTop, bool makePremium);
    Task<Response<List<GetProductDto>>> GetAllAsync(ProductFilter filter);
    Task<Response<GetProductDto>> CreateAsync(CreateProductDto request);
    Task<Response<GetProductDto>> GetByIdAsync(int Id);
    Task<Response<GetProductDto>> UpDateAsync(int Id, UpdateProductDto request);
    Task<Response<string>> DeleteAsync(int Id);
}
