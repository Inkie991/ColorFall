using System.Collections.Generic;
using System.Linq;
using ColorFall.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ColorFall.Game
{
    public class UIManager : MonoBehaviour, IGameManager
    {
        [SerializeField] private GameObject uiContainer;
        public ManagerStatus Status { get; private set; }

        private Dictionary<string, Canvas> _canvasMap;
        
        public void Startup()
        {
            DontDestroyOnLoad(uiContainer.gameObject);
            CollectCanvas();
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventManager.AddListener<PlayerWinEvent>(_ => ToggleWin(true));
            EventManager.AddListener<PlayerLoseEvent>(_ => ToggleLose(true));
            EventManager.AddListener<GameStartedEvent>(_ => ToggleSettings(false));
            EventManager.AddListener<SecondChanceEvent>(_ => ToggleLose(false));
            EventManager.AddListener<CannonEvent>(_ => ToggleTapBar(true));
            EventManager.AddListener<BoostEvent>(_ => this.Invoke(() => ToggleTapBar(false), 1f));
            Status = ManagerStatus.Started;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "_Preloader") return;

            ToggleSettings(true);
            ToggleScore(true);
            ToggleWin(false);
            ToggleLose(false);
            ToggleTapBar(false);
        }

        void CollectCanvas()
        {
            _canvasMap = new Dictionary<string, Canvas>();
            List<Canvas> canvasArray = uiContainer.transform.GetComponentsInChildren<Canvas>().ToList();
            canvasArray.ForEach(canvas => _canvasMap.Add(canvas.name, canvas));
        }

        private void ToggleScore(bool value)
        {
            _canvasMap["Score"].gameObject.SetActive(value);
        }

        private void ToggleSettings(bool value)
        {
            _canvasMap["Settings"].gameObject.SetActive(value);
        }

        private void ToggleWin(bool value)
        {
            _canvasMap["Win"].gameObject.SetActive(value);
        }

        private void ToggleLose(bool value)
        {
            _canvasMap["Lose"].gameObject.SetActive(value);
        }

        private void ToggleTapBar(bool value)
        {
            _canvasMap["Tap Bar"].gameObject.SetActive(value);
        }

        public void OnRestart()
        {
            Managers.Loader.Reload();
        }

        public void OnContinue()
        {
            Managers.Loader.NextLevel();
        }

        public void OnMusicClick()
        {
            Managers.Settings.OnMusicMute();
            Managers.Audio.PlaySound(Sound.ButtonIn);
        }

        public void OnSoundClick()
        {
            Managers.Settings.OnSoundMute();
            Managers.Audio.PlaySound(Sound.ButtonIn);
        }

        public void OnVibrationClick()
        {
            Managers.Settings.OnVibrationDisable();
            Managers.Audio.PlaySound(Sound.ButtonIn);
        }
    }
}
