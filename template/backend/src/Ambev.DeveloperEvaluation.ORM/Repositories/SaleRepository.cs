using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core.
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    public async Task<(IEnumerable<Sale> Sales, int TotalCount)> GetAllAsync(
        int page = 1,
        int size = 10,
        string? order = null,
        Dictionary<string, string>? filters = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include(s => s.Items)
            .AsQueryable();

        query = ApplyFilters(query, filters);
        query = ApplyOrdering(query, order);

        var totalCount = await query.CountAsync(cancellationToken);

        var sales = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (sales, totalCount);
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static IQueryable<Sale> ApplyFilters(IQueryable<Sale> query, Dictionary<string, string>? filters)
    {
        if (filters == null || filters.Count == 0) return query;

        foreach (var filter in filters)
        {
            var key = filter.Key.ToLowerInvariant();
            var value = filter.Value;

            if (key.StartsWith("_min"))
            {
                var field = key[4..];
                query = field switch
                {
                    "saledate" when DateTime.TryParse(value, out var v) => query.Where(s => s.SaleDate >= v),
                    "totalamount" when decimal.TryParse(value, out var v) => query.Where(s => s.TotalAmount >= v),
                    "createdat" when DateTime.TryParse(value, out var v) => query.Where(s => s.CreatedAt >= v),
                    _ => query
                };
                continue;
            }
            if (key.StartsWith("_max"))
            {
                var field = key[4..];
                query = field switch
                {
                    "saledate" when DateTime.TryParse(value, out var v) => query.Where(s => s.SaleDate <= v),
                    "totalamount" when decimal.TryParse(value, out var v) => query.Where(s => s.TotalAmount <= v),
                    "createdat" when DateTime.TryParse(value, out var v) => query.Where(s => s.CreatedAt <= v),
                    _ => query
                };
                continue;
            }

            query = key switch
            {
                "salenumber" => ApplySaleStringFilter(query, s => s.SaleNumber, value),
                "customername" => ApplySaleStringFilter(query, s => s.CustomerName, value),
                "branchname" => ApplySaleStringFilter(query, s => s.BranchName, value),
                "customerid" when Guid.TryParse(value, out var v) => query.Where(s => s.CustomerId == v),
                "branchid" when Guid.TryParse(value, out var v) => query.Where(s => s.BranchId == v),
                "iscancelled" when bool.TryParse(value, out var v) => query.Where(s => s.IsCancelled == v),
                _ => query
            };
        }

        return query;
    }

    private static IQueryable<Sale> ApplySaleStringFilter(
        IQueryable<Sale> query,
        System.Linq.Expressions.Expression<Func<Sale, string>> selector,
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
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<Sale, bool>>(containsCall, parameter);
            return query.Where(lambda);
        }
        if (value.EndsWith("*"))
        {
            var term = value.TrimEnd('*').ToLower();
            var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!;
            var startsWithCall = System.Linq.Expressions.Expression.Call(toLowerCall, startsWithMethod, System.Linq.Expressions.Expression.Constant(term));
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<Sale, bool>>(startsWithCall, parameter);
            return query.Where(lambda);
        }
        if (value.StartsWith("*"))
        {
            var term = value.TrimStart('*').ToLower();
            var endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!;
            var endsWithCall = System.Linq.Expressions.Expression.Call(toLowerCall, endsWithMethod, System.Linq.Expressions.Expression.Constant(term));
            var lambda = System.Linq.Expressions.Expression.Lambda<Func<Sale, bool>>(endsWithCall, parameter);
            return query.Where(lambda);
        }
        // Exact match (case-insensitive)
        var equalsCall = System.Linq.Expressions.Expression.Equal(toLowerCall, System.Linq.Expressions.Expression.Constant(value.ToLower()));
        var exactLambda = System.Linq.Expressions.Expression.Lambda<Func<Sale, bool>>(equalsCall, parameter);
        return query.Where(exactLambda);
    }

    private static IQueryable<Sale> ApplyOrdering(IQueryable<Sale> query, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return query.OrderByDescending(s => s.SaleDate);

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
                "salenumber" => ApplyOrder(query, s => s.SaleNumber, descending, isFirstOrder),
                "saledate" => ApplyOrder(query, s => s.SaleDate, descending, isFirstOrder),
                "customername" => ApplyOrder(query, s => s.CustomerName, descending, isFirstOrder),
                "branchname" => ApplyOrder(query, s => s.BranchName, descending, isFirstOrder),
                "totalamount" => ApplyOrder(query, s => s.TotalAmount, descending, isFirstOrder),
                "createdat" => ApplyOrder(query, s => s.CreatedAt, descending, isFirstOrder),
                _ => query
            };

            isFirstOrder = false;
        }

        return query;
    }

    private static IQueryable<Sale> ApplyOrder<TKey>(
        IQueryable<Sale> query,
        System.Linq.Expressions.Expression<Func<Sale, TKey>> keySelector,
        bool descending,
        bool isFirstOrder)
    {
        if (isFirstOrder)
        {
            return descending
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);
        }

        var orderedQuery = query as IOrderedQueryable<Sale>;
        return descending
            ? orderedQuery!.ThenByDescending(keySelector)
            : orderedQuery!.ThenBy(keySelector);
    }
}
