using AutoMapper;
using CartaoSeguro.Application.User.Request;
using CartaoSeguro.Application.User.Response;
using CartaoSeguro.Application.User.Service.Interface;
using CartaoSeguro.Domain.Card.Interface;
using CartaoSeguro.Domain.User.Interface;

namespace CartaoSeguro.Application.User.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ICardRepository _cardRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper, ICardRepository cardRepository)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _cardRepository = cardRepository;
    }

    public async Task<CardsByUserResponse> FindCardsByUser(CardsByUserRequest userRequest)
    {
        if (string.IsNullOrEmpty(userRequest.Email))
            throw new InvalidOperationException("Email is required.");

        var user = await _userRepository.GetByEmailAsync(userRequest.Email);

        if (user != null)
        {
            var cards = await _cardRepository.GetCardsByUserAsync(userRequest.Email) ?? new List<Domain.Card.Card>();

            return _mapper.Map<CardsByUserResponse>(cards);
        }

        throw new KeyNotFoundException("User not found.");
    }
}
