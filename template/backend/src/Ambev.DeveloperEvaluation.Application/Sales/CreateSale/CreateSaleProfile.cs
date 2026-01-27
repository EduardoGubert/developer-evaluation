using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Profile for mapping between Sale entity and CreateSale DTOs.
/// </summary>
public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        CreateMap<CreateSaleCommand, Sale>()
            .ForMember(dest => dest.Items, opt => opt.Ignore())
            .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
            .ForMember(dest => dest.IsCancelled, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<Sale, CreateSaleResult>();
        CreateMap<SaleItem, CreateSaleItemResult>();
    }
}
