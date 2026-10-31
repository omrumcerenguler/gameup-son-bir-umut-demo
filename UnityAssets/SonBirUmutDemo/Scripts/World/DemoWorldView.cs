using SonBirUmutDemo.Core;
using SonBirUmutDemo.Simulation;
using UnityEngine;

namespace SonBirUmutDemo.World
{
    public sealed class DemoWorldView : MonoBehaviour
    {
        // Original fields and Configure signature remain valid for existing scenes.
        [SerializeField]
        private Renderer[] treeCrowns;
        [SerializeField]
        private Renderer[] crops;
        [SerializeField]
        private Transform waterColumn;
        [SerializeField]
        private float waterBottom;
        [SerializeField]
        private float waterHeight;
        [SerializeField]
        private GameObject[] dryBranches;
        [SerializeField]
        private Transform[] cropRoots;
        [SerializeField]
        private Vector3[] cropScales;
        [SerializeField]
        private Renderer[] fieldSoil;
        [SerializeField]
        private Transform pondSurface;
        [SerializeField]
        private Vector3 pondScale;
        [SerializeField]
        private GameObject dryPondCracks;
        [SerializeField]
        private DemoGameController game;
        private MaterialPropertyBlock colors;
        private GameSession seenSession;
        private float plantView, waterView, targetPlants, targetWater;
        private bool initialized;
        public void Configure(Renderer[] trees, Renderer[] field, Transform gauge)
        {
            treeCrowns = trees;
            crops = field;
            waterColumn = gauge;
            waterHeight = gauge.localScale.y;
            waterBottom = gauge.localPosition.y - waterHeight * .5f;
        }

        public void ConfigurePresentation(GameObject[] branches, Transform[] plants, Renderer[] soil, Transform pond, GameObject cracks)
        {
            dryBranches = branches;
            cropRoots = plants;
            cropScales = new Vector3[plants.Length];
            for (int i = 0; i < plants.Length; i++)
                cropScales[i] = plants[i].localScale;
            fieldSoil = soil;
            pondSurface = pond;
            pondScale = pond.localScale;
            dryPondCracks = cracks;
        }

        public void SetGame(DemoGameController owner) => game = owner;
        public void Apply(ResourceState state)
        {
            if (state == null)
                return;
            targetPlants = state.PlantHealth;
            targetWater = state.Water;
            // A new session/replay snaps to its initial appearance. Choices animate visuals only.
            bool snap = !initialized || !Application.isPlaying || game == null || !ReferenceEquals(seenSession, game.Session);
            if (snap)
            {
                plantView = targetPlants;
                waterView = targetWater;
                RenderState();
            }

            seenSession = game != null ? game.Session : null;
            initialized = true;
        }

        private void Update()
        {
            if (!initialized || (Mathf.Approximately(plantView, targetPlants) && Mathf.Approximately(waterView, targetWater)))
                return;
            plantView = Mathf.MoveTowards(plantView, targetPlants, Time.deltaTime * 45f);
            waterView = Mathf.MoveTowards(waterView, targetWater, Time.deltaTime * 45f);
            RenderState();
        }

        private void RenderState()
        {
            bool dry = plantView < 30;
            Color foliage = plantView >= 55 ? new Color(.23f, .58f, .25f) : plantView >= 30 ? new Color(.75f, .57f, .20f) : new Color(.40f, .27f, .16f);
            Paint(treeCrowns, foliage);
            if (dryBranches != null && dryBranches.Length > 0)
            {
                foreach (var branches in dryBranches)
                    if (branches != null)
                        branches.SetActive(dry);
                if (treeCrowns != null)
                    foreach (var crown in treeCrowns)
                        if (crown != null)
                            crown.gameObject.SetActive(!dry);
            }

            Paint(crops, Color.Lerp(new Color(.40f, .24f, .12f), new Color(.25f, .65f, .23f), plantView / 100f));
            Paint(fieldSoil, Color.Lerp(new Color(.66f, .48f, .28f), new Color(.32f, .29f, .16f), plantView / 100f));
            if (cropRoots != null && cropScales != null)
                for (int i = 0; i < cropRoots.Length && i < cropScales.Length; i++)
                    if (cropRoots[i] != null)
                    {
                        Vector3 size = cropScales[i];
                        size.y *= Mathf.Lerp(.16f, 1f, plantView / 100f);
                        cropRoots[i].localScale = size;
                    }

            if (waterColumn != null)
            {
                var size = waterColumn.localScale;
                size.y = Mathf.Lerp(.04f, waterHeight, waterView / 100f);
                waterColumn.localScale = size;
                var position = waterColumn.localPosition;
                position.y = waterBottom + size.y * .5f;
                waterColumn.localPosition = position;
            }

            if (pondSurface != null)
            {
                pondSurface.gameObject.SetActive(waterView > .01f);
                float fill = Mathf.Lerp(.12f, 1f, waterView / 100f);
                pondSurface.localScale = new Vector3(pondScale.x * fill, pondScale.y, pondScale.z * fill);
            }

            if (dryPondCracks != null)
                dryPondCracks.SetActive(waterView < 20);
        }

        private void Paint(Renderer[] renderers, Color color)
        {
            if (renderers == null)
                return;
            if (colors == null)
                colors = new MaterialPropertyBlock();
            foreach (var renderer in renderers)
            {
                if (renderer == null)
                    continue;
                renderer.GetPropertyBlock(colors);
                colors.SetColor("_BaseColor", color);
                colors.SetColor("_Color", color);
                renderer.SetPropertyBlock(colors);
                colors.Clear();
            }
        }
    }
}
