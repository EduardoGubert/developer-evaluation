using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCarts;

/// <summary>
/// AutoMapper profile for GetCarts operation.
/// </summary>
public class GetCartsProfile : Profile
{
    public GetCartsProfile()
    {
        CreateMap<Cart, GetCartsItemResult>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));

        CreateMap<CartItem, GetCartsProductResult>();
    }
}
