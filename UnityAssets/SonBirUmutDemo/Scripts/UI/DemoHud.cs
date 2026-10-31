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
        private GUIStyle detailStyle;
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
                smallStyle = new GUIStyle(textStyle) { fontSize = 15 };
                detailStyle = new GUIStyle(textStyle) { fontSize = 14 };
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
                if (game.NoticeVisible) DrawNotification();
            }
            finally { GUI.matrix = previous; }
        }

        private void DrawStatus()
        {
            var state = game.Session.State;
            GUI.Box(new Rect(15, 12, 1250, 151), GUIContent.none);
            GUI.Label(new Rect(30, 18, 1200, 32), DemoStory.Title + $"  |  Kararlar {game.Session.CompletedCount}/{game.Session.PointCount}", titleStyle);
            GUI.Label(new Rect(30, 53, 1200, 24), "Hedef: " + DemoStory.MainGoal, textStyle);
            GUI.Label(new Rect(30, 80, 1200, 25), $"Bütçe: {state.Budget}   Enerji: {state.Energy}   Su: {state.Water}   Bitki: {state.PlantHealth}   Emisyon: {state.Emissions}", textStyle);
            GUI.Label(new Rect(30, 108, 1200, 22), game.IsComplete ? "Kuraklık dönemi: sonuçları incele." : "Sıradaki: " + game.Objective, smallStyle);
            GUI.Label(new Rect(30, 131, 1200, 27), DecisionMessaging.CriticalResources(state, !game.IsComplete), smallStyle);
        }

        private void DrawIntro()
        {
            GUI.Box(new Rect(180, 173, 920, 517), GUIContent.none);
            GUI.Label(new Rect(210, 185, 860, 40), "Köye dönüş", titleStyle);
            GUI.Label(new Rect(210, 230, 860, 230), DemoStory.Introduction, textStyle);
            GUI.Label(new Rect(210, 465, 860, 65), DemoStory.Controls, textStyle);
            GUI.Label(new Rect(210, 540, 860, 65), DemoStory.BalanceNote, smallStyle);
            if (GUI.Button(new Rect(430, 622, 420, 48), "Köyü araştırmaya başla [Enter]", buttonStyle)) game.StartGame();
        }

        private void DrawHint()
        {
            if (game.NoticeVisible) return;
            GUI.Box(new Rect(200, 660, 880, 45), GUIContent.none);
            string hint = game.CanInteract ? "[E] " + DemoStory.PointName(game.Session.CurrentPoint.Id) + " — karar ver" : "WASD: dolaş  |  E: etkileşim";
            GUI.Label(new Rect(220, 668, 840, 30), hint, textStyle);
        }

        private void DrawNotification()
        {
            // Only the bottom-left strip is occupied; the changed station stays visible at screen center.
            GUI.Box(new Rect(15, 600, 690, 105), GUIContent.none);
            GUI.Label(new Rect(27, 607, 633, 92), game.ResultMessage, detailStyle);
            if (GUI.Button(new Rect(668, 606, 27, 27), "×")) game.DismissNotice();
        }

        private void DrawCards()
        {
            var point = game.Session.CurrentPoint;
            if (point == null) return;
            GUI.Box(new Rect(55, 173, 1170, 532), GUIContent.none);
            GUI.Label(new Rect(75, 180, 1130, 33), DemoStory.Question(point.Id), cardTitle);
            GUI.Label(new Rect(75, 216, 1130, 38), DemoStory.Problem(point.Id), smallStyle);
            for (int i = 0; i < point.Options.Count; i++)
            {
                var option = point.Options[i];
                var preview = game.Session.GetPreview(point.Id, option.Id);
                float x = 75 + i * 380;
                GUI.Box(new Rect(x, 260, 365, 384), GUIContent.none);
                GUI.Label(new Rect(x + 12, 270, 341, 49), option.Title, cardTitle);
                string tradeoffs = string.IsNullOrEmpty(option.Advantage) ? option.Description :
                    "Kazanç: " + option.Advantage + "\nBedeli: " + option.Tradeoff + "\nSonrası: " + option.NextEffect;
                GUI.Label(new Rect(x + 12, 322, 341, 111), tradeoffs, detailStyle);
                string effects = preview.CanChoose ? DecisionMessaging.ChoiceEffects(preview) + "\n" +
                    DecisionMessaging.TurnEffects(preview) + "\n" + DecisionMessaging.NetEffects(preview) : preview.Reason;
                GUI.Label(new Rect(x + 12, 435, 341, 155), $"Maliyet: {preview.Cost} bütçe\n" + effects, detailStyle);
                bool wasEnabled = GUI.enabled;
                GUI.enabled = preview.CanChoose;
                bool clicked = GUI.Button(new Rect(x + 12, 597, 341, 36), preview.CanChoose ? "Seç" : "Uygulanamaz", buttonStyle);
                GUI.enabled = wasEnabled;
                if (clicked) { game.Choose(option.Id); return; }
            }
            GUI.Label(new Rect(75, 653, 830, 43), "Seçim, tur kullanımı ve kuraklık ayrıdır. Net etkiler 0–100 sınırlarını içerir. Bunlar örnek oyun puanlarıdır.", smallStyle);
            if (GUI.Button(new Rect(965, 656, 240, 36), "Vazgeç [Esc]", buttonStyle)) game.CloseCards();
        }

        private void DrawEnding()
        {
            // Preserve the camera/world view on the left; summarize consequences on the right.
            GUI.Box(new Rect(720, 173, 545, 532), GUIContent.none);
            string title = game.IsLegacy ? "Enerji prototipi tamamlandı" : game.Session.Ending.Title;
            GUI.Label(new Rect(740, 184, 505, 40), title, titleStyle);
            string explanation = game.IsLegacy ? "Tam demo için Tools menüsünden yeni sahne oluştur." :
                "Kuraklık dönemi: " + game.Session.Ending.Explanation;
            GUI.Label(new Rect(740, 230, 505, 65), explanation, detailStyle);
            if (!game.IsLegacy)
            {
                var summary = game.Session.Summary;
                GUI.Label(new Rect(740, 302, 505, 43), summary.DrinkingWater, detailStyle);
                GUI.Label(new Rect(740, 348, 505, 43), "Tarım: " + summary.Agriculture, detailStyle);
                GUI.Label(new Rect(740, 394, 505, 43), "Doğal yaşam: " + summary.Nature, detailStyle);
                GUI.Label(new Rect(740, 441, 505, 23), "Öne çıkan iki karar", smallStyle);
                for (int i = 0; i < summary.KeyDecisions.Count && i < 2; i++)
                    GUI.Label(new Rect(740, 466 + i * 45, 505, 43), summary.KeyDecisions[i], detailStyle);
                GUI.Label(new Rect(740, 558, 505, 53), summary.RemainingRisk, detailStyle);
                GUI.Label(new Rect(740, 613, 505, 39), EndingSummary.ProxyNote, detailStyle);
            }
            if (GUI.Button(new Rect(845, 660, 300, 34), "Yeniden Oyna", buttonStyle)) game.Restart();
        }
    }
}
