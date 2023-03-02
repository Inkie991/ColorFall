using System.Collections.Generic;
using ColorFall.Core;
using UnityEngine;

namespace ColorFall.Game
{
    public class AudioManager : MonoBehaviour, IGameManager
    {
        [SerializeField] private AudioClip mainMusic;
        [SerializeField] private List<AudioClip> bounceClips;
        [SerializeField] private List<AudioClip> winClips;
        [SerializeField] private List<AudioClip> loseClips;
        [SerializeField] private List<AudioClip> dropCollectClips;
        [SerializeField] private List<AudioClip> changeColorClips;
        [SerializeField] private List<AudioClip> buttonInClips;
        [SerializeField] private List<AudioClip> buttonOutClips;
        [SerializeField] private List<AudioClip> coinCollectClips;
        [SerializeField] private List<AudioClip> wrongColorClips;
        
        private AudioSource _musicSource;
        private AudioSource _soundSource;
        private AudioSource _dropCollectSoundSource;
        private Dictionary<Sound, List<AudioClip>> _soundsMap;
        private readonly Dictionary<Sound, int> _ordersMap = new();
        
        public ManagerStatus Status { get; private set; }
        
        public void Startup()
        {
            Debug.Log("Audio manager starting...");
            _soundSource = GameObject.Find("sound").GetComponent<AudioSource>();
            _musicSource = GameObject.Find("music").GetComponent<AudioSource>();
            _dropCollectSoundSource = GameObject.Find("drop").GetComponent<AudioSource>();
            _musicSource.clip = mainMusic;
            _musicSource.loop = true;
            _musicSource.Play();
            _soundsMap = new Dictionary<Sound, List<AudioClip>>()
            {
                { Sound.Bounce, bounceClips },
                { Sound.Win, winClips },
                { Sound.Lose, loseClips },
                { Sound.ButtonIn, buttonInClips },
                { Sound.ButtonOut, buttonOutClips },
                { Sound.DropCollect, dropCollectClips },
                { Sound.ChangeColor, changeColorClips },
                { Sound.CoinCollect, coinCollectClips },
                { Sound.WrongColor, wrongColorClips }
            };
            Status = ManagerStatus.Started;
        }

        private void Awake()
        {
            // Manager is the global object and initialize once, so we don't need to remove this listeners
            EventManager.AddListener<BouncedOffPlatformEvent>(_ => PlaySound(Sound.Bounce));
            EventManager.AddListener<PlayerWinEvent>(_ => PlaySound(Sound.Win));
            EventManager.AddListener<PlayerLoseEvent>(_ => PlaySound(Sound.Lose));
            EventManager.AddListener<CollectDropEvent>(evt =>
            {
                PlaySound(evt.collectedDrop == CollectedDrop.Correct ? Sound.DropCollect : Sound.WrongColor);
            });
            EventManager.AddListener<CollectMoneyEvent>(_ => PlaySound(Sound.CoinCollect));
        }

        public void PlaySound(Sound sound)
        {
            switch (sound)
            {
                case Sound.Bounce:
                    PlayConsistently(sound);
                    break;
                default:
                    Play(Utils.GetRandomItem(_soundsMap[sound]));
                    break;
            }
        }

        private void Play(AudioClip clip)
        {
            _soundSource.PlayOneShot(clip);
        }

        private void PlayCrashSound(AudioClip clip)
        {
            _dropCollectSoundSource.PlayOneShot(clip);
        }

        private void PlayConsistently(Sound sound)
        {
            List<AudioClip> clips = _soundsMap[sound];

            if (!_ordersMap.ContainsKey(sound))
                _ordersMap[sound] = 0;
            else if (_ordersMap[sound] == clips.Count - 1)
                _ordersMap[sound] = 0;
            else
                _ordersMap[sound]++;

            Play(clips[_ordersMap[sound]]);
        }

        public void MuteSounds(bool shouldMute)
        {
            _soundSource.mute = shouldMute;
            _dropCollectSoundSource.mute = shouldMute;
        }

        public void MuteMusic(bool shouldMute)
        {
            _musicSource.mute = shouldMute;
        }

        public void ApplyPitch(int value)
        {
            _dropCollectSoundSource.pitch = 1f + (value / 20f);
        }
    }
}
