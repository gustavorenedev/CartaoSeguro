using CartaoSeguro.Application.Authentication.Request;
using CartaoSeguro.Application.Authentication.Response;

namespace CartaoSeguro.Application.Authentication.Service.Interface;

public interface IAuthService
{
    Task<string> LoginAsync(LoginUserRequest loginUserRequest);
    Task<CreateUserResponse> RegisterAsync(CreateUserRequest userRequest);
}
