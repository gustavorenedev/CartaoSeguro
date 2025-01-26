using CartaoSeguro.Notifiers.DTOs;

namespace CartaoSeguro.Notifiers.Email.Service.Interface;

public interface IEmailService
{
    Task SendEmailAsync(CardAndUser cardAndUserRequest);
}
