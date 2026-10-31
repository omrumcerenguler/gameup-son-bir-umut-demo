using SonBirUmutDemo.Core;
using UnityEngine;

namespace SonBirUmutDemo.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class DemoPlayerMotor : MonoBehaviour
    {
        [SerializeField] private DemoGameController game;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float speed = 4f;
        private CharacterController controller;
        private float verticalSpeed;

        public void Configure(DemoGameController owner, Transform view)
        {
            game = owner;
            cameraTransform = view;
        }

        private void Awake() => controller = GetComponent<CharacterController>();

        private void Update()
        {
            if (game == null || cameraTransform == null || game.Session == null) return;
            Vector2 input = game.CardsOpen ? Vector2.zero : DemoInput.Movement;
            Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            Vector3 direction = Vector3.ClampMagnitude(forward * input.y + right * input.x, 1f);
            if (controller.isGrounded && verticalSpeed < 0) verticalSpeed = -2f;
            verticalSpeed += Physics.gravity.y * Time.deltaTime;
            controller.Move((direction * speed + Vector3.up * verticalSpeed) * Time.deltaTime);
            if (direction.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 12f * Time.deltaTime);
        }

        public void ResetPosition(Vector3 position, Quaternion rotation)
        {
            // Controller may not have received Awake yet (the game initializes first).
            if (controller == null) controller = GetComponent<CharacterController>();
            controller.enabled = false;
            transform.SetPositionAndRotation(position, rotation);
            verticalSpeed = 0;
            controller.enabled = true;
        }
    }
}
