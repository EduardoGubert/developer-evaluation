using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProducts;

/// <summary>
/// AutoMapper profile for GetProducts operation.
/// </summary>
public class GetProductsProfile : Profile
{
    public GetProductsProfile()
    {
        CreateMap<Product, GetProductsItemResult>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => new GetProductsRatingResult
            {
                Rate = src.RatingRate,
                Count = src.RatingCount
            }));
    }
}
