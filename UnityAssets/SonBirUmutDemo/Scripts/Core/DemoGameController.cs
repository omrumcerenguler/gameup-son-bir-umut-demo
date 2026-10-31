using SonBirUmutDemo.Decisions;
using SonBirUmutDemo.Interaction;
using SonBirUmutDemo.Player;
using SonBirUmutDemo.Simulation;
using UnityEngine;

namespace SonBirUmutDemo.Core
{
    [DefaultExecutionOrder(-100)]
    public sealed class DemoGameController : MonoBehaviour
    {
        [SerializeField] private DemoPlayerMotor player;
        [SerializeField] private EnergyStation station;
        private Vector3 spawn;
        private Quaternion spawnRotation;
        public GameSession Session { get; private set; }
        public bool CardsOpen { get; private set; }
        public string ResultMessage { get; private set; } = "Enerji istasyonuna yaklaş ve E tuşuna bas.";
        public bool IsComplete => Session != null && Session.GetChoice(DecisionCatalog.EnergyStationId) != null;
        public bool CanInteract => player != null && station != null && !IsComplete &&
            station.IsInRange(player.transform.position);

        public void Configure(DemoPlayerMotor motor, EnergyStation energyStation)
        {
            player = motor;
            station = energyStation;
        }

        private void Awake()
        {
            if (!DemoInput.SupportsRuntimeUi)
            {
                Debug.LogError("IMGUI kartları için Active Input Handling: Both veya Input Manager (Old) seçin ve Editor'ı yeniden başlatın.", this);
                enabled = false;
                return;
            }
            if (player == null || station == null)
            {
                Debug.LogError("Demo referansları eksik. Tools > GameUP > Create Demo Scene ile sahneyi yeniden oluşturun.", this);
                enabled = false;
                return;
            }
            spawn = player.transform.position;
            spawnRotation = player.transform.rotation;
            Restart();
        }

        private void Update()
        {
            if (CardsOpen && DemoInput.CancelPressed) CardsOpen = false;
            else if (!CardsOpen && CanInteract && DemoInput.InteractPressed) CardsOpen = true;
        }

        public void CloseCards() => CardsOpen = false;

        public void Choose(string optionId)
        {
            if (!CardsOpen || !CanInteract) return;
            if (!Session.TryChoose(DecisionCatalog.EnergyStationId, optionId, out var message))
            {
                ResultMessage = message;
                return;
            }
            station.ShowChoice(Session.GetChoice(DecisionCatalog.EnergyStationId));
            CardsOpen = false;
            ResultMessage = message;
        }

        public void Restart()
        {
            Session = new GameSession();
            CardsOpen = false;
            station.ShowChoice(null);
            player.ResetPosition(spawn, spawnRotation);
            ResultMessage = "Enerji istasyonuna yaklaş ve E tuşuna bas.";
        }
    }
}
