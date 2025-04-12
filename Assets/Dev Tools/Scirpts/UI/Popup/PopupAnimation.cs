using DG.Tweening;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using egl = UnityEditor.EditorGUILayout;
#endif

namespace FSF.UI
{
    public class PopupAnimation : MonoBehaviour
    {
        public Vector2 Hide_Position = new(-1050, -600);
        public Canvas referredCanvas;
        public CanvasGroup canvasGroup;
        public float duration = 0.35f;
        public bool customCurve = false;
        [HideInInspector] public Ease ease = Ease.OutExpo;
        [HideInInspector] public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        [HideInInspector] public bool isOpen = false;
        Sequence _animationSequence;
        

        private void Awake()
        {
            if (referredCanvas == null)
            {
                referredCanvas = GetComponentInParent<Canvas>();
            }
            canvasGroup.interactable = false;
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
                //.SetEase()
                .OnStart(() => canvasGroup.interactable = !isOpen));
            _animationSequence.Join(
                canvasGroup.DOFade(isOpen ? 0 : 1, isOpen ? duration * 1.3f : duration)
                //.SetEase(Ease.InOutSine)
                );
            _animationSequence.OnComplete(() => {
                isOpen = !isOpen;
                canvasGroup.blocksRaycasts = isOpen;
            });
            if (customCurve)
            {
                _animationSequence.SetEase(curve);
            }
            else{
                _animationSequence.SetEase(ease);
            }
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(PopupAnimation))]
    [CanEditMultipleObjects]
    public class PopupInterfaceEditor : Editor
    {
        SerializedProperty ease;
        SerializedProperty curve;

        private void OnEnable() 
        {
            ease = serializedObject.FindProperty("ease");
            curve = serializedObject.FindProperty("curve");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var T = target as PopupAnimation;
            if (T == null) return;
            serializedObject.Update();
            egl.PropertyField(T.customCurve ? curve : ease);
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endif
}