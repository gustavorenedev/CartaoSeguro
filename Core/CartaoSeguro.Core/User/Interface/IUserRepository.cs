namespace CartaoSeguro.Domain.User.Interface;

public interface IUserRepository
{
    Task<User> AddAsync(User user);
    Task<User> GetByEmailAsync(string email);
}
