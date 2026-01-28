using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCarts;

/// <summary>
/// Handler for processing GetCartsCommand requests.
/// </summary>
public class GetCartsHandler : IRequestHandler<GetCartsCommand, GetCartsResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public GetCartsHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<GetCartsResult> Handle(GetCartsCommand command, CancellationToken cancellationToken)
    {
        var (carts, totalCount) = await _cartRepository.GetAllAsync(
            command.Page,
            command.Size,
            command.Order,
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / command.Size);

        var items = carts.Select(c => new GetCartsItemResult
        {
            Id = c.Id,
            UserId = c.UserId,
            Date = c.Date,
            Products = c.Products.Select(p => new GetCartsProductResult
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity
            }).ToList()
        }).ToList();

        return new GetCartsResult
        {
            Data = items,
            TotalItems = totalCount,
            CurrentPage = command.Page,
            TotalPages = totalPages
        };
    }
}
