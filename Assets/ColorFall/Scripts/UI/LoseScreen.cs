using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using ColorFall.Game;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ColorFall.UI
{
    public class LoseScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI loseProgressLabel;
        [SerializeField] private Image ringImg;
        [SerializeField] private List<GameObject> _objectsToHide;

        private void Start()
        {
            EventManager.AddListener<PlayerLoseEvent>(_ =>
            {
                loseProgressLabel.text = $"{Managers.Gameplay.Progress.ToString()}%";
                StartCoroutine(AnimateRing());
            });
            SceneManager.sceneLoaded += (_, _) =>
            {
                ringImg.fillAmount = 1f;
                ShowSecondChance(true);
            };
        }

        IEnumerator AnimateRing()
        {
            while (ringImg.fillAmount > 0)
            {
                yield return new WaitForEndOfFrame();
                ringImg.fillAmount -= Time.deltaTime / 5;
            }
            
            OnContinue();
        }

        public void OnSecondChance()
        {
            if (Managers.Gameplay.HasSecondChance)
            {
                EventManager.Broadcast(Events.SecondChanceEvent);
            }
            else
            {
                Managers.Loader.Reload();
            }
            ShowSecondChance(false);
        }

        private void ShowSecondChance(bool show)
        {
            foreach (var obj in _objectsToHide)
            {
                obj.SetActive(show);
            }
        }

        public void OnContinue()
        {
            Managers.Loader.Reload();
        }
    }
}
