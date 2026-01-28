using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Carts.CheckoutCart;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.CheckoutCart;

/// <summary>
/// AutoMapper profile for CheckoutCart mappings.
/// </summary>
public class CheckoutCartProfile : Profile
{
    public CheckoutCartProfile()
    {
        CreateMap<CheckoutCartResult, CheckoutCartResponse>();
        CreateMap<CheckoutCartItemResult, CheckoutCartItemResponse>();
    }
}
