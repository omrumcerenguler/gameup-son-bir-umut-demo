using SonBirUmutDemo.Decisions;
using UnityEngine;

namespace SonBirUmutDemo.Interaction
{
    public sealed class DecisionStation : MonoBehaviour
    {
        [SerializeField] private string pointId;
        [SerializeField] private Transform interactionAnchor;
        [SerializeField, Min(0.1f)] private float interactionRadius = 3f;
        [SerializeField] private GameObject[] choiceVisuals;
        [SerializeField] private GameObject completedMarker;
        public string PointId => pointId;

        public void Configure(string id, Transform anchor, GameObject[] visuals, GameObject complete)
        {
            pointId = id;
            interactionAnchor = anchor;
            choiceVisuals = visuals;
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
            DecisionPointDefinition definition = null;
            foreach (var point in DecisionCatalog.All) if (point.Id == pointId) definition = point;
            if (choiceVisuals != null)
                for (int i = 0; i < choiceVisuals.Length; i++)
                    if (choiceVisuals[i] != null) choiceVisuals[i].SetActive(definition != null &&
                        i < definition.Options.Count && definition.Options[i].Id == choice);
            if (completedMarker != null) completedMarker.SetActive(choice != null);
        }

        private void OnDrawGizmosSelected()
        {
            if (interactionAnchor == null) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(interactionAnchor.position, interactionRadius);
        }
    }
}
