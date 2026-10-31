using UnityEngine;

namespace SonBirUmutDemo.Player
{
    public sealed class IsometricFollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothTime = 0.18f;
        private Vector3 velocity;
        private static readonly Quaternion ViewRotation = Quaternion.Euler(35.264f, 45f, 0f);

        public void Configure(Transform followTarget)
        {
            target = followTarget;
            Snap();
        }

        private Vector3 DesiredPosition => target.position + Vector3.up - ViewRotation * Vector3.forward * 22f;

        private void OnEnable() => Snap();

        private void Snap()
        {
            if (target == null) return;
            transform.SetPositionAndRotation(DesiredPosition, ViewRotation);
            velocity = Vector3.zero;
        }

        private void LateUpdate()
        {
            if (target == null) return;
            transform.position = Vector3.SmoothDamp(transform.position, DesiredPosition, ref velocity, smoothTime);
            transform.rotation = ViewRotation;
        }
    }
}
