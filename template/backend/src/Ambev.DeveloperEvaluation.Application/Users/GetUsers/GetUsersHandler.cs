using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Users.GetUsers;

/// <summary>
/// Handler for processing GetUsersCommand requests.
/// </summary>
public class GetUsersHandler : IRequestHandler<GetUsersCommand, GetUsersResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUsersHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<GetUsersResult> Handle(GetUsersCommand request, CancellationToken cancellationToken)
    {
        var (users, totalCount) = await _userRepository.GetAllAsync(
            request.Page,
            request.Size,
            request.Order,
            cancellationToken);

        var usersList = users.ToList();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.Size);

        return new GetUsersResult
        {
            Data = _mapper.Map<List<GetUsersItemResult>>(usersList),
            TotalItems = totalCount,
            CurrentPage = request.Page,
            TotalPages = totalPages
        };
    }
}
