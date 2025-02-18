using DG.Tweening;
using UnityEngine;

namespace FSF.Collection
{
    public class CanvasGroupGroup : MonoBehaviour
    {
        public CanvasGroup[] groups;
        public CanvasGroup current;
        public Ease ease = Ease.InOutSine;
        public float duration = 0.3f;
        private Tween showTween, hideTween;

        public void ShowPanel(int index)
        {
            if (index < 0 || index >= groups.Length) return;

            showTween?.Kill();
            hideTween?.Complete();

            if (current != null)
            {
                hideTween = current.DOFade(0, duration).SetEase(ease).OnComplete(() => {
                    current.interactable = false;
                });
            }

            showTween = groups[index].DOFade(1, duration).SetEase(ease).OnComplete(() => {
                groups[index].interactable = true;
            });

            current = groups[index];
        }

        public void Hide()
        {
            showTween?.Kill();
            hideTween?.Complete();

            if (current != null)
            {
                hideTween = current.DOFade(0, duration).SetEase(ease).OnComplete(() => {
                    current.interactable = false;
                });
            }
        }
    }
}