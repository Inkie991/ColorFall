using ColorFall.Core;
using UnityEngine;

namespace ColorFall.Game
{
    public class VibrationManager : MonoBehaviour, IGameManager
    {
        public ManagerStatus Status { get; private set; }

        public void Startup()
        {
            Debug.Log("Vibration manager starting...");
            
            EventManager.AddListener<CollectDropEvent>(OnCollectDrop);
            
            #if UNITY_IOS || UNITY_ANDROID
                Vibration.Init();
            #endif

            Status = ManagerStatus.Started;
        }

        private void OnCollectDrop(CollectDropEvent evt)
        {
            if (evt.collectedDrop == CollectedDrop.Incorrect) Vibrate();
        }

        public void Vibrate()
        {
            if (Managers.Settings.DisableVibration) return;
            
            #if UNITY_IOS || UNITY_ANDROID
                Vibration.Vibrate();
            #endif
        }
    }
}
