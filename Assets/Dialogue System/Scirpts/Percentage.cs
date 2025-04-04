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
    }
    
    [RequireComponent(typeof(Image))]
    public class Percentage : MonoSingleton<Percentage>
    {
        private const float COMPLETE_THRESHOLD = 0.999f;
        private const float FADE_DURATION = 0.1f;

        Tween _fader;
        Image _image;
        bool _faded;
        bool _eventInvoked = true;
        AudioSource _source;
        TypeWriter _typeWriter;
        PercentMode _percentMode;
        
        public UnityEvent OnComplete = new();
        
        private void Start()
        {
            _image = GetComponent<Image>();
            _image.fillAmount = 0f;
            _source = AudioManager.Instance.voice_Source;
        }

        private void Update()
        {
            var progress = CalculateProgress();
            
            UpdateVisuals(progress);
            HandleCompletion(progress);
        }

        private float CalculateProgress()
        {
            switch (_percentMode)
            {
                case PercentMode.VoiceAndText when _source != null && _source.clip != null:
                    var clipLength = _source.clip.length;
                    return clipLength > Mathf.Epsilon ? 
                        Mathf.Clamp01(_source.time / clipLength) : 1f;
                
                case PercentMode.Text when _typeWriter != null && _typeWriter.typer_Tween != null:
                    return Mathf.Clamp01(_typeWriter.typer_Tween.ElapsedPercentage());
                
                default:
                    return 1f;
            }
        }

        private void UpdateVisuals(float progress)
        {
            _image.fillAmount = progress;
            
            var shouldShow = progress > Mathf.Epsilon && 
                           progress < COMPLETE_THRESHOLD;
            
            if (shouldShow == _faded) return;
            
            _fader?.Kill();
            _fader = _image.DOFade(shouldShow ? 1f : 0f, FADE_DURATION);
            _faded = shouldShow;
        }

        private void HandleCompletion(float progress)
        {
            if (!(progress >= COMPLETE_THRESHOLD) || _eventInvoked) return;
            
            OnComplete?.Invoke();
            _eventInvoked = true;
        }

        private void OnDestroy()
        {
            _fader?.Kill();
            OnComplete.RemoveAllListeners();
        }
    }
}