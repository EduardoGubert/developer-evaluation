using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Handler for processing GetSalesCommand requests.
/// </summary>
public class GetSalesHandler : IRequestHandler<GetSalesCommand, GetSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<GetSalesResult> Handle(GetSalesCommand request, CancellationToken cancellationToken)
    {
        var (sales, totalCount) = await _saleRepository.GetAllAsync(
            request.Page,
            request.Size,
            request.Order,
            request.Filters,
            cancellationToken);

        var salesList = sales.ToList();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.Size);

        return new GetSalesResult
        {
            Data = _mapper.Map<List<GetSalesItemResult>>(salesList),
            TotalItems = totalCount,
            CurrentPage = request.Page,
            TotalPages = totalPages
        };
    }
}
