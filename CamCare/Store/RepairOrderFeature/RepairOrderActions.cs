using CamCare.Models;

namespace CamCare.Store.RepairOrderFeature
{
    public record SetCurrentRepairOrderAction(RepairOrderVm RepairOrder);
    public record ResetCurrentRepairOrderAction();
}
