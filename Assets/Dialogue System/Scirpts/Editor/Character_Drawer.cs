using UnityEditor;
using UnityEngine;
using egl = UnityEditor.EditorGUILayout;

namespace FSF.VNG
{
    [CustomEditor(typeof(Character))]
    public class CharacterDrawer : Editor
    {
        SerializedProperty defindID;
        SerializedProperty useExpression;
        SerializedProperty totalFade;
        SerializedProperty char_Image1;
        SerializedProperty char_Image2;
        SerializedProperty char_Renderer1;
        SerializedProperty char_Renderer2;
        SerializedProperty expression_Image1;
        SerializedProperty expression_Image2;
        SerializedProperty expression_Renderer1;
        SerializedProperty expression_Renderer2;
        
        private void OnEnable() 
        {
            defindID = serializedObject.FindProperty("characterDefindID");
            useExpression = serializedObject.FindProperty("UseExpression");
            totalFade = serializedObject.FindProperty("totalFade");
            char_Image1 = serializedObject.FindProperty("char_Image1");
            char_Image2 = serializedObject.FindProperty("char_Image2");
            char_Renderer1 = serializedObject.FindProperty("char_Renderer1");
            char_Renderer2 = serializedObject.FindProperty("char_Renderer2");
            expression_Image1 = serializedObject.FindProperty("expression_Image1");
            expression_Image2 = serializedObject.FindProperty("expression_Image2");
            expression_Renderer1 = serializedObject.FindProperty("expression_Renderer1");
            expression_Renderer2 = serializedObject.FindProperty("expression_Renderer2");
        }
        public override void OnInspectorGUI()
        {
            var T = target as Character;
            if (T == null) return;

            serializedObject.Update();
            egl.PropertyField(defindID);
            egl.PropertyField(useExpression);
            egl.PropertyField(totalFade);
            using (var verticalScope = new egl.VerticalScope(EditorStyles.helpBox))
            {
                egl.PropertyField(char_Image1);
                egl.PropertyField(char_Renderer1);
            }
            egl.Space(3);
            using (var verticalScope = new egl.VerticalScope(EditorStyles.helpBox))
            {
                egl.PropertyField(char_Image2);
                egl.PropertyField(char_Renderer2);
            }
            
            if (T.UseExpression)
            {
                egl.Space(3);
                using (var verticalScope = new egl.VerticalScope(EditorStyles.helpBox))
                {
                    egl.PropertyField(expression_Image1);
                    egl.PropertyField(expression_Renderer1);
                }
                egl.Space(3);
                using (var verticalScope = new egl.VerticalScope(EditorStyles.helpBox))
                {
                    egl.PropertyField(expression_Image2);
                    egl.PropertyField(expression_Renderer2);
                }
            }

            if (EditorApplication.isPlaying)
            {
                egl.Space(10);
                using (var disabled = new EditorGUI.DisabledGroupScope(true))
                {
                    egl.Slider("Total Fade", T.totalFade.alpha, 0, 1);
                    egl.Slider("Char Mix Weight", T.char_mixWeight, 0, 1);
                    egl.Slider("Expression Mix Weight", T.expression_mixWeight, 0, 1);
                }
            }
            serializedObject.ApplyModifiedProperties();

        }
    }
}