using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Booking.Api.DTOs;
using Xunit;

namespace Booking.Api.Tests;

public class PrenotazioniControllerTests : IClassFixture<BookingApiFactory>
{
    private readonly HttpClient _client;
    private readonly BookingApiFactory _factory;

    public PrenotazioniControllerTests(BookingApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", AuthTestHelper.GeneraTokenDiTest());

        _factory.CatalogClient.Spettacoli[1] = new SpettacoloDto(1, "Film Test", 1, DateTime.UtcNow);
        _factory.CatalogClient.Posti[10] = new PostoDto(10, 1, "A", 1);
        _factory.CatalogClient.Posti[11] = new PostoDto(11, 1, "A", 2);
        _factory.CatalogClient.Posti[12] = new PostoDto(12, 1, "A", 3);
    }

    [Fact]
    public async Task CreaPrenotazioneMultipla_TuttiPostiLiberi_LiPrenotaTutti()
    {
        var request = new PrenotazioneMultiplaRequest(1, "Mario Rossi", new List<int> { 10, 11 });
        var response = await _client.PostAsJsonAsync("/api/prenotazioni/multiple", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var prenotazione = await response.Content.ReadFromJsonAsync<PrenotazioneDto>();
        Assert.Equal(new List<int> { 10, 11 }, prenotazione!.PostiIds);
    }

    [Fact]
    public async Task CreaPrenotazioneMultipla_UnPostoGiaOccupato_NonPrenotaNessunoDeiDue()
    {
        // primo utente prenota il posto 12
        await _client.PostAsJsonAsync("/api/prenotazioni/multiple",
            new PrenotazioneMultiplaRequest(1, "Primo Cliente", new List<int> { 12 }));

        // secondo utente prova a prenotare il posto 12 (occupato) e un posto nuovo, libero
        _factory.CatalogClient.Posti[13] = new PostoDto(13, 1, "B", 1);
        var response = await _client.PostAsJsonAsync("/api/prenotazioni/multiple",
            new PrenotazioneMultiplaRequest(1, "Secondo Cliente", new List<int> { 12, 13 }));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        // verifica che il posto 13 non sia stato prenotato nonostante fosse nella stessa richiesta
        var disponibilita = await _client.GetFromJsonAsync<List<DisponibilitaPostoDto>>("/api/spettacoli/1/disponibilita");
        var posto13 = disponibilita!.First(p => p.PostoId == 13);
        Assert.False(posto13.Occupato);
    }
}