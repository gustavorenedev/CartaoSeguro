using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace CartaoSeguro.Domain.User;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; } = Guid.NewGuid();
    [BsonElement("Name")]
    public string? Name { get; set; }
    [BsonElement("Email")]
    public string? Email { get; set; }
    [BsonElement("Password")]
    public string? Password { get; set; }
    [BsonElement("Cards")]
    public IEnumerable<string> Cards { get; set; } = new List<string>();
}
