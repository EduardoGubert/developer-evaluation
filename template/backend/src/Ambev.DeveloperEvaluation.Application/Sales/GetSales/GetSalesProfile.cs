using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Profile for mapping Sale entity to GetSalesItemResult.
/// </summary>
public class GetSalesProfile : Profile
{
    public GetSalesProfile()
    {
        CreateMap<Sale, GetSalesItemResult>();
    }
}
