#if UNITY_EDITOR
using System.Collections.Generic;
using SonBirUmutDemo.Core;
using SonBirUmutDemo.Player;
using UnityEditor;
using UnityEngine;

namespace SonBirUmutDemo.Editor
{
    // All generated meshes/materials are real Editor assets, saved alongside the new scene.
    // This helper decorates the existing map; it never reads or writes gameplay resources.
    public sealed class DemoVisualBuilder
    {
        private readonly Mesh canopy, roofMesh, head;
        private readonly Material plaster, tiles, timber, stone, glass, path, skin, cloth, trousers, straw;
        public DemoVisualBuilder(Shader shader, string folder)
        {
            plaster = Mat("Plaster", new Color(0.88f, 0.79f, 0.60f), shader, folder);
            tiles = Mat("Terracotta", new Color(0.55f, 0.24f, 0.16f), shader, folder);
            timber = Mat("Timber", new Color(0.27f, 0.20f, 0.14f), shader, folder);
            stone = Mat("Stone", new Color(0.48f, 0.49f, 0.43f), shader, folder);
            glass = Mat("Window", new Color(0.18f, 0.38f, 0.44f), shader, folder);
            path = Mat("Path", new Color(0.76f, 0.65f, 0.44f), shader, folder);
            skin = Mat("Skin", new Color(0.73f, 0.46f, 0.30f), shader, folder);
            cloth = Mat("Jacket", new Color(0.16f, 0.39f, 0.38f), shader, folder);
            trousers = Mat("Trousers", new Color(0.20f, 0.25f, 0.28f), shader, folder);
            straw = Mat("Straw", new Color(0.84f, 0.68f, 0.35f), shader, folder);
            canopy = FlatMesh("Canopy", new[] { Vector3.up * .5f, Vector3.right * .5f, Vector3.forward * .5f, Vector3.left * .5f, Vector3.back * .5f, Vector3.down * .5f }, new[] { 0, 2, 1, 0, 3, 2, 0, 4, 3, 0, 1, 4, 5, 1, 2, 5, 2, 3, 5, 3, 4, 5, 4, 1 }, folder);
            roofMesh = FlatMesh("GableRoof", new[] { new Vector3(-.5f, 0, -.5f), new Vector3(.5f, 0, -.5f), new Vector3(0, .6f, -.5f), new Vector3(-.5f, 0, .5f), new Vector3(.5f, 0, .5f), new Vector3(0, .6f, .5f) }, new[] { 0, 2, 1, 3, 4, 5, 0, 3, 5, 0, 5, 2, 1, 2, 5, 1, 5, 4, 0, 1, 4, 0, 4, 3 }, folder);
            float g = (1f + Mathf.Sqrt(5f)) / 2f;
            var points = new[]
            {
                new Vector3(-1, g, 0),
                new Vector3(1, g, 0),
                new Vector3(-1, -g, 0),
                new Vector3(1, -g, 0),
                new Vector3(0, -1, g),
                new Vector3(0, 1, g),
                new Vector3(0, -1, -g),
                new Vector3(0, 1, -g),
                new Vector3(g, 0, -1),
                new Vector3(g, 0, 1),
                new Vector3(-g, 0, -1),
                new Vector3(-g, 0, 1)
            };
            for (int i = 0; i < points.Length; i++)
                points[i] = points[i].normalized * .5f;
            head = FlatMesh("Icosahedron", points, new[] { 0, 11, 5, 0, 5, 1, 0, 1, 7, 0, 7, 10, 0, 10, 11, 1, 5, 9, 5, 11, 4, 11, 10, 2, 10, 7, 6, 7, 1, 8, 3, 9, 4, 3, 4, 2, 3, 2, 6, 3, 6, 8, 3, 8, 9, 4, 9, 5, 2, 4, 11, 6, 2, 10, 8, 6, 7, 9, 8, 1 }, folder);
        }

