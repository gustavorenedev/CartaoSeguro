using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CartaoSeguro.Application.Authentication.Request;

public class CreateUserRequest
{
    [JsonIgnore]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required]
    public string? Name { get; set; }
    [Required]
    [DataType(DataType.EmailAddress)]
    public string? Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
    [Required]
    public string? ConfirmPassword { get; set; }
}