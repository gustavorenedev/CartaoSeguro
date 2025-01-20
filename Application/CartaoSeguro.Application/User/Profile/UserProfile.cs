using CartaoSeguro.Application.User.Response;

namespace CartaoSeguro.Application.User.Profile;

public class UserProfile : AutoMapper.Profile
{
    public UserProfile()
    {
        CreateMap<Domain.User.User, UserByEmailResponse>();
    }
}
