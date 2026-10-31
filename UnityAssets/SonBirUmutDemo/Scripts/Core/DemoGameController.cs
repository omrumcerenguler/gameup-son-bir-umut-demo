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
        [SerializeField, HideInInspector] private bool crisisDemo;
        [SerializeField] private CrisisStationMarker[] crisisMarkers;
        [Header("Crisis Demo — game balance")]
        [SerializeField, Range(0, 100)] private float stableThreshold = 35;
        [SerializeField, Min(.1f)] private float stableDuration = 10;
        [SerializeField, Min(0)] private float emergencyDelay = 12;
        [SerializeField, Min(2)] private float emergencyWindow = 45;
        [SerializeField, Min(.1f)] private float repairDuration = 2;
        [Header("Timed crisis scenes — example game values")]
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
        private bool applicationHasFocus = true;
        public GameRunState Run { get; private set; }
        public GameSession Session => Run?.Session;
        public RunOutcome Result => Run?.Result;
        public ResourceState CurrentResources => Run?.CurrentResources;
        public bool UsesCrisisDemo => Run != null && Run.UsesCrisisDemo;
        public bool EmergencyActive => UsesCrisisDemo && !IsComplete && Run.Crisis.Emergency.Status == EmergencyStatus.Active;
        public bool CardsOpen { get; private set; }
        public bool IntroOpen => Run != null && Run.Phase == GameRunPhase.Intro;
        public bool IsLegacy => !completeDemo;
        public bool UsesCrisisMode => Run != null && Run.UsesCrisisMode;
        public bool IsComplete => Run != null && Run.IsEnded;
        public bool MovementLocked => Run == null || Run.MovementLocked || CardsOpen || !applicationHasFocus || applicationPaused;
        public string ResultMessage { get; private set; } = "";
        public string TargetStationId
        {
            get
            {
                if (Run == null || IsComplete || IntroOpen) return null;
                if (Run.Phase == GameRunPhase.Preparation) return Session.CurrentPoint.Id;
                if (!UsesCrisisDemo) return null;
                if (EmergencyActive) return DecisionCatalog.WaterStationId;
                var nearest = NearestTask(false);
                if (nearest != null) return nearest.StationId;
                return Run.Crisis.Emergency.Status == EmergencyStatus.Waiting ? DecisionCatalog.WaterStationId : null;
            }
        }
        public string Objective
        {
            get
            {
                if (Run == null || IsComplete) return "Nöbet tamamlandı.";
                if (Run.Phase != GameRunPhase.CrisisManagement)
                    return DemoStory.PointName(Session.CurrentPoint.Id) + " noktasına git; [E] ile karar ver.";
                if (!UsesCrisisDemo) return "Hazırlık tamamlandı; köyün durumunu izle.";
                if (EmergencyActive) return "ACİL: Su hattı arızası! Su deposuna koş; yakında E'yi basılı tut.";
                var task = NearestTask(false);
                if (task != null) return task.Title + " — " + DemoStory.ShortPointName(task.StationId) + " [E]";
                if (!Run.Crisis.Emergency.IsResolved) return "Altyapı denetlendi; su hattındaki acil çağrıya hazır ol.";
                if (Run.CrisisPressure > Run.CrisisRules.StableThreshold)
                    return "Müdahaleler tamam; baskı yüksek. Nöbet sonunda hazırlığın değerlendirilecek.";
                return $"Baskıyı {Run.CrisisRules.StableThreshold:0} veya altında tut: {Run.Crisis.StableSeconds:0.0}/{Run.CrisisRules.StableDuration:0} sn.";
            }
        }
        public string InteractionHint
        {
            get
            {
                if (EmergencyActive) return IsStationInRange(DecisionCatalog.WaterStationId) ?
                    "[E basılı] Su hattını onar • Bırakmak veya uzaklaşmak ilerlemeyi sıfırlar" : "ACİL: Su deposuna git • Sol Shift ile koş";
                if (CanInteract) return "[E] " + DemoStory.PointName(Session.CurrentPoint.Id) + " — karar ver";
                var task = UsesCrisisDemo && Run.Phase == GameRunPhase.CrisisManagement ? NearestTask(true) : null;
                if (task != null) return "[E] " + task.Title;
                if (UsesCrisisDemo && Run.Phase == GameRunPhase.CrisisManagement)
                    return Run.Crisis.Emergency.Status == EmergencyStatus.Waiting ? "Su hattındaki acil çağrıya hazır ol • İşareti izle" : "Kalan süreyi ve istikrarı izle • Köyü inceleyebilirsin";
                return "WASD Dolaş • Sol Shift Koş • E Etkileşim";
            }
        }
        public bool NoticeVisible => !IntroOpen && !CardsOpen && !EmergencyActive && Time.unscaledTime < messageUntil && !string.IsNullOrEmpty(ResultMessage);

        private MaintenanceTask NearestTask(bool requireRange)
        {
            if (!UsesCrisisDemo || player == null) return null;
            MaintenanceTask nearest = null; float distance = float.PositiveInfinity;
            foreach (var task in Run.Crisis.Tasks)
            {
                if (task.Status != MaintenanceStatus.Available || (requireRange && !IsStationInRange(task.StationId))) continue;
                Transform target = StationTransform(task.StationId);
                if (target == null) continue;
                float next = (target.position - player.transform.position).sqrMagnitude;
                if (next < distance) { nearest = task; distance = next; }
            }
            return nearest;
        }
        private Transform StationTransform(string id) => id == DecisionCatalog.EnergyStationId ? station?.transform :
            id == DecisionCatalog.WaterStationId ? waterStation?.transform : id == DecisionCatalog.FarmStationId ? farmStation?.transform : null;
        private bool IsStationInRange(string id)
        {
            if (player == null) return false;
            if (id == DecisionCatalog.EnergyStationId) return station != null && station.IsInRange(player.transform.position);
            if (id == DecisionCatalog.WaterStationId) return waterStation != null && waterStation.IsInRange(player.transform.position);
            return id == DecisionCatalog.FarmStationId && farmStation != null && farmStation.IsInRange(player.transform.position);
        }

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
            player = motor; station = energyStation; completeDemo = false; crisisMode = false; crisisDemo = false;
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
            crisisMode = true; crisisDemo = false;
        }

        public void ConfigureCrisisDemo(CrisisStationMarker[] markers)
        {
            ConfigureCrisisFoundation(); crisisDemo = true; crisisMarkers = markers;
        }

        private void Awake()
        {
            if (!DemoInput.SupportsRuntimeUi)
            {
                Debug.LogError("IMGUI için Active Input Handling: Both veya Input Manager (Old) seçin ve Editor'ı yeniden başlatın.", this);
                enabled = false; return;
            }
            if (player == null || station == null || (completeDemo &&
                (waterStation == null || farmStation == null || world == null || followCamera == null)) ||
                (crisisDemo && (crisisMarkers == null || crisisMarkers.Length != 3)))
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
            if (!applicationHasFocus || applicationPaused) { Run.Tick(Time.unscaledDeltaTime, CardsOpen); return; }
            if (IntroOpen) { if (DemoInput.StartPressed) StartGame(); return; }
            bool wasEnded = IsComplete;
            var beforeIncident = UsesCrisisDemo ? Run.Crisis.Emergency.Status : EmergencyStatus.Waiting;
            var beforeResources = CurrentResources;
            var task = UsesCrisisDemo && Run.Phase == GameRunPhase.CrisisManagement ? NearestTask(true) : null;
            InteractionKind owner = InteractionPolicy.Resolve(CardsOpen || IsComplete, EmergencyActive, CanInteract, task != null);
            bool repairHeld = false, repairInRange = false;
            // One owner consumes this frame's E; station components never poll input independently.
            if (owner == InteractionKind.Modal)
            { if (CardsOpen && DemoInput.CancelPressed) CloseCards(); }
            else if (owner == InteractionKind.Emergency)
            {
                repairHeld = DemoInput.InteractHeld;
                repairInRange = IsStationInRange(DecisionCatalog.WaterStationId);
            }
            else if (owner == InteractionKind.StrategicDecision && DemoInput.InteractPressed) CardsOpen = true;
            else if (owner == InteractionKind.Maintenance && DemoInput.InteractPressed)
            {
                if (Run.TryCompleteMaintenance(task.StationId, out var message)) ShowMessage(message);
            }
            if (!CardsOpen && DemoInput.CancelPressed) DismissNotice();
            Run.Tick(Time.unscaledDeltaTime, CardsOpen, repairHeld, repairInRange);
            if (!ReferenceEquals(beforeResources, CurrentResources) && world != null) world.Apply(CurrentResources);
            if (UsesCrisisDemo && beforeIncident != Run.Crisis.Emergency.Status)
            {
                if (Run.Crisis.Emergency.Status == EmergencyStatus.Active) DismissNotice();
                else if (Run.Crisis.Emergency.Status == EmergencyStatus.Repaired) ShowMessage("Su hattı onarıldı. Kriz baskısı -18; bakım için E'yi bırakıp yeniden bas.");
                else if (Run.Crisis.Emergency.Status == EmergencyStatus.Expired) ShowMessage("Arızaya geç kalındı: su -6, baskı +8. Bakımlara devam edebilirsin.");
            }
            if (!wasEnded && IsComplete)
            {
                CardsOpen = false;
                if (followCamera != null) followCamera.SetOverview(completeDemo);
                DismissNotice();
            }
        }

        private void OnApplicationFocus(bool focused)
        { applicationHasFocus = focused; Run?.SetFocus(focused); }
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
            if (world != null) world.Apply(CurrentResources);
            if (followCamera != null) followCamera.SetOverview(completeDemo && IsComplete);
            ShowMessage(DecisionMessaging.SelectionFeedback(Session.History[Session.History.Count - 1]));
        }

        private void ShowMessage(string message) { ResultMessage = message; messageUntil = Time.unscaledTime + 10f; }

        public void Restart()
        {
            Run = new GameRunState(crisisDemo ? GameMode.CrisisDemo : crisisMode ? GameMode.CrisisFoundation : GameMode.LegacyDecisions,
                new GameRunSettings(durationSeconds, initialPressure, baseRise, waterRiskRise,
                    plantRiskRise, energyRiskRise, maximumRisePerSecond),
                definitions: completeDemo ? DecisionCatalog.All : new[] { DecisionCatalog.EnergyStation },
                crisisRules: new CrisisRules(stableThreshold, stableDuration, emergencyDelay, emergencyWindow, repairDuration));
            if (!completeDemo) Run.Start();
            applicationHasFocus = Application.isFocused;
            Run.SetFocus(applicationHasFocus); Run.SetApplicationPaused(applicationPaused);
            CardsOpen = false;
            station.ShowChoice(null);
            if (waterStation != null) waterStation.ShowChoice(null);
            if (farmStation != null) farmStation.ShowChoice(null);
            player.ResetPosition(spawn, spawnRotation);
            if (world != null) world.Apply(CurrentResources);
            if (followCamera != null) followCamera.ResetView();
            DismissNotice();
        }
    }
}
