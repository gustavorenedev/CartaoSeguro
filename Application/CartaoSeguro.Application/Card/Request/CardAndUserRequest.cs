using CartaoSeguro.Domain.Enum;

namespace CartaoSeguro.Application.Card.Request;

public class CardAndUserRequest
{
    public Domain.Card.Card? Card { get; set; }
    public Domain.User.User? User { get; set; }
    public Status? Act { get; set; }
    public int? Token { get; set; }
}
