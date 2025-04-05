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
                if (voice == voice_Source.clip && voice_Source.isPlaying) return;
                voice_Source.Stop();
                voice_Source.clip = voice;
                voice_Source.Play();
            }

            if (bgMusic)
            {
                if (bgMusic == bg_MusicSource.clip && bg_MusicSource.isPlaying) return;
                audioFader?.Kill();
                audioFader = bg_MusicSource.DOFade(0, 0.12f);
                var music = bgMusic;
                audioFader.OnComplete(()=>
                {
                    bg_MusicSource.Stop();
                    bg_MusicSource.clip = music;
                    bg_MusicSource.Play();
                    audioFader = bg_MusicSource.DOFade(1, 0.12f);
                });
            }
        }

        public void InterruptAudio()
        {
            voice_Source.Stop();
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