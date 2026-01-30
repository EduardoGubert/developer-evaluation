using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Common.Interfaces;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProducts;

/// <summary>
/// Handler for processing GetProductsCommand requests.
/// </summary>
public class GetProductsHandler : IRequestHandler<GetProductsCommand, GetProductsResult>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cache;
    private readonly IMapper _mapper;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);

    public GetProductsHandler(
        IProductRepository productRepository,
        ICacheService cache,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _cache = cache;
        _mapper = mapper;
    }

    public async Task<GetProductsResult> Handle(GetProductsCommand command, CancellationToken cancellationToken)
    {
        var filterKey = string.Join("&", command.Filters.OrderBy(f => f.Key).Select(f => $"{f.Key}={f.Value}"));
        var cacheKey = $"products:all:{command.Page}:{command.Size}:{command.Order ?? "default"}:{filterKey}";

        var cached = await _cache.GetAsync<GetProductsResult>(cacheKey, cancellationToken);
        if (cached != null)
            return cached;

        var (products, totalCount) = await _productRepository.GetAllAsync(
            command.Page,
            command.Size,
            command.Order,
            command.Filters,
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

        var result = new GetProductsResult
        {
            Data = items,
            TotalItems = totalCount,
            CurrentPage = command.Page,
            TotalPages = totalPages
        };
        
        await _cache.SetAsync(cacheKey, result, CacheExpiration, cancellationToken);

        return result;
    }
}