        public void House(Transform parent, Vector3 position)
        {
            Transform house = Group("Kiremit çatılı köy evi", parent, position);
            Cube("Taş temel", house, new Vector3(0, .15f, 0), new Vector3(3.3f, .3f, 3.3f), stone, true);
            Cube("Sıva duvar", house, new Vector3(0, 1.3f, 0), new Vector3(3, 2.3f, 3), plaster, true);
            Shape("Eğimli kiremit çatı", house, new Vector3(0, 2.45f, 0), new Vector3(3.65f, 2, 3.65f), roofMesh, tiles);
            Cube("Baca", house, new Vector3(.9f, 3.2f, .6f), new Vector3(.38f, 1, .4f), stone);
            Cube("Ahşap kapı", house, new Vector3(0, 1.15f, -1.53f), new Vector3(.85f, 2.05f, .1f), timber);
            Cube("Kapı eşiği", house, new Vector3(0, .14f, -1.8f), new Vector3(1, .18f, .5f), stone);
            foreach (float x in new[]
            {
                -.95f,
                .95f
            }

            )
            {
                Cube("Pencere pervazı", house, new Vector3(x, 1.5f, -1.54f), new Vector3(.65f, .8f, .12f), timber);
                Cube("Mavi pencere", house, new Vector3(x, 1.5f, -1.62f), new Vector3(.48f, .62f, .06f), glass);
                Cube("Pencere kaydı", house, new Vector3(x, 1.5f, -1.67f), new Vector3(.05f, .65f, .05f), plaster);
            }

            Cube("Yan pencere", house, new Vector3(-1.53f, 1.5f, 0), new Vector3(.1f, .7f, .7f), glass);
        }

        public Renderer Tree(Transform parent, Vector3 position, Material foliage, out GameObject dryBranches)
        {
            var root = Group("Çokgen ağaç - sağlık görünümü", parent, position);
            Cube("Gövde", root, new Vector3(0, 1, 0), new Vector3(.4f, 2, .4f), timber, true);
            var crown = Shape("Yaprak tacı", root, new Vector3(0, 2.45f, 0), new Vector3(2.8f, 2.6f, 2.5f), canopy, foliage);
            var dry = Group("Kurumuş dallar", root, Vector3.zero);
            foreach (float side in new[]
            {
                -1f,
                1f
            }

            )
            {
                var branch = Cube("Çıplak dal", dry, new Vector3(side * .45f, 1.9f, 0), new Vector3(.16f, 1.2f, .16f), timber);
                branch.transform.localRotation = Quaternion.Euler(0, 0, side * -42f);
            }

            dryBranches = dry.gameObject;
            dryBranches.SetActive(false);
            return crown.GetComponent<Renderer>();
        }

        public void VillageDetails(Transform environment)
        {
            var root = Group("Köy detayları", environment, Vector3.zero);
            Path(root, new Vector3(-9, 0, -8), new Vector3(1, 0, -4), 1.4f);
            Path(root, new Vector3(1, 0, -4), new Vector3(3, 0, -2), 1.5f);
            Path(root, new Vector3(1, 0, -2), new Vector3(-2, 0, 3), 1.3f);
            Path(root, new Vector3(2, 0, -4), new Vector3(6, 0, -7.5f), 1.3f);
            for (int i = 0; i < 9; i++)
                Fence(root, new Vector3(-12 + i * 3, 0, 10.7f));
            for (int i = 0; i < 6; i++)
            {
                var rock = Shape("Kenar kayası", root, new Vector3(-12.4f + i * 4.6f, .28f, -10.4f), new Vector3(.9f, .65f, .75f), head, stone);
                rock.transform.localRotation = Quaternion.Euler(0, i * 37, 12);
            }

            var well = Group("Kuyu ve kova", root, new Vector3(-5, 0, -8));
            Cube("Kuyu ağzı", well, new Vector3(0, .4f, 0), new Vector3(1.4f, .8f, 1.4f), stone, true);
            Cube("Kuyu karanlığı", well, new Vector3(0, .81f, 0), new Vector3(.95f, .02f, .95f), timber);
            Cube("Kova", well, new Vector3(.95f, .23f, 0), new Vector3(.4f, .45f, .4f), timber);
            var bench = Group("Köy bankı", root, new Vector3(0, 0, 6.7f));
            Cube("Oturak", bench, new Vector3(0, .55f, 0), new Vector3(1.8f, .15f, .6f), timber, true);
            foreach (float x in new[]
            {
                -.65f,
                .65f
            }

            )
                Cube("Bank ayağı", bench, new Vector3(x, .25f, 0), new Vector3(.12f, .5f, .4f), stone);
            for (int i = 0; i < 12; i++)
            {
                Vector3 p = new Vector3(-11 + i * 1.9f, .16f, 9.3f);
                var grass = Cube("Kuru ot", root, p, new Vector3(.08f, .35f, .45f), straw);
                grass.transform.localRotation = Quaternion.Euler(0, i * 71, 12);
            }
        }

