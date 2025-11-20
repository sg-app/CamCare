using Fluxor;

namespace CamCare.Store.RepairOrderFeature
{
    public static class RepairOrderReducers
    {
        [ReducerMethod]
        public static RepairOrderState ReduceSetCurrent(RepairOrderState state, SetCurrentRepairOrderAction action)
        {
            return state with { CurrentRepairOrder = action.RepairOrder };
        }

        [ReducerMethod]
        public static RepairOrderState ReduceResetCurrent(RepairOrderState state, ResetCurrentRepairOrderAction action)
        {
            return state with { CurrentRepairOrder = null };
        }
    }
}
