using DG.Tweening;
using URandom = UnityEngine.Random;
using UnityEngine;
using UnityEngine.UI;

namespace FSF.UI{
    public class PopupInterface : MonoBehaviour
    {
        public Canvas referredCanvas;
        public CanvasGroup canvasGroup;
        public Button escapeButton;

        Tween curveTween, fadingTween, scaleTween;
        bool opened = false;

        private void Awake() {
            canvasGroup.interactable = false;
            canvasGroup.transform.localScale = Vector3.zero;
            if(referredCanvas == null){
                referredCanvas = GetComponentInParent<Canvas>();
            }
            escapeButton.onClick.AddListener(()=>{
                OperatePanel(escapeButton.transform.position,false);
            });
        }
        public virtual void OperatePanel(Vector3 targetPos, bool adaptPosition,float duration = 0.3f){
            Vector3[] v3s = new Vector3[3];
            if(!opened){
                v3s[0] = targetPos;
                v3s[2] = referredCanvas.transform.position;
            }
            else{
                v3s[2] = targetPos;
                v3s[0] = referredCanvas.transform.position;
            }
            v3s[1] = (targetPos + referredCanvas.transform.position) / 2 + new Vector3(
                    URandom.Range(-70, 70), URandom.Range(-70,70)
                );
                if(adaptPosition) this.transform.position = v3s[0];
                curveTween?.Kill();
                curveTween = this.transform.DOPath(v3s, duration, PathType.CatmullRom, PathMode.TopDown2D);
                fadingTween?.Kill();
                fadingTween = canvasGroup.DOFade(opened ? 0 : 1,duration);
                canvasGroup.interactable = !opened;
                scaleTween?.Kill();
                scaleTween = canvasGroup.transform.DOScale(opened ? Vector3.zero : Vector3.one, duration);
                opened = !opened;
        }
    }
}

