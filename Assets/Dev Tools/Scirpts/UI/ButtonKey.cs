using UnityEngine;
using UnityEngine.UI;

namespace FSF.Collection
{
    [RequireComponent(typeof(Button))]
    public class ButtonKey : MonoBehaviour
    {
        public KeyCode keyCode = KeyCode.None;
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
            if (btn.interactable && DetectedCanvasGroup.interactable && DetectedRaycaster.enabled && Input.GetKeyDown(keyCode))
            {
                btn.onClick.Invoke();
            }
        }
    }
}
