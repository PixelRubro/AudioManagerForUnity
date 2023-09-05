using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelSpark.UnityAudioManager
{
    public class AudioManager : MonoBehaviour
    {
        #region Static fields

        public static AudioManager Instance;

        #endregion

        #region Actions
        #endregion

        #region Serialized fields

        [SerializeField]
        [Range(0f, 1f)]
        private float _masterVolume = 1f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _bgmVolume = 1f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _sfxVolume = 1f;

        [SerializeField]
        [Range(0f, 1f)]
        private float _voiceVolume = 1f;

        [SerializeField]
        private bool _isMuted = false;

        [SerializeField]
        private float _fadeDuration = 3f;

        [SerializeField]
        private bool _autoSaveToPlayerPrefs = true;

        [SerializeField]
        private bool _autoLoadFromPlayerPrefs = true;

        #endregion

        #region Non-serialized fields

        private AudioClip _currentBgm;

        private AudioClip _previousBgm;

        private List<AudioSource> _bgmPlayers;

        private AudioSource _currentBgmPlayer;

        private List<AudioSource> _sfxPlayers;

        private List<AudioSource> _voicePlayers;

        private Transform _transform;

        #endregion

        #region Constant fields

        private const int BgmPlayersCount = 2;

        private const int SfxPlayersCount = 5;

        private const int VoicePlayersCount = 2;

        #endregion

        #region Properties

        private float BgmVolume => _bgmVolume > _masterVolume ? _masterVolume : _bgmVolume;

        private float SfxVolume => _sfxVolume > _masterVolume ? _masterVolume : _sfxVolume;

        private float VoiceVolume => _voiceVolume > _masterVolume ? _masterVolume : _voiceVolume;

        #endregion

        #region Custom structures

        public struct AudioSettings
        {
            public float MasterVolume;

            public float BgmVolume;

            public float SfxVolume;

            public float VoiceVolume;

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

            if ((clip == _currentBgm) && (forceRestart))
            {
                _currentBgmPlayer.Stop();
                _currentBgmPlayer.Play();
                return;
            }

            _previousBgm = _currentBgm;
            _currentBgm = clip;

            if (_isMuted)
            {
                return;
            }

            if (!crossFade)
            {
                _currentBgmPlayer.Stop();
                _currentBgmPlayer.clip = _currentBgm;
                _currentBgmPlayer.volume = BgmVolume;
                _currentBgmPlayer.Play();
                return;
            }
            
            if (_currentBgmPlayer.isPlaying)
            {
                FadeAudioSource(_currentBgmPlayer, BgmVolume, 0f, _fadeDuration);
            }

            var audioSource = GetAvailableBgmPlayer();
            audioSource.clip = _currentBgm;
            audioSource.volume = 0f;
            _currentBgmPlayer = audioSource;
            _currentBgmPlayer.Play();
            FadeAudioSource(_currentBgmPlayer, 0f, BgmVolume, _fadeDuration);
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

            FadeAudioSource(_currentBgmPlayer, BgmVolume, 0f, fadeOutDuration);
        }

        /// <summary>
        /// Plays an AudioClip using the settings for sound effects.
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
                player.PlayOneShot(clip, SfxVolume);
            }
            else
            {
                player.loop = loop;
                player.clip = clip;
                player.volume = SfxVolume;
                player.Play();
            }
        }

        /// <summary>
        /// Plays an AudioClip using the settings for sound effects.
        /// </summary>
        /// <param name="clip">AudioClip to be played.</param>
        /// <param name="loop">Loops sound effect.</param>
        public void PlaySfx(AudioClip clip, Vector2 position, bool loop = false)
        {
            if ((_isMuted) || (clip == null))
            {
                return;
            }

            var player = GetAvailableSfxPlayer();
            player.transform.position = position;

            if (!loop)
            {
                player.PlayOneShot(clip, SfxVolume);
            }
            else
            {
                player.loop = loop;
                player.clip = clip;
                player.volume = SfxVolume;
                player.Play();
            }
        }

        /// <summary>
        /// Plays an AudioClip using the settings for sound effects.
        /// </summary>
        /// <param name="clip">AudioClip to be played.</param>
        /// <param name="loop">Loops sound effect.</param>
        public void PlaySfx(AudioClip clip, Vector3 position, bool loop = false)
        {
            if ((_isMuted) || (clip == null))
            {
                return;
            }

            var player = GetAvailableSfxPlayer();
            player.transform.position = position;

            if (!loop)
            {
                player.PlayOneShot(clip, SfxVolume);
            }
            else
            {
                player.loop = loop;
                player.clip = clip;
                player.volume = SfxVolume;
                player.Play();
            }
        }

        /// <summary>
        /// Plays an AudioClip using settings for voice sound.
        /// </summary>
        /// <param name="clip">AudioClip to be played.</param>
        public void PlayVoice(AudioClip clip)
        {
            if ((_isMuted) || (clip == null))
            {
                return;
            }

            var player = GetAvailableVoicePlayer();
            player.clip = clip;
            player.volume = VoiceVolume;
            player.Play();
        }

        /// <summary>
        /// Plays an AudioClip using settings for voice sound.
        /// </summary>
        /// <param name="clip">AudioClip to be played.</param>
        /// <param name="position">Position to spawn the audio source.</param>
        public void PlayVoice(AudioClip clip, Vector2 position)
        {
            if ((_isMuted) || (clip == null))
            {
                return;
            }

            var player = GetAvailableVoicePlayer();
            player.transform.position = position;
            player.clip = clip;
            player.volume = VoiceVolume;
            player.Play();
        }

        /// <summary>
        /// Plays an AudioClip using settings for voice sound.
        /// </summary>
        /// <param name="clip">AudioClip to be played.</param>
        /// <param name="position">Position to spawn the audio source.</param>
        public void PlayVoice(AudioClip clip, Vector3 position)
        {
            if ((_isMuted) || (clip == null))
            {
                return;
            }

            var player = GetAvailableVoicePlayer();
            player.transform.position = position;
            player.clip = clip;
            player.volume = VoiceVolume;
            player.Play();
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
            _masterVolume = settings.MasterVolume;
            _bgmVolume = settings.BgmVolume;
            _sfxVolume = settings.SfxVolume;
            _voiceVolume = settings.VoiceVolume;

            if (settings.IsMuted && (!_isMuted))
            {
                SetMute(settings.IsMuted);
            }
            else if ((!settings.IsMuted) && _isMuted)
            {
                SetMute(settings.IsMuted);
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
                MasterVolume = _masterVolume,
                BgmVolume = BgmVolume,
                SfxVolume = SfxVolume,
                VoiceVolume = VoiceVolume,
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
                
                foreach (var player in _voicePlayers)
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
            foreach (var player in _bgmPlayers)
            {
                if (player.isPlaying)
                {
                    player.volume = BgmVolume;
                }
            }

            foreach (var player in _sfxPlayers)
            {
                if (player.isPlaying)
                {
                    player.volume = SfxVolume;
                }
            }

            foreach (var player in _voicePlayers)
            {
                if (player.isPlaying)
                {
                    player.volume = VoiceVolume;
                }
            }
        }

        private void FadeAudioSource(AudioSource audioSource, float from, float to, float duration)
        {
            StartCoroutine(FadeAudioSourceRoutine(audioSource, from, to, duration));
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
        
        private AudioSource GetAvailableVoicePlayer()
        {
            foreach (var player in _voicePlayers)
            {
                if (!player.isPlaying)
                {
                    return player;
                }
            }

            var newSourceIndex = _voicePlayers.Count;
            var newSource = InstantiateAudioSource($"Voice Player {newSourceIndex}");
            newSource.loop = false;
            _voicePlayers.Add(newSource);
            return newSource;
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
                bgmPlayer.loop = true;
                _bgmPlayers.Add(bgmPlayer);
            }

            _currentBgmPlayer = _bgmPlayers.First();
            _sfxPlayers = new List<AudioSource>();

            for (int i = 0; i < SfxPlayersCount; i++)
            {
                var sfxPlayer = InstantiateAudioSource($"SFX Player {i}");
                sfxPlayer.loop = false;
                _sfxPlayers.Add(sfxPlayer);
            }

            _voicePlayers = new List<AudioSource>();

            for (int i = 0; i < VoicePlayersCount; i++)
            {
                var voicePlayer = InstantiateAudioSource($"Voice Player {i}");
                voicePlayer.loop = false;
                _voicePlayers.Add(voicePlayer);
            }
        }

        private AudioSource InstantiateAudioSource(string name)
        {
            var newGameObject = Instantiate(new GameObject(name), _transform, false);
            var newAudioSource = newGameObject.AddComponent<AudioSource>();
            newAudioSource.playOnAwake = false;
            return newAudioSource;
        }

        #endregion

        #region Coroutines

        private IEnumerator FadeAudioSourceRoutine(AudioSource audioSource, float from, float to, float duration)
        {
            var timeElapsed = 0f;

            while (timeElapsed < duration)
            {
                audioSource.volume = Mathf.Clamp01(Mathf.Lerp(from, to, timeElapsed / duration));
                yield return null;
                timeElapsed += Time.deltaTime;
            }

            if (to == 0f)
            {
                audioSource.Stop();
            }

            UpdateVolume();
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
            PlayerPrefs.SetFloat("master_volume", soundSettings.MasterVolume);
            PlayerPrefs.SetFloat("sfx_volume", soundSettings.SfxVolume);
            PlayerPrefs.SetFloat("bgm_volume", soundSettings.BgmVolume);
            PlayerPrefs.SetFloat("voice_volume", soundSettings.VoiceVolume);
        }

        private void LoadSettings()
        {
            if (!_autoLoadFromPlayerPrefs)
            {
                return;
            }

            _isMuted = PlayerPrefs.GetInt("is_sound_muted", 0) == 1;
            _masterVolume = PlayerPrefs.GetFloat("master_volume", 1f);
            _sfxVolume = PlayerPrefs.GetFloat("sfx_volume", 1f);
            _bgmVolume = PlayerPrefs.GetFloat("bgm_volume", 1f);
            _voiceVolume = PlayerPrefs.GetFloat("voice_volume", 1f);

            SetMute(_isMuted);
            UpdateVolume();
        }

        #endregion

        #endregion 
    }
}
