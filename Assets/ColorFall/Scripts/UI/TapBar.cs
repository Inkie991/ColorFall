using System.Collections;
using System.Collections.Generic;
using ColorFall.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ColorFall.Core;

namespace ColorFall.UI
{
    public class TapBar : MonoBehaviour
    {
        [SerializeField] private GameObject tapBar;
        [SerializeField] private Image fillArea; 
        [SerializeField] private List<TextMeshProUGUI> texts;
        private bool _isTapStarting;
        private int _touchesCount;
        private bool _subtrackStart;

        private void Awake()
        {
            _isTapStarting = false;
            _subtrackStart = false;
        }

        private void Start()
        {
            EventManager.AddListener<CannonEvent>(OnCannon);

            Preparation();
        }
        
        private void Preparation()
        {
            fillArea.fillAmount = 0.3f;
            _touchesCount = 0;
            foreach (var text in texts)
            {
                text.gameObject.SetActive(false);
            }
        }

        void Update()
        {
            if (_isTapStarting) HandleInput();
            if (_subtrackStart) ToEmptyBar();
        }
        
        void HandleInput()
        {
            if (!_subtrackStart) _subtrackStart = true;
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                _touchesCount++;
                if (touch.phase == TouchPhase.Began)
                {
                    if (fillArea.fillAmount > 0.7)
                    {
                        fillArea.fillAmount = 1;
                        ToEmptyBar();
                    }
                    else
                    {
                        fillArea.fillAmount += 0.3f;
                        ToEmptyBar();
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                _touchesCount++;
                if (fillArea.fillAmount > 0.7)
                {
                    fillArea.fillAmount = 1;
                    ToEmptyBar();
                }
                else
                {
                    fillArea.fillAmount += 0.3f;
                    ToEmptyBar();
                }
            }
        }
        
        private float GetPowerAndMessage()
        {
            if (fillArea.fillAmount <= 0.2f)
            {
                ShowText(0);
                return 0.5f;
            }
            if (fillArea.fillAmount > 0.2f && fillArea.fillAmount <= 0.4f)
            {
                ShowText(1);
                return 1f;
            }
            if (fillArea.fillAmount > 0.4f && fillArea.fillAmount <= 0.6f)
            {
                ShowText(2);
                return 1.5f;
            }
            if (fillArea.fillAmount > 0.6f && fillArea.fillAmount <= 0.8f)
            {
                ShowText(3);
                return 2.25f;
            }
            if (fillArea.fillAmount > 0.8f && fillArea.fillAmount <= 1f)
            {
                ShowText(4);
                return 3f;
            }

            return 0.5f;
        }

        private void ShowText(int id)
        {
            texts[id].gameObject.SetActive(true);
            texts[id].gameObject.GetComponent<Animator>().Play("TapBarText");
        }
        
        private void OnCannon(CannonEvent evt)
        {
            Preparation();
            tapBar.SetActive(true);
            _isTapStarting = true;
            this.Invoke(() =>
            {
                StartCoroutine(WaitToBoost());
            }, 0.2f);
            this.Invoke(() =>
            {
                if (_touchesCount == 0)
                {
                    _subtrackStart = true;
                    ToEmptyBar();
                }
            }, 0.5f);
        }

        private void SetBoostPower()
        {
            _subtrackStart = false;
            tapBar.SetActive(false);
            float barValue = GetPowerAndMessage();
            SetBoostPowerEvent evt = new() { power = barValue };
            EventManager.Broadcast(evt);
        }

        private void ToEmptyBar()
        {
            fillArea.fillAmount -= Time.deltaTime * 1.5f;
        }

        private IEnumerator WaitToBoost()
        {
            yield return new WaitForSeconds(GameplayManager.BoostTime);
            SetBoostPower();
            yield return null;
        }
    }
}
