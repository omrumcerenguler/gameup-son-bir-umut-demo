using SonBirUmutDemo.Core;
using SonBirUmutDemo.Simulation;
using UnityEngine;

namespace SonBirUmutDemo.UI
{
    public sealed class DemoHud : MonoBehaviour
    {
        [SerializeField] private DemoGameController game;
        private GUIStyle titleStyle;
        private GUIStyle cardTitle;
        private GUIStyle textStyle;
        private GUIStyle smallStyle;
        private GUIStyle buttonStyle;
        public void Configure(DemoGameController owner) => game = owner;

        private void OnGUI()
        {
            if (game == null || game.Session == null) return;
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 25, fontStyle = FontStyle.Bold, wordWrap = true };
                cardTitle = new GUIStyle(titleStyle) { fontSize = 21 };
                textStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, wordWrap = true };
                smallStyle = new GUIStyle(textStyle) { fontSize = 16 };
                buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 18, wordWrap = true };
            }
            Matrix4x4 previous = GUI.matrix;
            float scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            GUI.matrix = Matrix4x4.TRS(new Vector3((Screen.width - 1280f * scale) / 2,
                (Screen.height - 720f * scale) / 2, 0), Quaternion.identity, Vector3.one * scale);
            try
            {
                DrawStatus();
                if (game.IntroOpen) DrawIntro();
                else if (game.CardsOpen) DrawCards();
                else if (game.IsComplete) DrawEnding();
                else DrawHint();
            }
            finally { GUI.matrix = previous; }
        }

        private void DrawStatus()
        {
            var state = game.Session.State;
            GUI.Box(new Rect(15, 12, 1250, 125), GUIContent.none);
            GUI.Label(new Rect(30, 18, 1200, 34), DemoStory.Title + $"  |  Kararlar {game.Session.CompletedCount}/{game.Session.PointCount}", titleStyle);
            GUI.Label(new Rect(30, 58, 1200, 28), $"Bütçe: {state.Budget}   Enerji: {state.Energy}   Su: {state.Water}   Bitki: {state.PlantHealth}   Emisyon: {state.Emissions}", textStyle);
            GUI.Label(new Rect(30, 94, 1200, 32), game.IntroOpen ? "Köyün geleceği için üç karar." : game.Objective, textStyle);
        }

        private void DrawIntro()
        {
            GUI.Box(new Rect(180, 155, 920, 535), GUIContent.none);
            GUI.Label(new Rect(210, 175, 860, 40), "Köye dönüş", titleStyle);
            GUI.Label(new Rect(210, 230, 860, 230), DemoStory.Introduction, textStyle);
            GUI.Label(new Rect(210, 465, 860, 65), DemoStory.Controls, textStyle);
            GUI.Label(new Rect(210, 540, 860, 65), DemoStory.BalanceNote, smallStyle);
            if (GUI.Button(new Rect(430, 622, 420, 48), "Köyü araştırmaya başla [Enter]", buttonStyle)) game.StartGame();
        }

        private void DrawHint()
        {
            GUI.Box(new Rect(200, 610, 880, 95), GUIContent.none);
            string hint = game.CanInteract ? "[E] " + DemoStory.PointName(game.Session.CurrentPoint.Id) + " — karar ver" : "WASD: dolaş  |  E: etkileşim";
            GUI.Label(new Rect(220, 620, 840, 28), hint, textStyle);
            GUI.Label(new Rect(220, 653, 840, 42), game.Feedback, smallStyle);
        }

        private void DrawCards()
        {
            var point = game.Session.CurrentPoint;
            if (point == null) return;
            GUI.Box(new Rect(55, 150, 1170, 555), GUIContent.none);
            GUI.Label(new Rect(75, 162, 1130, 40), DemoStory.Question(point.Id), titleStyle);
            GUI.Label(new Rect(75, 210, 1130, 43), "Net etkiler aşağıdaki kaynaklara ve kuraklık turuna göre hesaplanır. Her karar yalnızca bir kez uygulanır.", smallStyle);
            for (int i = 0; i < point.Options.Count; i++)
            {
                var option = point.Options[i];
                var preview = game.Session.GetPreview(point.Id, option.Id);
                float x = 75 + i * 380;
                GUI.Box(new Rect(x, 262, 365, 355), GUIContent.none);
                GUI.Label(new Rect(x + 12, 272, 341, 78), option.Title, cardTitle);
                GUI.Label(new Rect(x + 12, 352, 341, 90), option.Description, smallStyle);
                string effects = preview.CanChoose ? NetEffects(preview) : preview.Reason;
                GUI.Label(new Rect(x + 12, 446, 341, 90), $"Maliyet: {preview.Cost} bütçe\n" + effects, smallStyle);
                bool wasEnabled = GUI.enabled;
                GUI.enabled = preview.CanChoose;
                bool clicked = GUI.Button(new Rect(x + 12, 561, 341, 42), preview.CanChoose ? "Seç" : "Uygulanamaz", buttonStyle);
                GUI.enabled = wasEnabled;
                if (clicked) { game.Choose(option.Id); return; }
            }
            GUI.Label(new Rect(75, 635, 830, 54), DemoStory.BalanceNote, smallStyle);
            if (GUI.Button(new Rect(965, 640, 240, 44), "Vazgeç [Esc]", buttonStyle)) game.CloseCards();
        }

        private void DrawEnding()
        {
            // Right panel leaves most of the village visible during the smooth camera overview.
            GUI.Box(new Rect(720, 150, 545, 555), GUIContent.none);
            string title = game.IsLegacy ? "Enerji prototipi tamamlandı" : game.Session.Ending.Title;
            GUI.Label(new Rect(740, 165, 505, 65), title, titleStyle);
            string explanation = game.IsLegacy ? "Tam demo için Tools menüsünden yeni sahne oluştur." : game.Session.Ending.Explanation;
            GUI.Label(new Rect(740, 235, 505, 100), explanation, smallStyle);
            float y = 343;
            foreach (var record in game.Session.History)
            {
                GUI.Label(new Rect(740, y, 505, 30), record.Option.Title, smallStyle);
                GUI.Label(new Rect(740, y + 28, 505, 47), "Bütçe " + Signed(record.Result.After.Budget - record.Result.Before.Budget) + " | " + NetEffects(record.Result), smallStyle);
                y += 82;
            }
            GUI.Label(new Rect(740, 591, 505, 45), DemoStory.Closing, smallStyle);
            if (GUI.Button(new Rect(845, 648, 300, 42), "Yeniden Oyna", buttonStyle)) game.Restart();
        }

        private static string NetEffects(DecisionPreview preview)
        {
            var a = preview.Before;
            var b = preview.After;
            return $"Enerji {Signed(b.Energy - a.Energy)} | Su {Signed(b.Water - a.Water)} | Bitki {Signed(b.PlantHealth - a.PlantHealth)} | Emisyon {Signed(b.Emissions - a.Emissions)}";
        }

        private static string Signed(int value) => value >= 0 ? "+" + value : value.ToString();
    }
}
