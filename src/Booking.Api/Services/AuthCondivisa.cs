// questo servizio è necessario per mantenere l'autenticazione per entrambi i container

namespace Booking.Api.Services;

public class AuthCondivisa : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthCondivisa(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();

        if (!string.IsNullOrEmpty(authHeader))
            request.Headers.TryAddWithoutValidation("Authorization", authHeader);

        return base.SendAsync(request, cancellationToken);
    }
}