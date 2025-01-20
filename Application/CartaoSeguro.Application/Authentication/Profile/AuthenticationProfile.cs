using CartaoSeguro.Application.Authentication.Request;
using CartaoSeguro.Application.Authentication.Response;

namespace CartaoSeguro.Application.Authentication.Profile;

public class AuthenticationProfile : AutoMapper.Profile
{
    public AuthenticationProfile()
    {
        CreateMap<CreateUserRequest, Domain.User.User>();
        CreateMap<CreateUserResponse, Domain.User.User>();
        CreateMap<Domain.User.User, CreateUserRequest>();
        CreateMap<Domain.User.User, CreateUserResponse>();
    }
}
