using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale transaction in the system.
/// This entity is the aggregate root for sales, containing items and business rules.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique sale number.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the external customer identifier.
    /// This is an external identity reference following DDD patterns.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the denormalized customer name for display purposes.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the external branch identifier.
    /// This is an external identity reference following DDD patterns.
    /// </summary>
    public Guid BranchId { get; set; }

    /// <summary>
    /// Gets or sets the denormalized branch name for display purposes.
    /// </summary>
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets the total amount of the sale.
    /// Calculated as the sum of all non-cancelled item totals.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Gets whether this sale has been cancelled.
    /// </summary>
    public bool IsCancelled { get; private set; }

    /// <summary>
    /// Gets or sets the collection of items in this sale.
    /// </summary>
    public List<SaleItem> Items { get; set; } = new();

    /// <summary>
    /// Gets the date and time when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the Sale class.
    /// </summary>
    public Sale()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        SaleDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds an item to the sale with automatic discount calculation.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="productName">The product name.</param>
    /// <param name="quantity">The quantity (must be between 1 and 20).</param>
    /// <param name="unitPrice">The unit price.</param>
    /// <returns>The created sale item.</returns>
    /// <exception cref="DomainException">Thrown when quantity exceeds 20 items.</exception>
    public SaleItem AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        var item = new SaleItem
        {
            SaleId = Id,
            ProductId = productId,
            ProductName = productName,
            Quantity = quantity,
            UnitPrice = unitPrice
        };

        item.CalculateDiscount();
        Items.Add(item);
        CalculateTotalAmount();

        return item;
    }

    /// <summary>
    /// Removes an item from the sale by its identifier.
    /// </summary>
    /// <param name="itemId">The item identifier to remove.</param>
    /// <returns>True if the item was removed, false otherwise.</returns>
    public bool RemoveItem(Guid itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            return false;

        Items.Remove(item);
        CalculateTotalAmount();
        return true;
    }

    /// <summary>
    /// Cancels a specific item in the sale.
    /// </summary>
    /// <param name="itemId">The item identifier to cancel.</param>
    /// <returns>The cancelled item, or null if not found.</returns>
    public SaleItem? CancelItem(Guid itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId && !i.IsCancelled);
        if (item == null)
            return null;

        item.Cancel();
        CalculateTotalAmount();
        UpdatedAt = DateTime.UtcNow;

        return item;
    }

    /// <summary>
    /// Cancels the entire sale.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the total amount of the sale.
    /// Only includes non-cancelled items.
    /// </summary>
    public void CalculateTotalAmount()
    {
        TotalAmount = Items
            .Where(i => !i.IsCancelled)
            .Sum(i => i.TotalAmount);
    }

    /// <summary>
    /// Updates the sale information.
    /// </summary>
    /// <param name="customerId">The new customer identifier.</param>
    /// <param name="customerName">The new customer name.</param>
    /// <param name="branchId">The new branch identifier.</param>
    /// <param name="branchName">The new branch name.</param>
    public void Update(Guid customerId, string customerName, Guid branchId, string branchName)
    {
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Performs validation of the sale entity using the SaleValidator rules.
    /// </summary>
    /// <returns>
    /// A <see cref="ValidationResultDetail"/> containing:
    /// - IsValid: Indicates whether all validation rules passed
    /// - Errors: Collection of validation errors if any rules failed
    /// </returns>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}
