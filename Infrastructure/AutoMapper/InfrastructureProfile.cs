using AutoMapper;
using Domain.DTOs.Category;
using Domain.DTOs.Order;
using Domain.DTOs.Product;
using Domain.Entities;

namespace Infrastructure.AutoMapper;

public class InfrastructureProfile : Profile
{
    public InfrastructureProfile()
    {
        // Category
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<GetCategoryDto, Category>().ReverseMap();
        CreateMap<Category, GetCategoryDto>();
        // product
        CreateMap<CreateProductDto,Product>();
        CreateMap<GetProductDto,Product>().ReverseMap();
        CreateMap<Product,GetProductDto>();
        // Order
        CreateMap<CreateOrderDto,Order>();
        CreateMap<GetOrderDto,Order>().ReverseMap();
        CreateMap<Order,GetOrderDto>();
    }
}
