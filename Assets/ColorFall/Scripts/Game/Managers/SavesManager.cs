using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ColorFall.Core;
using UnityEngine;

namespace ColorFall.Game
{
    public class SavesManager : MonoBehaviour, IGameManager
    {
        public static decimal MaxMultiplier => 100;
        private int TotalScore { get; set; }
        public float BestScore { get; private set; }
        public ManagerStatus Status { get; private set; }

        private string _filename;

        public void Startup()
        {
            Debug.Log("Saves manager starting...");

            _filename = Path.Combine(Application.persistentDataPath, "save");
            Load();

            Status = ManagerStatus.Started;
        }

        private void OnDestroy()
        {
            Save();
        }

        public void Load()
        {
            if (!File.Exists(_filename))
            {
                Debug.Log("No saved game");
                return;
            }

            Dictionary<string, object> gamestate;
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = File.Open(_filename, FileMode.Open);
            gamestate = formatter.Deserialize(stream) as Dictionary<string, object>;
            stream.Close();

            // Managers.Gameplay.Level = (int) (gamestate.TryGetValue("level", out var level) ? level : 1);
            Managers.Settings.DisableMusic =
                (bool) (gamestate.TryGetValue("disableMusic", out var disableMusic) ? disableMusic : false);
            Managers.Settings.DisableSound =
                (bool) (gamestate.TryGetValue("disableSound", out var disableSound) ? disableSound : false);
            Managers.Settings.DisableVibration =
                (bool) (gamestate.TryGetValue("disableVibration", out var disableVibration) ? disableVibration : false);
            string tempBestScore = 
                (string) (gamestate.TryGetValue("bestScore", out var bestScore) ? bestScore : "0");
            BestScore = Int32.Parse(tempBestScore);
            string tempTotalScore = 
                (string) (gamestate.TryGetValue("totalScore", out var totalScore) ? totalScore : "0");
            TotalScore = Int32.Parse(tempTotalScore);
            string totalMoneyCount = 
                (string) (gamestate.TryGetValue("totalMoney", out var totalMoney) ? totalMoney : "0");
            Managers.Money.LoadTotalMoney(Int32.Parse(totalMoneyCount));
        }

        public void Save()
        {
            Dictionary<string, object> gameState = new Dictionary<string, object>
            {
                { "level", Managers.Gameplay.Level },
                { "disableSound", Managers.Settings.DisableSound },
                { "disableMusic", Managers.Settings.DisableMusic },
                { "disableVibration", Managers.Settings.DisableVibration },
                { "bestScore", BestScore.ToString() },
                { "totalScore", TotalScore.ToString() },
                { "totalMoney", Managers.Money.TotalMoneyCount.ToString() }
            };

            FileStream stream = File.Create(_filename);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, gameState);
            stream.Close();
        }

        public void CheckBestScore(int score)
        {
            if (score > BestScore) BestScore = score;
        }
        
        public void UpdateTotalScore(int score)
        {
            TotalScore += score;
        }
    }
}