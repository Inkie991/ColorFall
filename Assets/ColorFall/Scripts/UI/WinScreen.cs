using System;
using UnityEngine;
using ColorFall.Game;
using TMPro;
using DG.Tweening;

namespace ColorFall.UI
{
    public class WinScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelScoreLabel;
        [SerializeField] private TextMeshProUGUI completeLevelLabel;
        [SerializeField] private TextMeshProUGUI bestScoreLabel;
        [SerializeField] private TextMeshProUGUI coinsCollectedLabel;
        [SerializeField] private TextMeshProUGUI bonusXLabel;
        [SerializeField] private GameObject levelProgress;

        private Animator _animator;

        private void Start()
        {
            EventManager.AddListener<PlayerWinEvent>(OnPlayerWin);
            _animator = GetComponent<Animator>();
        }
        
        private void OnPlayerWin(PlayerWinEvent evt) 
        {
            levelScoreLabel.text = Managers.Gameplay.MultipliedLevelScore.ToString();
            completeLevelLabel.text = $"LEVEL {Managers.Gameplay.Level}";
            bestScoreLabel.text = $"best score: {Managers.Saves.BestScore}";
            coinsCollectedLabel.text = Managers.Money.LevelMoneyCount.ToString();
            bonusXLabel.text = $"x{evt.multiplier}";
            _animator.SetTrigger("Start");
            
            //TODO: levelProgress...
        }

        public void OnContinue()
        {
            Managers.Loader.NextLevel();
        }
        
    }
}
