using SonBirUmutDemo.Core;
using SonBirUmutDemo.Simulation;
using UnityEngine;
using UnityEngine.Rendering;

namespace SonBirUmutDemo.World
{
    // Visual game indicator, independent of emissions and physical climate calculations.
    public sealed class DemoCrisisAtmosphere : MonoBehaviour
    {
        [SerializeField] private DemoGameController game;
        [SerializeField] private Light sun;
        [SerializeField] private Camera view;
        [SerializeField] private bool useBasicFog = true;
        [SerializeField, Min(.01f)] private float transitionSpeed = .12f;
        private bool captured, oldFog, supportsFog;
        private FogMode oldFogMode;
        private Color oldFogColor, oldAmbient, oldSunColor, oldBackground;
        private float oldStart, oldEnd, oldIntensity, visualPressure;
        private GameRunState shownRun;

        public void Configure(DemoGameController owner, Light light, Camera camera)
        { game = owner; sun = light; view = camera; }

        private void Update()
        {
            if (game == null || !game.UsesCrisisMode || sun == null || view == null) return;
            if (!captured) Capture();
            if (!ReferenceEquals(shownRun, game.Run))
            { shownRun = game.Run; visualPressure = (float)game.Run.CrisisPressure / 100; }
            visualPressure = Mathf.MoveTowards(visualPressure, (float)game.Run.CrisisPressure / 100,
                transitionSpeed * Time.unscaledDeltaTime);
            float p = Mathf.Clamp01(visualPressure);
            sun.color = Color.Lerp(oldSunColor, new Color(.83f, .86f, .89f), p * .65f);
            sun.intensity = oldIntensity * Mathf.Lerp(1f, .85f, p);
            RenderSettings.ambientLight = Color.Lerp(oldAmbient, new Color(.54f, .58f, .61f), p * .65f);
            view.backgroundColor = Color.Lerp(oldBackground, new Color(.57f, .63f, .65f), p * .65f);
            if (useBasicFog && supportsFog)
            {
                RenderSettings.fog = true;
                RenderSettings.fogMode = FogMode.Linear;
                RenderSettings.fogColor = view.backgroundColor;
                // Keep the player (~22 units from follow camera), roads and markers clear.
                RenderSettings.fogStartDistance = Mathf.Lerp(38, 24, p);
                RenderSettings.fogEndDistance = Mathf.Lerp(100, 75, p);
            }
            else RestoreFog(); // Light/ambient/background fallback; no pipeline asset changes.
        }

        private void Capture()
        {
            oldFog = RenderSettings.fog; oldFogMode = RenderSettings.fogMode;
            oldFogColor = RenderSettings.fogColor;
            oldStart = RenderSettings.fogStartDistance; oldEnd = RenderSettings.fogEndDistance;
            oldAmbient = RenderSettings.ambientLight;
            oldSunColor = sun.color; oldIntensity = sun.intensity; oldBackground = view.backgroundColor;
            var pipeline = GraphicsSettings.currentRenderPipeline;
            supportsFog = pipeline == null || pipeline.GetType().Name.Contains("Universal");
            captured = true;
        }

        private void RestoreFog()
        {
            RenderSettings.fog = oldFog; RenderSettings.fogMode = oldFogMode;
            RenderSettings.fogColor = oldFogColor;
            RenderSettings.fogStartDistance = oldStart; RenderSettings.fogEndDistance = oldEnd;
        }

        private void OnDisable()
        {
            if (!captured) return;
            RestoreFog(); RenderSettings.ambientLight = oldAmbient;
            if (sun != null) { sun.color = oldSunColor; sun.intensity = oldIntensity; }
            if (view != null) view.backgroundColor = oldBackground;
            captured = false; shownRun = null;
        }
    }
}
