using System.ComponentModel.DataAnnotations;

namespace CartaoSeguro.Application.Card.Request;

public class CardByIdRequest
{
    [Required]
    public string? Id { get; set; }
}
