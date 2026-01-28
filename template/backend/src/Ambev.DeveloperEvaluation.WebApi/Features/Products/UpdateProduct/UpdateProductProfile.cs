using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;

/// <summary>
/// AutoMapper profile for UpdateProduct WebApi mappings.
/// </summary>
public class UpdateProductProfile : Profile
{
    public UpdateProductProfile()
    {
        CreateMap<UpdateProductRequest, UpdateProductCommand>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => new UpdateProductRatingCommand
            {
                Rate = src.Rating.Rate,
                Count = src.Rating.Count
            }));

        CreateMap<UpdateProductResult, UpdateProductResponse>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => new UpdateProductRatingResponse
            {
                Rate = src.Rating.Rate,
                Count = src.Rating.Count
            }));
    }
}
