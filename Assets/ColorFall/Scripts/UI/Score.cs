using System;
using System.Collections;
using ColorFall.Core;
using ColorFall.Game;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ColorFall.UI
{
    public class Score : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreLabel;
        [SerializeField] private TextMeshProUGUI currentLevel;
        [SerializeField] private TextMeshProUGUI nextLevel;
        [SerializeField] private Slider progressBar;
        [SerializeField] private GameObject popupScorePrefab;
        [SerializeField] private TextMeshProUGUI moneyLabel;
        [SerializeField] private GameObject moneyPopupPrefab;
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private GameObject redIndicator;
        [SerializeField] private RectTransform chargeBar;
        [SerializeField] private Image chargeBarFillArea;


        private static readonly int ScoreHash = Animator.StringToHash("Score");
        
        private RectTransform _rect;
        private Coroutine _redIndicatorAnimationCoroutine;

        private void Awake()
        {
            ShowEnergyBar(false, 0.05f);
        }

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            EventManager.AddListener<CollectDropEvent>(OnCollectDrop);
            EventManager.AddListener<CollectMoneyEvent>(OnCollectMoney);
            EventManager.AddListener<ComboEvent>(ShowComboMessage);
            EventManager.AddListener<PlayerWinEvent>(OnPlayerWin);
            EventManager.AddListener<PlayerFinishEvent>(OnFinish);
            EventManager.AddListener<ChargedModeOffEvent>(OnChargedModeOff);
            _rect = GetComponent<RectTransform>();
            comboText.gameObject.SetActive(false);
            redIndicator.gameObject.SetActive(false);
        }

        private void Update()
        {
            progressBar.value = Managers.Gameplay.Progress;
        }

        private void OnFinish(PlayerFinishEvent evt)
        {
            ShowEnergyBar(false, 0.5f);
        }

        private void OnPlayerWin(PlayerWinEvent evt)
        {
            Invoke("MultiplyScore", 1f);
        }
        
        private void MultiplyScore()
        {
            int score = Managers.Gameplay.LevelScore;
            DOTween.To(() => score, value =>
                {
                    score = value;
                    scoreLabel.text = score.ToString();
                }, 
                Managers.Gameplay.MultipliedLevelScore , 0.5f);
            this.Invoke(() => gameObject.SetActive(false), 1f);
        }

        private void OnCollectMoney(CollectMoneyEvent evt)
        {
            GameObject moneyPopup = Instantiate(moneyPopupPrefab, _rect);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _rect,evt.screenPos, null, out var spawnPos);
            spawnPos.x += _rect.sizeDelta.x/2;
            moneyPopup.GetComponent<RectTransform>().anchoredPosition = spawnPos;
            moneyLabel.text = Managers.Money.TotalMoneyCount.ToString();
        }

        private void OnCollectDrop(CollectDropEvent evt)
        {
            if (chargeBarFillArea.fillAmount >= 0.2f)
            {
                ShowEnergyBar(true, 0.5f);
            }
            if (evt.collectedDrop == CollectedDrop.Correct)
            {
                StartScoreAnim();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _rect,evt.screenPos, null, out var spawnPos);
                GameObject scorePopup = Instantiate(popupScorePrefab, _rect);
                scorePopup.GetComponent<RectTransform>().anchoredPosition = spawnPos;
            }

            if (evt.collectedDrop == CollectedDrop.Incorrect)
            {
                if (_redIndicatorAnimationCoroutine != null) StopCoroutine(_redIndicatorAnimationCoroutine);
                _redIndicatorAnimationCoroutine = StartCoroutine(RedIndicatorAnimation());
            }
        }

        private void ShowComboMessage(ComboEvent evt)
        {
            StartCoroutine(OnCombo(evt.messageModifier));
        }

        private IEnumerator OnCombo(int modifier)
        {
            comboText.text = GetMessage(modifier);
            comboText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            comboText.gameObject.SetActive(false);
        }

        private string GetMessage(int modifier)
        {
            if (modifier <= 25) return "Valeron!";
            if (modifier > 25 && modifier <= 50) return "Try better!";
            if (modifier > 50 && modifier <= 75) return "Good job!";
            if (modifier > 75 && modifier <= 99) return "Awesome!";
            if (modifier == 100) return "Perfect!";
            if (modifier > 100) return "Incredible!";
            return "Error..........";
        }

        private IEnumerator RedIndicatorAnimation()
        {
            redIndicator.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            redIndicator.gameObject.SetActive(false);
        }

        private void StartScoreAnim()
        {
            scoreLabel.GetComponent<Animator>().SetTrigger(ScoreHash);
            scoreLabel.text = Managers.Gameplay.LevelScore.ToString();
        }

        private void ShowEnergyBar(bool show, float duration)
        {
            if (show) chargeBar.DOAnchorPos(new Vector2(64, 0), duration);
            else chargeBar.DOAnchorPos(new Vector2(-200, 0), duration);
        }

        private void OnChargedModeOff(ChargedModeOffEvent evt)
        {
            ShowEnergyBar(false, 0.5f);
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "_Preloader") return;

            ShowEnergyBar(false, 0.05f);
            currentLevel.text = Managers.Gameplay.Level.ToString();
            nextLevel.text = (Managers.Gameplay.Level + 1).ToString();
            scoreLabel.text = Managers.Gameplay.LevelScore.ToString();
            progressBar.value = Managers.Gameplay.Progress;
            moneyLabel.text = Managers.Money.TotalMoneyCount.ToString();
            
        }
    }
}
