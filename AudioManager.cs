using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

namespace YoukaiFox.Audio
{
    public class AudioManager : MonoBehaviour
    {
        #region Static fields

        public static AudioManager Instance;

        #endregion

        #region Actions
        #endregion

        #region Serialized fields

        // [SerializeField]
        // [Range(0f, 1f)]
        // private float _masterVolume = 1f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _bgmVolume = 1f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _sfxVolume = 1f;

        [SerializeField]
        private bool _isMuted = false;

        [SerializeField]
        private float _crossfadeDuration = 3f;

        [SerializeField]
        private bool _autoSaveToPlayerPrefs = true;

        [SerializeField]
        private bool _autoLoadFromPlayerPrefs = true;

        #endregion

        #region Non-serialized fields

        private bool _isCrossfading;

        private bool _isFading;

        private List<AudioSource> _bgmPlayers;

        private AudioSource _currentBgmPlayer;

        private List<AudioSource> _sfxPlayers;

        private AudioClip _currentBgm;

        private Transform _transform;

        #endregion

        #region Constant fields

        private const int BgmPlayersCount = 2;

        private const int SfxPlayersCount = 5;

        #endregion

        #region Properties

        private bool IsBusy => _isFading || _isCrossfading;

        #endregion

        #region Custom structures

        public struct AudioSettings
        {
            public float BgmVolume;

            public float SfxVolume;

            public bool IsMuted;
        }

        #endregion

        #region Unity events

        private void Awake()
        {
            _transform = transform;
            SetUpSingleton();
            SetupPlayers();
        }

