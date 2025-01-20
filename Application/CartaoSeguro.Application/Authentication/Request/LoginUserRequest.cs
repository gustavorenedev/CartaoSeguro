using System.ComponentModel.DataAnnotations;

namespace CartaoSeguro.Application.Authentication.Request;

public class LoginUserRequest
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
}
