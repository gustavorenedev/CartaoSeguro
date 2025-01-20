using CartaoSeguro.Application.Authentication.Profile;
using CartaoSeguro.Application.Authentication.Service;
using CartaoSeguro.Application.Authentication.Service.Interface;
using CartaoSeguro.Application.Card.Service;
using CartaoSeguro.Application.Card.Service.Interface;
using CartaoSeguro.Application.User.Service;
using CartaoSeguro.Application.User.Service.Interface;
using CartaoSeguro.Domain.Card.Interface;
using CartaoSeguro.Domain.User.Interface;
using CartaoSeguro.Infrastructure.Configuration;
using CartaoSeguro.Infrastructure.Persistence.DbContext;
using CartaoSeguro.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB Configuration
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("DatabaseSettings"));

builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new ApplicationDbContext(settings.ConnectionString!, settings.DatabaseName!);
});

builder.Services.AddAutoMapper(typeof(AuthenticationProfile));

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICardService, CardService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
