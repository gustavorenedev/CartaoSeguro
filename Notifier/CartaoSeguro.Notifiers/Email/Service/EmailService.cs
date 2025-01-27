using System.Net;
using System.Net.Mail;
using CartaoSeguro.Notifiers.DTOs;
using CartaoSeguro.Notifiers.Email.Service.Interface;
using Microsoft.Extensions.Configuration;

namespace CartaoSeguro.Notifiers.Email.Service;

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient;

    public EmailService(IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection("EmailSettings");

        var host = emailSettings["Host"];
        var port = int.Parse(emailSettings["Port"]!);
        var username = emailSettings["Username"];
        var password = emailSettings["Password"];
        var enableSsl = bool.Parse(emailSettings["EnableSsl"]!);

        _smtpClient = new SmtpClient(host)
        {
            Port = port,
            Credentials = new NetworkCredential(username, password),
            EnableSsl = enableSsl
        };
    }

    public async Task SendEmailAsync(CardAndUser cardAndUserRequest)
    {
        if (string.IsNullOrWhiteSpace(cardAndUserRequest.User!.Email))
            throw new ArgumentException("Email não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(cardAndUserRequest.User.Name))
            throw new ArgumentException("Nome não pode ser vazio.");

        var emailBody = GenerateEmailBody(cardAndUserRequest);

        var mailMessage = new MailMessage("cartaoseguro@cartaoseguro.com", cardAndUserRequest.User.Email)
        {
            Subject = "Token de Verificação",
            Body = emailBody,
            IsBodyHtml = true
        };

        await _smtpClient.SendMailAsync(mailMessage);
    }

    private static string Last4Digits(string cardNumber)
    {
        return cardNumber.Substring(cardNumber.Length - 4);
    }

    private static string GenerateEmailBody(CardAndUser cardAndUserRequest)
    {
        return $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        margin: 0;
                        padding: 0;
                        background-color: #f4f4f4;
                    }}
                    .email-container {{
                        max-width: 600px;
                        margin: 20px auto;
                        background-color: #ffffff;
                        border-radius: 8px;
                        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
                        overflow: hidden;
                    }}
                    .email-header {{
                        background-color: #0073e6;
                        color: #ffffff;
                        text-align: center;
                        padding: 20px;
                    }}
                    .email-header h1 {{
                        margin: 0;
                        font-size: 24px;
                    }}
                    .email-body {{
                        padding: 20px;
                        color: #333333;
                    }}
                    .email-body p {{
                        margin: 10px 0;
                        line-height: 1.6;
                    }}
                    .email-body strong {{
                        color: #0073e6;
                    }}
                    .email-footer {{
                        text-align: center;
                        padding: 10px;
                        background-color: #f4f4f4;
                        color: #777777;
                        font-size: 12px;
                    }}
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <div class='email-header'>
                        <h1>Token de Verificação</h1>
                    </div>
                    <div class='email-body'>
                        <p>Olá {cardAndUserRequest.User!.Name},</p>
                        <p>Seu token de verificação é: <strong>{cardAndUserRequest.Token.ToString()}</strong></p>
                        <p>Os 4 últimos dígitos do cartão: <strong>{Last4Digits(cardAndUserRequest.Card!.Number!)}</strong></p>
                    </div>
                    <div class='email-footer'>
                        <p>© 2025 Cartão Seguro. Todos os direitos reservados.</p>
                    </div>
                </div>
            </body>
            </html>";
    }
}