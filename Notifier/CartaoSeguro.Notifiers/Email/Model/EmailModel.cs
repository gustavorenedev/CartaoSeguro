namespace CartaoSeguro.Notifiers.Email.Model;

public class EmailModel
{
    public string? Name { get; set; }
    public string? CardLast4Digits { get; set; }
    public string? Token { get; set; }
}
