using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PixelSpark.UnityAudioManager
{
    public class AudioSettingsMenu : MonoBehaviour
    {
        #region Actions
        #endregion

        #region Serialized fields

        [SerializeField]
        private Slider _masterVolumeSlider;

        [SerializeField]
        private Slider _sfxVolumeSlider;

        [SerializeField]
        private Slider _bgmVolumeSlider;

        [SerializeField]
        private Slider _voiceVolumeSlider;

        [SerializeField]
        private Toggle _muteSoundToggle;

        [SerializeField]
        private bool _updateOnValueChanged = true;

        [SerializeField]
        [Tooltip("An audio clip to play when adjusting the volume.")]
        private AudioClip _audioTestClip;

        #endregion

        #region Non-serialized fields
        #endregion

        #region Constant fields
        #endregion

        #region Unity events

        private void Start()
        {
            LoadSettings();
            SetUpListeners();
        }

        #endregion

        #region Public methods

        public void UpdateSoundSettings()
        {
            if (AudioManager.Instance == null)
            {
                return;
            }

            if (_bgmVolumeSlider.value > _masterVolumeSlider.value)
            {
                _bgmVolumeSlider.value = _masterVolumeSlider.value;
            }

            if (_sfxVolumeSlider.value > _masterVolumeSlider.value)
            {
                _sfxVolumeSlider.value = _masterVolumeSlider.value;
            }

            if (_voiceVolumeSlider.value > _masterVolumeSlider.value)
            {
                _voiceVolumeSlider.value = _masterVolumeSlider.value;
            }

            var settings = new AudioManager.AudioSettings
            {
                IsMuted = _muteSoundToggle.isOn,
                MasterVolume = _masterVolumeSlider.value,
                BgmVolume = _bgmVolumeSlider.value,
                SfxVolume = _sfxVolumeSlider.value,
                VoiceVolume = _voiceVolumeSlider.value
            };

            AudioManager.Instance.UpdateSettings(settings);

            AudioManager.Instance.PlaySfx(_audioTestClip);
        }

        #endregion

        #region Protected methods
        #endregion

        #region Private methods

        private void LoadSettings()
        {
            if (AudioManager.Instance == null)
            {
                return;
            }

            var settings = AudioManager.Instance.GetCurrentSettings();
            _muteSoundToggle.isOn = settings.IsMuted;
            _masterVolumeSlider.value = settings.MasterVolume;
            _bgmVolumeSlider.value = settings.BgmVolume;
            _sfxVolumeSlider.value = settings.SfxVolume;
            _voiceVolumeSlider.value = settings.VoiceVolume;
        }

        private void SetUpListeners()
        {
            _masterVolumeSlider.onValueChanged.AddListener(delegate { UpdateSoundSettings(); });
            _bgmVolumeSlider.onValueChanged.AddListener(delegate { UpdateSoundSettings(); });
            _sfxVolumeSlider.onValueChanged.AddListener(delegate { UpdateSoundSettings(); });
            _voiceVolumeSlider.onValueChanged.AddListener(delegate { UpdateSoundSettings(); });
            _muteSoundToggle.onValueChanged.AddListener(delegate { UpdateSoundSettings(); });
        }

        #endregion 
    }
}
