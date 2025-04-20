#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace FSF.VNG
{
    public class DialogueProfileEditor : EditorWindow
    {
        private DialogueProfileContainer container;
        private SerializedObject serializedContainer;
        private SerializedProperty profileProp;
        private Vector2 scrollPos;
        private Texture2D headerTex;

        [MenuItem("Visual Novel/Dialogue Editor")]
        public static void ShowWindow()
        {
            GetWindow<DialogueProfileEditor>("Dialogue Editor").minSize = new Vector2(800, 600);
        }

        private void OnEnable()
        {
            headerTex = CreateHeaderTexture();
        }

        private Texture2D CreateHeaderTexture()
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, new Color(0.15f, 0.15f, 0.45f));
            tex.Apply();
            return tex;
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawMainLayout();
        }

        private void DrawHeader()
        {
            var headerStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 18,
                normal = { textColor = Color.white },
                padding = new RectOffset(15, 10, 10, 10)
            };

            EditorGUI.DrawRect(GUILayoutUtility.GetRect(position.width, 50), new Color(0.1f, 0.1f, 0.3f));
        }

        private void DrawMainLayout()
        {
            EditorGUILayout.BeginHorizontal();
            {
                DrawSidebar();
                DrawMainContent();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSidebar()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.Width(250));
            {
                EditorGUILayout.LabelField("Project Management", EditorStyles.boldLabel);
                DrawContainerSelector();
                EditorGUILayout.Space(15);
                DrawQuickTools();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawContainerSelector()
        {
            EditorGUI.BeginChangeCheck();
            container = (DialogueProfileContainer)EditorGUILayout.ObjectField(
                "Current Project", 
                container, 
                typeof(DialogueProfileContainer), 
                false);

            if (EditorGUI.EndChangeCheck())
            {
                InitializeSerializedData();
            }

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("New Project"))
                {
                    CreateNewContainer();
                }
                if (GUILayout.Button("Save Project"))
                {
                    SaveContainer();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void InitializeSerializedData()
        {
            if (container != null)
            {
                serializedContainer = new SerializedObject(container);
                profileProp = serializedContainer.FindProperty("profile");
            }
        }

        private void CreateNewContainer()
        {
            var newContainer = ScriptableObject.CreateInstance<DialogueProfileContainer>();
            string path = EditorUtility.SaveFilePanelInProject(
                "new Project", 
                "NewDialogue.asset", 
                "asset", 
                "Select save location");

            if (!string.IsNullOrEmpty(path))
            {
                AssetDatabase.CreateAsset(newContainer, path);
                AssetDatabase.SaveAssets();
                container = newContainer;
                InitializeSerializedData();
            }
        }

        private void SaveContainer()
        {
            if (container != null)
            {
                EditorUtility.SetDirty(container);
                AssetDatabase.SaveAssets();
            }
        }

        private void DrawQuickTools()
        {
            EditorGUILayout.LabelField("Operations", EditorStyles.boldLabel);
            if (GUILayout.Button("Export as JSON"))
            {
                ExportToJson();
            }
        }

        private void DrawMainContent()
        {
            if (container == null || serializedContainer == null)
            {
                EditorGUILayout.HelpBox("Select a project...", MessageType.Info);
                return;
            }

            EditorGUI.BeginChangeCheck();
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            {
                DrawEpisodeSection();
                DrawActionList();
            }
            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck())
            {
                SaveChanges();
            }
        }

        private new void SaveChanges()
        {
            serializedContainer.ApplyModifiedProperties();
            SaveContainer();
        }

        private void DrawEpisodeSection()
        {
            EditorGUILayout.LabelField("Episode", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal("GroupBox");
            {
                var episodeProp = profileProp.FindPropertyRelative("episode");
                EditorGUILayout.PropertyField(episodeProp.FindPropertyRelative("Chapter"));
                EditorGUILayout.PropertyField(episodeProp.FindPropertyRelative("Scene"));
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawActionList()
        {
            EditorGUILayout.LabelField("Dialogue Management", EditorStyles.boldLabel);
            var actionsProp = profileProp.FindPropertyRelative("actions");
            
            EditorGUILayout.PropertyField(actionsProp, new GUIContent("Dialogue List"), true);
        }

        private void ExportToJson()
        {
            if (container == null) return;

            string path = EditorUtility.SaveFilePanel(
                "Save as JSON", 
                Path.Combine(Application.streamingAssetsPath, "DialogueProfiles"), 
                $"Episode_{container.profile.episode[0]}_{container.profile.episode[1]}.json", 
                "json");

            if (!string.IsNullOrEmpty(path))
            {
                string json = JsonConvert.SerializeObject(container.profile, Formatting.Indented);
                File.WriteAllText(path, json);
                Debug.Log($"JSON saved to {path}");
            }
        }
    }
}
#endif