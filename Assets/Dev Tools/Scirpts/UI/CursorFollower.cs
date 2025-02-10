using UnityEngine;
using UnityEngine.EventSystems;

namespace FSF.UI
{
    public class CursorFollower : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Canvas referredCanvas;
        public RectTransform targetControl;
        public float lerpSpeed = 12f;
        bool follow;
        Vector2 temp;

        public void OnPointerEnter(PointerEventData eventData)
        {
            follow = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            follow = false;
        }

        private void Start()
        {
            if (referredCanvas == null)
            {
                referredCanvas = GetComponentInParent<Canvas>();
            }
        }

        private void Update()
        {
            if (follow)
            {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    this.transform as RectTransform,
                    Input.mousePosition,
                    referredCanvas.renderMode == RenderMode.ScreenSpaceCamera ? referredCanvas.worldCamera : null,
                    out pos
                );
                targetControl.anchoredPosition = Vector2.Lerp(targetControl.anchoredPosition, pos, lerpSpeed * Time.deltaTime);
            }
            else
            {
                RectTransform self = this.transform as RectTransform;
                temp = targetControl.anchoredPosition;
                targetControl.anchoredPosition = Vector2.Lerp(
                    temp, 
                    new Vector2(self.rect.width, self.rect.height) / 2, 
                    lerpSpeed * Time.deltaTime);
                temp = targetControl.anchoredPosition;
            }
        }
    }
}
