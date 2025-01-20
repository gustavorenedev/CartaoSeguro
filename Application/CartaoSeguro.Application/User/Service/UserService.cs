using AutoMapper;
using CartaoSeguro.Application.User.Request;
using CartaoSeguro.Application.User.Response;
using CartaoSeguro.Application.User.Service.Interface;
using CartaoSeguro.Domain.User.Interface;

namespace CartaoSeguro.Application.User.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserByEmailResponse> FindUserByEmail(UserByEmailRequest userRequest)
    {
        if (string.IsNullOrEmpty(userRequest.Email))
            throw new InvalidOperationException("Email is required.");

        var user = await _userRepository.GetByEmailAsync(userRequest.Email);

        return _mapper.Map<UserByEmailResponse>(user);
    }
}
