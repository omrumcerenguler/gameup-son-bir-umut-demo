using System;

namespace SonBirUmutDemo.Simulation
{
    // Tests cover input intent/lock policy, not Unity keyboard events or physical movement.
    public static class MovementPolicy
    {
        public static float TargetSpeed(float walkSpeed, bool sprintHeld, bool movementLocked, float multiplier = 1.6f)
        {
            RunNumbers.NonNegative(walkSpeed, nameof(walkSpeed));
            RunNumbers.Finite(multiplier, nameof(multiplier));
            if (multiplier < 1) throw new ArgumentOutOfRangeException(nameof(multiplier));
            if (movementLocked) return 0;
            float result = walkSpeed * (sprintHeld ? multiplier : 1);
            RunNumbers.Finite(result, nameof(result));
            return result;
        }
    }
}
