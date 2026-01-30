using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of IUserRepository using Entity Framework Core
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of UserRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public UserRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new user in the database
    /// </summary>
    /// <param name="user">The user to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created user</returns>
    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    /// <summary>
    /// Retrieves a user by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(o=> o.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves a user by their email address
    /// </summary>
    /// <param name="email">The email address to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <summary>
    /// Deletes a user from the database
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the user was deleted, false if not found</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(id, cancellationToken);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Retrieves all users with pagination support
    /// </summary>
    public async Task<(IEnumerable<User> Users, int TotalCount)> GetAllAsync(
        int page = 1,
        int size = 10,
        string? order = null,
        CancellationToken cancellationToken = default,
        Dictionary<string, string>? filters = null)
    {
        var query = _context.Users.AsQueryable();

        query = ApplyFilters(query, filters);
        query = ApplyOrdering(query, order);

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (users, totalCount);
    }

    /// <summary>
    /// Updates an existing user in the database
    /// </summary>
    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    private static IQueryable<User> ApplyFilters(IQueryable<User> query, Dictionary<string, string>? filters)
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
                    "createdat" when DateTime.TryParse(value, out var v) => query.Where(u => u.CreatedAt >= v),
                    _ => query
                };
                continue;
            }
            if (key.StartsWith("_max"))
            {
                var field = key[4..];
                query = field switch
                {
                    "createdat" when DateTime.TryParse(value, out var v) => query.Where(u => u.CreatedAt <= v),
                    _ => query
                };
                continue;
            }

            query = key switch
            {
                "username" => ApplyUserStringFilter(query, u => u.Username, value),
                "email" => ApplyUserStringFilter(query, u => u.Email, value),
                "phone" => ApplyUserStringFilter(query, u => u.Phone, value),
                "firstname" => ApplyUserStringFilter(query, u => u.Firstname, value),
                "lastname" => ApplyUserStringFilter(query, u => u.Lastname, value),
                "city" => ApplyUserStringFilter(query, u => u.City, value),
                "status" => query.Where(u => u.Status.ToString().ToLower() == value.ToLower()),
                "role" => query.Where(u => u.Role.ToString().ToLower() == value.ToLower()),
                _ => query
            };
        }

        return query;
    }

    private static IQueryable<User> ApplyUserStringFilter(
        IQueryable<User> query,
        System.Linq.Expressions.Expression<Func<User, string>> selector,
        string value)
    {
        var parameter = selector.Parameters[0];
        var memberAccess = selector.Body;
        var toLowerCall = System.Linq.Expressions.Expression.Call(memberAccess, typeof(string).GetMethod("ToLower", Type.EmptyTypes)!);

        if (value.StartsWith("*") && value.EndsWith("*"))
        {
            var term = value.Trim('*').ToLower();
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            var call = System.Linq.Expressions.Expression.Call(toLowerCall, containsMethod, System.Linq.Expressions.Expression.Constant(term));
            return query.Where(System.Linq.Expressions.Expression.Lambda<Func<User, bool>>(call, parameter));
        }
        if (value.EndsWith("*"))
        {
            var term = value.TrimEnd('*').ToLower();
            var method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!;
            var call = System.Linq.Expressions.Expression.Call(toLowerCall, method, System.Linq.Expressions.Expression.Constant(term));
            return query.Where(System.Linq.Expressions.Expression.Lambda<Func<User, bool>>(call, parameter));
        }
        if (value.StartsWith("*"))
        {
            var term = value.TrimStart('*').ToLower();
            var method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!;
            var call = System.Linq.Expressions.Expression.Call(toLowerCall, method, System.Linq.Expressions.Expression.Constant(term));
            return query.Where(System.Linq.Expressions.Expression.Lambda<Func<User, bool>>(call, parameter));
        }
        var eq = System.Linq.Expressions.Expression.Equal(toLowerCall, System.Linq.Expressions.Expression.Constant(value.ToLower()));
        return query.Where(System.Linq.Expressions.Expression.Lambda<Func<User, bool>>(eq, parameter));
    }

    private static IQueryable<User> ApplyOrdering(IQueryable<User> query, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return query.OrderBy(u => u.Username);

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
                "username" => ApplyOrder(query, u => u.Username, descending, isFirstOrder),
                "email" => ApplyOrder(query, u => u.Email, descending, isFirstOrder),
                "phone" => ApplyOrder(query, u => u.Phone, descending, isFirstOrder),
                "status" => ApplyOrder(query, u => u.Status, descending, isFirstOrder),
                "role" => ApplyOrder(query, u => u.Role, descending, isFirstOrder),
                "createdat" => ApplyOrder(query, u => u.CreatedAt, descending, isFirstOrder),
                _ => query
            };

            isFirstOrder = false;
        }

        return query;
    }

    private static IQueryable<User> ApplyOrder<TKey>(
        IQueryable<User> query,
        System.Linq.Expressions.Expression<Func<User, TKey>> keySelector,
        bool descending,
        bool isFirstOrder)
    {
        if (isFirstOrder)
        {
            return descending
                ? query.OrderByDescending(keySelector)
                : query.OrderBy(keySelector);
        }

        var orderedQuery = query as IOrderedQueryable<User>;
        return descending
            ? orderedQuery!.ThenByDescending(keySelector)
            : orderedQuery!.ThenBy(keySelector);
    }
}
