#if UNITY_EDITOR
using System.Collections.Generic;
using SonBirUmutDemo.Core;
using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.World;
using UnityEditor;
using UnityEngine;

namespace SonBirUmutDemo.Editor
{
    // Additional geometry only for the opt-in playtest scene. Decoration never blocks routes.
    public sealed class DemoPlaytestBuilder
    {
        private readonly Material path, water, timber, green, red, gold, stone, straw;
        public DemoPlaytestBuilder(Shader shader, string folder)
        {
            path = Material("WidePaths", new Color(.73f,.65f,.45f), shader, folder);
            water = Material("ChannelWater", new Color(.17f,.58f,.72f), shader, folder);
            timber = Material("ChannelBank", new Color(.42f,.32f,.20f), shader, folder);
            green = Material("MaintenanceDone", new Color(.35f,.82f,.45f), shader, folder);
            red = Material("MissedLeak", new Color(.72f,.24f,.15f), shader, folder);
            gold = Material("EnergyRegion", new Color(.82f,.66f,.29f), shader, folder);
            stone = Material("EdgeStones", new Color(.5f,.48f,.4f), shader, folder);
            straw = Material("DryGrasses", new Color(.63f,.52f,.29f), shader, folder);
        }

        public Transform[] Enrich(Transform environment, DemoVisualBuilder art, Material foliage,
            List<Renderer> trees, List<GameObject> branches)
        {
            var root = Group("Bölgeler, yollar ve kanal", environment);
            Route(root, new Vector3(1,0,-4), new Vector3(0,0,-2));
            Route(root, new Vector3(0,0,-2), new Vector3(8,0,-1.4f));
            Route(root, new Vector3(0,0,-2), new Vector3(-3,0,4.5f));
            Route(root, new Vector3(-3,0,4.5f), new Vector3(-8,0,5.8f));
            Route(root, new Vector3(0,0,-2), new Vector3(3,0,-10.1f));
            Route(root, new Vector3(3,0,-10.1f), new Vector3(8,0,-10.1f));
            Region(root, "Enerji bölgesi", new Vector3(8,0,1), new Vector3(5.8f,.02f,4.6f), gold);
            Region(root, "Su yönetimi bölgesi", new Vector3(-8,0,8), new Vector3(5.2f,.02f,5), water);
            Region(root, "Tarım ve doğa bölgesi", new Vector3(8,0,-7), new Vector3(7,.02f,6), green);
            var channels = new List<Transform>();
            Channel(root, new Vector3(-11,0,-6), new Vector3(-11,0,6.6f), channels);
            Channel(root, new Vector3(-11,0,6.6f), new Vector3(-8.9f,0,6.6f), channels);
            foreach (var p in new[] { new Vector3(-13,0,10),new Vector3(-11.7f,0,11.6f),
                new Vector3(12.8f,0,10.4f),new Vector3(14,0,8.7f),new Vector3(13.7f,0,-11.6f),
                new Vector3(-13,0,-10.2f),new Vector3(-12,0,-12) })
            {
                var crown = art.Tree(root, p, foliage, out var dry, false);
                crown.transform.parent.localScale = Vector3.one * .65f;
                trees.Add(crown); branches.Add(dry);
            }
            for (int i=0;i<18;i++)
            {
                float x = i < 9 ? -14.7f : 14.7f;
                float z = -11.5f + (i%9)*2.8f;
                var rock = Shape("Dekoratif kenar kayası", PrimitiveType.Sphere, root, new Vector3(x,.2f,z),new Vector3(.7f,.5f,.8f),stone);
                rock.transform.localRotation = Quaternion.Euler(0,i*41,12);
                for(int j=0;j<3;j++)
                    Shape("Kuru ot kümesi",PrimitiveType.Cube,root,new Vector3(x-.6f+j*.17f,.15f,z+.7f),new Vector3(.06f,.3f,.25f),straw);
            }
            for(int i=0;i<3;i++)
                Shape("Rezervuar korkuluğu",PrimitiveType.Cube,root,new Vector3(-.8f,.55f,6.8f+i*1.1f),new Vector3(.1f,.15f,1),timber);
            return channels.ToArray();
        }

