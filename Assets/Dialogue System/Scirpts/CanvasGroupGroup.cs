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
        Tween show_Tween, hide_Tween;

        public void ShowPanel(int index)
        {
            if (index < 0 || index > groups.Length - 1) return;

            show_Tween?.Kill();
            hide_Tween?.Complete();
            if (current != null)
            {
                hide_Tween = current.DOFade(0, duration).SetEase(ease);
                current.interactable = false;
            }
            show_Tween = groups[index].DOFade(1, duration).SetEase(ease);
            show_Tween.OnComplete(delegate() {
                groups[index].interactable = true;
            });
            current = groups[index];
        }

        public void Hide()
        {
            show_Tween?.Kill();
            hide_Tween?.Complete();
            hide_Tween = current.DOFade(0, duration).SetEase(ease);
            current.interactable = false;
        }
    }
}
