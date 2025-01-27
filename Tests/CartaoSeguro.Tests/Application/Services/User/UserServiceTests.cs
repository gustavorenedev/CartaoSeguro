using AutoMapper;
using CartaoSeguro.Application.User.Request;
using CartaoSeguro.Application.User.Response;
using CartaoSeguro.Application.User.Service;
using CartaoSeguro.Domain.User.Interface;
using Moq;

namespace CartaoSeguro.Tests.Application.Services.User;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IMapper> _mapper;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mapper = new Mock<IMapper>();

        _userService = new UserService(_mockUserRepository.Object, _mapper.Object);
    }

    [Fact(DisplayName = "FindUserByEmail - Should return UserByEmailResponse when valid email is provided")]
    [Trait("Category", "UserService")]
    public async Task FindUserByEmail_ShouldReturnUserByEmailResponse_WhenValidEmailIsProvided()
    {
        // Arrange
        var userRequest = new UserByEmailRequest { Email = "user@example.com" };

        var user = new Domain.User.User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            Name = "John Doe"
        };

        var userResponse = new UserByEmailResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name
        };

        _mockUserRepository.Setup(repo => repo.GetByEmailAsync(userRequest.Email))
                          .ReturnsAsync(user);

        _mapper.Setup(m => m.Map<UserByEmailResponse>(user))
                  .Returns(userResponse);

        // Act
        var result = await _userService.FindUserByEmail(userRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userResponse.Id, result.Id);
        Assert.Equal(userResponse.Email, result.Email);
        Assert.Equal(userResponse.Name, result.Name);
    }

    [Fact(DisplayName = "FindUserByEmail - Should throw InvalidOperationException when email is not provided")]
    [Trait("Category", "UserService")]
    public async Task FindUserByEmail_ShouldThrowInvalidOperationException_WhenEmailIsNotProvided()
    {
        // Arrange
        var userRequest = new UserByEmailRequest { Email = "" };

        var userRepositoryMock = new Mock<IUserRepository>();
        var mapperMock = new Mock<IMapper>();

        var userService = new UserService(userRepositoryMock.Object, mapperMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => userService.FindUserByEmail(userRequest));
        Assert.Equal("Email is required.", exception.Message);
    }

}
