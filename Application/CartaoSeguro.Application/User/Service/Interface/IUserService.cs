using CartaoSeguro.Application.User.Request;
using CartaoSeguro.Application.User.Response;

namespace CartaoSeguro.Application.User.Service.Interface;

public interface IUserService
{
    Task<UserByEmailResponse> FindUserByEmail(UserByEmailRequest userRequest);
}
