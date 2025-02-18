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
        Tween _fader;
        Image _image;
        bool _faded;
        bool _EventInvoked = true;
        float _fadeDuration = 0.1f;
        AudioSource _source;
        TypeWriter _typeWriter;
        PercentMode _percentMode;
        public UnityEvent OnComplete = new();

        private void Start()
        {
            _image = GetComponent<Image>();
            _source = AudioManager.Instance.voice_Source;
            _typeWriter = TypeWriter.Instance;
        }

        private void Update()
        {
            float progress;
            switch (_percentMode)
            {
                case PercentMode.VoiceAndText:
                    if (_source.clip)
                    {
                        progress = _source.time / _source.clip.length;
                        if (progress == 0)
                        {
                            progress = 1;
                        }
                    }
                    else goto case PercentMode.Text;
                    break;

                case PercentMode.Text:
                    if (_typeWriter.typer_Tween != null)
                    {
                        progress = _typeWriter.typer_Tween.ElapsedPercentage();
                    }
                    else progress = 1;
                    break;

                default:
                    progress = 1;
                    break;
            }

            if (progress > 0 && progress < 1)
            {
                if (!_faded)
                {
                    _fader?.Kill();
                    _fader = _image.DOFade(1, _fadeDuration);
                    _faded = true;
                    _EventInvoked = false;
                }
            }
            else if (progress == 1)
            {
                _fader?.Kill();
                _fader = _image.DOFade(0, _fadeDuration);
                _faded = false;
                if (!_EventInvoked)
                {
                    _EventInvoked = true;
                    OnComplete?.Invoke();
                }
            }
            _image.fillAmount = progress;
        }

        private void OnDestroy()
        {
            OnComplete.RemoveAllListeners();
        }

    }
}
