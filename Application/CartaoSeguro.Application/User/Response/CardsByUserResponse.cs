using CartaoSeguro.Application.Card.Response;

namespace CartaoSeguro.Application.User.Response;

public class CardsByUserResponse
{
    public List<CardResponse>? Cards { get; set; }
}
