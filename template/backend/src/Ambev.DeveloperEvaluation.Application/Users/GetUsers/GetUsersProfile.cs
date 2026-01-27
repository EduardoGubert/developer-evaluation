using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Users.GetUsers;

/// <summary>
/// Profile for mapping User entity to GetUsersItemResult.
/// </summary>
public class GetUsersProfile : Profile
{
    public GetUsersProfile()
    {
        CreateMap<User, GetUsersItemResult>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new GetUsersNameResult
            {
                Firstname = src.Firstname,
                Lastname = src.Lastname
            }))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new GetUsersAddressResult
            {
                City = src.City,
                Street = src.Street,
                Number = src.Number,
                Zipcode = src.Zipcode,
                Geolocation = new GetUsersGeolocationResult
                {
                    Lat = src.Latitude,
                    Long = src.Longitude
                }
            }))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));
    }
}
