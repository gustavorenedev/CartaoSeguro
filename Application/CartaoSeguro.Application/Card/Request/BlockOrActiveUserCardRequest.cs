using System.ComponentModel.DataAnnotations;
using CartaoSeguro.Domain.Enum;

namespace CartaoSeguro.Application.Card.Request;

public class BlockOrActiveUserCardRequest
{
    [Required]
    public string? Number { get; set; }
    [Required]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    [Required]
    public Status? Act { get; set; }
}
