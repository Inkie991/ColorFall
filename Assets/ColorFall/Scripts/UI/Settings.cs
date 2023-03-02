using ColorFall.Core;
using UnityEngine;
using ColorFall.Game;
using UnityEngine.SceneManagement;

namespace ColorFall.UI
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] private SettingButton musicButton;
        [SerializeField] private SettingButton soundButton;
        [SerializeField] private SettingButton vibrationButton;
        [SerializeField] private SettingButton settingsButton;
        [SerializeField] private Animator boxAnimator;
        private static readonly int IsOpen = Animator.StringToHash("IsOpen");

        void Start()
        {
            musicButton.IsEnabled = !Managers.Settings.DisableMusic;
            soundButton.IsEnabled = !Managers.Settings.DisableSound;
            vibrationButton.IsEnabled = !Managers.Settings.DisableVibration;
            settingsButton.IsEnabled = false;
            EventManager.AddListener<GameStartedEvent>(_ => boxAnimator.SetBool(IsOpen, false));
            SceneManager.sceneLoaded += (_, _) => settingsButton.IsEnabled = false;;
        }

        public void OnSettingsClick()
        {
            settingsButton.IsEnabled = !settingsButton.IsEnabled;
            boxAnimator.SetBool(IsOpen, settingsButton.IsEnabled);
            Managers.Audio.PlaySound(Sound.ButtonIn);
        }
        
        public void OnMusicClick()
        {
            Managers.Settings.DisableMusic = !Managers.Settings.DisableMusic;
            Managers.Audio.PlaySound(Sound.ButtonIn);
            musicButton.IsEnabled = !Managers.Settings.DisableMusic;
        }

        public void OnSoundClick()
        {
            Managers.Settings.DisableSound = !Managers.Settings.DisableSound;
            Managers.Audio.PlaySound(Sound.ButtonIn);
            soundButton.IsEnabled = !Managers.Settings.DisableSound;
        }

        public void OnVibrationClick()
        {
            Managers.Settings.DisableVibration = !Managers.Settings.DisableVibration;
            Managers.Audio.PlaySound(Sound.ButtonIn);
            vibrationButton.IsEnabled = !Managers.Settings.DisableVibration;
        }

        public void OnRemoveAds()
        {
            Managers.Audio.PlaySound(Sound.ButtonIn);
        }
    }
}
