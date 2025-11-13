using CamCare.Models;
using Fluxor;

namespace CamCare.Store.RepairOrderFeature
{
    [FeatureState]
    public record RepairOrderState
    {
        public RepairOrderVm? CurrentRepairOrder{ get; set; }
        private RepairOrderState() { }
    }
}
