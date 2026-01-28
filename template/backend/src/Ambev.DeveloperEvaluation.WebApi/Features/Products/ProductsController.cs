using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProducts;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Products.GetProducts;
using Ambev.DeveloperEvaluation.Application.Products.GetProductsByCategory;
using Ambev.DeveloperEvaluation.Application.Products.GetCategories;
using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products;

/// <summary>
/// Controller for managing product operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ProductsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="request">The product creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created product details.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateProductResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateProductRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateProductCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateProductResponse>
        {
            Success = true,
            Message = "Product created successfully",
            Data = _mapper.Map<CreateProductResponse>(response)
        });
    }

    /// <summary>
    /// Retrieves a paginated list of products.
    /// </summary>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="size">Page size (default: 10).</param>
    /// <param name="order">Ordering (e.g., "price desc, title asc").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of products.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<GetProductsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(
        [FromQuery(Name = "_page")] int page = 1,
        [FromQuery(Name = "_size")] int size = 10,
        [FromQuery(Name = "_order")] string? order = null,
        CancellationToken cancellationToken = default)
    {
        var command = new GetProductsCommand { Page = page, Size = size, Order = order };
        var response = await _mediator.Send(command, cancellationToken);

        var result = new GetProductsResponse
        {
            Data = response.Data.Select(p => new GetProductsItemResponse
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                Description = p.Description,
                Category = p.Category,
                Image = p.Image,
                Rating = new GetProductsRatingResponse
                {
                    Rate = p.Rating.Rate,
                    Count = p.Rating.Count
                }
            }).ToList(),
            TotalItems = response.TotalItems,
            CurrentPage = response.CurrentPage,
            TotalPages = response.TotalPages
        };

        return Ok(new ApiResponseWithData<GetProductsResponse>
        {
            Success = true,
            Message = "Products retrieved successfully",
            Data = result
        });
    }

    /// <summary>
    /// Retrieves a product by its ID.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product details if found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new GetProductCommand(id);
        var response = await _mediator.Send(command, cancellationToken);

        var result = new GetProductResponse
        {
            Id = response.Id,
            Title = response.Title,
            Price = response.Price,
            Description = response.Description,
            Category = response.Category,
            Image = response.Image,
            Rating = new GetProductRatingResponse
            {
                Rate = response.Rating.Rate,
                Count = response.Rating.Count
            }
        };

        return Ok(new ApiResponseWithData<GetProductResponse>
        {
            Success = true,
            Message = "Product retrieved successfully",
            Data = result
        });
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated product details.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var validator = new UpdateProductRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateProductCommand>(request);
        command.Id = id;
        var response = await _mediator.Send(command, cancellationToken);

        var result = new UpdateProductResponse
        {
            Id = response.Id,
            Title = response.Title,
            Price = response.Price,
            Description = response.Description,
            Category = response.Category,
            Image = response.Image,
            Rating = new UpdateProductRatingResponse
            {
                Rate = response.Rating.Rate,
                Count = response.Rating.Count
            }
        };

        return Ok(new ApiResponseWithData<UpdateProductResponse>
        {
            Success = true,
            Message = "Product updated successfully",
            Data = result
        });
    }

    /// <summary>
    /// Deletes a product by its ID.
    /// </summary>
    /// <param name="id">The product ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success response if deleted.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);
        await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Product deleted successfully"
        });
    }

    /// <summary>
    /// Retrieves all product categories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of category names.</returns>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(ApiResponseWithData<List<string>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var command = new GetCategoriesCommand();
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<List<string>>
        {
            Success = true,
            Message = "Categories retrieved successfully",
            Data = response.Categories
        });
    }

    /// <summary>
    /// Retrieves products by category with pagination.
    /// </summary>
    /// <param name="category">The category name.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="size">Page size (default: 10).</param>
    /// <param name="order">Ordering (e.g., "price desc").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of products in the category.</returns>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetProductsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductsByCategory(
        [FromRoute] string category,
        [FromQuery(Name = "_page")] int page = 1,
        [FromQuery(Name = "_size")] int size = 10,
        [FromQuery(Name = "_order")] string? order = null,
        CancellationToken cancellationToken = default)
    {
        var command = new GetProductsByCategoryCommand
        {
            Category = category,
            Page = page,
            Size = size,
            Order = order
        };
        var response = await _mediator.Send(command, cancellationToken);

        var result = new GetProductsResponse
        {
            Data = response.Data.Select(p => new GetProductsItemResponse
            {
                Id = p.Id,
                Title = p.Title,
                Price = p.Price,
                Description = p.Description,
                Category = p.Category,
                Image = p.Image,
                Rating = new GetProductsRatingResponse
                {
                    Rate = p.Rating.Rate,
                    Count = p.Rating.Count
                }
            }).ToList(),
            TotalItems = response.TotalItems,
            CurrentPage = response.CurrentPage,
            TotalPages = response.TotalPages
        };

        return Ok(new ApiResponseWithData<GetProductsResponse>
        {
            Success = true,
            Message = "Products retrieved successfully",
            Data = result
        });
    }
}
