using Fluxor;

namespace CamCare.Store.RepairOrderFeature
{
    public static class RepairOrderReducers
    {
        [ReducerMethod]
        public static RepairOrderState Reduce(RepairOrderState state, SetCurrentRepairOrderAction action)
        {
            return state with { CurrentRepairOrder = action.RepairOrder };
        }
    }
}
