// controller che gestisce l'autenticazione JWT

using Catalog.Api.DTOs;
using Catalog.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly TokenService _tokenService;

    public AuthController(IConfiguration config, TokenService tokenService)
    {
        _config = config;
        _tokenService = tokenService;
    }

    [HttpPost("token")]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        var usernameAtteso = _config["Auth:Username"];
        var passwordAttesa = _config["Auth:Password"];

        if (request.Username != usernameAtteso || request.Password != passwordAttesa)
            return Unauthorized("Credenziali non valide.");

        var token = _tokenService.GeneraToken(request.Username);
        var scadenza = DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiryMinutes"] ?? "60"));

        return Ok(new LoginResponse(token, scadenza));
    }
}