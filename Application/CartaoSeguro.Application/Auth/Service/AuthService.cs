using AutoMapper;
using CartaoSeguro.Application.Authentication.Request;
using CartaoSeguro.Application.Authentication.Response;
using CartaoSeguro.Application.Authentication.Service.Interface;
using CartaoSeguro.Domain.User.Interface;

namespace CartaoSeguro.Application.Authentication.Service;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public AuthService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<string> LoginAsync(LoginUserRequest loginUserRequest)
    {
        if (string.IsNullOrEmpty(loginUserRequest.Email) || string.IsNullOrEmpty(loginUserRequest.Password))
        {
            return "Email and password are required.";
        }

        var user = await _userRepository.GetByEmailAsync(loginUserRequest.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginUserRequest.Password, user.Password))
        {
            return "Invalid email or password.";
        }

        return "Login successful";
    }

    public async Task<CreateUserResponse> RegisterAsync(CreateUserRequest userRequest)
    {
        ValidateRequest(userRequest);

        if (await IsEmailInUseAsync(userRequest.Email!))
            throw new InvalidOperationException("Email already in use.");

        var createUser = PrepareUser(userRequest);

        var userCreated = await _userRepository.AddAsync(createUser);

        return _mapper.Map<CreateUserResponse>(userCreated);
    }

    private static void ValidateRequest(CreateUserRequest userRequest)
    {
        if (string.IsNullOrEmpty(userRequest.Email) || string.IsNullOrEmpty(userRequest.Password))
            throw new InvalidOperationException("Email and password are required.");

        if (userRequest.Password != userRequest.ConfirmPassword)
            throw new InvalidOperationException("Confirmation password not supported.");
    }

    private async Task<bool> IsEmailInUseAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email) != null;
    }

    private Domain.User.User PrepareUser(CreateUserRequest userRequest)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRequest.Password);

        var userWithHashedPassword = new CreateUserRequest
        {
            Name = userRequest.Name,
            Email = userRequest.Email,
            Password = hashedPassword
        };

        return _mapper.Map<Domain.User.User>(userWithHashedPassword);
    }
}
