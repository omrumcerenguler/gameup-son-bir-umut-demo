using System;
using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.Interaction;
using SonBirUmutDemo.Player;
using SonBirUmutDemo.Simulation;
using SonBirUmutDemo.World;
using UnityEngine;

namespace SonBirUmutDemo.Core
{
    [DefaultExecutionOrder(-100)]
    public sealed class DemoGameController : MonoBehaviour
    {
        // Preserve serialized names and the default three-decision mode of existing scenes.
        [SerializeField] private DemoPlayerMotor player;
        [SerializeField] private EnergyStation station;
        [SerializeField] private bool completeDemo;
        [SerializeField] private DecisionStation waterStation;
        [SerializeField] private DecisionStation farmStation;
        [SerializeField] private DemoWorldView world;
        [SerializeField] private IsometricFollowCamera followCamera;
        [SerializeField, HideInInspector] private bool crisisMode;
        [Header("Crisis Foundation scene only — example game values")]
        [SerializeField, Min(1f)] private float durationSeconds = 240;
        [SerializeField, Range(0, 100)] private float initialPressure = 60;
        [SerializeField, Range(0, 1)] private float baseRise = .04f;
        [SerializeField, Range(0, 1)] private float waterRiskRise = .04f;
        [SerializeField, Range(0, 1)] private float plantRiskRise = .03f;
        [SerializeField, Range(0, 1)] private float energyRiskRise = .02f;
        [SerializeField, Range(0, 1)] private float maximumRisePerSecond = .25f;
        private Vector3 spawn;
        private Quaternion spawnRotation;
        private float messageUntil;
        private bool applicationPaused;
        public GameRunState Run { get; private set; }
        public GameSession Session => Run?.Session;
        public RunOutcome Result => Run?.Result;
        public bool CardsOpen { get; private set; }
        public bool IntroOpen => Run != null && Run.Phase == GameRunPhase.Intro;
        public bool IsLegacy => !completeDemo;
        public bool UsesCrisisMode => Run != null && Run.UsesCrisisMode;
        public bool IsComplete => Run != null && Run.IsEnded;
        public bool MovementLocked => Run == null || Run.MovementLocked || CardsOpen;
        public string ResultMessage { get; private set; } = "";
        public string Objective => Run == null || IsComplete ? "Nöbet tamamlandı." :
            Run.Phase == GameRunPhase.CrisisManagement ? "Stratejik hazırlık tamamlandı; köyün durumunu izle." :
            DemoStory.PointName(Session.CurrentPoint.Id) + " noktasını araştır.";
        public bool NoticeVisible => !IntroOpen && !CardsOpen && Time.unscaledTime < messageUntil && !string.IsNullOrEmpty(ResultMessage);
        public bool CanInteract
        {
            get
            {
                if (Run == null || Run.Phase != GameRunPhase.Preparation || player == null) return false;
                string id = Session.CurrentPoint.Id;
                if (id == DecisionCatalog.EnergyStationId) return station != null && station.IsInRange(player.transform.position);
                var target = id == DecisionCatalog.WaterStationId ? waterStation : farmStation;
                return target != null && target.IsInRange(player.transform.position);
            }
        }

        public void Configure(DemoPlayerMotor motor, EnergyStation energyStation)
        {
            player = motor; station = energyStation; completeDemo = false; crisisMode = false;
        }

        public void ConfigureCompleteDemo(DemoPlayerMotor motor, EnergyStation energyStation,
            DecisionStation waterPoint, DecisionStation farmPoint, DemoWorldView view, IsometricFollowCamera camera)
        {
            Configure(motor, energyStation);
            completeDemo = true; waterStation = waterPoint; farmStation = farmPoint;
            world = view; followCamera = camera;
        }

        // Called only by the separate opt-in scene builder. Existing scenes keep the false flag.
        public void ConfigureCrisisFoundation()
        {
            if (!completeDemo) throw new InvalidOperationException("Kriz sahnesi üç karar noktası gerektirir.");
            crisisMode = true;
        }

