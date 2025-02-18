using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using URandom = UnityEngine.Random;

namespace FSF.UI
{
    public class PopupInterface : MonoBehaviour
    {
        readonly Vector2 Hide_Position = new(-1050, -600);
        public Canvas referredCanvas;
        public CanvasGroup canvasGroup;
        public float duration = 0.35f;
        public Ease ease = Ease.OutExpo;
        [HideInInspector] public bool isOpen = false;
        Sequence _animationSequence;
        

        private void Awake()
        {
            if (referredCanvas == null)
            {
                referredCanvas = GetComponentInParent<Canvas>();
            }
            canvasGroup.interactable = false;
            canvasGroup.transform.localScale = Vector3.zero;
            canvasGroup.alpha = 0;
            (canvasGroup.transform as RectTransform).anchoredPosition = Hide_Position;
        }

        public void TogglePanel()
        {
            _animationSequence?.Kill();
            _animationSequence = DOTween.Sequence().SetUpdate(true);
            _animationSequence.Append(
                (canvasGroup.transform as RectTransform).DOAnchorPos(
                    isOpen ? Hide_Position : Vector2.zero, isOpen ? duration * 1.3f : duration)
                .SetEase(ease)
                .OnStart(() => canvasGroup.interactable = !isOpen));
            _animationSequence.Join(
                canvasGroup.DOFade(isOpen ? 0 : 1, isOpen ? duration * 1.3f : duration)
                .SetEase(Ease.InOutSine));
            _animationSequence.Join(
                canvasGroup.transform.DOScale(isOpen ? Vector3.zero : Vector3.one, isOpen ? duration * 1.3f : duration)
                    .SetEase(ease));
            _animationSequence.OnComplete(() => {
                isOpen = !isOpen;
                canvasGroup.blocksRaycasts = isOpen;
            });

            _animationSequence.Join(
                canvasGroup.transform.DOPunchRotation(new Vector3(0, 0, URandom.Range(-3f, 3f)), 
                duration, 
                1, 
                0.5f));
        }
    }
}