using SonBirUmutDemo.Core;
using SonBirUmutDemo.Decisions;
using UnityEngine;

namespace SonBirUmutDemo.UI
{
    public sealed class DemoHud : MonoBehaviour
    {
        [SerializeField] private DemoGameController game;
        private GUIStyle titleStyle;
        private GUIStyle textStyle;
        private GUIStyle buttonStyle;

        public void Configure(DemoGameController owner) => game = owner;

        private void OnGUI()
        {
            if (game == null || game.Session == null) return;
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 25, fontStyle = FontStyle.Bold, wordWrap = true };
                textStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, wordWrap = true };
                buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 18, wordWrap = true };
            }
            // Fixed logical canvas, letterboxed to keep cards legible at common desktop sizes.
            Matrix4x4 previous = GUI.matrix;
            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            GUI.matrix = Matrix4x4.TRS(new Vector3((Screen.width - 1280f * scale) / 2,
                (Screen.height - 720f * scale) / 2, 0), Quaternion.identity, Vector3.one * scale);
            var state = game.Session.State;
            GUI.Box(new Rect(15, 12, 1250, 133), GUIContent.none);
            GUI.Label(new Rect(30, 20, 1200, 36), "SON BİR UMUT | Enerji istasyonu prototipi", titleStyle);
            GUI.Label(new Rect(30, 60, 1200, 30), $"Bütçe: {state.Budget}   Enerji: {state.Energy}   Su: {state.Water}   Bitki: {state.PlantHealth}   Emisyon: {state.Emissions}", textStyle);
            GUI.Label(new Rect(30, 96, 1200, 36), "WASD: hareket  |  E: etkileşim  |  Esc: kartları kapat  |  Değerler örnek oyun puanlarıdır.", textStyle);

            if (game.CardsOpen) DrawCards();
            else
            {
                GUI.Box(new Rect(240, 590, 800, 112), GUIContent.none);
                string hint = game.CanInteract ? "[E] Enerji istasyonunda karar ver" : game.ResultMessage;
                GUI.Label(new Rect(260, 605, 760, 80), hint, textStyle);
                if (game.IsComplete)
                {
                    GUI.Box(new Rect(350, 165, 580, 150), GUIContent.none);
                    GUI.Label(new Rect(370, 180, 540, 60), "Enerji istasyonu tamamlandı. Diğer seçenekleri yeniden oynayarak karşılaştır.", textStyle);
                    if (GUI.Button(new Rect(490, 250, 300, 45), "Yeniden oyna", buttonStyle)) game.Restart();
                }
            }
            GUI.matrix = previous;
        }

        private void DrawCards()
        {
            GUI.Box(new Rect(65, 155, 1150, 548), GUIContent.none);
            GUI.Label(new Rect(85, 170, 1110, 40), "Köyün enerjisini nasıl sağlayacaksın?", titleStyle);
            GUI.Label(new Rect(85, 218, 1110, 60), "Her karar bir tur ilerletir: su kullanımı 8 puan; bitki etkisi kalan su ve kuraklığa bağlıdır. Henüz pompa çalışmaz.", textStyle);
            var options = DecisionCatalog.EnergyStation.Options;
            for (int i = 0; i < options.Count; i++)
            {
                var option = options[i];
                float x = 85 + i * 375;
                GUI.Box(new Rect(x, 290, 360, 305), GUIContent.none);
                GUI.Label(new Rect(x + 14, 305, 332, 65), option.Title, titleStyle);
                GUI.Label(new Rect(x + 14, 375, 332, 80), option.Description, textStyle);
                GUI.Label(new Rect(x + 14, 460, 332, 70), $"Maliyet: {option.Cost} bütçe\nEnerji {Signed(option.EnergyDelta)} | Su {Signed(option.WaterDelta)} | Emisyon {Signed(option.EmissionsDelta)}", textStyle);
                bool previousEnabled = GUI.enabled;
                GUI.enabled = game.Session.State.Budget >= option.Cost;
                if (GUI.Button(new Rect(x + 14, 540, 332, 40), "Seç", buttonStyle)) game.Choose(option.Id);
                GUI.enabled = previousEnabled;
            }
            GUI.Label(new Rect(85, 605, 800, 80), "Emisyon ayrı bir etki göstergesidir; jeneratör anlık yerel sıcaklık artışı yaratmaz.", textStyle);
            if (GUI.Button(new Rect(955, 630, 235, 45), "Vazgeç (Esc)", buttonStyle)) game.CloseCards();
        }

        private static string Signed(int value) => value >= 0 ? "+" + value : value.ToString();
    }
}