        public Transform Crop(Transform parent, Vector3 position, Material foliage, List<Renderer> renderers)
        {
            var root = Group("Çokgen tarla ürünü", parent, position);
            renderers.Add(Cube("Sap", root, new Vector3(0, .35f, 0), new Vector3(.10f, .7f, .10f), foliage).GetComponent<Renderer>());
            foreach (float side in new[]
            {
                -1f,
                1f
            }

            )
            {
                var leaf = Shape("Yaprak", root, new Vector3(side * .16f, .42f, 0), new Vector3(.5f, .18f, .24f), canopy, foliage);
                leaf.transform.localRotation = Quaternion.Euler(0, 0, side * 25);
                renderers.Add(leaf.GetComponent<Renderer>());
            }

            return root;
        }

        public DemoCharacterVisual Character(Transform player)
        {
            var root = Group("Low-poly köylü", player, Vector3.down * 1.05f);
            root.localScale = Vector3.one * .94f;
            Cube("Ceket", root, new Vector3(0, 1.15f, 0), new Vector3(.55f, .7f, .35f), cloth);
            Cube("Sırt çantası", root, new Vector3(0, 1.15f, -.25f), new Vector3(.4f, .45f, .22f), tiles);
            Shape("Çokgen baş", root, new Vector3(0, 1.74f, 0), new Vector3(.52f, .57f, .5f), head, skin);
            Cube("Saç", root, new Vector3(0, 1.93f, -.02f), new Vector3(.42f, .17f, .4f), timber);
            Cube("Şapka kenarı", root, new Vector3(0, 2.01f, 0), new Vector3(.68f, .08f, .64f), straw);
            Cube("Şapka tepesi", root, new Vector3(0, 2.10f, -.03f), new Vector3(.4f, .15f, .4f), straw);
            foreach (float x in new[]
            {
                -.09f,
                .09f
            }

            )
                Cube("Göz", root, new Vector3(x, 1.77f, .215f), new Vector3(.045f, .045f, .04f), timber);
            Shape("Burun - ileri yön", root, new Vector3(0, 1.69f, .25f), new Vector3(.1f, .12f, .2f), canopy, skin);
            Transform leftLeg = Limb(root, "Sol bacak", new Vector3(-.16f, .82f, 0), new Vector3(.22f, .68f, .24f), trousers, true);
            Transform rightLeg = Limb(root, "Sağ bacak", new Vector3(.16f, .82f, 0), new Vector3(.22f, .68f, .24f), trousers, true);
            Transform leftArm = Limb(root, "Sol kol", new Vector3(-.39f, 1.43f, 0), new Vector3(.18f, .57f, .22f), cloth, false);
            Transform rightArm = Limb(root, "Sağ kol", new Vector3(.39f, 1.43f, 0), new Vector3(.18f, .57f, .22f), cloth, false);
            var visual = player.gameObject.AddComponent<DemoCharacterVisual>();
            visual.Configure(leftLeg, rightLeg, leftArm, rightArm);
            return visual;
        }

