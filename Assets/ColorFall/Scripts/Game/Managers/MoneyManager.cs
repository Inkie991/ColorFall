using ColorFall.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ColorFall.Game
{
    public class MoneyManager : MonoBehaviour, IGameManager
    {
        public ManagerStatus Status { get; private set; }

        public int TotalMoneyCount { get;private set; }

        public int LevelMoneyCount { get; private set; }

        public void Startup()
        {
            Debug.Log("Money manager starting...");

            LevelMoneyCount = 0;
            
            SceneManager.sceneLoaded += OnSceneLoaded;

            Status = ManagerStatus.Started;
        }

        private void Awake()
        {
            EventManager.AddListener<CollectMoneyEvent>(_ =>
            {
                LevelMoneyCount++;
                TotalMoneyCount++;
            });
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "_Preloader") return;

            LevelMoneyCount = 0;
        }

        public void LoadTotalMoney(int money)
        {
            if (TotalMoneyCount == 0) TotalMoneyCount = money;
        }
    }
}
