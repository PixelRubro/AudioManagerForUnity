using UnityEngine;
using UnityEngine.UI;

namespace PixelRouge.SimpleAudioManager.Demo
{
    public class DemoScript : MonoBehaviour
    {
        #region Actions
        #endregion

        #region Serialized fields

        public AudioClip _sfx1;

        public AudioClip _sfx2;

        public AudioClip _sfx3;

        public AudioClip _bgm1;

        public AudioClip _bgm2;

        public AudioClip _bgm3;

        public AudioClip _voice1;

        public AudioClip _voice2;

        public AudioClip _voice3;

        #endregion

        #region Non-serialized fields
        #endregion

        #region Properties
        #endregion

        #region Unity events
        #endregion

        #region Public methods

        public void PlayBgm1()
        {
            AudioManager.Instance.PlayBgm(_bgm1);
        }

        public void PlayBgm2()
        {
            AudioManager.Instance.PlayBgm(_bgm2);
        }

        public void PlayBgm3()
        {
            AudioManager.Instance.PlayBgm(_bgm3);
        }

        public void PlaySfx1()
        {
            AudioManager.Instance.PlaySfx(_sfx1);
        }

        public void PlaySfx2()
        {
            AudioManager.Instance.PlaySfx(_sfx2);
        }

        public void PlaySfx3()
        {
            AudioManager.Instance.PlaySfx(_sfx3);
        }

        public void PlayVoice1()
        {
            AudioManager.Instance.PlayVoice(_voice1);
        }

        public void PlayVoice2()
        {
            AudioManager.Instance.PlayVoice(_voice2);
        }

        public void PlayVoice3()
        {
            AudioManager.Instance.PlayVoice(_voice3);
        }

        #endregion

        #region Private methods
        #endregion    
    }
}