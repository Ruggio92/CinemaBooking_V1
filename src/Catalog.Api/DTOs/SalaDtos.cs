// DTO per la sala: quello che torna dalle api e quello che serve per crearne una nuova

namespace Catalog.Api.DTOs;

public record SalaDto(int Id, string Nome);

public record CreaSalaRequest(string Nome);