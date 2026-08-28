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

Tutte le rotte richiedono un token JWT, tranne il login (vedi sezione Autenticazione)

Catalog.Api:
- POST /api/auth/token - login, restituisce il token
- GET /api/sale - elenco sale
- GET /api/sale/{id}
- POST /api/sale - crea una sala
- GET /api/sale/{salaId}/posti
- POST /api/sale/{salaId}/posti - aggiunge un posto a una sala
- GET /api/spettacoli - elenco
- GET /api/spettacoli/{id}
- POST /api/spettacoli - crea uno spettacolo, deve appartenere a una sala esistente
- GET /api/posti/{id}

Booking.Api:
- POST /api/prenotazioni - crea prenotazione. Nel body: SpettacoloId, NomeCliente, e PostoId opzionale (se non lo metto, il sistema mi assegna il primo posto libero)
- POST /api/prenotazioni/multiple - crea una prenotazione con più posti insieme, per lo stesso spettacolo. Nel body: SpettacoloId, NomeCliente, PostiIds (lista). Se anche solo uno dei posti è già occupato, non prenota nessuno dei posti richiesti e dice quale posto ha dato conflitto
- GET /api/prenotazioni/{id}
- DELETE /api/prenotazioni/{id} - cancella, libera i posti
- GET /api/spettacoli/{spettacoloId}/disponibilita - posti liberi/occupati

Codici di errore: 404 se spettacolo/posto non esiste (controllato via Catalog), 409 se il posto è già stato prenotato da qualcun altro nel frattempo

## Istruzioni per avviare l'applicazione

Serve avere l'app Docker installata e attiva. Poi, dalla cartella principale aprire un terminale e lanciare:

```bash
docker compose up --build
```

Parte SQL Server sulla 1433, Catalog.Api su http://localhost:5080 e Booking.Api su http://localhost:5081

## Autenticazione

Tutte le chiamate sono protette con autenticazione JWT. Per usarle serve prima fare login.

Login (solo su Catalog.Api):

```
POST http://localhost:5080/api/auth/token
```
Body:
```json
{"Username": "sa", "Password": "Paperino123!"}
```

La risposta contiene il token. Va usato come header su ogni chiamata successiva, su entrambi i servizi (copiare solo il tocken senza le virgolette)
```
Authorization: Bearer <token>
```

Su Swagger c'è un pulsante Authorize in alto a destra dove si incolla solo il token e da lì in poi tutte le chiamate da quella pagina lo includono in automatico. Va fatto separatamente su Swagger di Catalog e su quello di Booking, con lo stesso token. Il token è una chiave condivisa tra i due servizi: Catalog lo genera, Booking lo valida da solo senza doverlo chiedere a Catalog ogni volta

Quando Booking chiama Catalog internamente inoltra anche lui il token che ha ricevuto dal client, altrimenti quella chiamata verrebbe rifiutata visto che anche Catalog richiede l'autenticazione su tutte le chiamate

## Appunti

Due servizi separati --> Catalogo e Prenotazioni cambiano in maniera diversa: il catalogo è quasi statico, mentre le prenotazioni sono ad alta scrittura. Ha senso tenerli separati, anche solo per poterli gestire in modo indipendente

Due DB separati --> se condividessi il database tra i due servizi, avrei un blocco unico con più deploy separati e un cambio di schema in un servizio romperebbe l'altro

IDPosto e IDSpettacolo in Booking non sono FK --> puntano a righe che stanno fisicamente in un altro database, quindi la validazione la faccio con una chiamata http a Catalog al momento della prenotazione

Prenotazione dello stesso posto --> ho messo un indice univoco sulla coppia "IDSpettacolo, IDPosto", nella tabella PostiPrenotati. Se arrivano due richieste di prenotazione dello stesso posto in contemporanea, la seconda INSERT viene rifiutata dal database e restituisco un errore 409

Cancellazione del PostoPrenotato --> alla cancellazione della prenotazione elimino il PostoPrenotato invece di marcarlo come "cancellato", così il posto torna subito libero. La Prenotazione invece resta a sistema con Stato = Cancellata, per tenere traccia dello storico

DTO separati dai Model tra i due servizi --> Booking non usa mai direttamente le classi di Catalog, ha i suoi DTO che rispecchiano quello che arriva via JSON. L'unica cosa da ricordasi è di tenere allineati i DTO dei due container

ICatalogClient come interfaccia --> serve per i test, in questo modo posso simulare le risposte di Catalog senza doverlo avviare per davvero

Comandi per Docker:

bash
docker compose up --build        # avvia tutto (build incluso)
docker compose down              # ferma tutto, tiene i dati
docker compose down -v           # ferma tutto e cancella i dati
docker compose up sqlserver      # avvia solo SQL Server

Per vedere cosa sta succedendo:

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