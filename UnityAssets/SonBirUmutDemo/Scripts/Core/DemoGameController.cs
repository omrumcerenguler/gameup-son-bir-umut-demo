using System;
using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.Audio;
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
        [SerializeField, HideInInspector] private bool playtestMode;
        [SerializeField, HideInInspector] private bool releaseCandidate;
        [SerializeField] private DemoAudioManager audioManager;
        [SerializeField] private DemoStationFeedback[] sessionFeedback;
        [SerializeField] private DemoCrisisAtmosphere sessionAtmosphere;
        [SerializeField, HideInInspector] private bool finalPacing;
        [SerializeField] private MaintenanceTaskNode[] taskNodes;
        private int nextTaskSeed;
        private ReleaseFlow releaseFlow;
        private GameRunState legacyRun;
        private int lastUiActionFrame = -1;
        public bool IsReleaseCandidate => releaseCandidate;
        public bool UiInputAvailable => applicationHasFocus && !applicationPaused;
        public ReleaseFlow Menu => releaseFlow;
        public DemoAudioManager Audio => audioManager;
        public bool ReleaseMenuVisible => releaseCandidate && releaseFlow != null && releaseFlow.MenuVisible;
        public string MenuNotice { get; private set; } = "";
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
        public GameRunState Run => releaseCandidate ? releaseFlow?.Run : legacyRun;
        public GameSession Session => Run?.Session;
        public bool UsesTaskNodes => Run != null && Run.UsesTaskNodes;
        public bool UsesPlaytestMode => Run != null && Run.UsesPlaytestMode;
        public Vector3 PlayerPosition => player != null ? player.transform.position : Vector3.zero;
        public bool SimulationPaused => ReleaseMenuVisible || IntroOpen || CardsOpen || IsComplete || !applicationHasFocus || applicationPaused;
        public bool HasInteraction => !ReleaseMenuVisible && (CanInteract || (EmergencyActive && IsStationInRange(DecisionCatalog.WaterStationId)) ||
            (UsesCrisisDemo && Run.Phase == GameRunPhase.CrisisManagement && !EmergencyActive && NearestTask(true) != null));
        public RunOutcome Result => Run?.Result;
        public ResourceState CurrentResources => Run?.CurrentResources;
        public bool UsesCrisisDemo => Run != null && Run.UsesCrisisDemo;
        public bool EmergencyActive => UsesCrisisDemo && !IsComplete && Run.Crisis.Emergency.Status == EmergencyStatus.Active;
        public bool CardsOpen { get; private set; }
        public bool IntroOpen => Run != null && Run.Phase == GameRunPhase.Intro;
        public bool IsLegacy => !completeDemo;
        public bool UsesCrisisMode => Run != null && Run.UsesCrisisMode;
        public bool IsComplete => Run != null && Run.IsEnded;
        public bool MovementLocked => Run == null || ReleaseMenuVisible || Run.MovementLocked || CardsOpen || !applicationHasFocus || applicationPaused;
        public string ResultMessage { get; private set; } = "";
        public string ShortNotice { get; private set; } = "";
        public string TargetStationId
        {
            get
            {
                if (Run == null || IsComplete || IntroOpen) return null;
                if (Run.Phase == GameRunPhase.Preparation) return Session.CurrentPoint.Id;
                if (!UsesCrisisDemo) return null;
                if (EmergencyActive) return DecisionCatalog.WaterStationId;
                var nearest = NearestTask(false);
                if (nearest != null) return UsesTaskNodes ? null : nearest.StationId;
                return !UsesTaskNodes && Run.Crisis.Emergency.Status == EmergencyStatus.Waiting ? DecisionCatalog.WaterStationId : null;
            }
        }
        public string Objective
        {
            get
            {
                if (Run == null || IsComplete) return "Nöbet tamamlandı.";
                if (Run.Phase != GameRunPhase.CrisisManagement)
                    return "Karar: " + DemoStory.ShortPointName(Session.CurrentPoint.Id);
                if (!UsesCrisisDemo) return "Hazırlık tamamlandı; köyün durumunu izle.";
                if (EmergencyActive) return "ACİL: Su deposuna ulaş ve hattı onar.";
                var task = NearestTask(false);
                if (task != null) return UsesTaskNodes ? $"Bakım {Run.Crisis.CompletedCount}/{Run.Crisis.RequiredCount} · {task.Title}" :
                    task.Title + " · " + DemoStory.ShortPointName(task.StationId);
                if (UsesTaskNodes && !double.IsPositiveInfinity(Run.Crisis.NextDispatchIn))
                    return $"Yeni bakım çağrısı {Math.Ceiling(Run.Crisis.NextDispatchIn):0} sn içinde · Köyü denetle";
                if (!Run.Crisis.Emergency.IsResolved) return "Su hattındaki çağrıya hazır ol.";
                if (Run.CrisisPressure > Run.CrisisRules.StableThreshold)
                    return "Müdahaleler tamam · Nöbet sonunu bekle.";
                return $"İstikrarı koru · {Run.Crisis.StableSeconds:0}/{Run.CrisisRules.StableDuration:0} sn";
            }
        }
        public string InteractionHint
        {
            get
            {
                if (EmergencyActive) return IsStationInRange(DecisionCatalog.WaterStationId) ?
                    "[E basılı] Onar" : "";
                if (CanInteract) return "[E] Karar ver";
                var task = UsesCrisisDemo && Run.Phase == GameRunPhase.CrisisManagement ? NearestTask(true) : null;
                if (task != null) return "[E] " + task.Title;
                return "";
            }
        }
        public bool NoticeVisible => !ReleaseMenuVisible && !IsComplete && !IntroOpen && !CardsOpen && !EmergencyActive && Time.unscaledTime < messageUntil && !string.IsNullOrEmpty(ResultMessage);

        public MaintenanceTask CurrentTask => !ReleaseMenuVisible && !IsComplete && Run!=null && Run.Phase==GameRunPhase.CrisisManagement && !EmergencyActive ? NearestTask(false) : null;
        public Transform TaskTarget => NodeFor(CurrentTask)?.transform;
        public string TaskAge => CurrentTask==null ? "" : $"Açık {Math.Floor(Run.Crisis.ElapsedSeconds-CurrentTask.AvailableSince):0} sn";
        private MaintenanceTaskNode NodeFor(MaintenanceTask task)
        {
            if(task==null || taskNodes==null) return null;
            foreach(var node in taskNodes) if(node!=null && node.TaskId==task.Id && node.LocationId==task.LocationId) return node;
            return null;
        }
        private MaintenanceTask NearestTask(bool requireRange)
        {
            if (!UsesCrisisDemo || player == null) return null;
            MaintenanceTask nearest = null; float distance = float.PositiveInfinity;
            foreach (var task in Run.Crisis.Tasks)
            {
                if (task.Status != MaintenanceStatus.Available) continue;
                var node=UsesTaskNodes ? NodeFor(task) : null;
                if(requireRange && (UsesTaskNodes ? node==null || !node.IsInRange(player.transform.position) : !IsStationInRange(task.StationId))) continue;
                Transform target = UsesTaskNodes ? node?.transform : StationTransform(task.StationId);
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
                if (Run == null || Run.Phase != GameRunPhase.Preparation || player == null || ReleaseMenuVisible) return false;
                string id = Session.CurrentPoint.Id;
                if (id == DecisionCatalog.EnergyStationId) return station != null && station.IsInRange(player.transform.position);
                var target = id == DecisionCatalog.WaterStationId ? waterStation : farmStation;
                return target != null && target.IsInRange(player.transform.position);
            }
        }

        public void Configure(DemoPlayerMotor motor, EnergyStation energyStation)
        {
            player = motor; station = energyStation; completeDemo = false; crisisMode = false; crisisDemo = false; playtestMode = false; releaseCandidate = false; finalPacing=false;
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
            crisisMode = true; crisisDemo = false; playtestMode = false; releaseCandidate = false; finalPacing=false;
        }

        public void ConfigureCrisisDemo(CrisisStationMarker[] markers)
        {
            ConfigureCrisisFoundation(); crisisDemo = true; crisisMarkers = markers;
        }

        public void ConfigurePlaytest(CrisisStationMarker[] markers)
        {
            ConfigureCrisisDemo(markers); playtestMode = true;
        }

        public void ConfigureReleaseCandidate(DemoAudioManager audio, DemoStationFeedback[] feedback, DemoCrisisAtmosphere atmosphere)
        {
            if (!playtestMode) throw new InvalidOperationException("Release scene needs the Playtest profile.");
            releaseCandidate = true; audioManager = audio; sessionFeedback = feedback; sessionAtmosphere = atmosphere;
        }

        public void ConfigureFinalPacing(MaintenanceTaskNode[] nodes)
        {
            if(!releaseCandidate || nodes==null || nodes.Length!=FinalPacingCatalog.All.Count)
                throw new InvalidOperationException("Final Pacing requires the release shell and ten task nodes.");
            finalPacing=true; taskNodes=nodes;
            if(!ValidTaskNodes()) throw new InvalidOperationException("Task node identities or positions do not match the catalog.");
        }

        private bool ValidTaskNodes()
        {
            if(taskNodes==null || taskNodes.Length!=FinalPacingCatalog.All.Count) return false;
            var locations=new System.Collections.Generic.HashSet<string>();
            foreach(var node in taskNodes)
            {
                if(node==null || !locations.Add(node.LocationId)) return false;
                var definition=FinalPacingCatalog.At(node.LocationId);
                if(definition==null || definition.TaskId!=node.TaskId) return false;
                Vector3 point=node.transform.position;
                if(Math.Abs(point.x-definition.Position.X)>.01 || Math.Abs(point.z-definition.Position.Z)>.01) return false;
            }
            return true;
        }

        // Shared by keyboard and IMGUI, including a one-command-per-frame guard.
        public bool SubmitMenuCommand(ReleaseCommand command)
        {
            if (!releaseCandidate || releaseFlow == null || !applicationHasFocus || applicationPaused || lastUiActionFrame == Time.frameCount) return false;
            if (command == ReleaseCommand.Pause && CardsOpen) return false;
            var before = Run;
            if (!releaseFlow.Command(command)) return false;
            lastUiActionFrame = Time.frameCount; MenuNotice = "";
            if (!ReferenceEquals(before,Run)) ResetPresentation();
            else if (command == ReleaseCommand.Pause) audioManager.StopEffects();
            audioManager.SavePreferences(); audioManager.Play(SoundCue.Menu);
            DismissNotice();
            return true;
        }

        public bool ConsumeLocalUiAction()
        {
            if (!releaseCandidate || !applicationHasFocus || applicationPaused || lastUiActionFrame == Time.frameCount) return false;
            lastUiActionFrame = Time.frameCount; audioManager.Play(SoundCue.Menu); return true;
        }

        public bool RequestQuit()
        {
            if (!releaseCandidate || releaseFlow == null || releaseFlow.Screen != ReleaseScreen.MainMenu ||
                !applicationHasFocus || applicationPaused || lastUiActionFrame == Time.frameCount) return false;
            lastUiActionFrame = Time.frameCount; audioManager.SavePreferences();
            if (Application.isEditor) MenuNotice = "Çıkış, Windows uygulamasında pencereyi kapatır.";
            else Application.Quit();
            return true;
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
                (crisisDemo && (crisisMarkers == null || crisisMarkers.Length != 3)) ||
                (finalPacing && !ValidTaskNodes()) ||
                (releaseCandidate && (audioManager == null || sessionAtmosphere == null || sessionFeedback == null || sessionFeedback.Length != 3)))
            {
                Debug.LogError("Demo referansları eksik. Tools > GameUP menüsünden yeni sahne oluşturun.", this);
                enabled = false; return;
            }
            spawn = player.transform.position; spawnRotation = player.transform.rotation;
            try
            {
                applicationHasFocus = Application.isFocused;
                if (releaseCandidate)
                {
                    releaseFlow = new ReleaseFlow(CreateRun);
                    releaseFlow.SetFocus(applicationHasFocus); releaseFlow.SetNativePaused(applicationPaused);
                    ResetPresentation();
                }
                else Restart();
            }
            catch (ArgumentException error)
            {
                Debug.LogError("Oturum ayarları geçersiz: " + error.Message, this);
                enabled = false;
            }
        }

        private void Update()
        {
            if (Run == null) return;
            if (!applicationHasFocus || applicationPaused) { TickRun(false,false); return; }
            if (releaseCandidate && DemoInput.CancelPressed)
            {
                if (lastUiActionFrame == Time.frameCount) return;
                var action = releaseFlow.Escape(CardsOpen);
                if (action != EscapeResult.None)
                {
                    lastUiActionFrame = Time.frameCount;
                    if (action == EscapeResult.CloseCards) CloseCards();
                    else { audioManager.StopEffects(); audioManager.SavePreferences(); audioManager.Play(SoundCue.Menu); }
                    return;
                }
            }
            if (ReleaseMenuVisible) { TickRun(false,false); return; }
            if (IntroOpen) { if (DemoInput.StartPressed) StartGame(); return; }
            bool wasEnded = IsComplete;
            var beforeIncident = UsesCrisisDemo ? Run.Crisis.Emergency.Status : EmergencyStatus.Waiting;
            var beforeResources = CurrentResources;
            string availableBefore = AvailableTasks();
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
                bool completed;
                string message;
                if(UsesTaskNodes)
                {
                    var node=NodeFor(task);
                    completed=Run.TryCompleteTask(task.Id,task.LocationId,node!=null && node.IsInRange(player.transform.position),CardsOpen || ReleaseMenuVisible,out message);
                }
                else completed=Run.TryCompleteMaintenance(task.StationId,out message);
                if(completed) { ShowMessage(message,task.Title+" tamamlandı",3f); audioManager?.Play(SoundCue.Maintenance); }
            }
            if (!releaseCandidate && !CardsOpen && DemoInput.CancelPressed) DismissNotice();
            TickRun(repairHeld,repairInRange);
            if(UsesTaskNodes && !IsComplete && availableBefore!=AvailableTasks() && !EmergencyActive)
            {
                var next=NearestTask(false);
                if(next!=null) ShowMessage(next.Description,(task!=null && task.Status==MaintenanceStatus.Completed ? "Bakım tamam · " : "Yeni hedef · ")+next.Title,3f);
            }
            if (!ReferenceEquals(beforeResources, CurrentResources) && world != null) world.Apply(CurrentResources);
            if (UsesCrisisDemo && beforeIncident != Run.Crisis.Emergency.Status)
            {
                if (Run.Crisis.Emergency.Status == EmergencyStatus.Active) { DismissNotice(); audioManager?.Play(SoundCue.Emergency); }
                else if (Run.Crisis.Emergency.Status == EmergencyStatus.Repaired) ShowMessage("Su hattı onarıldı. Kriz baskısı -18; bakım için E'yi bırakıp yeniden bas.", "Su hattı onarıldı", 4f);
                else if (Run.Crisis.Emergency.Status == EmergencyStatus.Expired) ShowMessage("Arızaya geç kalındı: su -6, baskı +8. Bakımlara devam edebilirsin.", "Rezerv kaybı · Bakıma devam et", 4f);
            }
            if (!wasEnded && IsComplete)
            {
                CardsOpen = false;
                if (releaseCandidate) { world.Apply(CurrentResources); sessionAtmosphere.RefreshNow(); }
                if (followCamera != null) followCamera.SetOverview(completeDemo);
                DismissNotice();
                audioManager?.Play(Result.Category == EndingCategory.Rebirth ? SoundCue.Success : SoundCue.Failure);
            }
        }

        private string AvailableTasks()
        {
            if(!UsesTaskNodes) return "";
            string ids=""; foreach(var item in Run.Crisis.Tasks) if(item.Status==MaintenanceStatus.Available) ids+=item.Id+";";
            return ids;
        }

        private void TickRun(bool held,bool inRange)
        {
            if (releaseCandidate) releaseFlow.Tick(Time.unscaledDeltaTime,CardsOpen,held,inRange);
            else Run.Tick(Time.unscaledDeltaTime,CardsOpen,held,inRange);
        }

        private void OnApplicationFocus(bool focused)
        {
            applicationHasFocus = focused;
            if (releaseCandidate) releaseFlow?.SetFocus(focused); else Run?.SetFocus(focused);
            if (!focused) audioManager?.StopEffects();
        }
        private void OnApplicationPause(bool paused)
        {
            applicationPaused = paused;
            if (releaseCandidate) releaseFlow?.SetNativePaused(paused); else Run?.SetApplicationPaused(paused);
        }

        public void StartGame()
        {
            if (releaseCandidate) SubmitMenuCommand(ReleaseCommand.StartGameplay);
            else if (Run != null && Run.Start()) DismissNotice();
        }
        public void CloseCards() => CardsOpen = false;
        public void DismissNotice() { ResultMessage = ShortNotice = ""; messageUntil = 0; }

        public void Choose(string optionId)
        {
            if (!CardsOpen || !CanInteract || !applicationHasFocus || applicationPaused) return;
            string pointId = Session.CurrentPoint.Id;
            if (!Run.TryChoose(pointId, optionId, out var message)) { ShowMessage(message); return; }
            if (pointId == DecisionCatalog.EnergyStationId) station.ShowChoice(optionId);
            else if (pointId == DecisionCatalog.WaterStationId) waterStation.ShowChoice(optionId);
            else farmStation.ShowChoice(optionId);
            CardsOpen = false;
            if (world != null) world.Apply(CurrentResources);
            if (followCamera != null) followCamera.SetOverview(completeDemo && IsComplete);
            ShowMessage(DecisionMessaging.SelectionFeedback(Session.History[Session.History.Count - 1]), "Karar uygulandı", 3f);
            audioManager?.Play(SoundCue.Decision);
        }

        private void ShowMessage(string message, string shortMessage = null, float duration = 4f)
        { ResultMessage = message; ShortNotice = shortMessage ?? "İşlem uygulanamadı"; messageUntil = Time.unscaledTime + duration; }

        private GameRunState CreateRun()
        {
            return new GameRunState(finalPacing ? GameMode.FinalPacing : playtestMode ? GameMode.PlaytestDemo : crisisDemo ? GameMode.CrisisDemo : crisisMode ? GameMode.CrisisFoundation : GameMode.LegacyDecisions,
                new GameRunSettings(durationSeconds, initialPressure, baseRise, waterRiskRise,
                    plantRiskRise, energyRiskRise, maximumRisePerSecond),
                definitions: completeDemo ? DecisionCatalog.All : new[] { DecisionCatalog.EnergyStation },
                crisisRules: new CrisisRules(stableThreshold, stableDuration, emergencyDelay, emergencyWindow, repairDuration), taskSeed: finalPacing ? nextTaskSeed++ : 0);
        }

        public void Restart()
        {
            if (releaseCandidate) { SubmitMenuCommand(ReleaseCommand.Restart); return; }
            legacyRun = CreateRun();
            if (!completeDemo) Run.Start();
            applicationHasFocus = Application.isFocused;
            Run.SetFocus(applicationHasFocus); Run.SetApplicationPaused(applicationPaused);
            ResetPresentation();
        }

        private void ResetPresentation()
        {
            CardsOpen = false;
            station.ShowChoice(null);
            if (waterStation != null) waterStation.ShowChoice(null);
            if (farmStation != null) farmStation.ShowChoice(null);
            player.ResetPosition(spawn, spawnRotation);
            if (world != null) world.Apply(CurrentResources);
            if (followCamera != null)
            {
                followCamera.ResetView();
                if (releaseCandidate && releaseFlow.MainMenuBackground) followCamera.ShowMenuOverview();
            }
            if (releaseCandidate)
            {
                audioManager.StopEffects();
                if(finalPacing) foreach(var node in taskNodes) if(node!=null) node.RefreshNow();
                foreach (var marker in crisisMarkers) if (marker != null) marker.RefreshNow();
                foreach (var feedback in sessionFeedback) if (feedback != null) feedback.RefreshNow();
                sessionAtmosphere.RefreshNow();
            }
            DismissNotice();
        }
    }
}
