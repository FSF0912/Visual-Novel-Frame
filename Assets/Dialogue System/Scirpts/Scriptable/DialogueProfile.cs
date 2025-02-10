using UnityEngine;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FSF.DialogueSystem
{
    #region Structures
    [System.Serializable]
    public struct SingleAction
    {
        public string name;
        public string dialogue;
        public AudioClip audio;
        
        public Sprite backGround;
        public EnvironmentSettings environmentSettings;
        public CharacterOption[] imageOptions;
        public bool TillingCharacters;

        public SingleAction(string name, string dialogue, CharacterOption[] options, 
        AudioClip audio, Sprite backGround, EnvironmentSettings settings, bool TillingCharacters){
            this.name = name;
            this.dialogue = dialogue;
            imageOptions = options;
            this.audio = audio;
            this.backGround = backGround;
            environmentSettings = settings;
            this.TillingCharacters = TillingCharacters;
        }

        
    }
    //
    [System.Serializable]
    public class CharacterOption
    {
        public int characterDefindID;
        public Sprite characterImage;
        public MotionPresents motionMode = MotionPresents.None;

        [Header("Enable when motionMode = MotionPresents.None.")]
        public CharacterBehaviourMode behaviourMode = CharacterBehaviourMode.None;
        public float action_Duration = 0.6f;
        public Ease action_Ease = Ease.InOutSine;
        [Header("Custom Options")]
        public bool useOrigin = false;
        public Vector2 origin = Vector2.zero;
        ///
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
    }
    //
    #endregion

    #region Enums
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
        Custom
    };

    public enum CharacterBehaviourMode
    {
        None,
        ShakeHeavily,
        ShackSlightly,
        [InspectorName("←HorizontalMove→")]
        HorizontalMove,//→→→→→→→→←←←←←←←←
        [InspectorName("↑VerticalMove↓")]
        VerticalMove//↑↑↑↑↑↑↑↑↑↓↓↓↓↓↓↓↓↓
    };

    public enum DistanceState
    {
        None,
        Normal,
        Far,
        Near
    };

    public enum CharacterFadeType
    {
        Normal,
        FadeIn,
        FadeOut
    };
#endregion



    [CreateAssetMenu(fileName = "DialogueProfile", menuName = "FSF_Custom/DialogueSystem/DialogueProfile", order = 30000)]
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
}