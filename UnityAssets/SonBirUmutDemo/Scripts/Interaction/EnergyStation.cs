using SonBirUmutDemo.Decisions;
using UnityEngine;

namespace SonBirUmutDemo.Interaction
{
    public sealed class EnergyStation : MonoBehaviour
    {
        [SerializeField] private Transform interactionAnchor;
        [SerializeField] private float interactionRadius = 3f;
        [SerializeField] private GameObject solarPanels;
        [SerializeField] private GameObject dieselGenerator;
        [SerializeField] private GameObject conservationMarker;
        [SerializeField] private GameObject completedMarker;

        public void Configure(Transform anchor, GameObject solar, GameObject diesel, GameObject conservation, GameObject complete)
        {
            interactionAnchor = anchor;
            solarPanels = solar;
            dieselGenerator = diesel;
            conservationMarker = conservation;
            completedMarker = complete;
            ShowChoice(null);
        }

        public bool IsInRange(Vector3 position)
        {
            if (interactionAnchor == null) return false;
            Vector3 delta = position - interactionAnchor.position;
            delta.y = 0;
            return delta.sqrMagnitude <= interactionRadius * interactionRadius;
        }

        public void ShowChoice(string choice)
        {
            if (solarPanels != null) solarPanels.SetActive(choice == DecisionCatalog.Solar);
            if (dieselGenerator != null) dieselGenerator.SetActive(choice == DecisionCatalog.Diesel);
            if (conservationMarker != null) conservationMarker.SetActive(choice == DecisionCatalog.Conservation);
            if (completedMarker != null) completedMarker.SetActive(choice != null);
        }

        private void OnDrawGizmosSelected()
        {
            if (interactionAnchor == null) return;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(interactionAnchor.position, interactionRadius);
        }
    }
}
