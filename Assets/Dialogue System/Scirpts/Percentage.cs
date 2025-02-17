using DG.Tweening;
using FSF.Collection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FSF.VNG
{
    public enum PercentMode
    {
        VoiceAndText,
        Text
    };
    
    [RequireComponent(typeof(Image))]
    public class Percentage : MonoSingleton<Percentage>
    {
        Tween fader;
        Image image;
        bool faded;
        bool EventInvoked = true;
        float fadeDuration = 0.1f;
        AudioSource source;
        TypeWriter typeWriter;
        PercentMode percentMode;
        public UnityEvent OnComplete = new();

        private void Start()
        {
            image = GetComponent<Image>();
            source = AudioManager.Instance.voice_Source;
            typeWriter = TypeWriter.Instance;
        }

        private void Update()
        {
            float progress;
            switch (percentMode)
            {
                case PercentMode.VoiceAndText:
                    if (source.clip)
                    {
                        progress = source.time / source.clip.length;
                        if (progress == 0)
                        {
                            progress = 1;
                        }
                    }
                    else goto case PercentMode.Text;
                    break;

                case PercentMode.Text:
                    if (typeWriter.typer_Tween != null)
                    {
                        progress = typeWriter.typer_Tween.ElapsedPercentage();
                    }
                    else progress = 1;
                    break;

                default:
                    progress = 1;
                    break;
            }

            if (progress > 0 && progress < 1)
            {
                if (!faded)
                {
                    fader?.Kill();
                    fader = image.DOFade(1, fadeDuration);
                    faded = true;
                    EventInvoked = false;
                }
            }
            else if (progress == 1)
            {
                fader?.Kill();
                fader = image.DOFade(0, fadeDuration);
                faded = false;
                if (!EventInvoked)
                {
                    EventInvoked = true;
                    OnComplete?.Invoke();
                }
            }
            image.fillAmount = progress;
        }

        private void OnDestroy()
        {
            OnComplete.RemoveAllListeners();
        }

    }
}
