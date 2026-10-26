namespace CamCare.Interfaces.Services
{
    /// <summary>
    /// Versendet die konfigurierte Status-E-Mail für einen Reparaturauftrag, sofern der
    /// Status dem konfigurierten Auslöser-Status entspricht und für diesen Auftrag und
    /// Status noch keine E-Mail erfolgreich versendet wurde.
    /// </summary>
    public interface IRepairOrderMailDispatcher
    {
        /// <summary>
        /// Führt einen Versandversuch aus. Die Methode wirft keine Ausnahme, damit ein
        /// Mailfehler das Speichern des Reparaturauftrags nicht beeinflusst.
        /// </summary>
        Task DispatchAsync(int repairOrderId, int repairOrderStatusId);
    }
}
