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
        // Keep original serialized names so existing single-station scenes retain their references.
        [SerializeField] private DemoPlayerMotor player;
        [SerializeField] private EnergyStation station;
        [SerializeField] private bool completeDemo;
        [SerializeField] private DecisionStation waterStation;
        [SerializeField] private DecisionStation farmStation;
        [SerializeField] private DemoWorldView world;
        [SerializeField] private IsometricFollowCamera followCamera;
        private Vector3 spawn;
        private Quaternion spawnRotation;
        private float messageUntil;
        public GameSession Session { get; private set; }
        public bool CardsOpen { get; private set; }
        public bool IntroOpen { get; private set; }
        public bool IsLegacy => !completeDemo;
        public bool IsComplete => Session != null && Session.IsComplete;
        public bool MovementLocked => IntroOpen || CardsOpen || (completeDemo && IsComplete);
        public string ResultMessage { get; private set; } = "";
        public string Objective => Session == null || IsComplete ? "Kararlar tamamlandı." :
            DemoStory.PointName(Session.CurrentPoint.Id) + " noktasını araştır.";
        public bool NoticeVisible => !IntroOpen && !CardsOpen && Time.unscaledTime < messageUntil && !string.IsNullOrEmpty(ResultMessage);
        public bool CanInteract
        {
            get
            {
                if (IntroOpen || Session == null || IsComplete || player == null) return false;
                string id = Session.CurrentPoint.Id;
                if (id == DecisionCatalog.EnergyStationId) return station != null && station.IsInRange(player.transform.position);
                var target = id == DecisionCatalog.WaterStationId ? waterStation : farmStation;
                return target != null && target.IsInRange(player.transform.position);
            }
        }

        // Older saved scenes remain a one-station mode; regenerate to try the complete demo.
        public void Configure(DemoPlayerMotor motor, EnergyStation energyStation)
        {
            player = motor;
            station = energyStation;
            completeDemo = false;
        }

        public void ConfigureCompleteDemo(DemoPlayerMotor motor, EnergyStation energyStation,
            DecisionStation waterPoint, DecisionStation farmPoint, DemoWorldView view, IsometricFollowCamera camera)
        {
            Configure(motor, energyStation);
            completeDemo = true;
            waterStation = waterPoint;
            farmStation = farmPoint;
            world = view;
            followCamera = camera;
        }

        private void Awake()
        {
            if (!DemoInput.SupportsRuntimeUi)
            {
                Debug.LogError("IMGUI için Active Input Handling: Both veya Input Manager (Old) seçin ve Editor'ı yeniden başlatın.", this);
                enabled = false;
                return;
            }
            if (player == null || station == null || (completeDemo &&
                (waterStation == null || farmStation == null || world == null || followCamera == null)))
            {
                Debug.LogError("Demo referansları eksik. Tools > GameUP > Create Demo Scene ile yeni sahne oluşturun.", this);
                enabled = false;
                return;
            }
            spawn = player.transform.position;
            spawnRotation = player.transform.rotation;
            Restart();
        }

        private void Update()
        {
            if (IntroOpen) { if (DemoInput.StartPressed) StartGame(); return; }
            if (CardsOpen && DemoInput.CancelPressed) CloseCards();
            else if (!CardsOpen && DemoInput.CancelPressed) DismissNotice();
            else if (!CardsOpen && CanInteract && DemoInput.InteractPressed) CardsOpen = true;
        }

        public void StartGame()
        {
            IntroOpen = false;
            DismissNotice();
        }

        public void CloseCards() => CardsOpen = false;

        public void DismissNotice()
        {
            ResultMessage = "";
            messageUntil = 0;
        }

        public void Choose(string optionId)
        {
            if (!CardsOpen || !CanInteract) return;
            string pointId = Session.CurrentPoint.Id;
            if (!Session.TryChoose(pointId, optionId, out var message)) { ShowMessage(message); return; }
            if (pointId == DecisionCatalog.EnergyStationId) station.ShowChoice(optionId);
            else if (pointId == DecisionCatalog.WaterStationId) waterStation.ShowChoice(optionId);
            else farmStation.ShowChoice(optionId);
            CardsOpen = false;
            if (world != null) world.Apply(Session.State);
            if (followCamera != null) followCamera.SetOverview(completeDemo && IsComplete);
            ShowMessage(DecisionMessaging.SelectionFeedback(Session.History[Session.History.Count - 1]));
        }

        private void ShowMessage(string message)
        {
            ResultMessage = message;
            messageUntil = Time.unscaledTime + 10f;
        }

        public void Restart()
        {
            Session = completeDemo ? new GameSession() : new GameSession(definitions: new[] { DecisionCatalog.EnergyStation });
            CardsOpen = false;
            IntroOpen = completeDemo;
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
