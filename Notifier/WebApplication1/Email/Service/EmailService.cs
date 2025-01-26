using CartaoSeguro.Notifiers.DTOs;
using CartaoSeguro.Notifiers.Email.Model;
using CartaoSeguro.Notifiers.Email.Service.Interface;
using Razor.Templating.Core;
using System.Net;
using System.Net.Mail;

namespace CartaoSeguro.Notifiers.Email.Service;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;

    public EmailService()
    {
        _smtpClient = new SmtpClient("smtp.freesmtpservers.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("teste@yahoo.com", ""),
            EnableSsl = true
        };
    }

    public async Task SendEmailAsync(CardAndUser cardAndUserRequest)
    {
        if (string.IsNullOrWhiteSpace(cardAndUserRequest.User!.Email))
            throw new ArgumentException("Email não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(cardAndUserRequest.User.Name))
            throw new ArgumentException("Nome não pode ser vazio.");

        if (cardAndUserRequest.Card!.Number!.Length != 4)
            throw new ArgumentException("Os últimos 4 dígitos do cartão são inválidos.");

        var token = GenerateToken();

        var emailBody = await RazorTemplateEngine.RenderAsync("Views/Emails/EmailTemplate", new EmailModel
        {
            Name = cardAndUserRequest.User.Name,
            CardLast4Digits = cardAndUserRequest.Card.Number,
            Token = token
        });

        var mailMessage = new MailMessage("teste@yahoo.com", cardAndUserRequest.User.Email)
        {
            Subject = "Token de Verificação",
            Body = emailBody,
            IsBodyHtml = true
        };

        await _smtpClient.SendMailAsync(mailMessage);
    }

    private static string GenerateToken()
    {
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }
}