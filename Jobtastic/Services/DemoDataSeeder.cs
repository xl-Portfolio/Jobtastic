using Jobtastic.Data;
using Jobtastic.Enums;
using Jobtastic.Identity;
using Jobtastic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jobtastic.Services
{
    /// <summary>
    /// Fills an empty database with a coherent sample data set, so the application can
    /// be started and reviewed without any manual setup.
    ///
    /// Runs in Development only and skips entirely once any account exists, so it never
    /// touches a database that is already in use.
    /// </summary>
    public class DemoDataSeeder
    {
        /// <summary>Valid for every demo account;.</summary>
        public const string Password = "Demo!2026";

        public const string OwnerEmail = "admin@jobtastic.demo";
        public const string RecruiterEmail = "recruiter1@firma.demo";
        private static string LogoUrlFor(string companyName) =>
            "https://api.dicebear.com/9.x/initials/svg?seed=" + Uri.EscapeDataString(companyName);

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<DemoDataSeeder> _logger;

        public DemoDataSeeder(ApplicationDbContext context, UserManager<User> userManager, ILogger<DemoDataSeeder> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            if (await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Demo data skipped: the database already contains accounts.");
                return;
            }

            _logger.LogInformation("Seeding demo data.");

            var owner = await CreateAccountAsync(OwnerEmail, RoleNames.User, RoleNames.Admin, RoleNames.Owner);
            var recruiter1 = await CreateAccountAsync(RecruiterEmail, RoleNames.User);
            var recruiter2 = await CreateAccountAsync("recruiter2@firma.demo", RoleNames.User);
            //shows the "profile incomplete" state
            await CreateAccountAsync("recruiter3@firma.demo", RoleNames.User);

            var futuriva = new Company
            {
                Name = "Futuriva Labs",
                Description = "Futuriva Labs entwickelt innovative digitale Lösungen, die Unternehmen dabei helfen, "
                            + "Arbeitsprozesse einfacher, schneller und nachhaltiger zu gestalten.",
                WebsiteURL = "https://futuriva.demo",
                LogoImageSource = LogoUrlFor("Futuriva Labs")
            };
            var greennest = new Company
            {
                Name = "GreenNest Solutions",
                Description = "GreenNest Solutions bietet umweltfreundliche Produkte und intelligente Konzepte für ein "
                            + "nachhaltigeres Zuhause – von energiesparender Technik bis hin zu recycelbaren Alltagsprodukten.",
                WebsiteURL = "https://greennest.demo",
                LogoImageSource = LogoUrlFor("GreenNest Solutions")
            };
            var nordlicht = new Company
            {
                Name = "Nordlicht Medien",
                Description = "Nordlicht Medien produziert Podcasts, Dokumentationen und digitale Formate für Kunden "
                            + "aus Kultur, Bildung und Wirtschaft.",
                WebsiteURL = "https://nordlicht-medien.demo",
                LogoImageSource = LogoUrlFor("Nordlicht Medien")
            };
            var kessler = new Company
            {
                Name = "Kessler Logistik",
                Description = "Kessler Logistik plant und steuert Warenströme für mittelständische Unternehmen – "
                            + "vom einzelnen Transport bis zur kompletten Lieferkette.",
                WebsiteURL = "https://kessler-logistik.demo",
                LogoImageSource = LogoUrlFor("Kessler Logistik")
            };

            recruiter1.Companies.AddRange(new[] { futuriva, nordlicht });
            recruiter2.Companies.AddRange(new[] { greennest, nordlicht, kessler });
            await _context.SaveChangesAsync();

            var jane = NewContact("Jane", "Doe", "janedoe@futuriva.demo", "+49 15899745", "HR", futuriva, recruiter1);
            var tarek = NewContact("Tarek", "Bilal", "tbilal@nordlicht-medien.demo", "+49 40 5512090", "Redaktion", nordlicht, recruiter1);
            var john = NewContact("John", "Doe", "johndoe@greennest.demo", "+43 89034555", "IT", greennest, recruiter2);
            var marlene = NewContact("Marlene", "Ostrowski", "m.ostrowski@kessler-logistik.demo", "+49 231 447120", "Personal", kessler, recruiter2);

            _context.Contacts.AddRange(jane, tarek, john, marlene);
            await _context.SaveChangesAsync();

            _context.Postings.AddRange(
                NewPosting(recruiter1, futuriva, jane,
                    "Projektmanager", "Projektmanager (berufserfahren) (m/w/d)", "Bonn",
                    50000, Mode.OnSite, Experience.Professional, DateTime.Today.AddMonths(3), online: true,
                    intro: "Du bringst mehrere Jahre Erfahrung im Projektmanagement mit und behältst auch bei komplexen Vorhaben den Überblick? "
                         + "Bei Futuriva übernimmst du Verantwortung für spannende Projekte, koordinierst interdisziplinäre Teams und sorgst dafür, "
                         + "dass aus Ideen erfolgreiche Lösungen werden.",
                    tasks: new[]
                    {
                        "Planung, Steuerung und Umsetzung anspruchsvoller Projekte",
                        "Koordination interner Teams und externer Partner",
                        "Überwachung von Zeitplänen, Budgets und Projektzielen",
                        "Kommunikation mit Stakeholdern auf verschiedenen Ebenen"
                    },
                    profile: new[]
                    {
                        "Mehrjährige Berufserfahrung im Projektmanagement",
                        "Ausgeprägte Organisations- und Kommunikationsfähigkeit",
                        "Strukturierte, selbstständige und lösungsorientierte Arbeitsweise",
                        "Erfahrung mit gängigen Projektmanagement-Methoden und -Tools"
                    }),

                NewPosting(recruiter1, futuriva, jane,
                    "Cloud Engineer", "Cloud Engineer (m/w/d) – Azure", "Köln",
                    68000, Mode.Hybrid, Experience.Senior, DateTime.Today.AddMonths(4).AddDays(10), online: true,
                    intro: "Du denkst in Architekturen statt in Servern und willst unsere Plattform von Grund auf mitgestalten? "
                         + "Bei Futuriva verantwortest du den Betrieb und die Weiterentwicklung unserer Cloud-Infrastruktur.",
                    tasks: new[]
                    {
                        "Aufbau und Betrieb unserer Azure-Infrastruktur",
                        "Automatisierung von Deployments und Betriebsprozessen",
                        "Monitoring, Kostenkontrolle und Performance-Optimierung",
                        "Technische Beratung der Entwicklungsteams"
                    },
                    profile: new[]
                    {
                        "Mehrjährige Erfahrung mit Cloud-Plattformen, idealerweise Azure",
                        "Sicherer Umgang mit Infrastructure as Code",
                        "Kenntnisse in CI/CD und Containerisierung",
                        "Analytische Denkweise und Freude an sauberen Lösungen"
                    }),

                NewPosting(recruiter1, nordlicht, tarek,
                    "Audio-Producer", "Audio-Producer (m/w/d) für Podcast-Formate", "Hamburg",
                    41000, Mode.OnSite, Experience.Junior, DateTime.Today.AddMonths(3).AddDays(18), online: true,
                    intro: "Zwischen Schnittplatz und Studio: Bei Nordlicht Medien produzierst du Podcasts, die tatsächlich gehört werden – "
                         + "von der ersten Aufnahme bis zur fertigen Folge.",
                    tasks: new[]
                    {
                        "Aufnahme und Schnitt von Podcast-Folgen",
                        "Tontechnische Betreuung von Studioproduktionen",
                        "Mitarbeit an Konzeption und Dramaturgie neuer Formate",
                        "Qualitätssicherung vor der Veröffentlichung"
                    },
                    profile: new[]
                    {
                        "Erste Berufserfahrung in der Audioproduktion",
                        "Sicherer Umgang mit gängiger Schnitt-Software",
                        "Gutes Gehör und Gespür für Erzählrhythmus",
                        "Selbstständige und zuverlässige Arbeitsweise"
                    }),

                NewPosting(recruiter2, greennest, john,
                    "Web-Developer", "Junior Web Developer – GreenNest Solutions (Hybrid)", "Berlin",
                    43000, Mode.Hybrid, Experience.Entry, DateTime.Today.AddMonths(5), online: true,
                    intro: "Du hast deine ersten Projekte hinter dir und willst jetzt richtig einsteigen? "
                         + "Bei GreenNest entwickelst du an einer Plattform mit, die Nachhaltigkeit im Alltag einfacher macht.",
                    tasks: new[]
                    {
                        "Entwicklung und Pflege unserer Weboberflächen",
                        "Umsetzung von Features gemeinsam mit dem Produktteam",
                        "Mitarbeit an Code-Reviews und Tests",
                        "Behebung von Fehlern im laufenden Betrieb"
                    },
                    profile: new[]
                    {
                        "Grundkenntnisse in HTML, CSS und JavaScript",
                        "Erste Erfahrung mit einem Web-Framework",
                        "Bereitschaft, dich in neue Themen einzuarbeiten",
                        "Teamgeist und Interesse an nachhaltigen Produkten"
                    }),

                NewPosting(recruiter2, greennest, john,
                    "Produktmanager", "Produktmanager (m/w/d) Nachhaltige Haustechnik", "Berlin",
                    58000, Mode.FullRemote, Experience.Professional, DateTime.Today.AddMonths(4), online: true,
                    intro: "Du übersetzt Kundenbedürfnisse in Produkte, die wirklich gebraucht werden. "
                         + "Bei GreenNest verantwortest du eine Produktlinie von der Idee bis zur Markteinführung.",
                    tasks: new[]
                    {
                        "Verantwortung für Roadmap und Positionierung einer Produktlinie",
                        "Enge Zusammenarbeit mit Entwicklung, Einkauf und Vertrieb",
                        "Markt- und Wettbewerbsanalysen",
                        "Begleitung von Produkteinführungen"
                    },
                    profile: new[]
                    {
                        "Berufserfahrung im Produktmanagement, gern im technischen Umfeld",
                        "Fähigkeit, zwischen Technik und Vertrieb zu übersetzen",
                        "Strukturierte Arbeitsweise und Freude an Verantwortung",
                        "Interesse an nachhaltigen Technologien"
                    }),

                NewPosting(recruiter2, kessler, marlene,
                    "Disponent", "Disponent (m/w/d) im Nahverkehr", "Dortmund",
                    39000, Mode.OnSite, Experience.Entry, DateTime.Today.AddMonths(3).AddDays(6), online: true,
                    intro: "Ohne dich steht alles still: Als Disponent planst du Touren, koordinierst Fahrer und hältst "
                         + "den Betrieb auch dann am Laufen, wenn etwas dazwischenkommt.",
                    tasks: new[]
                    {
                        "Planung und Disposition von Touren im Nahverkehr",
                        "Ansprechpartner für Fahrerinnen und Fahrer",
                        "Bearbeitung von Störungen und Umplanungen",
                        "Pflege der Transportdaten im System"
                    },
                    profile: new[]
                    {
                        "Kaufmännische Ausbildung, gern im Speditionsumfeld",
                        "Ruhiger Kopf in hektischen Situationen",
                        "Organisationstalent und Kommunikationsstärke",
                        "Sicherer Umgang mit gängigen Office-Anwendungen"
                    }),

                // Not published: shows the owner-only preview path.
                NewPosting(recruiter2, kessler, marlene,
                    "Werkstudent Logistik", "Werkstudent (m/w/d) Logistik & Prozesse", "Dortmund",
                    14000, Mode.Hybrid, Experience.Intern, DateTime.Today.AddMonths(6), online: false,
                    intro: "Du studierst und willst Logistik nicht nur aus Vorlesungen kennen? "
                         + "Bei Kessler bekommst du Einblick in echte Prozesse und übernimmst früh eigene Aufgaben.",
                    tasks: new[]
                    {
                        "Unterstützung bei der Auswertung von Transportdaten",
                        "Mitarbeit an der Optimierung interner Abläufe",
                        "Pflege von Stammdaten und Dokumentationen"
                    },
                    profile: new[]
                    {
                        "Laufendes Studium, idealerweise mit Logistikbezug",
                        "Sorgfältige Arbeitsweise und Interesse an Zahlen",
                        "Verfügbarkeit von etwa 16 Stunden pro Woche"
                    }),

                // Published, expired
                NewPosting(recruiter1, nordlicht, tarek,
                    "Social-Media-Redakteur", "Social-Media-Redakteur (m/w/d) (abgelaufen)", "Hamburg",
                    38000, Mode.FullRemote, Experience.Junior, DateTime.Today.AddMonths(-5), online: true,
                    intro: "Diese Anzeige lief bereits und ist abgelaufen – sie erscheint deshalb nicht mehr in der "
                         + "öffentlichen Jobbörse, bleibt für den Eigentümer aber einsehbar.",
                    tasks: new[]
                    {
                        "Redaktionelle Betreuung unserer Social-Media-Kanäle",
                        "Erstellung von Text-, Bild- und Videobeiträgen",
                        "Community-Management"
                    },
                    profile: new[]
                    {
                        "Erste Erfahrung im Social-Media-Bereich",
                        "Sicheres Sprachgefühl",
                        "Eigenständige und kreative Arbeitsweise"
                    })
            );

            await _context.SaveChangesAsync();
            _logger.LogInformation("Demo data seeded: {Users} accounts, {Companies} companies, {Postings} postings.",
                await _context.Users.CountAsync(), await _context.Companies.CountAsync(), await _context.Postings.CountAsync());
        }

        private async Task<User> CreateAccountAsync(string email, params string[] roles)
        {
            var user = new User { UserName = email, Email = email, EmailConfirmed = true };

            var result = await _userManager.CreateAsync(user, Password);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Demo account '{email}' could not be created: {string.Join("; ", result.Errors.Select(e => e.Description))}");

            await _userManager.AddToRolesAsync(user, roles);
            return user;
        }

        private static JobContact NewContact(string firstName, string lastName, string email, string phone,
            string department, Company company, User owner) => new()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Department = department,
                Company = company,
                UserID = owner.Id
            };

        /// <summary>
        /// Start dates are relative to the seeding date.
        /// </summary>
        private static JobPosting NewPosting(User owner, Company company, JobContact contact,
            string jobTitle, string header, string location, double salary, Mode mode, Experience experience,
            DateTime startDate, bool online, string intro, string[] tasks, string[] profile)
        {
            var posting = new JobPosting
            {
                Company = company,
                Contact = contact,
                Owner = owner,
                JobTitle = jobTitle,
                Header = header,
                JobLocation = location,
                AnnualSalary = salary,
                Fulltime = experience != Experience.Intern,
                VolumeHours = experience == Experience.Intern ? 16 : 40,
                Mode = mode,
                Experience = experience,
                JobDescription = BuildDescription(intro, tasks, profile),
                StartDate = startDate,
                IsOnline = online
            };

            if (online)
            {
                posting.UploadDate = DateTime.Now;
                posting.ExpiryDate = startDate.AddMonths(3);
            }

            return posting;
        }

        private static string BuildDescription(string intro, string[] tasks, string[] profile)
        {
            var benefits = new[]
            {
                "Ein modernes, kollegiales Arbeitsumfeld",
                "Flexible Arbeitsmodelle und mobiles Arbeiten",
                "Individuelle Weiterbildungs- und Entwicklungsmöglichkeiten",
                "Attraktive Rahmenbedingungen und langfristige Perspektiven"
            };

            return intro
                 + "\n\nDeine Aufgaben\n\n" + string.Join("\n", tasks.Select(t => "    " + t))
                 + "\n\nDein Profil\n\n" + string.Join("\n", profile.Select(p => "    " + p))
                 + "\n\nWas wir bieten\n\n" + string.Join("\n", benefits.Select(b => "    " + b));
        }
    }
}
