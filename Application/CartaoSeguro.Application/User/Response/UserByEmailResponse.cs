namespace CartaoSeguro.Application.User.Response;

public class UserByEmailResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}