        private void Start()
        {
            LoadSettings();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Play an audio clip as a track for background music.
        /// </summary>
        /// <param name="clip">Target track.</param>
        /// <param name="crossFade">Should the player fade out the current track as it fades the desired one? </param>
        /// <param name="forceRestart">If target track is already playing should it be restarted?</param>
        public void PlayBgm(AudioClip clip, bool crossFade = true, bool forceRestart = false)
        {
            if (clip == null)
            {
                return;
            }

            if ((clip == _currentBgm) && (!forceRestart))
            {
                return;
            }

            _currentBgm = clip;

            if ((_isMuted) || (IsBusy))
            {
                return;
            }

            if ((!_currentBgmPlayer.isPlaying) || (crossFade = false))
            {
                _currentBgmPlayer.clip = clip;
                _currentBgmPlayer.volume = _bgmVolume;
                _currentBgmPlayer.Play();
                return;
            }

            var nextBgmPlayer = GetAvailableBgmPlayer();
            Crossfade(_currentBgmPlayer, nextBgmPlayer);
        }

        /// <summary>
        /// Pause or resume the current background music track.
        /// </summary>
        public void ToggleBgm()
        {
            if ((_isMuted) || (_currentBgm == null))
            {
                return;
            }

            if (_currentBgmPlayer.isPlaying)
            {
                _currentBgmPlayer.Pause();
            }
            else
            {
                _currentBgmPlayer.UnPause();
            }
        }

        public void StopBgm(bool fadeOut = true, float fadeOutDuration = 3f)
        {
            if (!_currentBgmPlayer.isPlaying)
            {
                return;
            }

            if (!fadeOut)
            {
                _currentBgmPlayer.Stop();
                return;
            }

            FadeOutBgm(fadeOutDuration);
        }

        /// <summary>
        /// Plays an AudioClip.
        /// </summary>
        /// <param name="clip">AudioClip to be played.</param>
        /// <param name="loop">Loops sound effect.</param>
        public void PlaySfx(AudioClip clip, bool loop = false)
        {
            if ((_isMuted) || (clip == null))
            {
                return;
            }

            var player = GetAvailableSfxPlayer();

            if (!loop)
            {
                player.PlayOneShot(clip, _sfxVolume);
            }
            else
            {
                player.loop = loop;
                player.clip = clip;
                player.volume = _sfxVolume;
                player.Play();
            }
        }

        public void StopAllSfx()
        {
            foreach (var player in _sfxPlayers)
            {
                if (player.isPlaying)
                {
                    player.Stop();
                }
            }
        }

        /// <summary>
        /// Update audio settings with the values received.
        /// </summary>
        /// <param name="settings">New values for the settings.</param>
        public void UpdateSettings(AudioSettings settings)
        {
            _bgmVolume = settings.BgmVolume;
            _sfxVolume = settings.SfxVolume;

            if ((settings.IsMuted) && (!_isMuted))
            {
                SetMute(true);
            }
            else if ((!settings.IsMuted) && (!_isMuted))
            {
                
                SetMute(false);
            }

            _isMuted = settings.IsMuted;
            UpdateVolume();
            SaveSettings();
        }

        /// <summary>
        /// Get a copy of the settings being currently used.
        /// </summary>
        public AudioSettings GetCurrentSettings()
        {
            var settings = new AudioSettings
            {
                BgmVolume = _bgmVolume,
                SfxVolume = _sfxVolume,
                IsMuted = _isMuted
            };

            return settings;
        }

        #endregion

        #region Protected methods
        #endregion

        #region Private methods

        private void SetMute(bool mute)
        {
            if (!_isMuted)
            {
                return;
            }

            foreach (var player in _bgmPlayers)
            {
                player.mute = mute;
            }

            if (mute)
            {
                foreach (var player in _sfxPlayers)
                {
                    if (player.isPlaying)
                    {
                        player.Stop();
                    }
                }
            }
        }

        private void UpdateVolume()
        {
            if (!_isMuted)
            {
                return;
            }

            if (_currentBgmPlayer.isPlaying)
            {
                _currentBgmPlayer.volume = _bgmVolume;
            }

            foreach (var player in _sfxPlayers)
            {
                if (player.isPlaying)
                {
                    player.volume = _sfxVolume;
                }
            }
        }

        private void Crossfade(AudioSource previous, AudioSource current)
        {
            if (_isCrossfading)
            {
                return;
            }

            _isCrossfading = true;
            current.volume = 0f;
            previous.DOFade(0f, _crossfadeDuration).OnComplete(() => current.DOFade(_bgmVolume, _crossfadeDuration).OnComplete(() => _isCrossfading = false));
        }

        private AudioSource GetAvailableBgmPlayer()
        {
            foreach (var player in _bgmPlayers)
            {
                if (!player.isPlaying)
                {
                    return player;
                }
            }

            var newSourceIndex = _bgmPlayers.Count;
            var newSource = InstantiateAudioSource($"BGM Player {newSourceIndex}");
            newSource.loop = true;
            _bgmPlayers.Add(newSource);
            return newSource;
        }

        private AudioSource GetAvailableSfxPlayer()
        {
            foreach (var player in _sfxPlayers)
            {
                if (!player.isPlaying)
                {
                    return player;
                }
            }

            var newSourceIndex = _sfxPlayers.Count;
            var newSource = InstantiateAudioSource($"BGM Player {newSourceIndex}");
            newSource.loop = false;
            _sfxPlayers.Add(newSource);
            return newSource;
        }

        private void FadeOutBgm(float fadeOutDuration)
        {
            _isFading = true;
            _currentBgmPlayer.DOFade(0f, fadeOutDuration).OnComplete(FinishFadingOut);
        }

        private void FinishFadingOut()
        {
            _isFading = false;
            _currentBgmPlayer.Stop();
        }

        #region Setup operations

        private void SetUpSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private void SetupPlayers()
        {
            _bgmPlayers = new List<AudioSource>();

            for (int i = 0; i < BgmPlayersCount; i++)
            {
                var bgmPlayer = InstantiateAudioSource($"BGM Player {i}");
                bgmPlayer.playOnAwake = false;
                bgmPlayer.loop = true;
                _bgmPlayers.Add(bgmPlayer);
            }

            _currentBgmPlayer = _bgmPlayers.First();
            _sfxPlayers = new List<AudioSource>();

            for (int i = 0; i < SfxPlayersCount; i++)
            {
                var sfxPlayer = InstantiateAudioSource($"SFX Player {i}");
                sfxPlayer.playOnAwake = false;
                sfxPlayer.loop = false;
                _sfxPlayers.Add(sfxPlayer);
            }
        }

        private AudioSource InstantiateAudioSource(string name)
        {
            var newGameObject = Instantiate(new GameObject(name), _transform, false);
            var newAudioSource = newGameObject.AddComponent<AudioSource>();
            return newAudioSource;
        }

        #endregion

        #region Saving Management

        private void SaveSettings()
        {
            if (!_autoSaveToPlayerPrefs)
            {
                return;
            }

            var soundSettings = GetCurrentSettings();
            PlayerPrefs.SetInt("is_sound_muted", soundSettings.IsMuted ? 1 : 0);
            PlayerPrefs.SetFloat("sfx_volume", soundSettings.SfxVolume);
            PlayerPrefs.SetFloat("bgm_volume", soundSettings.BgmVolume);
        }

        private void LoadSettings()
        {
            if (!_autoLoadFromPlayerPrefs)
            {
                return;
            }

            _isMuted = PlayerPrefs.GetInt("is_sound_muted", 0) == 1;
            _sfxVolume = PlayerPrefs.GetFloat("sfx_volume", 1f);
            _bgmVolume = PlayerPrefs.GetFloat("bgm_volume", 1f);

            SetMute(_isMuted);
            UpdateVolume();
        }

        #endregion

        #endregion 
    }
}
