using System.Collections;
using ColorFall.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ColorFall.Game
{
    public class EnergyManager : MonoBehaviour, IGameManager
    {
        [SerializeField] private int modeDuration = 5;
        [SerializeField] private int secondChanceDuration;
        [SerializeField] private int maxEnergyCount;
        [SerializeField] private Image energyBar;

        private readonly int _energyPerDrop = 2;

        public ManagerStatus Status { get; private set; }
        public bool IsCharged { get; private set; }

        private int _energyCount;

        private int EnergyCount
        {
            get => _energyCount;
            set
            {
                _energyCount = value;
                if (_energyCount > maxEnergyCount) _energyCount = maxEnergyCount;
                if (_energyCount < 0) _energyCount = 0;
                energyBar.fillAmount = _energyCount / 100f;
            }
        }
        

        public void Startup()
        {
            Debug.Log("Energy manager starting...");
            
            // Activate charged mode after bounce
            EventManager.AddListener<SecondChanceEvent>(_ => Invoke(nameof(SecondChanceCharge), 1f));
            EventManager.AddListener<CollectDropEvent>(OnCollectDrop);
            EventManager.AddListener<PlayerFinishEvent>(_ => OnFinishOrLose());
            EventManager.AddListener<PlayerLoseEvent>(_ => OnFinishOrLose());
            EventManager.AddListener<ChargedModeOffEvent>(_ => StopCharge());
            
            Status = ManagerStatus.Started;
        }

        void OnFinishOrLose()
        {
            if (IsCharged)
                EventManager.Broadcast(Events.ChargedModeOffEvent);
            EnergyCount = 0;
        }

        void OnCollectDrop(CollectDropEvent evt)
        {
            if (IsCharged) return;
            if (evt.collectedDrop == CollectedDrop.Correct)
                IncreaseEnergy();
            else
                DecreaseEnergy();
        }

        private void IncreaseEnergy()
        {
            EnergyCount += _energyPerDrop;
            if (EnergyCount == maxEnergyCount && !IsCharged) StartDefaultCharge();
        }

        private void DecreaseEnergy()
        {
            EnergyCount--;
        }

        void StopCharge()
        {
            EnergyCount = 0;
            IsCharged = false;
        }
        
        void SecondChanceCharge()
        {
            StartCharge(secondChanceDuration);
        }

        private void StartDefaultCharge()
        {
            StartCharge(modeDuration);
        }

        private void StartCharge(int duration)
        {
            IsCharged = true;
            EventManager.Broadcast(Events.ChargedModeOnEvent);
            StartCoroutine(ChargedMode(duration));
        }

        private IEnumerator ChargedMode(float duration)
        {
            var delta = 1f / duration;
            while (duration > 0)
            {
                yield return new WaitForSecondsRealtime(0.1f);
                duration -= 0.1f;
                energyBar.fillAmount -= delta / 10f;
            }
            EventManager.Broadcast(Events.ChargedModeOffEvent);
        }
    }
}
