using SonBirUmutDemo.Core;
using SonBirUmutDemo.Player;
using SonBirUmutDemo.Simulation;
using UnityEngine;

namespace SonBirUmutDemo.UI
{
    public sealed class DemoHud : MonoBehaviour
    {
        [SerializeField]
        private DemoGameController game;
        [SerializeField]
        private IsometricFollowCamera followCamera;
        [SerializeField]
        private Transform[] stationMarkers;
        private DemoGuiTheme theme;
        private Camera view;
        private GameSession shownSession;
        private Vector2 finalScroll;
        private int finalTab;
        private bool overviewOnly;
        private float scale;
        private Vector2 canvasOffset;
        public void Configure(DemoGameController owner, IsometricFollowCamera camera = null, Transform[] markers = null)
        {
            game = owner;
            followCamera = camera;
            stationMarkers = markers;
        }

        private void Awake()
        {
            if (followCamera == null && Camera.main != null)
                followCamera = Camera.main.GetComponent<IsometricFollowCamera>();
            if (followCamera != null)
                view = followCamera.GetComponent<Camera>();
        }

        private void OnGUI()
        {
            if (game == null || game.Session == null)
                return;
            if (theme == null)
                theme = new DemoGuiTheme();
            if (!ReferenceEquals(shownSession, game.Session))
            {
                shownSession = game.Session;
                finalScroll = Vector2.zero;
                finalTab = 0;
                overviewOnly = false;
                if (followCamera != null)
                    followCamera.SetResultPanelVisible(true);
            }

            Matrix4x4 previous = GUI.matrix;
            scale = Mathf.Min(Screen.width / 1280f, Screen.height / 720f);
            canvasOffset = new Vector2((Screen.width - 1280f * scale) / 2, (Screen.height - 720f * scale) / 2);
            GUI.matrix = Matrix4x4.TRS(new Vector3(canvasOffset.x, canvasOffset.y, 0), Quaternion.identity, Vector3.one * scale);
            try
            {
                DrawStatus();
                if (game.IntroOpen)
                    DrawIntro();
                else if (game.CardsOpen)
                    DrawCards();
                else if (game.IsComplete)
                    DrawEnding();
                else
                {
                    DrawStationMarker();
                    DrawHint();
                }

                if (game.NoticeVisible && !overviewOnly)
                    DrawNotification();
            }
            finally
            {
                GUI.matrix = previous;
            }
        }

        private void DrawStatus()
        {
            var state = game.Session.State;
            theme.Panel(new Rect(15, 12, 1250, 153), DemoGuiTheme.Green);
            GUI.Label(new Rect(30, 21, 900, 32), DemoStory.Title, theme.Title);
            GUI.Label(new Rect(30, 56, 900, 25), DemoStory.MainGoal, theme.Small);
            GUI.Label(new Rect(1020, 25, 215, 25), $"HAZIRLIK  {game.Session.CompletedCount}/{game.Session.PointCount}", theme.Body);
            DemoGuiTheme.Bar(new Rect(1020, 59, 215, 5), game.Session.CompletedCount / (float)game.Session.PointCount, DemoGuiTheme.Green);
            Resource(0, "Bütçe", state.Budget, DemoGuiTheme.Gold);
            Resource(1, "Enerji", state.Energy, DemoGuiTheme.Gold);
            Resource(2, "Su", state.Water, DemoGuiTheme.Blue);
            Resource(3, "Bitki sağlığı", state.PlantHealth, DemoGuiTheme.Green);
            Resource(4, "Emisyon etkisi", state.Emissions, DemoGuiTheme.Red);
            GUI.Label(new Rect(30, 140, 1200, 23), game.IsComplete ? "Kuraklık dönemi — köyün durumunu ve kararlarının etkisini incele." : "Sıradaki: " + game.Objective, theme.Small);
            theme.Panel(new Rect(15, 170, 1250, 29));
            GUI.Label(new Rect(28, 176, 1220, 21), DecisionMessaging.CriticalResources(state, !game.IsComplete), theme.Tiny);
        }

        private void Resource(int index, string name, int value, Color color)
        {
            float x = 28 + index * 246;
            GUI.Label(new Rect(x, 89, 165, 25), name, theme.Small);
            GUI.Label(new Rect(x + 168, 84, 60, 32), value.ToString(), theme.Heading);
            DemoGuiTheme.Bar(new Rect(x, 122, 225, 5), value / 100f, color);
        }

