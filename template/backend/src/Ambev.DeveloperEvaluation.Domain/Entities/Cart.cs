using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a shopping cart in the system.
/// </summary>
public class Cart : BaseEntity
{
    /// <summary>
    /// Gets or sets the user ID who owns the cart.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the cart date.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the list of products in the cart.
    /// </summary>
    public List<CartItem> Products { get; set; } = new();

    /// <summary>
    /// Gets or sets the creation date.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the last update date.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the Cart class.
    /// </summary>
    public Cart()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        Date = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a product to the cart.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="quantity">The quantity.</param>
    /// <returns>The cart item.</returns>
    public CartItem AddProduct(Guid productId, int quantity)
    {
        if (quantity > 20)
            throw new DomainException("Cannot add more than 20 identical items.");

        var existingItem = Products.FirstOrDefault(p => p.ProductId == productId);
        if (existingItem != null)
        {
            var newQuantity = existingItem.Quantity + quantity;
            if (newQuantity > 20)
                throw new DomainException($"Cannot have more than 20 identical items. Current: {existingItem.Quantity}, Requested: {quantity}.");
            existingItem.Quantity = newQuantity;
            return existingItem;
        }

        var item = new CartItem
        {
            CartId = Id,
            ProductId = productId,
            Quantity = quantity
        };
        Products.Add(item);
        return item;
    }

    /// <summary>
    /// Removes a product from the cart.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <returns>True if removed, false otherwise.</returns>
    public bool RemoveProduct(Guid productId)
    {
        var item = Products.FirstOrDefault(p => p.ProductId == productId);
        if (item == null) return false;
        Products.Remove(item);
        return true;
    }

    /// <summary>
    /// Updates the cart information.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="date">The date.</param>
    public void Update(Guid userId, DateTime date)
    {
        UserId = userId;
        Date = date;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Validates the cart entity.
    /// </summary>
    /// <returns>The validation result.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new CartValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}
