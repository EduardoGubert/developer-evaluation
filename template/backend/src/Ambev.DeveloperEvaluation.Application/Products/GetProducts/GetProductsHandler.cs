using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProducts;

/// <summary>
/// Handler for processing GetProductsCommand requests.
/// </summary>
public class GetProductsHandler : IRequestHandler<GetProductsCommand, GetProductsResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<GetProductsResult> Handle(GetProductsCommand command, CancellationToken cancellationToken)
    {
        var (products, totalCount) = await _productRepository.GetAllAsync(
            command.Page,
            command.Size,
            command.Order,
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / command.Size);

        var items = products.Select(p => new GetProductsItemResult
        {
            Id = p.Id,
            Title = p.Title,
            Price = p.Price,
            Description = p.Description,
            Category = p.Category,
            Image = p.Image,
            Rating = new GetProductsRatingResult
            {
                Rate = p.RatingRate,
                Count = p.RatingCount
            }
        }).ToList();

        return new GetProductsResult
        {
            Data = items,
            TotalItems = totalCount,
            CurrentPage = command.Page,
            TotalPages = totalPages
        };
    }
}
