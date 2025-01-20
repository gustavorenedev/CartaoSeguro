using System.ComponentModel.DataAnnotations;

namespace CartaoSeguro.Application.User.Request;

public class CardsByUserRequest
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
}
