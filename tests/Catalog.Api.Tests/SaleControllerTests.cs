using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Catalog.Api.DTOs;
using Xunit;

namespace Catalog.Api.Tests;

public class SaleControllerTests : IClassFixture<CatalogApiFactory>
{
    private readonly HttpClient _client;
    private readonly CatalogApiFactory _factory;

    public SaleControllerTests(CatalogApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthTestHelper.GeneraTokenDiTest());
    }

    [Fact]
    public async Task GetSale_SenzaToken_Restituisce401()
    {
        using var clientSenzaAuth = _factory.CreateClient();
        var response = await clientSenzaAuth.GetAsync("/api/sale");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreaSala_ConDatiValidi_Restituisce201ELaCreaDavvero()
    {
        var response = await _client.PostAsJsonAsync("/api/sale", new CreaSalaRequest("Sala Test"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var sala = await response.Content.ReadFromJsonAsync<SalaDto>();
        Assert.NotNull(sala);
        Assert.Equal("Sala Test", sala!.Nome);
    }

    [Fact]
    public async Task CreaPosto_StessaFilaENumeroDellaStessaSala_Restituisce409()
    {
        var sala = await (await _client.PostAsJsonAsync("/api/sale", new CreaSalaRequest("Sala Conflitto"))).Content.ReadFromJsonAsync<SalaDto>();

        var primo = await _client.PostAsJsonAsync($"/api/sale/{sala!.Id}/posti", new CreaPostoRequest("A", 1));
        Assert.Equal(HttpStatusCode.Created, primo.StatusCode);

        var duplicato = await _client.PostAsJsonAsync($"/api/sale/{sala.Id}/posti", new CreaPostoRequest("A", 1));
        Assert.Equal(HttpStatusCode.Conflict, duplicato.StatusCode);
    }
}