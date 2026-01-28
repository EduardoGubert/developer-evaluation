using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;

/// <summary>
/// AutoMapper profile for CreateCart mappings.
/// </summary>
public class CreateCartProfile : Profile
{
    public CreateCartProfile()
    {
        CreateMap<CreateCartRequest, CreateCartCommand>();
        CreateMap<CreateCartProductRequest, CreateCartProductCommand>();
        CreateMap<CreateCartResult, CreateCartResponse>();
        CreateMap<CreateCartProductResult, CreateCartProductResponse>();
    }
}
