using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Users.GetUsers;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUsers;

/// <summary>
/// Profile for mapping between Application and API GetUsers responses
/// </summary>
public class GetUsersProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetUsers feature
    /// </summary>
    public GetUsersProfile()
    {
        CreateMap<GetUsersRequest, GetUsersCommand>();
        CreateMap<GetUsersResult, GetUsersResponse>();
        CreateMap<GetUsersItemResult, GetUsersItemResponse>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new GetUsersNameResponse
            {
                Firstname = src.Name.Firstname,
                Lastname = src.Name.Lastname
            }))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new GetUsersAddressResponse
            {
                City = src.Address.City,
                Street = src.Address.Street,
                Number = src.Address.Number,
                Zipcode = src.Address.Zipcode,
                Geolocation = new GetUsersGeolocationResponse
                {
                    Lat = src.Address.Geolocation.Lat,
                    Long = src.Address.Geolocation.Long
                }
            }));
    }
}
