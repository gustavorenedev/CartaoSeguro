using CartaoSeguro.Application.User.Request;
using CartaoSeguro.Application.User.Response;
using CartaoSeguro.Application.User.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CartaoSeguro.API.Controllers.User;

/// <summary>
/// Controlador para procurar informações sobre o usuário.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Construtor do UserController.
    /// </summary>
    /// <param name="userService">Serviço para busca de informações do usuário.</param>
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Endpoint para buscar usuário pelo email.
    /// </summary>
    /// <param name="userRequest">Email do usuário a ser buscado.</param>
    /// <returns>Informações básicas do usuário.</returns>
    /// <response code="200">Consulta realizada com sucesso.</response>
    /// <response code="400">Erro de validação nos dados enviados.</response>
    [HttpGet("FindUserByEmail")]
    [ProducesResponseType(typeof(UserByEmailResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> FindUserByEmail([FromQuery] UserByEmailRequest userRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest("Dados inválidos.");

        var response = await _userService.FindUserByEmail(userRequest);

        return Ok(response);
    }
}
