using ColorFall.Core;
using UnityEngine;

namespace ColorFall.Game
{
    public class SettingsManager : MonoBehaviour, IGameManager
    {
        public ManagerStatus Status { get; private set; }

        private bool _disableSound;
        public bool DisableSound
        {
            get => _disableSound;
            set
            {
                _disableSound = value;
                Managers.Audio.MuteSounds(value);
            }
        }

        private bool _disableMusic;
        public bool DisableMusic
        {
            get => _disableMusic;
            set
            {
                _disableMusic = value;
                Managers.Audio.MuteMusic(value);
                Managers.Saves.Save();
            }
        }

        private bool _disableVibration;

        public bool DisableVibration
        {
            get => _disableVibration;
            set
            {
                _disableVibration = value;
                Managers.Saves.Save();
            }
        }

        public void Startup()
        {
            Debug.Log("Settings manager starting...");

            Status = ManagerStatus.Started;
        }

        public void OnMusicMute()
        {
            DisableMusic = !DisableMusic;
        }

        public void OnSoundMute()
        {
            DisableSound = !DisableSound;
        }

        public void OnVibrationDisable()
        {
            DisableVibration = !DisableVibration;
        }
    }
}