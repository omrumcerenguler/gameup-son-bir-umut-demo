using SonBirUmutDemo.Core;
using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.Simulation;
using UnityEngine;

namespace SonBirUmutDemo.World
{
    // Presentation on permanent station roots. Never changes resources or reads input.
    public sealed class DemoStationFeedback : MonoBehaviour
    {
        [SerializeField] private DemoGameController game;
        [SerializeField] private string stationId;
        [SerializeField] private GameObject checkedVisual;
        [SerializeField] private GameObject repairedSeal;
        [SerializeField] private GameObject missedPatch;
        [SerializeField] private Transform[] leakDrops;
        [SerializeField] private Renderer solarLamp;
        private MaterialPropertyBlock color;
        private GameRunState shownRun;
        private float visualTime;

        public void Configure(DemoGameController owner, string id, GameObject checkedMark,
            Transform[] drops, GameObject seal, GameObject missed, Renderer panelLamp)
        {
            game = owner; stationId = id; checkedVisual = checkedMark;
            leakDrops = drops; repairedSeal = seal; missedPatch = missed; solarLamp = panelLamp;
            if (checkedVisual != null) checkedVisual.SetActive(false);
            if (repairedSeal != null) repairedSeal.SetActive(false);
            if (missedPatch != null) missedPatch.SetActive(false);
            if (leakDrops != null) foreach (var drop in leakDrops) if (drop != null) drop.gameObject.SetActive(false);
            if (solarLamp != null) solarLamp.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (game == null || !game.UsesPlaytestMode) return;
            if (!ReferenceEquals(shownRun, game.Run)) { shownRun = game.Run; visualTime = 0; }
            if (!game.SimulationPaused) visualTime += Time.unscaledDeltaTime;
            bool completed = false;
            foreach (var task in game.Run.Crisis.Tasks)
                if (task.StationId == stationId && task.Status == MaintenanceStatus.Completed) completed = true;
            var currentTask = game.Run.Crisis.AtStation(stationId);
            bool available = currentTask != null && currentTask.Status == MaintenanceStatus.Available;
            if (checkedVisual != null) checkedVisual.SetActive(completed && !available);
            bool water = stationId == DecisionCatalog.WaterStationId;
            var incident = game.Run.Crisis.Emergency;
            if (repairedSeal != null) repairedSeal.SetActive(water && incident.Status == EmergencyStatus.Repaired);
            if (missedPatch != null) missedPatch.SetActive(water && incident.Status == EmergencyStatus.Expired);
            if (leakDrops != null)
                for (int i = 0; i < leakDrops.Length; i++)
                {
                    var drop = leakDrops[i];
                    if (drop == null) continue;
                    drop.gameObject.SetActive(water && incident.Status == EmergencyStatus.Active);
                    float t = Mathf.Repeat(visualTime * 1.7f + i / 3f, 1);
                    drop.localPosition = new Vector3(-.7f + i * .2f, .13f + (1 - t) * .8f, -1.25f - t * .5f);
                }
            if (solarLamp != null)
            {
                bool solar = game.Session.GetChoice(DecisionCatalog.EnergyStationId) == DecisionCatalog.Solar;
                solarLamp.gameObject.SetActive(solar);
                if (solar)
                {
                    if (color == null) color = new MaterialPropertyBlock();
                    Color light = Color.Lerp(new Color(.2f, .65f, .3f), new Color(.7f, 1f, .6f), .5f + .5f * Mathf.Sin(visualTime * 3));
                    solarLamp.GetPropertyBlock(color); color.SetColor("_BaseColor", light); color.SetColor("_Color", light);
                    solarLamp.SetPropertyBlock(color); color.Clear();
                }
            }
        }
    }
}
