using System;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FSF.Collection
{
    public enum PopupMode
    {
        Cover,
        Await
    };
    public class PopupMenu : MonoSingleton<PopupMenu>
    {
        public CanvasScaler referredCanvas;
        [Space]
        public RectTransform TopNoticeHolder;
        public Text TopNoticeText;
        [Space]
        public RectTransform central_NoticeHolder;
        public RectTransform central_ButtonsHolder;
        public CanvasGroup central_CanvasGroup;
        public Text central_Text;
        public GameObject button_Prefab;
        [Space]
        public float animationDuration = 0.3f;
        public Ease ease = Ease.InCubic;
        Sequence _top_Tween, _central_Tween;
        Vector2 refer;

       //central
        protected override void OnAwake()
        {
            if (referredCanvas == null)
            {
                referredCanvas = GetComponentInParent<CanvasScaler>();
            }
            refer = referredCanvas.referenceResolution;
        }

        public void Panel_Top(string message)
        {
            _top_Tween?.Kill();
            TopNoticeHolder.position = new (refer.x + TopNoticeHolder.rect.width / 2, refer.y - TopNoticeHolder.rect.height / 2);
            TopNoticeHolder.localScale = new (0.5f, 0.5f, 0.5f);

            _top_Tween.Append(
                TopNoticeHolder.DOMoveX(refer.x - TopNoticeHolder.rect.width / 2, animationDuration)
                .SetEase(ease)
            );

            _top_Tween.Join(
                TopNoticeHolder.DOScale(Vector3.one, animationDuration)
                .SetEase(ease)
            );

            _top_Tween.OnComplete(()=> {
                    _top_Tween.Join(
                    TopNoticeHolder.DOMoveX(refer.x + TopNoticeHolder.rect.width / 2, animationDuration)
                    .SetEase(ease)
                );

                _top_Tween.Join(
                    TopNoticeHolder.DOScale(new Vector3(0.5f, 0.5f, 0.5f), animationDuration)
                    .SetEase(ease)
                );
            });

            TopNoticeText.text = message;
        }

        public void Panel_Central(string message, ValueTuple<string, Action> action1)
        {
            _central_Tween?.Kill();

            foreach (GameObject btn in central_ButtonsHolder.transform)
            {
                Destroy(btn);
            }

            var current = Instantiate(button_Prefab, central_ButtonsHolder).GetComponent<PopupButtonSingle>();
            current.text.text = action1.Item1;
            current.button.onClick.AddListener(()=> {
                action1.Item2.Invoke();
                });

            LayoutRebuilder.ForceRebuildLayoutImmediate(central_ButtonsHolder);

            central_CanvasGroup.alpha = 0;
            central_NoticeHolder.localScale = new (0.4f, 0.4f, 0.4f); 
            _central_Tween.Append(
                central_CanvasGroup.DOFade(1, animationDuration)
                .SetEase(ease)
            );

            _central_Tween.Join(
                central_NoticeHolder.DOScale(Vector3.one, animationDuration)
                .SetEase(ease)
            );

            central_Text.text = message;
        }

        public void Panel_Central(string message, params ValueTuple<string, Action>[] actions)
        {
            _central_Tween?.Kill();

            foreach (GameObject btn in central_ButtonsHolder.transform)
            {
                Destroy(btn);
            }

            foreach (var action in actions)
            {
                var current = Instantiate(button_Prefab, central_ButtonsHolder).GetComponent<PopupButtonSingle>();
                current.text.text = action.Item1;
                current.button.onClick.AddListener(()=> {
                action.Item2.Invoke();
                central_CanvasGroup.interactable = false;
                //_central_Tween
                });
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(central_ButtonsHolder);

            central_CanvasGroup.alpha = 0;
            central_CanvasGroup.interactable = true;
            central_NoticeHolder.localScale = new (0.4f, 0.4f, 0.4f); 
            _central_Tween.Append(
                central_CanvasGroup.DOFade(1, animationDuration)
                .SetEase(ease)
            );

            _central_Tween.Join(
                central_NoticeHolder.DOScale(Vector3.one, animationDuration)
                .SetEase(ease)
            );

            central_Text.text = message;
        }
    }
}
