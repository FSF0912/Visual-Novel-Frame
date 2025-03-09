using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FSF.VNG
{
    #region Data/Structures
    [System.Serializable]
    public struct SingleAction
    {
        public string name;
        [TextArea(1, 15)] public string dialogue;
        public AudioClip audio;
        public Sprite backGround;
        public AudioClip bg_Music;
        public EnvironmentSettings environmentSettings;
        public CharacterOption[] characterOptions;

        public SingleAction(
            string name, string dialogue, 

        AudioClip audio, Sprite backGround, AudioClip bg_Music, 

        EnvironmentSettings environmentSettings, CharacterOption[] characterOptions
        )
        {
            this.name = name;
            this.dialogue = dialogue;
            this.audio = audio;
            this.backGround = backGround;
            this.bg_Music = bg_Music;
            this.environmentSettings = environmentSettings;
            this.characterOptions = characterOptions;
        }
    }
    //
    [System.Serializable]
    public class CharacterOption
    {
        public int characterDefindID;
        public Sprite characterImage;
        public CharacterPresenceStatus presenceStatus = CharacterPresenceStatus.Normal;
        public MotionPresents motionMode = MotionPresents.None;
        [Space(10f)]
        public float action_Duration = 0.6f;
        public Ease action_Ease = Ease.InOutSine;
        [Header("Custom Options")]
        public bool useOrigin = false;
        public Vector2 origin = Vector2.zero;
        public Vector2 appointedPosition = Vector2.zero;

        /// <summary>
        /// <para>
        /// 采用列表顺序决定场景中的角色顺序。
        /// </para>
        /// </summary>
        public bool ArrangeByListOrder = true;

        /// <summary>
        /// <para>
        /// 顺序值越大，层级越前。
        /// </para>
        /// 顺序为2的角色会将顺序为1的角色挡住。
        /// <para>
        /// 如在单个Action中检测到相同或无效的Order值，将会采用列表顺序来决定角色顺序。
        /// </para>
        /// </summary>
        public int CustomOrder;

        public CharacterOption(
            int characterDefindID, Sprite characterImage, 

            MotionPresents motionMode,

            float action_Duration, Ease action_Ease, 

            bool useOrigin, Vector2 origin, Vector2 appointedPosition, 

            bool ArrangeByListOrder, int CustomOrder
        )
        {
            this.characterDefindID = characterDefindID;
            this.characterImage = characterImage;
            this.motionMode = motionMode;
            this.action_Duration = action_Duration;
            this.action_Ease = action_Ease;
            this.useOrigin = useOrigin;
            this.origin = origin;
            this.appointedPosition = appointedPosition;
            this.ArrangeByListOrder = ArrangeByListOrder;
            this.CustomOrder = CustomOrder;
        }
    }
    //
    [System.Serializable]
    public struct BranchNode
    {
        public string branchName;
        public int startIndex;
        public int endIndex;

        public BranchNode(string branchName, int startIndex, int endIndex)
        {
            this.branchName = branchName;
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }
    }
    //
    #endregion

    #region Data/Enums
    public enum EnvironmentSettings
    {
        None,
        Day,
        Twilight,
        Night
    };

    public enum MotionPresents
    {
        None,
        LeftEnterToCenter,
        RightEnterToCenter,
        BottomEnterToCenter,
        LeftEscape,
        RightEscape,
        ToCenter,
        ToLeft,
        ToRight,
        ShakeHeavily,
        ShackSlightly,
        [InspectorName("←HorizontalMove→")]
        HorizontalMove,
        [InspectorName("↑VerticalMove↓")]
        VerticalMove,
        Custom
    };

    public enum DistanceState
    {
        None,
        Normal,
        Far,
        Near
    };

    public enum CharacterPresenceStatus
    {
        None,
        Normal,
        Enter,
        Exit
    };
#endregion



    [CreateAssetMenu(fileName = "DialogueProfile", menuName = "VNG/DialogueProfile", order = 30000)]
    public class DialogueProfile : ScriptableObject
    {
        public SingleAction[] actions;
        public SingleAction this[int index]
        {
            get
            {
                return actions[index];
            }
        }
    }
    /*#if UNITY_EDITOR
    [CustomEditor(typeof(DialogueProfile))]
    public class DialogueProfileEditor : Editor
    {
        #region Editor/Styles
        private GUIStyle _boldStyle;
        private GUIStyle _boxStyle;
        private GUIStyle _headerStyle;
        
        private void InitializeStyles()
        {
            if (_boldStyle == null)
            {
                _boldStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
                _boxStyle = new GUIStyle("box");
                _headerStyle = new GUIStyle(GUI.skin.box)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold
                };
            }
        }
        #endregion

        #region Editor/Foldout Management
        private SerializedProperty _actionsProperty;
        private readonly Dictionary<int, bool> _actionFoldouts = new Dictionary<int, bool>();
        private readonly Dictionary<string, bool> _characterFoldouts = new Dictionary<string, bool>();

        private void OnEnable()
        {
            _actionsProperty = serializedObject.FindProperty("actions");
            EnsureArrayInitialization();
        }

        private bool GetActionFoldout(int index) => 
            _actionFoldouts.TryGetValue(index, out var value) ? value : true;

        private bool GetCharacterFoldout(int actionIndex, int characterIndex) => 
            _characterFoldouts.TryGetValue($"{actionIndex}_{characterIndex}", out var value) ? value : true;
        #endregion

        public override void OnInspectorGUI()
        {
            InitializeStyles();
            serializedObject.Update();
            
            EditorGUILayout.LabelField("Dialogue Profile Editor-beta", _headerStyle);
            DrawGlobalControls();
            DrawActionsList();
            DrawGlobalControls();
            
            if (GUI.changed) serializedObject.ApplyModifiedProperties();
        }

        #region Editor.Drawing Methods
        private void DrawGlobalControls()
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Action")) AddAction();
            EditorGUILayout.Space();
        }

        private void DrawActionsList()
        {
            for (int i = 0; i < _actionsProperty.arraySize; i++)
            {
                var action = _actionsProperty.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginVertical(_boxStyle);
                
                DrawActionHeader(i, action);
                if (GetActionFoldout(i)) DrawActionContent(i, action);
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(10);
            }
        }

        private void DrawActionHeader(int index, SerializedProperty action)
        {
            var foldoutKey = $"Action_{index}";
            var nameProperty = action.FindPropertyRelative("name");
            
            EditorGUILayout.BeginHorizontal();
            _actionFoldouts[index] = EditorGUILayout.Foldout(
                GetActionFoldout(index),
                $"Action {index + 1}: {nameProperty.stringValue}",
                true,
                _boldStyle
            );

            if (GUILayout.Button("×", GUILayout.Width(20)))
            {
                RemoveAction(index);
                return;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawActionContent(int actionIndex, SerializedProperty action)
        {
            EditorGUI.indentLevel++;
            
            DrawBasicActionFields(action);
            DrawCharacterSettings(actionIndex, action);
            
            EditorGUI.indentLevel--;
        }

        private void DrawBasicActionFields(SerializedProperty action)
        {
            //try
            //{
                EditorGUILayout.PropertyField(action.FindPropertyRelative("name"));
                EditorGUILayout.PropertyField(action.FindPropertyRelative("dialogue"));
                EditorGUILayout.PropertyField(action.FindPropertyRelative("audio"));
                //EditorGUILayout.PropertyField(action.FindPropertyRelative("backGround"));
                var imageProp = action.FindPropertyRelative("backGround");
                    var rect = EditorGUILayout.GetControlRect(GUILayout.Height(70));
                    imageProp.objectReferenceValue = EditorGUI.ObjectField(
                        rect,
                        "backGround",
                        imageProp.objectReferenceValue, 
                        typeof(Sprite), 
                        false
                    );
                EditorGUILayout.PropertyField(action.FindPropertyRelative("bg_Music"));
                EditorGUILayout.PropertyField(action.FindPropertyRelative("environmentSettings"));
                
                Separator(4,0);
            //}
            //catch{}
        }

        private void DrawCharacterSettings(int actionIndex, SerializedProperty action)
        {
            var characters = action.FindPropertyRelative("characterOptions");
            EditorGUILayout.LabelField("Character Settings", _headerStyle);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Characters ({characters.arraySize})");
            if (GUILayout.Button("+", GUILayout.Width(30))) AddCharacter(characters);
            EditorGUILayout.EndHorizontal();
            Separator(2,0);
            for (int i = 0; i < characters.arraySize; i++)
            {
                var character = characters.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginVertical(_boxStyle);
                
                DrawCharacterHeader(actionIndex, i, characters, character);
                if (GetCharacterFoldout(actionIndex, i)) DrawCharacterContent(character);
                EditorGUILayout.EndVertical();
                Separator(0.25f, 0f);
            }
        }

        private void DrawCharacterHeader(int actionIndex, int characterIndex, SerializedProperty characters, SerializedProperty character)
        {
            EditorGUILayout.BeginHorizontal();
            var foldoutKey = $"{actionIndex}_{characterIndex}";
            _characterFoldouts[foldoutKey] = EditorGUILayout.Foldout(
                GetCharacterFoldout(actionIndex, characterIndex),
                $"Character {characterIndex + 1}",
                true
            );

            if (GUILayout.Button("×", GUILayout.Width(20)))
            {
                RemoveCharacter(characters, characterIndex);
                return;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCharacterContent(SerializedProperty character)
        {
            try
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.PropertyField(character.FindPropertyRelative("characterDefindID"));
                //EditorGUILayout.PropertyField(character.FindPropertyRelative("characterImage"));
                var imageProp = character.FindPropertyRelative("characterImage");
                var rect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Min(((Sprite)imageProp.objectReferenceValue).texture.height, 70)));
                imageProp.objectReferenceValue = EditorGUI.ObjectField(
                    rect,
                    "Character Image",
                    imageProp.objectReferenceValue, 
                    typeof(Sprite), 
                    false
                );
                //var imageProp = character.FindPropertyRelative("characterImage");
                //var currentSprite = imageProp.objectReferenceValue as Sprite;
                //EditorGUILayout.BeginHorizontal();
                //if (GUILayout.Button(currentSprite?.texture ?? Texture2D.whiteTexture, _thumbnailStyle))
                //{
                //    EditorGUIUtility.ShowObjectPicker<Sprite>(currentSprite, false, "", GUIUtility.GetControlID(FocusType.Passive));
                //}

                //if (Event.current.commandName == "ObjectSelectorUpdated")
                //{
                //    if (EditorGUIUtility.GetObjectPickerControlID() == GUIUtility.GetControlID(FocusType.Passive))
                //    {
                 //       imageProp.objectReferenceValue = EditorGUIUtility.GetObjectPickerObject();
                //    }
                //}

                //if (GUILayout.Button("Clear", GUILayout.Width(60)))
                //{
                //    imageProp.objectReferenceValue = null;
                //}
                //EditorGUILayout.EndHorizontal();
                

                EditorGUILayout.PropertyField(character.FindPropertyRelative("motionMode"));
                
                var motionMode = (MotionPresents)character.FindPropertyRelative("motionMode").enumValueIndex;
                if (motionMode == MotionPresents.None)
                {
                    EditorGUILayout.PropertyField(character.FindPropertyRelative("behaviourMode"));
                }
                
                EditorGUILayout.PropertyField(character.FindPropertyRelative("action_Duration"));
                EditorGUILayout.PropertyField(character.FindPropertyRelative("action_Ease"));

                if (motionMode == MotionPresents.Custom)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(character.FindPropertyRelative("useOrigin"));
                    if (character.FindPropertyRelative("useOrigin").boolValue)
                    {
                        EditorGUILayout.PropertyField(character.FindPropertyRelative("origin"));
                    }
                    EditorGUILayout.PropertyField(character.FindPropertyRelative("appointedPosition"));
                }
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(character.FindPropertyRelative("ArrangeByListOrder"));
                if (!character.FindPropertyRelative("ArrangeByListOrder").boolValue)
                {
                    EditorGUILayout.PropertyField(character.FindPropertyRelative("CustomOrder"));
                }
                
                EditorGUI.indentLevel--;
                Separator(1);
            }
            catch{}
        }
        #endregion

        #region Deitor/Utility Methods
        private void EnsureArrayInitialization()
        {
            if (_actionsProperty.arraySize == 0) 
                _actionsProperty.InsertArrayElementAtIndex(0);
        }

        private void AddAction()
        {
            _actionsProperty.InsertArrayElementAtIndex(_actionsProperty.arraySize);
            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveAction(int index)
        {
            _actionsProperty.DeleteArrayElementAtIndex(index);
            _actionFoldouts.Remove(index);
            serializedObject.ApplyModifiedProperties();
        }

        private void AddCharacter(SerializedProperty characters)
        {
            characters.InsertArrayElementAtIndex(characters.arraySize);
            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveCharacter(SerializedProperty characters, int index)
        {
            characters.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
        }

        private void Separator(float height = 1, float space = 5)
        {
            GUILayout.Space(space);
            var rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.DrawRect(rect, Color.gray);
            GUILayout.Space(space);
        }
        #endregion
    }
    #endif
    */
}