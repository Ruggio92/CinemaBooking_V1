//DTO per il token dell'autenticazione

namespace Catalog.Api.DTOs;

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, DateTime ScadeIl);