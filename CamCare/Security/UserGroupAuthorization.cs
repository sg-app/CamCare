namespace CamCare.Security
{
    /// <summary>
    /// Namen der festen Benutzergruppen. Sie werden zusätzlich als Rollen-Claims verwendet.
    /// </summary>
    public static class UserGroupRoles
    {
        public const string Admin = "admin";
        public const string Buero = "buero";
        public const string Werkstatt = "werkstatt";
    }

    /// <summary>
    /// Namen der Authorization-Policies.
    /// </summary>
    public static class AuthPolicies
    {
        public const string AdminOnly = nameof(AdminOnly);
        public const string CanCreateOrders = nameof(CanCreateOrders);
    }

    /// <summary>
    /// Beschreibung einer festen Benutzergruppe (einheitliche Quelle für Name
    /// und Standard-Passwort, damit kein Text dupliziert wird).
    /// </summary>
    public sealed record UserGroupInfo(string Name, string DefaultPassword);

    /// <summary>
    /// Katalog der festen Benutzergruppen. Wird sowohl für das Seeding der Datenbank
    /// als auch für die UI-Optionen (Login-Dialog) verwendet.
    /// </summary>
    public static class UserGroups
    {
        public static readonly UserGroupInfo Admin = new(UserGroupRoles.Admin, "admin");
        public static readonly UserGroupInfo Buero = new(UserGroupRoles.Buero, "buero");
        public static readonly UserGroupInfo Werkstatt = new(UserGroupRoles.Werkstatt, "werk");

        public static readonly IReadOnlyList<UserGroupInfo> All = [Admin, Buero, Werkstatt];
    }
}
