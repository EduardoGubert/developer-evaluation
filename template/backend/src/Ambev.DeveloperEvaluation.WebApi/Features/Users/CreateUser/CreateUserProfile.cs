using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;

/// <summary>
/// Profile for mapping between Application and API CreateUser responses
/// </summary>
public class CreateUserProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CreateUser feature
    /// </summary>
    public CreateUserProfile()
    {
        CreateMap<CreateUserRequest, CreateUserCommand>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new CreateUserNameDto
            {
                Firstname = src.Name.Firstname,
                Lastname = src.Name.Lastname
            }))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new CreateUserAddressDto
            {
                City = src.Address.City,
                Street = src.Address.Street,
                Number = src.Address.Number,
                Zipcode = src.Address.Zipcode,
                Geolocation = new CreateUserGeolocationDto
                {
                    Lat = src.Address.Geolocation.Lat,
                    Long = src.Address.Geolocation.Long
                }
            }));

        CreateMap<CreateUserResult, CreateUserResponse>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new CreateUserNameResponse
            {
                Firstname = src.Name.Firstname,
                Lastname = src.Name.Lastname
            }))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new CreateUserAddressResponse
            {
                City = src.Address.City,
                Street = src.Address.Street,
                Number = src.Address.Number,
                Zipcode = src.Address.Zipcode,
                Geolocation = new CreateUserGeolocationResponse
                {
                    Lat = src.Address.Geolocation.Lat,
                    Long = src.Address.Geolocation.Long
                }
            }));
    }
}
