using CartaoSeguro.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CartaoSeguro.Application.Card.Request;

public class CreateCardRequest
{
    [JsonIgnore]
    public Guid Id { get; set; }
    [JsonIgnore]
    public string? Number { get; set; }
    [Required]
    public TypeCard? Type { get; set; }
    [JsonIgnore]
    public int CVV { get; set; }
    [Required]
    public string? CardFlag { get; set; }
    [JsonIgnore]
    public DateTime ExpirationDate { get; set; }
    [JsonIgnore]
    public Status Status { get; set; }
    [Required]
    public string? UserId { get; set; }
}
