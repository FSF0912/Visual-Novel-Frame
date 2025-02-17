using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using egl = UnityEditor.EditorGUILayout;
namespace FSF.Collection{
    public class Editor_Debug : MonoSingleton<Editor_Debug>
    {
        public CanvasGroup DebugInterface;
        [HideInInspector] public KeyCode ReStartSceneCode = KeyCode.R;

        protected override void OnAwake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        private void Update() 
        {
            if(Input.GetKeyDown(ReStartSceneCode))
            {
                ReStartScene();
            }
        }

        #region Public Methods
        public void ReStartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        #endregion
    }

    [CustomEditor(typeof(Editor_Debug))]
    public class Editor_Debug_Editor : Editor
    {
        SerializedProperty re_Key;

        private void OnEnable() 
        {
            re_Key = serializedObject.FindProperty("ReStartSceneCode");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var T = target as Editor_Debug;
            if (T == null) return;
            serializedObject.Update();

            using (var VerticalScope = new egl.VerticalScope(EditorStyles.helpBox))
            {
                egl.LabelField("↓Restart Scene Code↓");
                egl.PropertyField(re_Key, GUIContent.none);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
