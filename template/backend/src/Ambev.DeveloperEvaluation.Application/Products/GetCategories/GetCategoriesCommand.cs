using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.GetCategories;

/// <summary>
/// Command for retrieving all product categories.
/// </summary>
public class GetCategoriesCommand : IRequest<GetCategoriesResult>
{
}
