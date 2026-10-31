#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using SonBirUmutDemo.Core;
using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.Simulation;
using SonBirUmutDemo.World;
using SonBirUmutDemo.Interaction;
using SonBirUmutDemo.Player;
using SonBirUmutDemo.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace SonBirUmutDemo.Editor
{
    public static class DemoSceneBuilder
    {
        private const string Root = "Assets/SonBirUmutDemo";

        [MenuItem("Tools/GameUP/Create Demo Scene")]
        public static void CreateDemoScene()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) return;
            if (!DemoInput.SupportsRuntimeUi)
            {
                EditorUtility.DisplayDialog("IMGUI giriş ayarı gerekli",
                    "Edit > Project Settings > Player > Other Settings > Active Input Handling ayarını Both veya Input Manager (Old) yapın ve Editor'ı yeniden başlatın. New tek başına IMGUI kart tıklamalarını desteklemez. Sahne oluşturulmadı.", "Tamam");
                return;
            }
            if (PrefabStageUtility.GetCurrentPrefabStage() != null)
            {
                EditorUtility.DisplayDialog("Önce sahne görünümüne dönün",
                    "Prefab düzenleme modundan çıkın ve sonra tekrar deneyin. Hiçbir sahne değiştirilmedi.", "Tamam");
                return;
            }
            // Do not save over, discard or modify any existing scene, even on repeated invocation.
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (!SceneManager.GetSceneAt(i).isDirty) continue;
                EditorUtility.DisplayDialog("Önce sahnenizi koruyun",
                    "Açık sahnelerde kaydedilmemiş değişiklik var. Önce kendiniz kaydedin veya değişiklikleri geri alın, sonra aracı tekrar çalıştırın. Hiçbir sahne değiştirilmedi.", "Tamam");
                return;
            }

            var pipeline = GraphicsSettings.currentRenderPipeline;
            bool urp = pipeline != null && pipeline.GetType().Name.Contains("Universal");
            if (pipeline != null && !urp)
            {
                EditorUtility.DisplayDialog("Render pipeline",
                    "Bu prototip Built-in 3D veya Universal 3D (URP) proje bekler. HDRP desteklenmez.", "Tamam");
                return;
            }
            Shader shader = Shader.Find(urp ? "Universal Render Pipeline/Lit" : "Standard");
            if (shader == null) throw new InvalidOperationException("Uygun Lit shader bulunamadı. Projenin render pipeline ayarını kontrol edin.");

            EnsureFolder("Assets", "SonBirUmutDemo");
            EnsureFolder(Root, "Scenes");
            EnsureFolder(Root, "Generated");
            string materialsFolder = AssetDatabase.GenerateUniqueAssetPath(Root + "/Generated/CompleteDemo");
            AssetDatabase.CreateFolder(Root + "/Generated", System.IO.Path.GetFileName(materialsFolder));
            string scenePath = AssetDatabase.GenerateUniqueAssetPath(Root + "/Scenes/SonBirUmutCompleteDemo.unity");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var terrain = Material("DrySoil", new Color(0.66f, 0.53f, 0.31f), shader, materialsFolder);
            var wall = Material("ClayWalls", new Color(0.86f, 0.77f, 0.56f), shader, materialsFolder);
            var roof = Material("Roof", new Color(0.47f, 0.22f, 0.12f), shader, materialsFolder);
            var wood = Material("Wood", new Color(0.30f, 0.20f, 0.11f), shader, materialsFolder);
            var leaves = Material("DryLeaves", new Color(0.44f, 0.49f, 0.20f), shader, materialsFolder);
            var metal = Material("Metal", new Color(0.36f, 0.42f, 0.44f), shader, materialsFolder);
            var water = Material("Water", new Color(0.22f, 0.57f, 0.72f), shader, materialsFolder);
            var solar = Material("SolarCells", new Color(0.09f, 0.19f, 0.38f), shader, materialsFolder);
            var yellow = Material("StationYellow", new Color(0.96f, 0.68f, 0.15f), shader, materialsFolder);
            var green = Material("CompleteGreen", new Color(0.18f, 0.70f, 0.31f), shader, materialsFolder);
            var playerMaterial = Material("Player", new Color(0.88f, 0.31f, 0.17f), shader, materialsFolder);

            Transform environment = new GameObject("Kırsal harita - Son Bir Umut").transform;
            var treeCrowns = new List<Renderer>();
            Primitive("Zemin", PrimitiveType.Cube, environment, new Vector3(0, -0.25f, 0), new Vector3(28, 0.5f, 24), terrain);
            // Invisible boundaries keep the character on the small map.
            Boundary(environment, new Vector3(-14, 1, 0), new Vector3(0.3f, 3, 24));
            Boundary(environment, new Vector3(14, 1, 0), new Vector3(0.3f, 3, 24));
            Boundary(environment, new Vector3(0, 1, -12), new Vector3(28, 3, 0.3f));
            Boundary(environment, new Vector3(0, 1, 12), new Vector3(28, 3, 0.3f));
            foreach (Vector3 position in new[] { new Vector3(-7, 0, 3), new Vector3(-7, 0, -4), new Vector3(7, 0, 7) })
            {
                Primitive("Köy evi", PrimitiveType.Cube, environment, position + Vector3.up * 1.2f, new Vector3(3, 2.4f, 3), wall);
                Primitive("Çatı", PrimitiveType.Cube, environment, position + Vector3.up * 2.6f, new Vector3(3.5f, 0.5f, 3.5f), roof);
                Primitive("Kapı", PrimitiveType.Cube, environment, position + new Vector3(0, 0.8f, -1.53f), new Vector3(0.8f, 1.6f, 0.1f), wood);
            }
            foreach (Vector3 position in new[] { new Vector3(-10, 0, 8), new Vector3(-10, 0, -8), new Vector3(9, 0, -8), new Vector3(10, 0, -3), new Vector3(0, 0, 8) })
            {
                Primitive("Ağaç gövdesi", PrimitiveType.Cylinder, environment, position + Vector3.up, new Vector3(0.4f, 1, 0.4f), wood);
                treeCrowns.Add(Primitive("Ağaç tacı", PrimitiveType.Sphere, environment, position + Vector3.up * 2.4f, new Vector3(2, 1.8f, 2), leaves).GetComponent<Renderer>());
            }
            var waterPoint = CreateWaterStation(environment, metal, water, green, yellow, out var waterGauge);
            var farmPoint = CreateFarmStation(environment, terrain, wood, leaves, water, green, out var crops);
            var world = environment.gameObject.AddComponent<DemoWorldView>();
            world.Configure(treeCrowns.ToArray(), crops, waterGauge);
            world.Apply(ResourceState.Initial());

            var stationObject = new GameObject("01 - ENERJİ İSTASYONU");
            stationObject.transform.SetParent(environment, false);
            stationObject.transform.localPosition = new Vector3(3, 0, 0);
            var station = stationObject.AddComponent<EnergyStation>();
            Primitive("İstasyon kaidesi", PrimitiveType.Cube, stationObject.transform, new Vector3(0, 0.15f, 0), new Vector3(3, 0.3f, 2), metal);
            Primitive("Kontrol kutusu", PrimitiveType.Cube, stationObject.transform, new Vector3(0, 0.85f, 0.35f), new Vector3(0.8f, 1.1f, 0.5f), yellow);
            var anchor = new GameObject("Etkileşim konumu").transform;
            anchor.SetParent(stationObject.transform, false);
            anchor.localPosition = new Vector3(0, 1, -1.2f);
            Transform panels = Group("Güneş panelleri", stationObject.transform);
            for (int i = 0; i < 2; i++)
            {
                float x = -1.1f + i * 2.2f;
                Primitive("Panel desteği", PrimitiveType.Cylinder, panels, new Vector3(x, 0.9f, 1.8f), new Vector3(0.12f, 0.9f, 0.12f), metal, false);
                var panel = Primitive("Güneş paneli", PrimitiveType.Cube, panels, new Vector3(x, 1.8f, 1.8f), new Vector3(1.9f, 0.1f, 1.5f), solar, false);
                panel.transform.localRotation = Quaternion.Euler(22, 0, 0);
            }
            Transform generator = Group("Dizel jeneratör", stationObject.transform);
            Primitive("Jeneratör gövdesi", PrimitiveType.Cube, generator, new Vector3(1.6f, 0.65f, 1.5f), new Vector3(1.8f, 1.1f, 1.1f), yellow, false);
            Primitive("Egzoz (sıcaklık etkisi yok)", PrimitiveType.Cylinder, generator, new Vector3(2.2f, 1.5f, 1.7f), new Vector3(0.18f, 0.5f, 0.18f), metal, false);
            var conservation = Primitive("Tasarruf işareti", PrimitiveType.Cube, stationObject.transform, new Vector3(-1.5f, 1.3f, 0.4f), new Vector3(0.6f, 1, 0.2f), green, false);
            var completed = Primitive("Karar tamamlandı lambası", PrimitiveType.Sphere, stationObject.transform, new Vector3(0, 1.7f, 0.35f), Vector3.one * 0.4f, green, false);
            station.Configure(anchor, panels.gameObject, generator.gameObject, conservation, completed);

            var playerObject = new GameObject("Oyuncu");
            playerObject.transform.position = new Vector3(1, 1.05f, -4);
            var controller = playerObject.AddComponent<CharacterController>();
            controller.height = 2;
            controller.radius = 0.4f;
            controller.center = Vector3.zero;
            controller.stepOffset = 0.35f;
            Primitive("Kapsül", PrimitiveType.Capsule, playerObject.transform, Vector3.zero, new Vector3(0.8f, 1, 0.8f), playerMaterial, false);
            Primitive("Yön işareti", PrimitiveType.Cube, playerObject.transform, new Vector3(0, 0.4f, 0.4f), new Vector3(0.2f, 0.15f, 0.3f), wall, false);
            var motor = playerObject.AddComponent<DemoPlayerMotor>();

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 8.5f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 100;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.67f, 0.75f, 0.77f);
            cameraObject.AddComponent<AudioListener>();
            var follow = cameraObject.AddComponent<IsometricFollowCamera>();
            follow.Configure(playerObject.transform, environment);

            var light = new GameObject("Güneş ışığı").AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.4f;
            light.shadows = LightShadows.Soft;
            light.transform.rotation = Quaternion.Euler(50, -30, 0);
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.65f, 0.63f, 0.57f);

            var gameObject = new GameObject("Demo oturumu");
            var game = gameObject.AddComponent<DemoGameController>();
            game.ConfigureCompleteDemo(motor, station, waterPoint, farmPoint, world, follow);
            motor.Configure(game, cameraObject.transform);
            gameObject.AddComponent<DemoHud>().Configure(game);

            if (!EditorSceneManager.SaveScene(scene, scenePath))
                throw new InvalidOperationException("Demo sahnesi kaydedilemedi. Console ve dosya izinlerini kontrol edin.");
            Selection.activeGameObject = playerObject;
            Debug.Log("Üç kararlı Son Bir Umut demo sahnesi oluşturuldu: " + scenePath + ". Game görünümünü açıp Play'e basın. Unity çalışma testi henüz yapılmalıdır.");
        }

        private static DecisionStation CreateWaterStation(Transform environment, Material metal, Material water,
            Material green, Material yellow, out Transform gauge)
        {
            var root = Group("02 - SU DEPOSU", environment);
            root.localPosition = new Vector3(-2, 0, 5);
            Primitive("Mevcut su deposu", PrimitiveType.Cylinder, root, new Vector3(0, 1.1f, 0), new Vector3(2, 1.1f, 2), metal);
            gauge = Primitive("Kullanılabilir su - seviye göstergesi", PrimitiveType.Cube, root,
                new Vector3(-0.6f, 1.1f, -1.02f), new Vector3(0.28f, 2, 0.12f), water, false).transform;
            var pump = Group("A - Arıtma ve pompa", root);
            Primitive("Pompa gövdesi", PrimitiveType.Cube, pump, new Vector3(1.7f, 0.55f, 0), new Vector3(1.1f, 1.1f, 1), water, false);
            Primitive("Bağlantı borusu", PrimitiveType.Cube, pump, new Vector3(0.9f, 0.5f, 0), new Vector3(1.4f, 0.18f, 0.18f), metal, false);
            var efficiency = Group("B - Verimlilik vanası", root);
            Primitive("Onarılmış vana", PrimitiveType.Sphere, efficiency, new Vector3(-1.4f, 0.7f, 0), Vector3.one * 0.6f, green, false);
            var unchanged = Group("C - Mevcut düzen uyarısı", root);
            Primitive("Sızıntı uyarı levhası", PrimitiveType.Cube, unchanged, new Vector3(1.3f, 0.8f, 0), new Vector3(0.7f, 0.9f, 0.1f), yellow, false);
            var anchor = Group("Etkileşim konumu", root);
            anchor.localPosition = new Vector3(0, 1, -2);
            var complete = Primitive("Tamamlandı lambası", PrimitiveType.Sphere, root, new Vector3(0, 2.5f, 0), Vector3.one * 0.4f, green, false);
            var station = root.gameObject.AddComponent<DecisionStation>();
            station.Configure(DecisionCatalog.WaterStationId, anchor,
                new[] { pump.gameObject, efficiency.gameObject, unchanged.gameObject }, complete);
            return station;
        }

        private static DecisionStation CreateFarmStation(Transform environment, Material soil, Material wood,
            Material leaves, Material water, Material green, out Renderer[] crops)
        {
            var root = Group("03 - TARIM VE DOĞAL ALAN", environment);
            root.localPosition = new Vector3(6, 0, -5);
            Primitive("Tarla zemini", PrimitiveType.Cube, root, new Vector3(0, 0.04f, 0), new Vector3(5, 0.08f, 3.8f), soil, false);
            var field = Group("Ürün sıraları - sağlık görünümü", root);
            var renderers = new List<Renderer>();
            for (int row = 0; row < 3; row++)
                for (int column = 0; column < 5; column++)
                    renderers.Add(Primitive("Ürün " + row + "-" + column, PrimitiveType.Cube, field,
                        new Vector3(-1.8f + column * 0.9f, 0.4f, -1.1f + row * 1.1f), new Vector3(0.3f, 0.65f, 0.3f), leaves, false).GetComponent<Renderer>());
            crops = renderers.ToArray();
            Primitive("Araştırma işareti", PrimitiveType.Cube, root, new Vector3(-2.5f, 0.8f, -1.5f), new Vector3(0.5f, 1.6f, 0.2f), wood);
            var protection = Group("A - Doğal alan koruması", root);
            Primitive("Koruma çiti", PrimitiveType.Cube, protection, new Vector3(0, 0.65f, 2), new Vector3(5.2f, 0.2f, 0.15f), wood, false);
            Primitive("Koruma işareti", PrimitiveType.Cube, protection, new Vector3(-2.5f, 1.5f, -1.5f), new Vector3(0.7f, 0.5f, 0.15f), green, false);
            var irrigation = Group("B - Sulama kanalları", root);
            for (int i = 0; i < 3; i++)
                Primitive("Sulama hattı " + i, PrimitiveType.Cube, irrigation, new Vector3(0, 0.11f, -0.7f + i), new Vector3(4.6f, 0.07f, 0.12f), water, false);
            var storage = Group("C - Kurak dönem depolaması", root);
            for (int i = 0; i < 2; i++)
                Primitive("Su varili " + i, PrimitiveType.Cylinder, storage, new Vector3(3, 0.6f, -0.8f + i * 1.4f), new Vector3(0.8f, 0.6f, 0.8f), water, false);
            var anchor = Group("Etkileşim konumu", root);
            anchor.localPosition = new Vector3(0, 1, -2.5f);
            var complete = Primitive("Tamamlandı lambası", PrimitiveType.Sphere, root, new Vector3(-2.5f, 1.9f, -1.5f), Vector3.one * 0.4f, green, false);
            var station = root.gameObject.AddComponent<DecisionStation>();
            station.Configure(DecisionCatalog.FarmStationId, anchor,
                new[] { protection.gameObject, irrigation.gameObject, storage.gameObject }, complete);
            return station;
        }

        private static void EnsureFolder(string parent, string name)
        {
            if (!AssetDatabase.IsValidFolder(parent + "/" + name)) AssetDatabase.CreateFolder(parent, name);
        }

        private static Transform Group(string name, Transform parent)
        {
            var group = new GameObject(name).transform;
            group.SetParent(parent, false);
            return group;
        }

        private static Material Material(string name, Color color, Shader shader, string folder)
        {
            var material = new Material(shader) { name = name };
            material.SetColor(material.HasProperty("_BaseColor") ? "_BaseColor" : "_Color", color);
            AssetDatabase.CreateAsset(material, folder + "/" + name + ".mat");
            return material;
        }

        private static GameObject Primitive(string name, PrimitiveType type, Transform parent,
            Vector3 localPosition, Vector3 scale, Material material, bool collides = true)
        {
            var item = GameObject.CreatePrimitive(type);
            item.name = name;
            item.transform.SetParent(parent, false);
            item.transform.localPosition = localPosition;
            item.transform.localScale = scale;
            item.GetComponent<Renderer>().sharedMaterial = material;
            if (!collides) UnityEngine.Object.DestroyImmediate(item.GetComponent<Collider>());
            return item;
        }

        private static void Boundary(Transform parent, Vector3 position, Vector3 size)
        {
            var wall = new GameObject("Harita sınırı");
            wall.transform.SetParent(parent, false);
            wall.transform.localPosition = position;
            wall.AddComponent<BoxCollider>().size = size;
        }
    }
}
#endif