        public void Feedback(DemoGameController game, Transform station, string id, Transform panels)
        {
            var checkedMark=Group("Bakım onayı",station);
            var left=Shape("Onay kısa çizgi",PrimitiveType.Cube,checkedMark,new Vector3(-.13f,2.2f,-1.3f),new Vector3(.14f,.4f,.14f),green);
            left.transform.localRotation=Quaternion.Euler(0,0,40);
            var right=Shape("Onay uzun çizgi",PrimitiveType.Cube,checkedMark,new Vector3(.12f,2.36f,-1.3f),new Vector3(.14f,.65f,.14f),green);
            right.transform.localRotation=Quaternion.Euler(0,0,-35);
            Transform[] drops=null; GameObject seal=null,missed=null; Renderer lamp=null;
            if(id==DecisionCatalog.WaterStationId)
            {
                Shape("Depo giriş süzgeci - pompadan bağımsız",PrimitiveType.Cylinder,station,new Vector3(-1.25f,.55f,-.2f),new Vector3(.35f,.25f,.35f),stone);
                drops=new Transform[3];
                for(int i=0;i<3;i++)drops[i]=Shape("Arıza su damlası",PrimitiveType.Sphere,station,Vector3.zero,new Vector3(.1f,.22f,.1f),water).transform;
                seal=Shape("Onarılmış hat",PrimitiveType.Cube,station,new Vector3(-.7f,.45f,-1.3f),new Vector3(.45f,.25f,.25f),green);
                missed=Shape("Rezerv kaybı izi",PrimitiveType.Cylinder,station,new Vector3(-.7f,.04f,-1.6f),new Vector3(.8f,.025f,.6f),red);
            }
            if(id==DecisionCatalog.FarmStationId)
                Shape("Ortak tasarruf vanası - sulama tesisi değil",PrimitiveType.Cylinder,station,new Vector3(-1.7f,.35f,-1.7f),new Vector3(.35f,.15f,.35f),stone);
            if(id==DecisionCatalog.EnergyStationId && panels!=null)
                lamp=Shape("Yalnız seçilen güneş paneli çalışma ışığı",PrimitiveType.Sphere,panels,new Vector3(0,2.2f,1.8f),Vector3.one*.23f,green).GetComponent<Renderer>();
            station.gameObject.AddComponent<DemoStationFeedback>().Configure(game,id,checkedMark.gameObject,drops,seal,missed,lamp);
        }

        private void Region(Transform root,string name,Vector3 p,Vector3 size,Material color)
        {
            var region=Group(name,root); region.localPosition=p;
            // Only boundary stripes: a blue/green floor must not pretend water/crops are healthy.
            foreach (float side in new[] { -1f, 1f })
            {
                Shape("Bölge sınırı",PrimitiveType.Cube,region,new Vector3(0,.022f,side*size.z/2),new Vector3(size.x,.025f,.08f),color);
                Shape("Bölge sınırı",PrimitiveType.Cube,region,new Vector3(side*size.x/2,.022f,0),new Vector3(.08f,.025f,size.z),color);
            }
            Shape("Bölge direği",PrimitiveType.Cube,region,new Vector3(-2.4f,.75f,-1.7f),new Vector3(.1f,1.5f,.1f),timber);
            Shape("Bölge rengi",PrimitiveType.Cube,region,new Vector3(-2.1f,1.4f,-1.7f),new Vector3(.6f,.4f,.08f),color);
        }
        private void Route(Transform root,Vector3 a,Vector3 b)
        {
            var road=Shape("Geniş köy yolu",PrimitiveType.Cube,root,(a+b)/2+Vector3.up*.025f,new Vector3(1.9f,.025f,Vector3.Distance(a,b)),path);
            road.transform.localRotation=Quaternion.LookRotation(b-a,Vector3.up);
        }
        private void Channel(Transform root,Vector3 a,Vector3 b,List<Transform> strips)
        {
            var bank=Shape("Kanal yatağı",PrimitiveType.Cube,root,(a+b)/2+Vector3.up*.025f,new Vector3(.95f,.04f,Vector3.Distance(a,b)),timber);
            bank.transform.localRotation=Quaternion.LookRotation(b-a,Vector3.up);
            var strip=Shape("Kaynağa bağlı kanal suyu",PrimitiveType.Cube,root,(a+b)/2+Vector3.up*.055f,new Vector3(.65f,.025f,Vector3.Distance(a,b)),water);
            strip.transform.localRotation=bank.transform.localRotation; strips.Add(strip.transform);
        }
        private static Transform Group(string name,Transform parent)
        {var t=new GameObject(name).transform;t.SetParent(parent,false);return t;}
        private static GameObject Shape(string name,PrimitiveType type,Transform parent,Vector3 position,Vector3 scale,Material material)
        {
            var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(parent,false);
            go.transform.localPosition=position;go.transform.localScale=scale;go.GetComponent<Renderer>().sharedMaterial=material;
            var collider=go.GetComponent<Collider>();if(collider!=null)Object.DestroyImmediate(collider);
            return go;
        }
        private static Material Material(string name,Color color,Shader shader,string folder)
        {
            var m=new Material(shader){name=name};
            if(m.HasProperty("_BaseColor"))m.SetColor("_BaseColor",color);
            if(m.HasProperty("_Color"))m.SetColor("_Color",color);
            AssetDatabase.CreateAsset(m,AssetDatabase.GenerateUniqueAssetPath(folder+"/"+name+".mat"));return m;
        }
    }
}
#endif
