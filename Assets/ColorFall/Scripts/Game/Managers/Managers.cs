using System.Collections;
using System.Collections.Generic;
using ColorFall.Core;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace ColorFall.Game
{
    [RequireComponent(typeof(AudioManager))]
    [RequireComponent(typeof(GameplayManager))]
    [RequireComponent(typeof(LoaderManager))]
    [RequireComponent(typeof(GeneratorManager))]
    [RequireComponent(typeof(SettingsManager))]
    [RequireComponent(typeof(SavesManager))]
    [RequireComponent(typeof(UIManager))]
    [RequireComponent(typeof(VibrationManager))]
    [RequireComponent(typeof(EnergyManager))]
    [RequireComponent(typeof(MoneyManager))]
    public class Managers : MonoBehaviour
    {
        [SerializeField] private Image preloaderBar;
        public static AudioManager Audio { get; private set; }
        public static GameplayManager Gameplay { get; private set; }
        public static LoaderManager Loader { get; private set; }
        private static GeneratorManager Generator { get; set; }
        public static SavesManager Saves { get; private set; }
        public static SettingsManager Settings { get; private set; }
        private static UIManager UI { get; set; }
        public static VibrationManager Vibration { get; private set; }
        public static EnergyManager Energy { get; private set; }
        public static MoneyManager Money { get; private set; }

        private List<IGameManager> _startSequence;

        void Awake()
        {
            DOTween.Init();
            Application.targetFrameRate = 60;
            preloaderBar.fillAmount = 0f;
            Audio = GetComponent<AudioManager>();
            Gameplay = GetComponent<GameplayManager>();
            Loader = GetComponent<LoaderManager>();
            Generator = GetComponent<GeneratorManager>();
            Saves = GetComponent<SavesManager>();
            Settings = GetComponent<SettingsManager>();
            UI = GetComponent<UIManager>();
            Vibration = GetComponent<VibrationManager>();
            Energy = GetComponent<EnergyManager>();
            Money = GetComponent<MoneyManager>();

            _startSequence = new List<IGameManager>
            {
                Audio,
                Gameplay,
                Loader,
                Generator,
                Saves,
                Settings,
                UI,
                Vibration,
                Energy,
                Money
            };

            DontDestroyOnLoad(gameObject);
            StartCoroutine(StartupManagers());
        }

        private IEnumerator<Object> StartupManagers()
        {
            foreach (IGameManager manager in _startSequence)
            {
                manager.Startup();
            }

            yield return null;

            int numModules = _startSequence.Count;
            int numReady = 0;

            while (numReady < numModules)
            {
                int lastReady = numReady;
                numReady = 0;

                foreach (IGameManager manager in _startSequence)
                {
                    if (manager.Status == ManagerStatus.Started)
                    {
                        numReady++;
                    }
                }

                if (numReady > lastReady)
                    Debug.Log("Progress: " + numReady + "/" + numModules);
                yield return null;
            }

            Debug.Log("All managers started up");
            StartCoroutine(PreloaderAnimation());
        }

        private IEnumerator PreloaderAnimation()
        {
            while (preloaderBar.fillAmount < 1f)
            {
                preloaderBar.fillAmount += 0.01f;
                yield return new WaitForSecondsRealtime(0.01f);
            }

            Loader.StartGame();
        }
    }
}