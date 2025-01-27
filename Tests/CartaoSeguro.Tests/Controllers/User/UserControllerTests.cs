using CartaoSeguro.API.Controllers.User;
using CartaoSeguro.Application.User.Request;
using CartaoSeguro.Application.User.Response;
using CartaoSeguro.Application.User.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CartaoSeguro.Tests.Controllers.User;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly UserController _userController;

    public UserControllerTests()
    {
        _mockUserService = new Mock<IUserService>();

        _userController = new UserController(_mockUserService.Object);
    }

    [Fact(DisplayName = "FindUserByEmail - Should return OK when valid email is provided")]
    [Trait("Category", "UserController")]
    public async Task FindUserByEmail_ShouldReturnOk_WhenValidEmailIsProvided()
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

        _mockUserService.Setup(service => service.FindUserByEmail(userRequest))
                       .ReturnsAsync(userResponse);

        // Act
        var result = await _userController.FindUserByEmail(userRequest);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<UserByEmailResponse>(okResult.Value);
        Assert.Equal(userResponse.Id, returnValue.Id);
        Assert.Equal(userResponse.Email, returnValue.Email);
        Assert.Equal(userResponse.Name, returnValue.Name);
    }

    [Fact(DisplayName = "FindUserByEmail - Should return BadRequest when invalid data is provided")]
    [Trait("Category", "UserController")]
    public async Task FindUserByEmail_ShouldReturnBadRequest_WhenInvalidDataIsProvided()
    {
        // Arrange
        var userRequest = new UserByEmailRequest { Email = "" };

        _userController.ModelState.AddModelError("Email", "O campo 'Email' é obrigatório.");

        // Act
        var result = await _userController.FindUserByEmail(userRequest);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Dados inválidos.", badRequestResult.Value);
    }
}