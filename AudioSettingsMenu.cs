using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace YoukaiFox.Audio
{
    public class AudioSettingsMenu : MonoBehaviour
    {
        #region Actions
        #endregion

        #region Serialized fields
        [SerializeField]
        private Slider _musicVolumeSlider;

        [SerializeField]
        private Slider _sfxVolumeSlider;

        [SerializeField]
        private Toggle _muteSoundToggle;

        [SerializeField]
        private bool _updateOnValueChanged = true;


        // [SerializeField]
        // [Tooltip("An audio clip to play when adjusting the volume.")]
        // private AudioClip _audioTestClip;

        #endregion

        #region Non-serialized fields

        private AudioManager _audioManager;

        #endregion

        #region Constant fields
        #endregion

        #region Unity events

        private void Start()
        {
            _audioManager = FindObjectOfType<AudioManager>();
            SetUpListeners();
        }

        private void OnEnable()
        {
            LoadSettings();
        }

        #endregion

        #region Public methods

        public void UpdateSoundSettings()
        {
            if (AudioManager.Instance == null)
            {
                return;
            }

            var settings =  new AudioManager.AudioSettings();
            settings.IsMuted = _muteSoundToggle.isOn;
            settings.BgmVolume = _musicVolumeSlider.value;
            settings.SfxVolume = _sfxVolumeSlider.value;
            AudioManager.Instance.UpdateSettings(settings);
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
            _musicVolumeSlider.value = settings.BgmVolume;
            _sfxVolumeSlider.value = settings.SfxVolume;
        }

        private void SetUpListeners()
        {
            if (_updateOnValueChanged)
            {
                _muteSoundToggle.onValueChanged.AddListener(delegate {UpdateSoundSettings();});
                _musicVolumeSlider.onValueChanged.AddListener(delegate {UpdateSoundSettings();});
                _sfxVolumeSlider.onValueChanged.AddListener(delegate {UpdateSoundSettings();});
            }
        }

        #endregion 
    }
}