        private Transform Limb(Transform parent, string name, Vector3 position, Vector3 size, Material material, bool boot)
        {
            var pivot = Group(name, parent, position);
            Cube(name + " model", pivot, Vector3.down * size.y * .5f, size, material);
            Cube(boot ? "Bot" : "El", pivot, new Vector3(0, -size.y, boot ? .055f : 0), boot ? new Vector3(.25f, .18f, .36f) : new Vector3(.15f, .17f, .16f), boot ? timber : skin);
            return pivot;
        }

        private void Path(Transform parent, Vector3 a, Vector3 b, float width)
        {
            var item = Cube("Toprak patika", parent, (a + b) * .5f + Vector3.up * .022f, new Vector3(width, .04f, Vector3.Distance(a, b)), path);
            item.transform.rotation = Quaternion.LookRotation(b - a);
        }

        private void Fence(Transform parent, Vector3 position)
        {
            var root = Group("Ahşap çit", parent, position);
            foreach (float x in new[]
            {
                -1.2f,
                1.2f
            }

            )
                Cube("Çit direği", root, new Vector3(x, .6f, 0), new Vector3(.15f, 1.2f, .15f), timber);
            foreach (float y in new[]
            {
                .4f,
                .9f
            }

            )
                Cube("Çit kirişi", root, new Vector3(0, y, 0), new Vector3(2.5f, .12f, .12f), timber, true);
        }

        private static Mesh FlatMesh(string name, Vector3[] points, int[] indices, string folder)
        {
            var vertices = new Vector3[indices.Length];
            var triangles = new int[indices.Length];
            var uv = new Vector2[indices.Length];
            for (int i = 0; i < indices.Length; i++)
            {
                vertices[i] = points[indices[i]];
                triangles[i] = i;
                uv[i] = new Vector2(i % 3 == 1 ? 1 : 0, i % 3 == 2 ? 1 : 0);
            }

            var mesh = new Mesh
            {
                name = "Art_" + name,
                vertices = vertices,
                triangles = triangles,
                uv = uv
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            AssetDatabase.CreateAsset(mesh, folder + "/Art_" + name + ".asset");
            return mesh;
        }

        private static Material Mat(string name, Color color, Shader shader, string folder)
        {
            var mat = new Material(shader)
            {
                name = "Art_" + name
            };
            mat.SetColor(mat.HasProperty("_BaseColor") ? "_BaseColor" : "_Color", color);
            if (mat.HasProperty("_Smoothness"))
                mat.SetFloat("_Smoothness", .18f);
            if (mat.HasProperty("_Glossiness"))
                mat.SetFloat("_Glossiness", .18f);
            AssetDatabase.CreateAsset(mat, folder + "/Art_" + name + ".mat");
            return mat;
        }

        private static Transform Group(string name, Transform parent, Vector3 position)
        {
            var root = new GameObject(name).transform;
            root.SetParent(parent, false);
            root.localPosition = position;
            return root;
        }

        private static GameObject Shape(string name, Transform parent, Vector3 position, Vector3 size, Mesh mesh, Material material)
        {
            var root = Group(name, parent, position);
            root.localScale = size;
            root.gameObject.AddComponent<MeshFilter>().sharedMesh = mesh;
            root.gameObject.AddComponent<MeshRenderer>().sharedMaterial = material;
            return root.gameObject;
        }

        private static GameObject Cube(string name, Transform parent, Vector3 position, Vector3 size, Material material, bool solid = false)
        {
            var item = GameObject.CreatePrimitive(PrimitiveType.Cube);
            item.name = name;
            item.transform.SetParent(parent, false);
            item.transform.localPosition = position;
            item.transform.localScale = size;
            item.GetComponent<Renderer>().sharedMaterial = material;
            if (!solid)
                Object.DestroyImmediate(item.GetComponent<Collider>());
            return item;
        }
    }
}
#endif
