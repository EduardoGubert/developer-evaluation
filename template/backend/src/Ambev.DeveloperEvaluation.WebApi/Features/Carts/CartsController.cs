using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.GetCart;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.GetCarts;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Carts.GetCarts;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.Application.Carts.CheckoutCart;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.CheckoutCart;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts;

/// <summary>
/// Controller for managing cart operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CartsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public CartsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new cart.
    /// </summary>
    /// <param name="request">The cart creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created cart details.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateCartResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCart([FromBody] CreateCartRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateCartRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateCartCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateCartResponse>
        {
            Success = true,
            Message = "Cart created successfully",
            Data = _mapper.Map<CreateCartResponse>(response)
        });
    }

    /// <summary>
    /// Retrieves a paginated list of carts.
    /// </summary>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="size">Page size (default: 10).</param>
    /// <param name="order">Ordering (e.g., "date desc").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of carts.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<GetCartsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCarts(
        [FromQuery(Name = "_page")] int page = 1,
        [FromQuery(Name = "_size")] int size = 10,
        [FromQuery(Name = "_order")] string? order = null,
        CancellationToken cancellationToken = default)
    {
        var command = new GetCartsCommand { Page = page, Size = size, Order = order };
        var response = await _mediator.Send(command, cancellationToken);

        var result = new GetCartsResponse
        {
            Data = response.Data.Select(c => new GetCartsItemResponse
            {
                Id = c.Id,
                UserId = c.UserId,
                Date = c.Date,
                Products = c.Products.Select(p => new GetCartsProductResponse
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList()
            }).ToList(),
            TotalItems = response.TotalItems,
            CurrentPage = response.CurrentPage,
            TotalPages = response.TotalPages
        };

        return Ok(new ApiResponseWithData<GetCartsResponse>
        {
            Success = true,
            Message = "Carts retrieved successfully",
            Data = result
        });
    }

    /// <summary>
    /// Retrieves a cart by its ID.
    /// </summary>
    /// <param name="id">The cart ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cart details if found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetCartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCart([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new GetCartCommand(id);
        var response = await _mediator.Send(command, cancellationToken);

        var result = new GetCartResponse
        {
            Id = response.Id,
            UserId = response.UserId,
            Date = response.Date,
            Products = response.Products.Select(p => new GetCartProductResponse
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity
            }).ToList()
        };

        return Ok(new ApiResponseWithData<GetCartResponse>
        {
            Success = true,
            Message = "Cart retrieved successfully",
            Data = result
        });
    }

    /// <summary>
    /// Updates an existing cart.
    /// </summary>
    /// <param name="id">The cart ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated cart details.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateCartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCart([FromRoute] Guid id, [FromBody] UpdateCartRequest request, CancellationToken cancellationToken)
    {
        var validator = new UpdateCartRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateCartCommand>(request);
        command.Id = id;
        var response = await _mediator.Send(command, cancellationToken);

        var result = new UpdateCartResponse
        {
            Id = response.Id,
            UserId = response.UserId,
            Date = response.Date,
            Products = response.Products.Select(p => new UpdateCartProductResponse
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity
            }).ToList()
        };

        return Ok(new ApiResponseWithData<UpdateCartResponse>
        {
            Success = true,
            Message = "Cart updated successfully",
            Data = result
        });
    }

    /// <summary>
    /// Deletes a cart by its ID.
    /// </summary>
    /// <param name="id">The cart ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success response if deleted.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCart([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCartCommand(id);
        await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Cart deleted successfully"
        });
    }

    /// <summary>
    /// Checks out a cart, creating a sale with automatic discount calculation.
    /// </summary>
    /// <param name="id">The cart ID to checkout.</param>
    /// <param name="request">The checkout request with branch information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created sale details with discounts.</returns>
    [HttpPost("{id}/checkout")]
    [ProducesResponseType(typeof(ApiResponseWithData<CheckoutCartResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CheckoutCart([FromRoute] Guid id, [FromBody] CheckoutCartRequest request, CancellationToken cancellationToken)
    {
        var command = new CheckoutCartCommand
        {
            CartId = id,
            BranchId = request.BranchId,
            BranchName = request.BranchName
        };

        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<CheckoutCartResponse>
        {
            Success = true,
            Message = "Cart checked out successfully. Sale created.",
            Data = _mapper.Map<CheckoutCartResponse>(response)
        });
    }
}
