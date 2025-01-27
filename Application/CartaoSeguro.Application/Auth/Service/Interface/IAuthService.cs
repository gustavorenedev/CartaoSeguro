using CartaoSeguro.Application.Auth.Request;
using CartaoSeguro.Application.Auth.Response;

namespace CartaoSeguro.Application.Auth.Service.Interface;

public interface IAuthService
{
    Task<string> LoginAsync(LoginUserRequest loginUserRequest);
    Task<CreateUserResponse> RegisterAsync(CreateUserRequest userRequest);
}
