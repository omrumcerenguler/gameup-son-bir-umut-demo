#if UNITY_EDITOR
using SonBirUmutDemo.Core;
using SonBirUmutDemo.Interaction;
using SonBirUmutDemo.Simulation;
using UnityEngine;

namespace SonBirUmutDemo.Editor
{
    // Independent permanent roots. All small props are non-solid and cannot obstruct a route.
    public static class DemoPacingBuilder
    {
        public static MaintenanceTaskNode[] Build(DemoGameController game,Transform world,Material metal,
            Material energy,Material water,Material nature,Material active,Material done)
        {
            var root=new GameObject("10 bağımsız bakım noktası").transform;
            root.SetParent(world,false);
            var nodes=new MaintenanceTaskNode[FinalPacingCatalog.All.Count];
            for(int i=0;i<nodes.Length;i++)
            {
                var definition=FinalPacingCatalog.All[i];
                var nodeRoot=new GameObject(definition.LocationId+" - "+definition.Title).transform;
                nodeRoot.SetParent(root,false);
                nodeRoot.localPosition=new Vector3((float)definition.Position.X,0,(float)definition.Position.Z);
                Material region=definition.Region==TaskRegion.Energy ? energy : definition.Region==TaskRegion.Water ? water : nature;
                Shape("Erişim alanı",PrimitiveType.Cylinder,nodeRoot,new Vector3(0,.04f,0),new Vector3(.85f,.035f,.85f),metal);
                // Distinct silhouettes built from primitives; no invented working strategic facility.
                if(definition.Region==TaskRegion.Energy)
                {
                    Shape("Kontrol kutusu",PrimitiveType.Cube,nodeRoot,new Vector3(.55f,.65f,.45f),new Vector3(.55f,1.2f,.4f),metal);
                    var face=Shape("Bağlantı yüzeyi",PrimitiveType.Cube,nodeRoot,new Vector3(.55f,1,.2f),new Vector3(.45f,.5f,.08f),region);
                    if(definition.TaskId=="solar-link") face.transform.localRotation=Quaternion.Euler(25,0,0);
                    if(definition.TaskId=="saving-check") Shape("Tasarruf anahtarı",PrimitiveType.Sphere,nodeRoot,new Vector3(.55f,.55f,.19f),Vector3.one*.2f,region);
                }
                else if(definition.Region==TaskRegion.Water)
                {
                    Shape("Hat bağlantısı",PrimitiveType.Cylinder,nodeRoot,new Vector3(.5f,.25f,.4f),new Vector3(.55f,.25f,.55f),metal);
                    Shape(definition.TaskId=="filter-clean" ? "Filtre kapağı" : "Vana",PrimitiveType.Cylinder,nodeRoot,new Vector3(.5f,.6f,.4f),new Vector3(.6f,.08f,.6f),region);
                    if(definition.TaskId=="reserve-line") Shape("Yedek boru",PrimitiveType.Cube,nodeRoot,new Vector3(.5f,.15f,.8f),new Vector3(.2f,.2f,.8f),region);
                }
                else
                {
                    Shape("Bakım direği",PrimitiveType.Cube,nodeRoot,new Vector3(.5f,.65f,.4f),new Vector3(.12f,1.3f,.12f),metal);
                    Shape(definition.TaskId=="soil-check" ? "Nem kontrol levhası" : definition.TaskId=="tree-care" ? "Ağaç koruma levhası" : "Sulama göstergesi",
                        PrimitiveType.Cube,nodeRoot,new Vector3(.5f,1.15f,.4f),new Vector3(.6f,.4f,.1f),region);
                }
                var flag=Shape("Aktif bakım çağrısı",PrimitiveType.Cube,nodeRoot,new Vector3(0,2,0),Vector3.one*.32f,active);
                flag.transform.localRotation=Quaternion.Euler(0,0,45);
                var check=Shape("Tamamlandı",PrimitiveType.Sphere,nodeRoot,new Vector3(0,1.7f,0),Vector3.one*.28f,done);
                var node=nodeRoot.gameObject.AddComponent<MaintenanceTaskNode>(); node.Configure(game,definition,flag,check);
                nodes[i]=node;
            }
            return nodes;
        }
        private static GameObject Shape(string name,PrimitiveType type,Transform parent,Vector3 position,Vector3 scale,Material material)
        {
            var go=GameObject.CreatePrimitive(type); go.name=name; go.transform.SetParent(parent,false);
            go.transform.localPosition=position; go.transform.localScale=scale; go.GetComponent<Renderer>().sharedMaterial=material;
            Object.DestroyImmediate(go.GetComponent<Collider>()); return go;
        }
    }
}
#endif
