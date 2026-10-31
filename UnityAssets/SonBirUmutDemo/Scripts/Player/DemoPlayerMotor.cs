using SonBirUmutDemo.Core;
using SonBirUmutDemo.Simulation;
using UnityEngine;

namespace SonBirUmutDemo.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class DemoPlayerMotor : MonoBehaviour
    {
        [SerializeField] private DemoGameController game;
        [SerializeField] private Transform cameraTransform;
        [SerializeField, Min(0.1f)] private float speed = 3.5f;
        [SerializeField, Min(1f)] private float sprintMultiplier = 1.9f;
        [SerializeField, Min(0.1f)] private float acceleration = 12f;
        [SerializeField, Min(0.1f)] private float deceleration = 16f;
        [SerializeField, Min(1f)] private float turnSpeed = 600f;
        private CharacterController controller;
        private float verticalSpeed;
        private Vector3 horizontalVelocity;

        public void Configure(DemoGameController owner, Transform view)
        {
            game = owner;
            cameraTransform = view;
        }

        public void ConfigureSprint(float multiplier)
        {
            MovementPolicy.TargetSpeed(speed, true, false, multiplier);
            sprintMultiplier = multiplier;
        }

        private void Awake() => controller = GetComponent<CharacterController>();

        private void Update()
        {
            if (game == null || cameraTransform == null || game.Session == null) return;
            Vector2 input = game.MovementLocked ? Vector2.zero : DemoInput.Movement;
            Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            Vector3 direction = Vector3.ClampMagnitude(forward * input.y + right * input.x, 1f);
            // Digital input still has eight targets; velocity and facing move continuously between them.
            // Modal UI is an immediate stop, independent of the normal braking distance.
            float targetSpeed = MovementPolicy.TargetSpeed(speed, DemoInput.SprintHeld, game.MovementLocked, sprintMultiplier);
            Vector3 targetVelocity = direction * targetSpeed;
            if (game.MovementLocked) horizontalVelocity = Vector3.zero;
            else horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity,
                (targetVelocity.sqrMagnitude < horizontalVelocity.sqrMagnitude ? deceleration : acceleration) * Time.deltaTime);
            if (controller.isGrounded && verticalSpeed < 0) verticalSpeed = -2f;
            verticalSpeed = Mathf.Max(-30f, verticalSpeed + Physics.gravity.y * Time.deltaTime);
            controller.Move((horizontalVelocity + Vector3.up * verticalSpeed) * Time.deltaTime);
            if (horizontalVelocity.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                    Quaternion.LookRotation(horizontalVelocity, Vector3.up), turnSpeed * Time.deltaTime);
        }

        private void OnDisable() => horizontalVelocity = Vector3.zero;

        private void OnApplicationFocus(bool focused)
        {
            if (!focused) horizontalVelocity = Vector3.zero;
        }

        public void ResetPosition(Vector3 position, Quaternion rotation)
        {
            // Controller may not have received Awake yet (the game initializes first).
            if (controller == null) controller = GetComponent<CharacterController>();
            controller.enabled = false;
            transform.SetPositionAndRotation(position, rotation);
            verticalSpeed = 0;
            horizontalVelocity = Vector3.zero;
            controller.enabled = true;
        }
    }
}
