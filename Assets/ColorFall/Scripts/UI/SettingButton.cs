using UnityEngine;
using UnityEngine.UI;

namespace ColorFall.UI
{
    public class SettingButton : MonoBehaviour
    {
        [SerializeField] private Sprite onIcon;
        [SerializeField] private Sprite offIcon;
        private Image _image;

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                _image.sprite = value ? offIcon : onIcon;
            }
        }
        
        private void Awake()
        {
            _image = GetComponent<Image>();
        }
    }
}