using SonBirUmutDemo.Core;
using SonBirUmutDemo.Decisions;
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
        private bool detailsOpen, wasCardsOpen;
        private string shownPoint;
        private Vector2 detailsScroll;
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
                detailsOpen = false; wasCardsOpen = false; shownPoint = null;
                if (followCamera != null)
                    followCamera.SetResultPanelVisible(true);
            }

            string pointId = game.Session.CurrentPoint?.Id;
            if ((game.CardsOpen && !wasCardsOpen) || pointId != shownPoint)
            { detailsOpen = false; detailsScroll = Vector2.zero; }
            wasCardsOpen = game.CardsOpen; shownPoint = pointId;
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
                    DrawCrisisActivity();
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
            var state = game.CurrentResources;
            theme.Panel(new Rect(15, 12, 1250, 61));
            int seconds = Mathf.CeilToInt((float)game.Run.RemainingSeconds);
            GUI.Label(new Rect(29, 26, 110, 30), game.UsesCrisisMode ? $"{seconds / 60:00}:{seconds % 60:00}" : "KÖY", theme.Heading);
            Color pressureColor = game.Run.CrisisPressure >= 80 ? DemoGuiTheme.Red : DemoGuiTheme.Gold;
            GUI.Label(new Rect(145, 21, 170, 25), game.UsesCrisisMode ? $"Baskı {game.Run.CrisisPressure:0}" : "Hazırlık", theme.Body);
            if (game.UsesCrisisMode) DemoGuiTheme.Bar(new Rect(145, 54, 155, 4), (float)game.Run.CrisisPressure / 100, pressureColor);
            theme.ResourceMeter(new Rect(325, 24, 140, 34), "Bütçe", state.Budget, DemoGuiTheme.Gold);
            theme.ResourceMeter(new Rect(475, 24, 140, 34), "Enerji", state.Energy, DemoGuiTheme.Gold);
            theme.ResourceMeter(new Rect(625, 24, 140, 34), "Su", state.Water, DemoGuiTheme.Blue);
            theme.ResourceMeter(new Rect(775, 24, 140, 34), "Bitki", state.PlantHealth, DemoGuiTheme.Green);
            theme.ResourceMeter(new Rect(925, 24, 140, 34), "Emisyon", state.Emissions, DemoGuiTheme.Red, true);
            string progress = game.UsesCrisisDemo && game.Session.IsComplete ?
                $"Bakım {game.Run.Crisis.CompletedCount}/{game.Run.Crisis.Tasks.Count}" : $"Karar {game.Session.CompletedCount}/{game.Session.PointCount}";
            GUI.Label(new Rect(1090, 27, 155, 25), progress, theme.Small);
            if (!game.IntroOpen && !game.IsComplete && !game.EmergencyActive)
            {
                theme.Panel(new Rect(330, 83, 620, 34));
                GUI.Label(new Rect(345, 90, 590, 23), game.Objective, theme.Small);
            }
        }

        private void DrawIntro()
        {
            theme.Panel(new Rect(300, 235, 680, 365), DemoGuiTheme.Gold);
            GUI.Label(new Rect(330, 255, 620, 65), DemoStory.Title, theme.Title);
            GUI.Label(new Rect(330, 330, 620, 84), "Yağışlar azalıyor, köy kuraklığa hazırlanıyor. Enerji, su ve tarım hakkında üç karar ver.", theme.Body);
            string goal = game.UsesCrisisDemo ?
                $"Ardından en az {game.Run.Crisis.RequiredCount} bakım yap ve su hattındaki arızaya müdahale et. Baskıyı {game.Run.CrisisRules.StableThreshold:0} veya altında {game.Run.CrisisRules.StableDuration:0} saniye koru." : "İşaretli noktaları sırayla ziyaret et ve köyün geleceğini şekillendir.";
            GUI.Label(new Rect(330, 414, 620, 65), goal, theme.Body);
            GUI.Label(new Rect(330, 490, 620, 32), "WASD yürü · Sol Shift koş · E etkileşim / arızada basılı tut", theme.Small);
            if (theme.Action(new Rect(440, 540, 400, 44), "Köye dön  [Enter]")) game.StartGame();
        }

        private void DrawHint()
        {
            if (game.NoticeVisible || !game.HasInteraction) return;
            theme.Panel(new Rect(400, 650, 480, 46), DemoGuiTheme.Green);
            GUI.Label(new Rect(418, 662, 444, 26), game.InteractionHint, theme.Body);
        }

        private void DrawCrisisActivity()
        {
            if (!game.EmergencyActive) return;
            var incident = game.Run.Crisis.Emergency;
            float urgency = (float)AtmosphereProfile.Urgency(incident.RemainingSeconds);
            float pulse = .5f + .5f * Mathf.Sin(Time.unscaledTime * Mathf.Lerp(3, 10, urgency));
            Color accent = Color.Lerp(DemoGuiTheme.Gold, DemoGuiTheme.Red, .6f + .4f * urgency * pulse);
            theme.Panel(new Rect(420, 132, 440, 83), accent);
            DemoGuiTheme.Fill(new Rect(420, 132, 5 + urgency * 4, 83), accent);
            GUI.Label(new Rect(440, 145, 240, 26), "ACİL · SU HATTI", theme.ChoiceTitle);
            GUI.Label(new Rect(720, 142, 120, 30), $"{Mathf.CeilToInt((float)incident.RemainingSeconds)} sn", theme.Heading);
            GUI.Label(new Rect(440, 177, 390, 23), incident.RepairSeconds > 0 ? "Onarılıyor… E'yi tut" : "Su deposuna ulaş · E'yi basılı tut", theme.Small);
            DemoGuiTheme.Bar(new Rect(440, 204, 400, 5), (float)incident.RepairFraction, DemoGuiTheme.Blue);
        }

        private void DrawStationMarker()
        {
            string id = game.TargetStationId;
            if (id == null) return;
            int index = id == DecisionCatalog.EnergyStationId ? 0 : id == DecisionCatalog.WaterStationId ? 1 : 2;
            if (view == null || stationMarkers == null || index >= stationMarkers.Length || stationMarkers[index] == null || scale <= 0)
                return;
            Vector3 screen = view.WorldToScreenPoint(stationMarkers[index].position + Vector3.up * 3.5f);
            float rawX = (screen.x - canvasOffset.x) / scale;
            float rawY = (Screen.height - screen.y - canvasOffset.y) / scale;
            if (screen.z <= 0) { rawX = 1280 - rawX; rawY = 720 - rawY; }
            float x = Mathf.Clamp(rawX, 145, 1135);
            float y = Mathf.Clamp(rawY, game.EmergencyActive ? 255 : 155, 580);
            bool offscreen = screen.z <= 0 || x != rawX || y != rawY;
            Color color = game.EmergencyActive ? DemoGuiTheme.Red : DemoGuiTheme.Gold;
            if (!offscreen && !game.EmergencyActive && !game.HasInteraction) return;
            theme.Panel(new Rect(x - 130, y - 16, 260, 39), color);
            string prefix = offscreen ? "Yön: " : game.EmergencyActive ? "ACİL: " : "";
            GUI.Label(new Rect(x - 120, y - 8, 240, 26), prefix + DemoStory.ShortPointName(id), theme.Small);
        }

        private void DrawNotification()
        {
            theme.Panel(new Rect(340, 650, 600, 46), DemoGuiTheme.Green);
            GUI.Label(new Rect(358, 661, 540, 27), game.ShortNotice, theme.Body);
            if (theme.Action(new Rect(902, 659, 28, 28), "×", quietStyle: true)) game.DismissNotice();
        }

        private void DrawCards()
        {
            var point = game.Session.CurrentPoint;
            if (point == null)
                return;
            theme.Panel(new Rect(205, 350, 870, 355), DemoGuiTheme.Gold);
            GUI.Label(new Rect(225, 363, 760, 29), DemoStory.EventTitle(point.Id), theme.Heading);
            GUI.Label(new Rect(225, 399, 830, 37), DemoStory.Problem(point.Id), theme.Small);
            if (detailsOpen)
            {
                detailsScroll = GUI.BeginScrollView(new Rect(225, 441, 830, 217), detailsScroll, new Rect(0, 0, 800, 460));
                for (int i = 0; i < point.Options.Count; i++)
                {
                    var option = point.Options[i];
                    var preview = game.Session.GetPreview(point.Id, option.Id);
                    GUI.Label(new Rect(0, i * 150, 790, 40), option.Title, theme.Body);
                    GUI.Label(new Rect(0, i * 150 + 43, 790, 100), preview.CanChoose ?
                        DecisionMessaging.ChoiceEffects(preview) + "\n" + DecisionMessaging.TurnEffects(preview) +
                        "\n" + DecisionMessaging.NetEffects(preview) : preview.Reason, theme.Small);
                }
                GUI.EndScrollView();
            }
            else for (int i = 0; i < point.Options.Count; i++)
            {
                var option = point.Options[i];
                var preview = game.Session.GetPreview(point.Id, option.Id);
                float x = 220 + i * 285;
                DemoGuiTheme.Fill(new Rect(x, 443, 270, 218), DemoGuiTheme.Card);
                DemoGuiTheme.Fill(new Rect(x, 443, 270, 3), preview.CanChoose ? DemoGuiTheme.Green : DemoGuiTheme.Red);
                GUI.Label(new Rect(x + 10, 452, 250, 40), option.Title, theme.ChoiceTitle);
                GUI.Label(new Rect(x + 10, 496, 250, 34), DemoStory.CardBenefit(option.Id), theme.Small);
                GUI.Label(new Rect(x + 10, 531, 250, 34), $"{preview.Cost} bütçe · " + DemoStory.CardCost(option.Id), theme.Small);
                if (preview.CanChoose)
                {
                    var before = preview.Before; var after = preview.After;
                    theme.EffectChip(new Rect(x + 10, 568, 120, 22), "Enerji", after.Energy - before.Energy);
                    theme.EffectChip(new Rect(x + 140, 568, 120, 22), "Su", after.Water - before.Water);
                    theme.EffectChip(new Rect(x + 10, 592, 120, 22), "Bitki", after.PlantHealth - before.PlantHealth);
                    theme.EffectChip(new Rect(x + 140, 592, 120, 22), "Emisyon", after.Emissions - before.Emissions, true);
                }
                else GUI.Label(new Rect(x + 10, 570, 250, 43), preview.Reason, theme.Tiny);
                if (theme.Action(new Rect(x + 10, 618, 250, 40), preview.CanChoose ? "Kararı uygula" : "Seçilemiyor", preview.CanChoose))
                { game.Choose(option.Id); return; }
            }
            if (detailsOpen) GUI.Label(new Rect(225, 665, 390, 32), "Net etkiler tur ve kuraklığı içerir.", theme.Tiny);
            if (theme.Action(new Rect(625, 661, 225, 32), detailsOpen ? "Seçeneklere dön" : "Etki ayrıntısı", quietStyle: true))
                detailsOpen = !detailsOpen;
            if (theme.Action(new Rect(865, 661, 190, 32), "Vazgeç [Esc]", quietStyle: true)) game.CloseCards();
        }

        private void DrawEnding()
        {
            Color accent = game.IsLegacy ? DemoGuiTheme.Green : game.Result.Category == EndingCategory.Rebirth ? DemoGuiTheme.Green : game.Result.Category == EndingCategory.PartialTransition ? DemoGuiTheme.Gold : DemoGuiTheme.Red;
            string title = game.IsLegacy ? "Enerji prototipi tamamlandı" : game.Result.Title;
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
                finalScroll = GUI.BeginScrollView(new Rect(730, 322, 515, 284), finalScroll, new Rect(0, 0, 485, game.UsesCrisisDemo ? 930 : finalTab == 0 ? 555 : 530));
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
            var summary = game.Result.Summary;
            float offset = game.UsesCrisisDemo ? 210 : 0;
            if (game.UsesCrisisDemo)
                GUI.Label(new Rect(0, 0, 480, 108), game.Result.ManagementSummary, theme.Body);
            GUI.Label(new Rect(0, game.UsesCrisisDemo ? 115 : 0, 480, game.UsesCrisisDemo ? 225 : 130), game.Result.Explanation, theme.Body);
            SummaryBlock(139 + offset, "SU GÜVENLİĞİ", summary.DrinkingWater, DemoGuiTheme.Blue);
            SummaryBlock(221 + offset, "TARIM", summary.Agriculture, DemoGuiTheme.Gold);
            SummaryBlock(303 + offset, "DOĞAL YAŞAM", summary.Nature, DemoGuiTheme.Green);
            GUI.Label(new Rect(0, 387 + offset, 480, 105), summary.RemainingRisk, theme.Body);
            GUI.Label(new Rect(0, 498 + offset, 480, 75), EndingSummary.ProxyNote, theme.Small);
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
            if (game.Session.History.Count == 0)
            { GUI.Label(new Rect(0, 0, 480, 55), "Henüz stratejik karar seçilmedi. Hazırlık tamamlanamadı.", theme.Body); return; }
            foreach (var record in game.Session.History)
            {
                GUI.Label(new Rect(0, y, 480, 28), record.Option.Title, theme.Body);
                GUI.Label(new Rect(0, y + 30, 480, 57), $"Bütçe -{record.Result.Cost} | " + DecisionMessaging.NetEffects(record.Result), theme.Small);
                y += 94;
            }

            GUI.Label(new Rect(0, y + 8, 480, 28), "Öne çıkan iki karar", theme.Heading);
            for (int i = 0; i < game.Result.Summary.KeyDecisions.Count; i++)
                GUI.Label(new Rect(0, y + 45 + i * 83, 480, 78), game.Result.Summary.KeyDecisions[i], theme.Body);
            if (game.UsesCrisisDemo)
            {
                float taskY = y + 220;
                GUI.Label(new Rect(0, taskY, 480, 28), "Altyapı müdahaleleri", theme.Heading);
                foreach (var task in game.Run.Crisis.Tasks)
                {
                    taskY += 40;
                    GUI.Label(new Rect(0, taskY, 480, 38), task.Title + " — " +
                        (task.Status == MaintenanceStatus.Completed ? "tamamlandı" : "eksik"), theme.Small);
                }
                GUI.Label(new Rect(0, taskY + 50, 480, 100), "Kaynak özeti bakım/arıza sonrası güncel durumu kullanır. Yukarıdaki üç kararın net etkileri yalnız stratejik turlara aittir.", theme.Small);
            }
        }

        private void OnDestroy()
        {
            if (theme != null)
                theme.Dispose();
        }
    }
}
