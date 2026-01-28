using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;

/// <summary>
/// AutoMapper profile for UpdateCart mappings.
/// </summary>
public class UpdateCartProfile : Profile
{
    public UpdateCartProfile()
    {
        CreateMap<UpdateCartRequest, UpdateCartCommand>();
        CreateMap<UpdateCartProductRequest, UpdateCartProductCommand>();
        CreateMap<UpdateCartResult, UpdateCartResponse>();
        CreateMap<UpdateCartProductResult, UpdateCartProductResponse>();
    }
}