        private void Awake()
        {
            if (!DemoInput.SupportsRuntimeUi)
            {
                Debug.LogError("IMGUI için Active Input Handling: Both veya Input Manager (Old) seçin ve Editor'ı yeniden başlatın.", this);
                enabled = false; return;
            }
            if (player == null || station == null || (completeDemo &&
                (waterStation == null || farmStation == null || world == null || followCamera == null)))
            {
                Debug.LogError("Demo referansları eksik. Tools > GameUP menüsünden yeni sahne oluşturun.", this);
                enabled = false; return;
            }
            spawn = player.transform.position; spawnRotation = player.transform.rotation;
            try { Restart(); }
            catch (ArgumentException error)
            {
                Debug.LogError("Oturum ayarları geçersiz: " + error.Message, this);
                enabled = false;
            }
        }

        private void Update()
        {
            if (Run == null) return;
            if (IntroOpen) { if (DemoInput.StartPressed) StartGame(); return; }
            if (CardsOpen && DemoInput.CancelPressed) CloseCards();
            else if (!CardsOpen && DemoInput.CancelPressed) DismissNotice();
            else if (!CardsOpen && CanInteract && DemoInput.InteractPressed) CardsOpen = true;
            bool wasEnded = IsComplete;
            Run.Tick(Time.unscaledDeltaTime, CardsOpen);
            if (!wasEnded && IsComplete)
            {
                CardsOpen = false;
                if (followCamera != null) followCamera.SetOverview(completeDemo);
                DismissNotice();
            }
        }

        private void OnApplicationFocus(bool focused) => Run?.SetFocus(focused);
        private void OnApplicationPause(bool paused)
        {
            applicationPaused = paused;
            Run?.SetApplicationPaused(paused);
        }

        public void StartGame() { if (Run != null && Run.Start()) DismissNotice(); }
        public void CloseCards() => CardsOpen = false;
        public void DismissNotice() { ResultMessage = ""; messageUntil = 0; }

        public void Choose(string optionId)
        {
            if (!CardsOpen || !CanInteract) return;
            string pointId = Session.CurrentPoint.Id;
            if (!Run.TryChoose(pointId, optionId, out var message)) { ShowMessage(message); return; }
            if (pointId == DecisionCatalog.EnergyStationId) station.ShowChoice(optionId);
            else if (pointId == DecisionCatalog.WaterStationId) waterStation.ShowChoice(optionId);
            else farmStation.ShowChoice(optionId);
            CardsOpen = false;
            if (world != null) world.Apply(Session.State);
            if (followCamera != null) followCamera.SetOverview(completeDemo && IsComplete);
            ShowMessage(DecisionMessaging.SelectionFeedback(Session.History[Session.History.Count - 1]));
        }

        private void ShowMessage(string message) { ResultMessage = message; messageUntil = Time.unscaledTime + 10f; }

        public void Restart()
        {
            Run = new GameRunState(crisisMode ? GameMode.CrisisFoundation : GameMode.LegacyDecisions,
                new GameRunSettings(durationSeconds, initialPressure, baseRise, waterRiskRise,
                    plantRiskRise, energyRiskRise, maximumRisePerSecond),
                definitions: completeDemo ? DecisionCatalog.All : new[] { DecisionCatalog.EnergyStation });
            if (!completeDemo) Run.Start();
            Run.SetFocus(Application.isFocused); Run.SetApplicationPaused(applicationPaused);
            CardsOpen = false;
            station.ShowChoice(null);
            if (waterStation != null) waterStation.ShowChoice(null);
            if (farmStation != null) farmStation.ShowChoice(null);
            player.ResetPosition(spawn, spawnRotation);
            if (world != null) world.Apply(Session.State);
            if (followCamera != null) followCamera.ResetView();
            DismissNotice();
        }
    }
}
