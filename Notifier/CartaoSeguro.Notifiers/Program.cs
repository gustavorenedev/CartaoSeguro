using CartaoSeguro.Notifiers.Email.Service;
using CartaoSeguro.Notifiers.Email.Service.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CartaoSeguro.Notifiers;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddRazorTemplating();
                services.AddScoped<IEmailService, EmailService>();
                services.AddHostedService<Worker>();
            });
}
