namespace CamCare.Security
{
    /// <summary>
    /// Browser-seitige Helfer für Anmeldung/Abmeldung (fetch + Full-Reload).
    /// Sie werden per eval() direkt aus den Blazor-Komponenten registriert, damit die
    /// Anmeldung nicht von einer statischen JS-Datei (und deren Browser-Cache) abhängt.
    /// </summary>
    public static class AuthJsScript
    {
        public const string Definitions = """
            window.camcareLogin = async function (group, password) {
                const response = await fetch('/api/auth/login', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ group: group, password: password })
                });
                if (!response.ok) return false;
                const result = await response.json();
                window.location.href = result.redirect || '/';
                return true;
            };
            window.camcareLogout = async function () {
                const response = await fetch('/api/auth/logout', { method: 'POST' });
                const result = await response.json();
                window.location.href = result.redirect || '/Login';
            };
            true;
            """;
    }
}
