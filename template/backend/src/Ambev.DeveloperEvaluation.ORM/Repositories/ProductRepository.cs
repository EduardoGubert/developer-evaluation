using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of IProductRepository using Entity Framework Core.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly DefaultContext _context;

    public ProductRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetAllAsync(
        int page = 1,
        int size = 10,
        string? order = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsQueryable();

        query = ApplyOrdering(query, order);

        var totalCount = await query.CountAsync(cancellationToken);

        var products = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetByCategoryAsync(
        string category,
        int page = 1,
        int size = 10,
        string? order = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products
            .Where(p => p.Category.ToLower() == category.ToLower())
            .AsQueryable();

        query = ApplyOrdering(query, order);

        var totalCount = await query.CountAsync(cancellationToken);

        var products = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }

    public async Task<IEnumerable<string>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(id, cancellationToken);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static IQueryable<Product> ApplyOrdering(IQueryable<Product> query, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return query.OrderBy(p => p.Title);

        var orderParams = order.Split(',', StringSplitOptions.RemoveEmptyEntries);
        var isFirstOrder = true;

        foreach (var param in orderParams)
        {
            var trimmedParam = param.Trim();
            var parts = trimmedParam.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var propertyName = parts[0].ToLowerInvariant();
            var descending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            query = propertyName switch
            {
                "title" => ApplyOrder(query, p => p.Title, descending, isFirstOrder),
                "price" => ApplyOrder(query, p => p.Price, descending, isFirstOrder),
                "category" => ApplyOrder(query, p => p.Category, descending, isFirstOrder),
                "ratingrate" or "rating" => ApplyOrder(query, p => p.RatingRate, descending, isFirstOrder),
                "ratingcount" => ApplyOrder(query, p => p.RatingCount, descending, isFirstOrder),
                "createdat" => ApplyOrder(query, p => p.CreatedAt, descending, isFirstOrder),
                _ => query
            };

            isFirstOrder = false;
        }

        return query;
    }

    private static IQueryable<Product> ApplyOrder<TKey>(
        IQueryable<Product> query,
        System.Linq.Expressions.Expression<Func<Product, TKey>> keySelector,
        bool descending,
        bool isFirstOrder)
    {
        if (isFirstOrder)
        {
            return descending
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);
        }

        var orderedQuery = query as IOrderedQueryable<Product>;
        return descending
            ? orderedQuery!.ThenByDescending(keySelector)
            : orderedQuery!.ThenBy(keySelector);
    }
}
