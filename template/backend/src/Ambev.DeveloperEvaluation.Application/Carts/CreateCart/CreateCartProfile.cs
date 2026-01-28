using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

/// <summary>
/// AutoMapper profile for CreateCart operation.
/// </summary>
public class CreateCartProfile : Profile
{
    public CreateCartProfile()
    {
        CreateMap<Cart, CreateCartResult>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));

        CreateMap<CartItem, CreateCartProductResult>();
    }
}
