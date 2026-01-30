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

    public async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        return await _context.Products
            .Where(p => idList.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> GetAllAsync(
        int page = 1,
        int size = 10,
        string? order = null,
        Dictionary<string, string>? filters = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsQueryable();

        query = ApplyFilters(query, filters);
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

    private static IQueryable<Product> ApplyFilters(IQueryable<Product> query, Dictionary<string, string>? filters)
    {
        if (filters == null || filters.Count == 0) return query;

        foreach (var filter in filters)
        {
            var key = filter.Key.ToLowerInvariant();
            var value = filter.Value;

            if (key.StartsWith("_min"))
            {
                var field = key[4..]; // remove "_min"
                query = field switch
                {
                    "price" when decimal.TryParse(value, out var v) => query.Where(p => p.Price >= v),
                    "ratingrate" when decimal.TryParse(value, out var v) => query.Where(p => p.RatingRate >= v),
                    "ratingcount" when int.TryParse(value, out var v) => query.Where(p => p.RatingCount >= v),
                    _ => query
                };
                continue;
            }

            if (key.StartsWith("_max"))
            {
                var field = key[4..];
                query = field switch
                {
                    "price" when decimal.TryParse(value, out var v) => query.Where(p => p.Price <= v),
                    "ratingrate" when decimal.TryParse(value, out var v) => query.Where(p => p.RatingRate <= v),
                    "ratingcount" when int.TryParse(value, out var v) => query.Where(p => p.RatingCount <= v),
                    _ => query
                };
                continue;
            }

            query = key switch
            {
                "title" => ApplyStringFilter(query, p => p.Title, value),
                "category" => ApplyStringFilter(query, p => p.Category, value),
                "description" => ApplyStringFilter(query, p => p.Description, value),
                "price" when decimal.TryParse(value, out var v) => query.Where(p => p.Price == v),
                _ => query
            };
        }

        return query;
    }

    private static IQueryable<Product> ApplyStringFilter(
        IQueryable<Product> query,
        System.Linq.Expressions.Expression<Func<Product, string>> selector,
        string value)
    {
        var parameter = selector.Parameters[0];
        var memberAccess = selector.Body;
        var toLowerCall = System.Linq.Expressions.Expression.Call(memberAccess, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!);

        if (value.StartsWith("*") && value.EndsWith("*"))
        {
            var term = value.Trim('*').ToLower();
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            var containsCall = System.Linq.Expressions.Expression.Call(toLowerCall, containsMethod, System.Linq.Expressions.Expression.Constant(term));
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<Product, bool>>(containsCall, parameter);
            return query.Where(lambda);
        }
        if (value.EndsWith("*"))
        {
            var term = value.TrimEnd('*').ToLower();
            var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!;
            var startsWithCall = System.Linq.Expressions.Expression.Call(toLowerCall, startsWithMethod, System.Linq.Expressions.Expression.Constant(term));
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<Product, bool>>(startsWithCall, parameter);
            return query.Where(lambda);
        }
        if (value.StartsWith("*"))
        {
            var term = value.TrimStart('*').ToLower();
            var endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!;
            var endsWithCall = System.Linq.Expressions.Expression.Call(toLowerCall, endsWithMethod, System.Linq.Expressions.Expression.Constant(term));
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<Product, bool>>(endsWithCall, parameter);
            return query.Where(lambda);
        }
        // Exact match (case-insensitive)
        var equalsCall = System.Linq.Expressions.Expression.Equal(toLowerCall, System.Linq.Expressions.Expression.Constant(value.ToLower()));
        var exactLambda = System.Linq.Expressions.Expression.Lambda<Func<Product, bool>>(equalsCall, parameter);
        return query.Where(exactLambda);
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
