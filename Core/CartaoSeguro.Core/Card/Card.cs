using CartaoSeguro.Domain.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CartaoSeguro.Domain.Card;

public class Card
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    [BsonElement("Number")]
    public string? Number { get; set; }
    [BsonElement("Type")]
    public TypeCard? Type { get; set; }
    [BsonElement("CVV")]
    public int CVV { get; set; }
    [BsonElement("CardFlag")]
    public string? CardFlag { get; set; }
    [BsonElement("ExpirationDate")]
    public DateTime ExpirationDate { get; set; }
    [BsonElement("Status")]
    public Status? Status { get; set; }
    [BsonElement("UserId")]
    public string? UserId { get; set; }
}