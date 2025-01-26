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
        _smtpClient = new SmtpClient("sandbox.smtp.mailtrap.io")
        {
            Port = 587,
            Credentials = new NetworkCredential("5697ff1d223362", "1740b04f7b1b8e"),
            EnableSsl = true
        };
    }

    public async Task SendEmailAsync(CardAndUser cardAndUserRequest)
    {
        if (string.IsNullOrWhiteSpace(cardAndUserRequest.User!.Email))
            throw new ArgumentException("Email não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(cardAndUserRequest.User.Name))
            throw new ArgumentException("Nome não pode ser vazio.");

        var token = GenerateToken();

        var emailBody = $@"
        <html>
        <body>
            <h1>Token de Verificação</h1>
            <p>Olá {cardAndUserRequest.User.Name},</p>
            <p>Seu token de verificação é: <strong>{token}</strong></p>
            <p>Últimos 4 dígitos do cartão: {Last4Digits(cardAndUserRequest.Card!.Number!)}</p>
        </body>
        </html>";

        var mailMessage = new MailMessage("cartaoseguro@cartaoseguro.com", cardAndUserRequest.User.Email)
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

    private static string Last4Digits(string cardNumber)
    {
        return cardNumber.Substring(cardNumber.Length - 4);
    }
}