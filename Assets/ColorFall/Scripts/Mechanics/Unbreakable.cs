using ColorFall.Game;
using UnityEngine;

namespace ColorFall.Mechanics
{
    public abstract class Unbreakable : MonoBehaviour
    {
        protected virtual void Awake()
        {
            EventManager.AddListener<ChargedModeOnEvent>(ChargedModeOn);
            EventManager.AddListener<ChargedModeOffEvent>(ChargedModeOff);
        }

        protected virtual void Start()
        {
            gameObject.tag = "Unbreakable";
        }

        protected virtual void OnDestroy()
        {
            EventManager.RemoveListener<ChargedModeOnEvent>(ChargedModeOn);
            EventManager.RemoveListener<ChargedModeOffEvent>(ChargedModeOff);
        }

        protected virtual void ChargedModeOn(ChargedModeOnEvent evt)
        {
            ShowUnbreakable(false);
        }
        
        protected virtual void ChargedModeOff(ChargedModeOffEvent evt)
        {
            ShowUnbreakable(true);
        }

        private void ShowUnbreakable(bool show)
        {
            gameObject.SetActive(show);
        }
    }
}
