using UnityEngine;

namespace SonBirUmutDemo.Player
{
    [RequireComponent(typeof(Camera))]
    public sealed class IsometricFollowCamera : MonoBehaviour
    {
        [SerializeField]
        private Transform target;
        [SerializeField]
        private Transform overviewTarget;
        [SerializeField, Min(0.01f)]
        private float smoothTime = 0.18f;
        [SerializeField, Min(9f)]
        private float overviewSize = 20f;
        [SerializeField, Min(0f)]
        private float overviewHorizontalOffset = 14f;
        private Camera view;
        private Vector3 velocity;
        private float zoomVelocity;
        private float distanceVelocity;
        private float normalSize = 8.5f;
        private float distance = 22f;
        private bool overview;
        private bool resultPanelVisible = true;
        private float horizontalOffset, offsetVelocity;
        private static readonly Quaternion ViewRotation = Quaternion.Euler(35.264f, 45f, 0f);
        public void Configure(Transform followTarget) => Configure(followTarget, null);
        public void Configure(Transform followTarget, Transform villageCenter)
        {
            target = followTarget;
            overviewTarget = villageCenter;
            view = GetComponent<Camera>();
            normalSize = view.orthographicSize;
            ResetView();
        }

        private void Awake()
        {
            EnsureView();
        }

        private void OnEnable()
        {
            if (target != null)
                ResetView();
        }

        public void SetOverview(bool value) => overview = value;
        public void SetResultPanelVisible(bool value) => resultPanelVisible = value;
        public void ResetView()
        {
            overview = false;
            resultPanelVisible = true;
            horizontalOffset = offsetVelocity = 0;
            velocity = Vector3.zero;
            zoomVelocity = distanceVelocity = 0;
            distance = 22f;
            EnsureView();
            if (target == null)
                return;
            view.orthographicSize = normalSize;
            transform.SetPositionAndRotation(target.position + Vector3.up - ViewRotation * Vector3.forward * distance, ViewRotation);
        }

        private void LateUpdate()
        {
            if (target == null)
                return;
            Transform focus = overview && overviewTarget != null ? overviewTarget : target;
            float blend = overview ? 0.8f : smoothTime;
            distance = Mathf.SmoothDamp(distance, overview ? 38f : 22f, ref distanceVelocity, blend);
            view.orthographicSize = Mathf.SmoothDamp(view.orthographicSize, overview ? overviewSize : normalSize, ref zoomVelocity, blend);
            // Frame the village to the left of the results panel, with a fixed isometric angle.
            horizontalOffset = Mathf.SmoothDamp(horizontalOffset, overview && resultPanelVisible ? overviewHorizontalOffset : 0f, ref offsetVelocity, blend);
            Vector3 framingOffset = ViewRotation * Vector3.right * horizontalOffset;
            Vector3 desired = focus.position + Vector3.up + framingOffset - ViewRotation * Vector3.forward * distance;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, blend);
            transform.rotation = ViewRotation;
        }

        private void EnsureView()
        {
            if (view != null)
                return;
            view = GetComponent<Camera>();
            normalSize = view.orthographicSize;
        }
    }
}
