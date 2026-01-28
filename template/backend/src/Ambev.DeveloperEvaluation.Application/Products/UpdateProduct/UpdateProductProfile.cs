using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

/// <summary>
/// AutoMapper profile for UpdateProduct operation.
/// </summary>
public class UpdateProductProfile : Profile
{
    public UpdateProductProfile()
    {
        CreateMap<Product, UpdateProductResult>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => new UpdateProductRatingResult
            {
                Rate = src.RatingRate,
                Count = src.RatingCount
            }));
    }
}
