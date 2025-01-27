using CartaoSeguro.Application.Auth.Request;
using CartaoSeguro.Application.Auth.Response;

namespace CartaoSeguro.Application.Auth.Profile;

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
