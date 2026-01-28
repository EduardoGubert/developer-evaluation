using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProductsByCategory;

/// <summary>
/// Handler for processing GetProductsByCategoryCommand requests.
/// </summary>
public class GetProductsByCategoryHandler : IRequestHandler<GetProductsByCategoryCommand, GetProductsByCategoryResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsByCategoryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryCommand command, CancellationToken cancellationToken)
    {
        var (products, totalCount) = await _productRepository.GetByCategoryAsync(
            command.Category,
            command.Page,
            command.Size,
            command.Order,
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / command.Size);

        var items = products.Select(p => new GetProductsByCategoryItemResult
        {
            Id = p.Id,
            Title = p.Title,
            Price = p.Price,
            Description = p.Description,
            Category = p.Category,
            Image = p.Image,
            Rating = new GetProductsByCategoryRatingResult
            {
                Rate = p.RatingRate,
                Count = p.RatingCount
            }
        }).ToList();

        return new GetProductsByCategoryResult
        {
            Data = items,
            TotalItems = totalCount,
            CurrentPage = command.Page,
            TotalPages = totalPages
        };
    }
}
