using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ICartRepository using Entity Framework Core.
/// </summary>
public class CartRepository : ICartRepository
{
    private readonly DefaultContext _context;

    public CartRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Cart> CreateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        await _context.Carts.AddAsync(cart, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return cart;
    }

    public async Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<(IEnumerable<Cart> Carts, int TotalCount)> GetAllAsync(
        int page = 1,
        int size = 10,
        string? order = null,
        Dictionary<string, string>? filters = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Carts
            .Include(c => c.Products)
            .AsQueryable();

        query = ApplyFilters(query, filters);
        query = ApplyOrdering(query, order);

        var totalCount = await query.CountAsync(cancellationToken);

        var carts = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (carts, totalCount);
    }

    public async Task<Cart> UpdateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        _context.Carts.Update(cart);
        await _context.SaveChangesAsync(cancellationToken);
        return cart;
    }

    public async Task<CartItem> CreateItemAsync(CartItem cartItem, CancellationToken cancellationToken = default)
    {
        await _context.CartItems.AddAsync(cartItem, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return cartItem;
    }

    public async Task<bool> DeleteItemAsync(Guid cartItemId, CancellationToken cancellationToken = default)
    {
        var cartItem = await _context.CartItems.FindAsync(new object[] { cartItemId }, cancellationToken);
        if (cartItem == null)
            return false;

        _context.CartItems.Remove(cartItem);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cart = await GetByIdAsync(id, cancellationToken);
        if (cart == null)
            return false;

        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static IQueryable<Cart> ApplyFilters(IQueryable<Cart> query, Dictionary<string, string>? filters)
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
                    "date" when DateTime.TryParse(value, out var v) => query.Where(c => c.Date >= v),
                    "createdat" when DateTime.TryParse(value, out var v) => query.Where(c => c.CreatedAt >= v),
                    _ => query
                };
                continue;
            }
            if (key.StartsWith("_max"))
            {
                var field = key[4..];
                query = field switch
                {
                    "date" when DateTime.TryParse(value, out var v) => query.Where(c => c.Date <= v),
                    "createdat" when DateTime.TryParse(value, out var v) => query.Where(c => c.CreatedAt <= v),
                    _ => query
                };
                continue;
            }

            query = key switch
            {
                "userid" when Guid.TryParse(value, out var v) => query.Where(c => c.UserId == v),
                _ => query
            };
        }

        return query;
    }

    private static IQueryable<Cart> ApplyOrdering(IQueryable<Cart> query, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return query.OrderByDescending(c => c.Date);

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
                "id" => ApplyOrder(query, c => c.Id, descending, isFirstOrder),
                "userid" => ApplyOrder(query, c => c.UserId, descending, isFirstOrder),
                "date" => ApplyOrder(query, c => c.Date, descending, isFirstOrder),
                "createdat" => ApplyOrder(query, c => c.CreatedAt, descending, isFirstOrder),
                _ => query
            };

            isFirstOrder = false;
        }

        return query;
    }

    private static IQueryable<Cart> ApplyOrder<TKey>(
        IQueryable<Cart> query,
        System.Linq.Expressions.Expression<Func<Cart, TKey>> keySelector,
        bool descending,
        bool isFirstOrder)
    {
        if (isFirstOrder)
        {
            return descending
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);
        }

        var orderedQuery = query as IOrderedQueryable<Cart>;
        return descending
            ? orderedQuery!.ThenByDescending(keySelector)
            : orderedQuery!.ThenBy(keySelector);
    }
}
