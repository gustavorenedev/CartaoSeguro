using System.ComponentModel.DataAnnotations;

namespace CartaoSeguro.Application.Card.Request;

public class BlockUserCardRequest
{
    [Required]
    public string? Number { get; set; }
    [Required]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
}
