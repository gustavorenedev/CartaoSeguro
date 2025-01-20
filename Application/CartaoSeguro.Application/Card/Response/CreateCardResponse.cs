using CartaoSeguro.Domain.Enum;

namespace CartaoSeguro.Application.Card.Response;

public class CreateCardResponse
{
    public string? Number { get; set; }
    public TypeCard? Type { get; set; }
    public string? CardFlag { get; set; }
    public DateTime ExpirationDate { get; set; }
    public Status Status { get; set; }
}
