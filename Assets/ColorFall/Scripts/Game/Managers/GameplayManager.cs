using System;
using ColorFall.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ColorFall.Game
{
    public class GameplayManager : MonoBehaviour, IGameManager
    {
        private const float Gravity = 8f;
        public const float BoostTime = 3f;
        
        private float _firstPlatformY;
        private float _levelLength;
        private float _finishPositionY;
        private float _endSpeed;
        
        public ManagerStatus Status { get; private set; }
        public int Level { get; set; }
        public int Progress { get; private set; }
        public bool GameOver { get; private set; }
        public int LevelScore { get; private set; }
        public int MultipliedLevelScore { get; private set; }
        public int CollectedDropsCount { get; private set; } // Now it the same as score, but it future we'll multiply score
        public int IncorrectDropsCount { get; private set; }

        private int _combo;
        private int Combo
        {
            get => _combo;
            set
            {
                _combo = value;
                Managers.Audio.ApplyPitch(value);
                CancelInvoke("ResetCombo");
                if (value == 0) return;
                Invoke("ResetCombo", ComboDuration);
            }
        }
        private void ResetCombo() => Combo = 0;

        private const float ComboDuration = 0.1f;
        
        public bool HasSecondChance { get; private set; }

        public void Startup()
        {
            Debug.Log("Gameplay manager starting...");

            Physics.gravity = Vector3.down * Gravity;
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventManager.AddListener<CollectDropEvent>(OnCollectDrop);
            EventManager.AddListener<SecondChanceEvent>(OnSecondChance);
            EventManager.AddListener<PlayerFinishEvent>(OnFinish);
            EventManager.AddListener<CannonEvent>(OnCannon);
            EventManager.AddListener<PlayerWinEvent>(OnWin);
            EventManager.AddListener<SetBoostPowerEvent>(SetBoostPower);

            Status = ManagerStatus.Started;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "_Preloader") return;

            Level = SceneManager.GetActiveScene().buildIndex;
            LevelScore = 0;
            MultipliedLevelScore = 0;
            Progress = 0;
            GameOver = false;
            HasSecondChance = true;
            CollectedDropsCount = 0;
            IncorrectDropsCount = 0;
            Combo = 0;
            Physics.gravity = Vector3.down * Gravity;
            CalculateLevelLength();
        }

        private void CalculateLevelLength()
        {
            _firstPlatformY = GameObject.FindGameObjectWithTag("FirstPlatform").transform.position.y;
            _finishPositionY = GameObject.FindGameObjectWithTag("FinishPlatform").transform.position.y;
            _levelLength = (_firstPlatformY - _finishPositionY) * -1;
        }

        public void CalculateLevelProgress(float posY)
        {
            Progress = Mathf.RoundToInt((_firstPlatformY - posY) / _levelLength * -100);
            if (Progress < 0) Progress = 0;
        }

        private void OnCollectDrop(CollectDropEvent evt)
        {
            if (evt.collectedDrop == CollectedDrop.Correct)
            {
                LevelScore++;
                Combo++;
                CollectedDropsCount++;
            }
            else
            {
                Combo = 0;
                IncorrectDropsCount++;
                Managers.Vibration.Vibrate();
            }
        }

        private void OnWin(PlayerWinEvent evt)
        {
            GameOver = true;
            Progress = 100;
            Combo = 0;

            MultipliedLevelScore = Mathf.RoundToInt(LevelScore * evt.multiplier);
            Managers.Saves.CheckBestScore(MultipliedLevelScore);
            Managers.Saves.UpdateTotalScore(MultipliedLevelScore);
        }

        private void OnSecondChance(SecondChanceEvent evt)
        {
            HasSecondChance = false;
            Combo = 0;
        }

        private void OnFinish(PlayerFinishEvent evt)
        {
            _endSpeed = evt.endSpeed;
        }

        private void OnCannon(CannonEvent cannonEvent)
        {
            Physics.gravity = Vector3.zero;
        }

        private void SetBoostPower(SetBoostPowerEvent evt)
        {
            BoostEvent boostEvent = new(){ power = _endSpeed * 5 * evt.power };
            EventManager.Broadcast(boostEvent);
            Physics.gravity = Vector3.down * Gravity;
        }
    }
}