        private void DrawIntro()
        {
            theme.Panel(new Rect(180, 215, 920, 490), DemoGuiTheme.Gold);
            GUI.Label(new Rect(215, 233, 830, 22), "GAMEUP TÜRKİYE 2026   /   KÖYE DÖNÜŞ", theme.Small);
            GUI.Label(new Rect(215, 271, 830, 38), "Bir köy. Üç karar. Bir gelecek.", theme.Title);
            GUI.Label(new Rect(215, 324, 830, 176), DemoStory.Introduction, theme.Body);
            GUI.Label(new Rect(215, 505, 830, 54), DemoStory.Controls, theme.Small);
            GUI.Label(new Rect(215, 568, 830, 43), DemoStory.BalanceNote, theme.Tiny);
            if (theme.Action(new Rect(430, 640, 420, 44), "Köyü araştırmaya başla  [Enter]"))
                game.StartGame();
        }

        private void DrawHint()
        {
            if (game.NoticeVisible)
                return;
            theme.Panel(new Rect(250, 658, 780, 46), DemoGuiTheme.Green);
            GUI.Label(new Rect(270, 670, 740, 26), game.CanInteract ? "[E]  " + DemoStory.PointName(game.Session.CurrentPoint.Id) + " — karar ver" : "WASD  Dolaş     •     E  Etkileşim     •     Üç noktayı sırayla araştır", theme.Body);
        }

        private void DrawStationMarker()
        {
            int index = game.Session.CompletedCount;
            if (view == null || stationMarkers == null || index >= stationMarkers.Length || stationMarkers[index] == null)
                return;
            Vector3 screen = view.WorldToScreenPoint(stationMarkers[index].position + Vector3.up * 3.2f);
            if (screen.z <= 0 || scale <= 0)
                return;
            float x = (screen.x - canvasOffset.x) / scale;
            float y = (Screen.height - screen.y - canvasOffset.y) / scale;
            if (x < 130 || x > 1150 || y < 225 || y > 580)
                return;
            theme.Panel(new Rect(x - 125, y - 16, 250, 39), DemoGuiTheme.Gold);
            GUI.Label(new Rect(x - 114, y - 8, 228, 26), (game.CanInteract ? "[E]  " : "") + DemoStory.PointName(game.Session.CurrentPoint.Id), theme.Small);
        }

        private void DrawNotification()
        {
            theme.Panel(new Rect(15, 600, 690, 105), DemoGuiTheme.Blue);
            GUI.Label(new Rect(28, 610, 631, 87), game.ResultMessage, theme.Tiny);
            if (theme.Action(new Rect(666, 609, 29, 27), "×", quietStyle: true))
                game.DismissNotice();
        }

        private void DrawCards()
        {
            var point = game.Session.CurrentPoint;
            if (point == null)
                return;
            theme.Panel(new Rect(55, 205, 1170, 500), DemoGuiTheme.Gold);
            GUI.Label(new Rect(75, 215, 1130, 30), DemoStory.Question(point.Id), theme.Heading);
            GUI.Label(new Rect(75, 250, 1130, 32), DemoStory.Problem(point.Id), theme.Small);
            for (int i = 0; i < point.Options.Count; i++)
            {
                var option = point.Options[i];
                var preview = game.Session.GetPreview(point.Id, option.Id);
                float x = 75 + i * 380;
                Color accent = preview.CanChoose ? DemoGuiTheme.Green : DemoGuiTheme.Red;
                DemoGuiTheme.Fill(new Rect(x, 286, 365, 352), DemoGuiTheme.Card);
                DemoGuiTheme.Fill(new Rect(x, 286, 365, 4), accent);
                GUI.Label(new Rect(x + 12, 298, 341, 49), option.Title, theme.Heading);
                string tradeoffs = string.IsNullOrEmpty(option.Advantage) ? option.Description : "Kazanç: " + option.Advantage + "\nBedeli: " + option.Tradeoff + "\nSonrası: " + option.NextEffect;
                GUI.Label(new Rect(x + 12, 350, 341, 94), tradeoffs, theme.Tiny);
                string effects = preview.CanChoose ? DecisionMessaging.ChoiceEffects(preview) + "\n" + DecisionMessaging.TurnEffects(preview) + "\n" + DecisionMessaging.NetEffects(preview) : preview.Reason;
                GUI.Label(new Rect(x + 12, 447, 341, 137), $"Maliyet: {preview.Cost} bütçe\n" + effects, theme.Tiny);
                if (theme.Action(new Rect(x + 12, 593, 341, 34), preview.CanChoose ? "Bu kararı uygula" : "Kaynak yetersiz", preview.CanChoose))
                {
                    game.Choose(option.Id);
                    return;
                }
            }

            GUI.Label(new Rect(75, 651, 830, 43), "Seçim, tur kullanımı ve kuraklık ayrıdır. Net değerler 0–100 sınırını içerir; bunlar örnek oyun puanlarıdır.", theme.Small);
            if (theme.Action(new Rect(965, 654, 240, 37), "Vazgeç  [Esc]", quietStyle: true))
                game.CloseCards();
        }

