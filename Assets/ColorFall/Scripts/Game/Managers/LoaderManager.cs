using ColorFall.Core;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ColorFall.Game
{
    public class LoaderManager : MonoBehaviour, IGameManager
    {
        [SerializeField] private bool isTesting = true;
        [SerializeField] private string levelName = "Level 1";
        public ManagerStatus Status { get; private set; }

        public void Startup()
        {
            Debug.Log("Loader manager starting...");

            Status = ManagerStatus.Started;
        }

        public void StartGame()
        {
            if (isTesting) LoadTestingScene();
            else LoadScene();
        }

        void LoadTestingScene()
        {
            SceneManager.LoadScene(levelName);
        }

        void LoadScene()
        {
            KillTweens();
            string path = $"Assets/ColorFall/Scenes/Level {Managers.Gameplay.Level}.unity";
            int index = SceneUtility.GetBuildIndexByScenePath(path);

            if (index == -1)
                LoadTestingScene();
            else
                SceneManager.LoadScene(index);
        }

        public void Reload()
        {
            KillTweens();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        public void NextLevel()
        {
            Managers.Gameplay.Level++;
            Managers.Saves.Save();

            if (isTesting) Reload();
            else LoadScene();
        }

        void KillTweens()
        {
            DOTween.KillAll();
        }
    }
}
