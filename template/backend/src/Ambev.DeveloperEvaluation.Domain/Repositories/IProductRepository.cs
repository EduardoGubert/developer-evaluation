using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Product entity operations.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Creates a new product in the repository.
    /// </summary>
    /// <param name="product">The product to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created product.</returns>
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product if found, null otherwise.</returns>
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of all products.
    /// </summary>
    /// <param name="page">Page number.</param>
    /// <param name="size">Page size.</param>
    /// <param name="order">Ordering string.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple with products and total count.</returns>
    Task<(IEnumerable<Product> Products, int TotalCount)> GetAllAsync(
        int page = 1,
        int size = 10,
        string? order = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves products by category with pagination.
    /// </summary>
    /// <param name="category">The category name.</param>
    /// <param name="page">Page number.</param>
    /// <param name="size">Page size.</param>
    /// <param name="order">Ordering string.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A tuple with products and total count.</returns>
    Task<(IEnumerable<Product> Products, int TotalCount)> GetByCategoryAsync(
        string category,
        int page = 1,
        int size = 10,
        string? order = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all distinct product categories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of category names.</returns>
    Task<IEnumerable<string>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="product">The product to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated product.</returns>
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a product by its unique identifier.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted, false otherwise.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves multiple products by their unique identifiers.
    /// </summary>
    /// <param name="ids">The product IDs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The products found.</returns>
    Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}
