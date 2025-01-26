namespace CartaoSeguro.Notifiers.DTOs;

public class CardAndUser
{
    public Domain.Card.Card? Card { get; set; }
    public Domain.User.User? User { get; set; }
}
