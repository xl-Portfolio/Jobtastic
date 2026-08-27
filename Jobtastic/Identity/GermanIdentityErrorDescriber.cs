using Microsoft.AspNetCore.Identity;

namespace Jobtastic.Identity
{
    /// <summary>
    /// German texts for the errors ASP.NET Identity raises itself. Without this the
    /// app mixes its own German validation messages with English framework ones -
    /// visible during registration, password changes and email changes.
    ///
    /// Only the errors this application can actually produce are overridden; anything
    /// else falls back to the base implementation.
    /// </summary>
    public class GermanIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() => new()
        {
            Code = nameof(DefaultError),
            Description = "Es ist ein unbekannter Fehler aufgetreten."
        };

        public override IdentityError PasswordMismatch() => new()
        {
            Code = nameof(PasswordMismatch),
            Description = "Das aktuelle Passwort ist nicht korrekt."
        };

        public override IdentityError DuplicateEmail(string email) => new()
        {
            Code = nameof(DuplicateEmail),
            Description = $"Die E-Mail-Adresse '{email}' wird bereits verwendet."
        };

        public override IdentityError DuplicateUserName(string userName) => new()
        {
            Code = nameof(DuplicateUserName),
            Description = $"Der Benutzername '{userName}' wird bereits verwendet."
        };

        public override IdentityError InvalidEmail(string? email) => new()
        {
            Code = nameof(InvalidEmail),
            Description = $"Die E-Mail-Adresse '{email}' ist ungültig."
        };

        public override IdentityError InvalidUserName(string? userName) => new()
        {
            Code = nameof(InvalidUserName),
            Description = $"Der Benutzername '{userName}' enthält unzulässige Zeichen."
        };

        public override IdentityError PasswordTooShort(int length) => new()
        {
            Code = nameof(PasswordTooShort),
            Description = $"Das Passwort muss mindestens {length} Zeichen lang sein."
        };

        public override IdentityError PasswordRequiresDigit() => new()
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "Das Passwort muss mindestens eine Ziffer enthalten."
        };

        public override IdentityError PasswordRequiresLower() => new()
        {
            Code = nameof(PasswordRequiresLower),
            Description = "Das Passwort muss mindestens einen Kleinbuchstaben enthalten."
        };

        public override IdentityError PasswordRequiresUpper() => new()
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "Das Passwort muss mindestens einen Großbuchstaben enthalten."
        };

        public override IdentityError PasswordRequiresNonAlphanumeric() => new()
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "Das Passwort muss mindestens ein Sonderzeichen enthalten."
        };

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) => new()
        {
            Code = nameof(PasswordRequiresUniqueChars),
            Description = $"Das Passwort muss mindestens {uniqueChars} verschiedene Zeichen enthalten."
        };

        public override IdentityError UserAlreadyInRole(string role) => new()
        {
            Code = nameof(UserAlreadyInRole),
            Description = $"Das Konto hat die Rolle '{role}' bereits."
        };

        public override IdentityError UserNotInRole(string role) => new()
        {
            Code = nameof(UserNotInRole),
            Description = $"Das Konto hat die Rolle '{role}' nicht."
        };
    }
}
