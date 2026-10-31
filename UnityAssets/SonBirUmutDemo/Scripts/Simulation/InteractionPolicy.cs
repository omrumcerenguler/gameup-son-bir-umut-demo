namespace SonBirUmutDemo.Simulation
{
    public enum InteractionKind { None, Modal, Emergency, StrategicDecision, Maintenance }

    public static class InteractionPolicy
    {
        // An emergency takes ownership even when out of range: guide the player to water.
        public static InteractionKind Resolve(bool modal, bool emergencyActive,
            bool strategicInRange, bool maintenanceInRange)
        {
            if (modal) return InteractionKind.Modal;
            if (emergencyActive) return InteractionKind.Emergency;
            if (strategicInRange) return InteractionKind.StrategicDecision;
            return maintenanceInRange ? InteractionKind.Maintenance : InteractionKind.None;
        }
    }
}