        private void DrawEnding()
        {
            Color accent = game.IsLegacy ? DemoGuiTheme.Green : game.Session.Ending.Category == EndingCategory.Rebirth ? DemoGuiTheme.Green : game.Session.Ending.Category == EndingCategory.PartialTransition ? DemoGuiTheme.Gold : DemoGuiTheme.Red;
            string title = game.IsLegacy ? "Enerji prototipi tamamlandı" : game.Session.Ending.Title;
            if (followCamera != null)
                followCamera.SetResultPanelVisible(!overviewOnly);
            if (overviewOnly)
            {
                theme.Panel(new Rect(165, 646, 950, 58), accent);
                GUI.Label(new Rect(185, 663, 350, 30), title, theme.Heading);
                if (theme.Action(new Rect(550, 657, 250, 36), "Sonuç raporuna dön", quietStyle: true))
                    overviewOnly = false;
                if (theme.Action(new Rect(815, 657, 280, 36), "Yeniden Oyna"))
                    game.Restart();
                return;
            }

            theme.Panel(new Rect(710, 210, 555, 495), accent);
            GUI.Label(new Rect(730, 225, 515, 37), title, theme.Title);
            if (theme.Action(new Rect(730, 274, 250, 34), "Köyün durumu", quietStyle: finalTab != 0))
            {
                finalTab = 0;
                finalScroll = Vector2.zero;
            }

            if (theme.Action(new Rect(990, 274, 255, 34), "Kararlarının etkisi", quietStyle: finalTab != 1))
            {
                finalTab = 1;
                finalScroll = Vector2.zero;
            }

            if (game.IsLegacy)
                GUI.Label(new Rect(730, 330, 515, 130), "Görsel köy ve tam demo için Tools menüsünden yeni sahne oluştur.", theme.Body);
            else
            {
                finalScroll = GUI.BeginScrollView(new Rect(730, 322, 515, 284), finalScroll, new Rect(0, 0, 485, finalTab == 0 ? 470 : 530));
                if (finalTab == 0)
                    DrawVillageSummary();
                else
                    DrawDecisionSummary();
                GUI.EndScrollView();
            }

            if (theme.Action(new Rect(730, 622, 515, 32), "Köyü izle", quietStyle: true))
                overviewOnly = true;
            if (theme.Action(new Rect(730, 664, 515, 32), "Yeniden Oyna"))
                game.Restart();
        }

        private void DrawVillageSummary()
        {
            var summary = game.Session.Summary;
            GUI.Label(new Rect(0, 0, 480, 85), game.Session.Ending.Explanation, theme.Body);
            SummaryBlock(94, "SU GÜVENLİĞİ", summary.DrinkingWater, DemoGuiTheme.Blue);
            SummaryBlock(176, "TARIM", summary.Agriculture, DemoGuiTheme.Gold);
            SummaryBlock(258, "DOĞAL YAŞAM", summary.Nature, DemoGuiTheme.Green);
            GUI.Label(new Rect(0, 342, 480, 75), summary.RemainingRisk, theme.Body);
            GUI.Label(new Rect(0, 421, 480, 48), EndingSummary.ProxyNote, theme.Small);
        }

        private void SummaryBlock(float y, string title, string text, Color accent)
        {
            DemoGuiTheme.Fill(new Rect(0, y, 3, 72), accent);
            GUI.Label(new Rect(14, y, 465, 20), title, theme.Small);
            GUI.Label(new Rect(14, y + 24, 465, 48), text, theme.Body);
        }

        private void DrawDecisionSummary()
        {
            float y = 0;
            foreach (var record in game.Session.History)
            {
                GUI.Label(new Rect(0, y, 480, 28), record.Option.Title, theme.Body);
                GUI.Label(new Rect(0, y + 30, 480, 57), $"Bütçe -{record.Result.Cost} | " + DecisionMessaging.NetEffects(record.Result), theme.Small);
                y += 94;
            }

            GUI.Label(new Rect(0, y + 8, 480, 28), "Öne çıkan iki karar", theme.Heading);
            for (int i = 0; i < game.Session.Summary.KeyDecisions.Count; i++)
                GUI.Label(new Rect(0, y + 45 + i * 83, 480, 78), game.Session.Summary.KeyDecisions[i], theme.Body);
        }

        private void OnDestroy()
        {
            if (theme != null)
                theme.Dispose();
        }
    }
}
