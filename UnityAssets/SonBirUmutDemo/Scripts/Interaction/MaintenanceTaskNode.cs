using SonBirUmutDemo.Core;
using SonBirUmutDemo.Simulation;
using UnityEngine;

namespace SonBirUmutDemo.Interaction
{
    // Permanent independent location. No input polling or resource mutation here.
    public sealed class MaintenanceTaskNode : MonoBehaviour
    {
        [SerializeField] private DemoGameController game;
        [SerializeField] private string taskId;
        [SerializeField] private string locationId;
        [SerializeField] private GameObject activeVisual, completedVisual;
        public string TaskId => taskId;
        public string LocationId => locationId;
        public void Configure(DemoGameController owner,PacingTaskDefinition definition,GameObject active,GameObject completed)
        {
            game=owner; taskId=definition.TaskId; locationId=definition.LocationId;
            activeVisual=active; completedVisual=completed; RefreshNow();
        }
        public bool IsInRange(Vector3 position)
        {
            Vector3 delta=position-transform.position; delta.y=0;
            return delta.sqrMagnitude<=FinalPacingCatalog.InteractionRadius*FinalPacingCatalog.InteractionRadius;
        }
        public void RefreshNow() => LateUpdate();
        private void LateUpdate()
        {
            var task=game!=null && game.UsesTaskNodes ? game.Run.Crisis.AtLocation(locationId) : null;
            bool playing=task!=null && !game.IsComplete && game.Run.Phase==GameRunPhase.CrisisManagement;
            if(activeVisual!=null) activeVisual.SetActive(playing && !game.EmergencyActive && task.Status==MaintenanceStatus.Available);
            if(completedVisual!=null) completedVisual.SetActive(task!=null && task.Status==MaintenanceStatus.Completed);
        }
    }
}
