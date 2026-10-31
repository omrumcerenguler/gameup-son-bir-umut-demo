using SonBirUmutDemo.Core;
using UnityEngine;

namespace SonBirUmutDemo.Player
{
    // Visual-only joint motion. Root movement/collisions remain in DemoPlayerMotor.
    [RequireComponent(typeof(CharacterController))]
    public sealed class DemoCharacterVisual : MonoBehaviour
    {
        [SerializeField]
        private Transform leftLeg, rightLeg, leftArm, rightArm;
        [SerializeField]
        private DemoGameController game;
        private CharacterController controller;
        private float phase, blend;
        public void Configure(Transform legA, Transform legB, Transform armA, Transform armB)
        {
            leftLeg = legA;
            rightLeg = legB;
            leftArm = armA;
            rightArm = armB;
        }

        public void SetGame(DemoGameController owner) => game = owner;
        private void Awake() => controller = GetComponent<CharacterController>();
        private void LateUpdate()
        {
            if (game == null || game.MovementLocked)
            {
                ResetPose();
                return;
            }

            float speed = Vector3.ProjectOnPlane(controller.velocity, Vector3.up).magnitude;
            blend = Mathf.MoveTowards(blend, Mathf.Clamp01(speed / 3.5f), Time.deltaTime * 8f);
            phase += speed * Time.deltaTime * 4f;
            float stride = Mathf.Sin(phase) * 24f * blend;
            Pose(leftLeg, stride);
            Pose(rightLeg, -stride);
            Pose(leftArm, -stride * .7f);
            Pose(rightArm, stride * .7f);
        }

        private void OnDisable() => ResetPose();
        private void ResetPose()
        {
            phase = blend = 0;
            Pose(leftLeg, 0);
            Pose(rightLeg, 0);
            Pose(leftArm, 0);
            Pose(rightArm, 0);
        }

        private static void Pose(Transform joint, float angle)
        {
            if (joint != null)
                joint.localRotation = Quaternion.Euler(angle, 0, 0);
        }
    }
}
