# Jobtastic

## About

Jobtastic was developed as part of my retraining as a software developer (IHK). The project
demonstrates the use of object-oriented programming principles as well as the
Model-View-Controller architecture commonly used for web applications.

AI support took place at the following points: Claude AI for researching concepts and for
debugging in the early development phase, later Claude Code to implement the AJAX JavaScript,
the administration area including the central access/permission check, and the tests quickly.

## Features

Job portal with

- a publicly accessible job board
- a recruiter area (create company mandates, manage contact persons, publish postings)
- an administration area
- a public read-only API
- pre-built accounts for demo purposes

## Technologies

- ASP.NET Core 8 / Entity Framework Core / ASP.NET Core Identity
- C# / JavaScript / HTML5
- MVC + Razor Pages
- SQL Server / LocalDB
- CSS / Bootstrap 5 / DataTables.net
- AJAX
- jQuery Validation Unobtrusive / SweetAlert2
- NUnit

## Requirements

- Windows
- .NET 8
- Visual Studio 2022 or later

## Getting Started (demo with LocalDB)

1. Clone the repository.
2. Open `Jobtastic` in Visual Studio.

or

```bash
dotnet run --project Jobtastic
```

The first start automatically:

1. creates the database and migrates the model (LocalDB, see [Configuration](#configuration))
2. creates the roles `Owner`, `Admin` and `User`
3. seeds sample data

Step 3 runs **in the development environment only** and **only while the database is empty**.


### Demo Access

To reach the user features, either use one of the demo accounts listed below or register a new
account.

> **Note:** without seeding, i.e. outside the development environment,
> the first person to register becomes the owner. Every later registration only receives
> `User`. This does not apply in the demo, because the seeding already creates accounts and
> `admin@jobtastic.demo` is therefore the owner.

All demo accounts use the password: **`Demo!2026`**

| Role | E-mail |
|---|---|
| Administrator (Owner) | `admin@jobtastic.demo` |
| Recruiter1 | `recruiter1@firma.demo` |
| Recruiter2 | `recruiter2@firma.demo` |
| Recruiter3 (empty) | `recruiter3@firma.demo` |

When the application is run from the development environment, the credentials are additionally
shown directly on the login page.
![Demo credentials on the login page](Screenshots/dev_login.png)



## Configuration

### Database (your own SQL Server instance)

If you are using your own SQL Server instance, a user secret has to be set up that overrides the
default (LocalDB):

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=localhost;Initial Catalog=Jobtastic;User ID=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True"
```

### API Key

In the development environment, the demo key **`Demo!2026-Api`** is already configured, so the
API works without any further setup step. For your own key, or outside the development
environment, it has to be set:

```bash
dotnet user-secrets set "ApiKey" "YOUR_KEY"
```

> **Note:** without a configured key the API answers **every** request outside the development
> environment with `401`. See
> [Known Limitations](#known-limitations--necessary-improvements)

### Endpoints

```
GET /api/ApiJobposting/GetAll     all published postings
GET /api/ApiJobposting/GetById    a single posting by ID (query parameter, e.g. ?id=5)
```

Both expect the `ApiKey` header.

**Example call**

```bash
curl -k -H "ApiKey: Demo!2026-Api" "https://localhost:7172/api/ApiJobposting/GetById?id=5"
```

In PowerShell, `curl` is only an alias for `Invoke-WebRequest`; use this instead:

```bash
Invoke-RestMethod -Uri "https://localhost:7172/api/ApiJobposting/GetAll" -Headers @{ ApiKey = "Demo!2026-Api" }
```

**Response** (one element from `GetAll`, abridged):

```json
[
  {
  "id": 5,
  "company": {
    "id": 3,
    "name": "GreenNest Solutions",
    "websiteURL": "https://greennest.demo"
  },
  "contact": {
    "id": 3,
    "firstName": "John",
    "lastName": "Doe",
    "email": "johndoe@greennest.demo",
    "phone": "+43 89034555"
  },
  "jobTitle": "Produktmanager",
  "header": "Produktmanager (m/w/d) Nachhaltige Haustechnik",
  "jobDescription": "Du übersetzt Kundenbedürfnisse in ..."
  "jobLocation": "Berlin",
  "annualSalary": 58000,
  "fulltime": true,
  "volumeHours": 40,
  "mode": 1,
  "experience": 3,
  "startDate": "2026-12-28T00:00:00"
}
]
```

## User Documentation

**Visitor (without login)**
The start page, accessible without an account, lists all postings that have been released for
publication and have not expired. Each entry is linked to the detail view of the posting,
including a short profile of the relevant company and of the contact person (if any).
![Start page with published postings](Screenshots/public_listings.png)

**Recruiter**
Under "Mein Profil", account data, company mandates and the contact persons for the postings are
entered and managed. Signed-in users who hold at least one company mandate can publish, edit and
delete postings themselves. "Meine Inserate" shows an overview of all postings they created
(including drafts and expired postings, which are not publicly visible).
![Managing company mandates](Screenshots/company_mandates.png)
![Managing contact persons](Screenshots/managed_contacts.png)
![Overview of own postings](Screenshots/owned_listings.png)
![Posting form](Screenshots/posting_form.png)

**Administrators** (`admin@jobtastic.demo`)
Administrators have access to every function of a regular user account and additionally to the
admin area. It holds four overviews (users, postings, companies, contacts). In the user overview,
accounts can be locked and unlocked and the admin role granted and revoked. Mandates and contacts
of foreign accounts are reachable through a link in the user overview to the respective account
page. Access to a foreign account is marked by a warning banner.
![Admin dashboard](Screenshots/admin_dashboard.png)

## Project Structure

```
Jobtastic/
  Areas/Identity/Pages/     Identity as Razor Pages (login, registration, account management)
  Authorization/            Decision logic: scopes, rules, current user
  Identity/                 Identity additions: role names, German error messages
  Controllers/              MVC controllers including admin area and API
  Filters/                  API key check as an action filter
  Services/                 Application logic, demo data seeding
  Models/                   Domain models, input models and list models
  Enums/
  DTO/                      Contract of the public API
  Data/                     DbContext
  Migrations/               EF Core migrations
  Views/
    Home/                   Job board, detail view, privacy
    Posting/                "Meine Inserate" and the posting form
    Admin/                  the four overviews and the admin navigation
    Shared/                 Layout, error page, partials
  wwwroot/
    js/                     AJAX handlers, form and UI behaviour
    css/                    site.css
    lib/                    Client libraries (Bootstrap, jQuery, DataTables, SweetAlert2)
TestJobtastic/              NUnit tests
```

## Architecture & Design Decisions

### Layering: MVC, Services and Decision Logic

Controllers form the HTTP boundary, call the views assigned to them and delegate interactions
with the database to the corresponding services. (These hang off managers or the `ICurrentUser`
interface instead of `HttpContext` and are therefore testable without a web request.) The
essential access and decision logic as well as, naturally, the data models sit isolated and are
partly called across layers.

### The Basic Data Model

- A posting belongs to exactly one user through `OwnerID`.
- A contact belongs to a user *and* a company: the basis for later assignment checks.
- Mandates are n:m between `User` and `Company`.
- `JobContact` is a pure data structure and not an actor.
- Delete behaviour is deliberately `Restrict`: content does not break away from underneath its
  owner (see [account closure](#known-limitations--necessary-improvements)).

Overall there are three model types/contracts for three purposes:
1) domain models for persistence
2) `JobPostingInputModel` as a whitelist of allowed form fields
3) `Admin*ListModel` as flat projections for the overviews

The API projects onto `DTO/` types instead of entities.


### Central Permission Check on Two Levels

Problem: an admin, who is always also a user, must be able to edit other users' postings.

| | Question | Location |
|---|---|---|
| **Access** | Which rows may I load? | `IQueryable` scopes in `PostingQueries` |
| **Invariant** | Is the state valid? | Check in the service |

Guiding principle: roles widen the field of view, without changing the business rules.
- **Access** is filtered in SQL against OwnerID. (GetJobById returns null for foreign postings.)
- The **invariant** (the owner must hold a mandate for the posting's company) therefore also
checks against the owner of the posting, not the one acting.

-> Admin can edit foreign postings.
-> Access rules in service and PostingQueries consistent / invariant intact.

### Role Model

Roles are stored additively:
1) All users are `User`.
2) A user can additionally be `Admin`.
3) An admin can additionally be `Owner`.

Only the highest role is visible in the interface, which mimics a single-role model.
This compromise was made against the background of possible problems of a genuinely exclusive
role model in ASP.NET Identity, such as demoted administrators ending up roleless or a
duplication of write operations.

Addendum: owner protection. The founding account (`Owner`) can neither be locked nor demoted,
which protects against an admin deposing the founder.

### Frontend

Bootstrap 5 for layout, DataTables.net for sorting/searching/paging, AJAX over `fetch` for
locking, role changes and deletion (JSON in the same controllers as the views), SweetAlert2 for
confirmation prompts before destructive actions. (AJAX handlers in `wwwroot/js/` written with
Claude Code).

---

## Tests

```bash
dotnet test
```

25 NUnit tests for the pure decision logic. Testable without a database or `HttpContext`.
(`AccountAccessTests`, `PostingQueriesTests`, `AdminUserListModelTests`, `AdminPostingListModelTests`).

## Known Limitations & Necessary Improvements

### Functional

**E-mail Functions**
The functions created by ASP.NET Core Identity, registration confirmation, password reset and
notifications, are currently simply commented out. As a result there is currently no way to
reset a forgotten password. This could be fixed by integrating an e-mail server.

**Account Closure**
Not implemented. The foreign keys for postings and contacts are set to `Restrict`, which is why
an account holding content cannot be deleted. A possible solution would be anonymising the
dependent entities plus a permanent lock on the account.

**Verification of Mandates**
Every account can claim every company as a mandate. In a real system, the right to advertise on
behalf of a company would have to be proven.

**Shared Companies Without Change Protection**
Currently, every mandate holder can silently change the company data for everyone else, a
consequence of the shared model. Possible solutions would be a history and notifications, or
setting up an approval step.

**Click Counter**
The `Klicks` field exists on the model and is displayed in the admin overview. However, the
counting is not implemented anywhere yet, which is why the counter permanently shows 0.

**Admin Functions**
An urgent addition to the admin area would be an audit log, to make administrative interventions
traceable. Planned but left out for time reasons are, e.g., also an ownership transfer of
postings and contacts, as well as a company merge to combine possible company duplicates.


### Technical

**User Interface**
The layout, especially with respect to accessibility, needs further work.

**API Rudimentary**
Building an API was not originally planned. It came about as a digression during class and is
still incomplete in its current state. In particular, no JsonStringEnumConverter is registered.
System.Text.Json therefore serialises the enums as their ordinal values, losing the associated
values. There is also no versioning, paging, rate limiting or dedicated documentation yet.

**Connection to a Secret Store**
For a real deployment, something like Azure Key Vault would have to be integrated at startup
instead of user secrets.

**No Database-Bound Tests**
Role assignment, mandate checks, locking logic and the admin overviews have so far only been
verified manually.

**Untested LocalDB Path**
On the development machine, a user secret overrides the connection, so work has actually always
been done against a SQL Server instance. The route described in the quick start is plausible,
but has not been executed yet, for time reasons.

**Mixed Languages in the Project**
The interface was designed in German to match the project's target audience. The code, including
naming and comments, was written in English for practice. In the commit statements on GitHub in
particular, however, linguistic inconsistencies crept in that should be avoided in the future.

