using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace FSF.Collection
{
    [RequireComponent(typeof(Button))]
    public class ButtonKey_Multiple : MonoBehaviour
    {
        public KeyCode[] keyCodes = new KeyCode[] {KeyCode.None};
        public CanvasGroup DetectedCanvasGroup;
        public GraphicRaycaster DetectedRaycaster;
        Button btn;

        private void Awake()
        {
            btn = GetComponent<Button>();
            if (DetectedCanvasGroup == null)
            {
                DetectedCanvasGroup = GetComponentInParent<CanvasGroup>();
            }
            if (DetectedRaycaster == null)
            {
                DetectedRaycaster = GetComponentInParent<GraphicRaycaster>();
            }
        }

        private void Update()
        {
            if (btn.interactable && DetectedCanvasGroup.interactable && DetectedRaycaster.enabled &&
                keyCodes.Length > 0 && keyCodes.Any(keyCode => Input.GetKeyDown(keyCode)))
            {
                btn.onClick.Invoke();
            }
        }
    }
}
