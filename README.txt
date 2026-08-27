## Jobtastic

Stellenportal mit öffentlicher Jobbörse, Recruiter-Bereich (Mandate, Kontakte,
Anzeigen), Administrationsbereich und einer öffentlichen Lese-API.


## Schnellstart

Projekt klonen und starten – mehr ist nicht nötig:

/*bash
dotnet run --project Jobtastic
*/

Beim ersten Start passiert automatisch:

  1. Die Datenbank wird angelegt und migriert (LocalDB, siehe unten)
  2. Die Rollen Owner, Admin und User werden angelegt
  3. Beispieldaten werden eingespielt: 4 Konten, 4 Firmen, 4 Kontakte, 8 Anzeigen

Schritt 3 läuft nur in der Entwicklungsumgebung und nur, solange die Datenbank
noch leer ist. Eine bereits genutzte Datenbank wird nie verändert.


## Demo-Zugang

Alle Demo-Konten verwenden dasselbe Passwort: Demo!2026

  Administrator (Owner)   admin@jobtastic.demo
  Recruiter                recruiter1@firma.demo
  Recruiter                recruiter2@firma.demo
  Recruiter (ohne Daten)   recruiter3@firma.demo

Die Zugangsdaten stehen zusätzlich direkt auf der Login-Seite – ebenfalls nur in
der Entwicklungsumgebung.

recruiter3 ist bewusst ohne Mandate angelegt und zeigt den Zustand eines frisch
registrierten Kontos.


## Datenbank

Standardmäßig wird LocalDB verwendet (kommt mit Visual Studio, keine Installation
und kein Passwort nötig). Die Verbindung steht in appsettings.Development.json.

Für eine eigene SQL-Server-Instanz genügt ein User-Secret, das den Standard
überschreibt:

/*bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=localhost;Initial Catalog=Jobtastic;User ID=sa;Password=DEIN_PASSWORT;TrustServerCertificate=True"
*/


## Öffentliche API

GET /api/ApiJobposting/GetAll     alle veröffentlichten Anzeigen
GET /api/ApiJobposting/GetById    eine Anzeige nach ID

Beide Endpunkte erwarten den Header "ApiKey". Der Schlüssel wird über die
Konfiguration gesetzt:

/*bash
dotnet user-secrets set "ApiKey" "DEIN_SCHLUESSEL"
*/

Die API ist bewusst ein identitätsloser Lesezugriff für externe Systeme
(z. B. Job-Aggregatoren). Verwaltungsfunktionen sind über sie nicht erreichbar.


## Tests

/*bash
dotnet test
*/


## Bekannte Einschränkungen / geplante Verbesserungen

Rollenmodell: Rollen werden additiv gespeichert – jedes Konto hat User, ein
Administrator zusätzlich Admin, das Gründerkonto zusätzlich Owner. Die Oberfläche
zeigt jeweils nur die höchste Rolle, sodass sich das System nach außen wie ein
exklusives Rollenmodell verhält. Fachlich sauberer wäre ein tatsächlich
exklusives Modell (genau eine Rolle je Konto); dagegen sprach, dass die
Rollen-API von ASP.NET Identity mengenbasiert ist: ein Rollenwechsel bräuchte
zwei Schreibvorgänge ohne gemeinsame Transaktion, die Invariante "genau eine
Rolle" hätte keinen Wächter, und [Authorize(Roles = "User")] würde Administratoren
aussperren.

Ownership-Transfer ist nicht implementiert. Das Owner-Konto wird beim Seeding
bzw. bei der Erstregistrierung festgelegt und kann anschließend weder gesperrt
noch degradiert werden.

Validierungsfehler im Nutzerkonto werden noch als JSON ausgegeben statt als
Meldung – die übrigen Formulare nutzen bereits SweetAlert.

Datenbankgebundene Tests fehlen noch (Rollenvergabe, Mandatsprüfungen,
Benutzerübersicht). Die vorhandenen Tests decken die reine Berechtigungs- und
Sichtbarkeitslogik ab.



## Setup

Diese App braucht eine SQL-Server-Verbindung. Derzeit ist sie lokal eingerichtet:

/*bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=localhost;Initial Catalog=JobtasticDb;User ID=sa;Password=DEIN_PASSWORT;TrustServerCertificate=True"
*/




appsettings.json: //hier könnte angepasst werden, dass beim Start zB ms azure keyvault geladen wird (um keine credentials in git zu pushen)


Model: //1 Kontakt gehört zu einem User. 1 User kann mehrere Kontaktseiten betreuen



Die API veröffentlicht ausschließlich das öffentliche Stellenportal. Verwaltungsfunktionen sind nicht per API erreichbar.
Das ist als bewusste Entscheidung im README mehr wert als eine halbe CRUD-API —

Idee der Mandate: Müsste eigentlich verifiziert werden


Eine Ehrlichkeit noch: Ohne „Ownership übertragen" ist der Eigentümer dauerhaft festgelegt. Für ein Portfolio-Projekt halte ich das für völlig in Ordnung — es ist die konsequentere Haltung als ein halbgarer Übertragungs-Flow. In der Doku wäre es ein sauberer Satz („Ownership-Transfer nicht implementiert, Eigentümer wird beim Seeding gesetzt").

Der Owner-Schutz adressiert nicht „das System hat keinen Admin mehr" — das verhindert schon die Selbst-Regel. Er adressiert, dass ein beförderter Admin den Gründer absetzt. Das ist genau das Modell von GitHub und Discord


Kontolöschung ist bewusst nicht implementiert. Die Fremdschlüssel für Anzeigen und Kontakte stehen auf Restrict — ein Konto mit Inhalten lässt sich nicht löschen, ohne entweder die Inhalte mitzulöschen (fachlich falsch, sie gehören der Firma) oder verwaisen zu lassen. Der saubere Weg wäre Anonymisierung plus dauerhafte Sperre; das wurde aus Zeitgründen zurückgestellt.