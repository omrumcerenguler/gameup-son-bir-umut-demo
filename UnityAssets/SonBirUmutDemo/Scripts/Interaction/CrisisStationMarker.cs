using SonBirUmutDemo.Core;
using SonBirUmutDemo.Simulation;
using UnityEngine;

namespace SonBirUmutDemo.Interaction
{
    // Parent this component to the permanent station root, never to a choice visual.
    // Presentation only: input and rewards belong to DemoGameController/GameRunState.
    public sealed class CrisisStationMarker : MonoBehaviour
    {
        [SerializeField] private DemoGameController game;
        [SerializeField] private string stationId;
        [SerializeField] private GameObject maintenanceVisual;
        [SerializeField] private GameObject emergencyVisual;

        public void Configure(DemoGameController owner, string id, GameObject maintenance, GameObject emergency)
        {
            game = owner; stationId = id; maintenanceVisual = maintenance; emergencyVisual = emergency;
            if (maintenanceVisual != null) maintenanceVisual.SetActive(false);
            if (emergencyVisual != null) emergencyVisual.SetActive(false);
        }

        private void LateUpdate()
        {
            bool active = game != null && game.UsesCrisisDemo && !game.IsComplete &&
                game.Run.Phase == GameRunPhase.CrisisManagement;
            bool emergency = active && game.EmergencyActive;
            var task = active ? game.Run.Crisis.AtStation(stationId) : null;
            if (maintenanceVisual != null) maintenanceVisual.SetActive(active && !emergency &&
                task != null && task.Status == MaintenanceStatus.Available);
            if (emergencyVisual != null) emergencyVisual.SetActive(emergency && game.Run.Crisis.Emergency.StationId == stationId);
        }
    }
}
