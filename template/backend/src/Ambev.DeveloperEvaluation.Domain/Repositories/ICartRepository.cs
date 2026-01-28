using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Cart entity operations.
/// </summary>
public interface ICartRepository
{
    /// <summary>
    /// Creates a new cart in the repository.
    /// </summary>
    /// <param name="cart">The cart to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created cart.</returns>
    Task<Cart> CreateAsync(Cart cart, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a cart by its unique identifier.
    /// </summary>
    /// <param name="id">The cart ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cart if found, null otherwise.</returns>
    Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a cart by user ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cart if found, null otherwise.</returns>
    Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of all carts.
    /// </summary>
    /// <param name="page">Page number.</param>
    /// <param name="size">Page size.</param>
    /// <param name="order">Ordering string.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple with carts and total count.</returns>
    Task<(IEnumerable<Cart> Carts, int TotalCount)> GetAllAsync(
        int page = 1,
        int size = 10,
        string? order = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing cart.
    /// </summary>
    /// <param name="cart">The cart to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated cart.</returns>
    Task<Cart> UpdateAsync(Cart cart, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a cart by its unique identifier.
    /// </summary>
    /// <param name="id">The cart ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted, false otherwise.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
