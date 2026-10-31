using SonBirUmutDemo.Simulation;
using UnityEngine;

namespace SonBirUmutDemo.World
{
    public sealed class DemoWorldView : MonoBehaviour
    {
        [SerializeField] private Renderer[] treeCrowns;
        [SerializeField] private Renderer[] crops;
        [SerializeField] private Transform waterColumn;
        [SerializeField] private float waterBottom;
        [SerializeField] private float waterHeight;
        private MaterialPropertyBlock colors;

        public void Configure(Renderer[] trees, Renderer[] field, Transform gauge)
        {
            treeCrowns = trees;
            crops = field;
            waterColumn = gauge;
            waterHeight = gauge.localScale.y;
            waterBottom = gauge.localPosition.y - waterHeight * 0.5f;
        }

        public void Apply(ResourceState state)
        {
            if (state == null) return;
            Color foliage = state.PlantHealth >= 55 ? new Color(0.25f, 0.63f, 0.22f) :
                state.PlantHealth >= 30 ? new Color(0.70f, 0.61f, 0.19f) : new Color(0.40f, 0.28f, 0.16f);
            Paint(treeCrowns, foliage);
            Paint(crops, Color.Lerp(new Color(0.42f, 0.27f, 0.12f), new Color(0.27f, 0.68f, 0.18f), state.PlantHealth / 100f));
            if (waterColumn != null)
            {
                var scale = waterColumn.localScale;
                scale.y = Mathf.Lerp(0.04f, waterHeight, state.Water / 100f);
                waterColumn.localScale = scale;
                var position = waterColumn.localPosition;
                position.y = waterBottom + scale.y * 0.5f;
                waterColumn.localPosition = position;
            }
        }

        private void Paint(Renderer[] renderers, Color color)
        {
            if (renderers == null) return;
            if (colors == null) colors = new MaterialPropertyBlock();
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;
                renderer.GetPropertyBlock(colors);
                colors.SetColor("_BaseColor", color);
                colors.SetColor("_Color", color);
                renderer.SetPropertyBlock(colors);
                colors.Clear();
            }
        }
    }
}
