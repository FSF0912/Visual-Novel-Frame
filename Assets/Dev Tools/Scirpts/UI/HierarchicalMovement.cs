using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using egl = UnityEditor.EditorGUILayout;
#endif

namespace FSF.UI
{
    [ExecuteAlways]
    public class HierarchicalMovement : MonoBehaviour
    {
        public RectTransform followedObject;
        [HideInInspector] public bool lerp = false;
        [HideInInspector] public float lerpSpeed = 12f;
        RectTransform canvas;
        RectTransform background;

        private void Update()
        {
            if(background == null){
                background = this.transform as RectTransform;
                }
            if(canvas == null){
                canvas = GetComponentInParent<Canvas>().transform as RectTransform;
            }
            #if UNITY_EDITOR
            if(followedObject == null){return;}
            #endif
            float canvasWidth = canvas.rect.width;
            float canvasHeight = canvas.rect.height;

            Vector3 uiElementPosition = followedObject.position;

            float backgroundX = uiElementPosition.x;
            float backgroundY = uiElementPosition.y;

            if (backgroundX < -canvasWidth / 2)
            {
                backgroundX = -canvasWidth / 2;
            }
            else if (backgroundX > canvasWidth / 2)
            {
                backgroundX = canvasWidth / 2;
            }

            if (backgroundY < -canvasHeight / 2)
            {
                backgroundY = -canvasHeight / 2;
            }
            else if (backgroundY > canvasHeight / 2)
            {
                backgroundY = canvasHeight / 2;
            }

            if(lerp){
                float deltaTime = Time.deltaTime;
                background.anchoredPosition = Vector2.Lerp(
                    background.anchoredPosition, 
                    new Vector2(backgroundX, backgroundY), 
                    lerpSpeed * deltaTime);
                background.sizeDelta = Vector2.Lerp(background.sizeDelta, 
                new Vector2(
                    Mathf.Max(background.rect.width, canvasWidth), 
                    Mathf.Max(background.rect.height, canvasHeight))
                    ,
                    lerpSpeed * deltaTime);
            }
            else{
                background.anchoredPosition = new Vector2(backgroundX, backgroundY);
                background.sizeDelta = new Vector2(Mathf.Max(background.rect.width, canvasWidth), Mathf.Max(background.rect.height, canvasHeight));
            }
        }
    }
    #if UNITY_EDITOR
    [CustomEditor(typeof(HierarchicalMovement))]
    public class HierarchicalMovementEditor : Editor{
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var T = target as HierarchicalMovement;
            if(T == null){return;}
            T.lerp = egl.Toggle("Lerp?", T.lerp);
            if(T.lerp){
                T.lerpSpeed = egl.FloatField("lerpSpeed", T.lerpSpeed);
            }
        }
    }
    #endif
}
