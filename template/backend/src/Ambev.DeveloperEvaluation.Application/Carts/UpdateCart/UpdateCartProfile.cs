using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

/// <summary>
/// AutoMapper profile for UpdateCart operation.
/// </summary>
public class UpdateCartProfile : Profile
{
    public UpdateCartProfile()
    {
        CreateMap<Cart, UpdateCartResult>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));

        CreateMap<CartItem, UpdateCartProductResult>();
    }
}
