using FSF.Collection;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FSF.VNG
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        public AudioSource bg_MusicSource {get; set;}
        public AudioSource voice_Source {get; set;}
        public float lerpSpeed = 12f;
        Tween audioFader;

        protected override void OnAwake()
        {
            bg_MusicSource = new GameObject("bg_MusicPlayer").AddComponent<AudioSource>();
            voice_Source = new GameObject("voice_Player").AddComponent<AudioSource>();
            if (!TryGetComponent(out AudioListener _))
            {
                this.gameObject.AddComponent<AudioListener>();
            }
        }

        public void PlayAudio(AudioClip voice = null, AudioClip bgMusic = null)
        {
            if (voice)
            {
                voice_Source.Stop();
                voice_Source.clip = voice;
                voice_Source.Play();
            }

            if (bgMusic)
            {
                audioFader?.Kill();
                audioFader = bg_MusicSource.DOFade(0, 0.12f);
                audioFader.OnComplete(()=>
                {
                    bg_MusicSource.Stop();
                    bg_MusicSource.clip = bgMusic;
                    bg_MusicSource.Play();
                    audioFader = bg_MusicSource.DOFade(1, 0.12f);
                });
            }
        }

        public void SetVolumeVoice(float volume)
        {
            voice_Source.volume = Mathf.Clamp01(volume);
        }

        public void SetVolumeBGMusic(float volume)
        {
            bg_MusicSource.volume = Mathf.Clamp01(volume);
        }
    }
}