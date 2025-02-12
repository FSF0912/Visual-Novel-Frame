using DG.Tweening;
using FSF.Collection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace FSF.VNG
{
    [RequireComponent(typeof(Image))]
    public class PercentageFadeAuto : MonoSingleton<PercentageFadeAuto>
    {
        Tween fader;
        Image self;
        bool faded;
        bool invoked = true;
        public UnityEvent OnVoiceComplete = new();

        private void Awake()
        {
            self = GetComponent<Image>();
        }

        private void Update()
        {
            if(self.fillAmount > 0 && self.fillAmount < 1)
            {
                if (!faded)
                {
                    fader?.Kill();
                    fader = self.DOFade(1, 0.2f);
                    faded = true;
                    invoked = false;
                }
            }
            else if (self.fillAmount == 1)
            {
                fader?.Kill();
                fader = self.DOFade(0, 0.1f);
                faded = false;
                if (!invoked)
                {
                    invoked = true;
                    OnVoiceComplete?.Invoke();
                }
            }
        }
    }
}
