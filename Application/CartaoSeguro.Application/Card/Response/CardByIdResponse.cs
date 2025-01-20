using CartaoSeguro.Domain.Enum;

namespace CartaoSeguro.Application.Card.Response;

public class CardByIdResponse
{
    public Guid Id { get; set; }
    public string? Number { get; set; }
    public TypeCard? Type { get; set; }
    public int CVV { get; set; }
    public string? CardFlag { get; set; }
    public DateTime ExpirationDate { get; set; }
    public Status Status { get; set; }
    public string? UserId { get; set; }
}
