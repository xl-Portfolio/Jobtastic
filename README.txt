## Setup

Diese App braucht eine SQL-Server-Verbindung. Derzeit ist sie lokal eingerichtet:

/*bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=localhost;Initial Catalog=JobtasticDb;User ID=sa;Password=DEIN_PASSWORT;TrustServerCertificate=True"
*/
