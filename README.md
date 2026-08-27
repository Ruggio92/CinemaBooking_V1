## Architettura

Ho diviso il sistema in due servizi:

- Catalog.Api: sale, spettacoli, posti. Dati che non cambiano, o cambiano di poco
- Booking.Api: prenotazioni vere e proprie. Qui c'è la parte dedicata ai controlli che vengono fatti automaticamente (ad esempio quando si prova a prenotare un posto già prenotato)

Ciascun container ha il suo database. Quando Booking riceve una richiesta di prenotazione, prima chiama Catalog per controllare che il posto/spettacolo esista davvero, poi scrive sul proprio DB

## Struttura

```
CinemaBooking.sln
docker-compose.yml
src/
  Catalog.Api/     -> Sala, Posto, Spettacolo
  Booking.Api/     -> Prenotazione, PostoPrenotato
```

## API

Catalog.Api:
- GET /api/sale - elenco sale
- GET /api/sale/{id}
- GET /api/sale/{salaId}/posti
- GET /api/spettacoli - elenco
- GET /api/spettacoli/{id}
- GET /api/posti/{id}

Booking.Api:
- POST /api/prenotazioni - crea prenotazione. Nel body: SpettacoloId, NomeCliente, e PostoId opzionale (se non lo metto, il sistema mi assegna il primo posto libero)
- GET /api/prenotazioni/{id}
- DELETE /api/prenotazioni/{id} - cancella, libera i posti
- GET /api/spettacoli/{spettacoloId}/disponibilita - posti liberi/occupati

Codici di errore: 404 se spettacolo/posto non esiste (controllato via Catalog), 409 se il posto è già stato prenotato da qualcun altro nel frattempo.

## Istruzioni per avviare l'applicazione

Serve avere l'app Docker installata e attiva. Poi, dalla cartella principale:

```bash
docker compose up --build
```

Parte tutto: SQL Server sulla 1433, Catalog.Api su http://localhost:5080 , Booking.Api su http://localhost:5081. Non serve lanciare `dotnet ef database update` a mano: le migration le applica ogni servizio all'avvio (l'ho spostato dentro `Program.cs` apposta, così basta il comando sopra e non c'è bisogno di avere `dotnet-ef` installato per provare il progetto).

## Appunti

Due servizi separati --> Catalogo e Prenotazioni cambiano in maniera diversa: il catalogo è quasi statico, mentre le prenotazioni sono ad alta scrittura. Ha senso tenerli separati, anche solo per poterli gestire in modo indipendente

Due DB separati, non uno condiviso --> Se condividessi il database tra i due servizi, avrei un blocco unico con più deploy separati e un cambio di schema in un servizio romperebbe l'altro

IDPosto e IDSpettacolo in Booking non sono FK --> Puntano a righe che stanno fisicamente in un altro database, quindi la validazione la faccio con una chiamata HTTP a Catalog al momento della prenotazione

Prenotazione dello stesso posto --> Ho messo un indice univoco sulla coppia "IDSpettacolo, IDPosto", nella tabella PostiPrenotati. Se arrivano due richieste di prenotazione dello stesso posto in contemporanea, la seconda INSERT viene rifiutata dal database e restituisco un errore 409

Cancellazione del PostoPrenotato --> Alla cancellazione della prenotazione elimino il PostoPrenotato invece di marcarlo come "cancellato", così il posto torna subito libero. La Prenotazione invece resta a sistema con Stato = Cancellata, per tenere traccia dello storico

DTO separati dai Model tra i due servizi --> Booking non usa mai direttamente le classi di Catalog, ha i suoi DTO che rispecchiano quello che arriva via JSON. Così il collegamento tra i due è il JSON che viaggia in rete, non codice condiviso. In questo modo posso deployare Catalog senza dover ricompilare Booking insieme. L'unica cosa da ricordasi è di tenere allineati i due DTO.

ICatalogClient come interfaccia --> Serve per i test, in questo modo posso simulare le risposte di Catalog senza doverlo avviare per davvero






Comandi per Docker:

bash
docker compose up --build       # avvia tutto (build incluso)
docker compose down              # ferma tutto, tiene i dati
docker compose down -v           # ferma tutto E cancella i volumi (riparte pulito)
docker compose up sqlserver      # avvia solo SQL Server, utile per lavorare con i breakpoint

Vedere cosa sta succedendo:

bash
docker ps                        # lista dei container attivi
docker logs cinema-sqlserver     # log di un container specifico

Entrare nel database:

bash
docker exec -it cinema-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'Paperino123!' -C

Se invece sono già dentro il container:

bash
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'Paperino123!' -C

Query utili:

SELECT name FROM sys.databases;
GO

USE CatalogDb o BookingDb;
GO

SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES;
GO

SELECT * FROM Sale;
GO

INSERT INTO Sale (Nome) VALUES ('Sala 1');
GO