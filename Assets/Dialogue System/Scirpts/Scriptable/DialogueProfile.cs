using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FSF.VNG
{
    #region Data/Structures
    [Serializable]
    public struct SingleAction
    {
        public string name;
        [TextArea(1, 15)] public string dialogue;
        public AudioClip audio;
        public Sprite backGround;
        public AudioClip bg_Music;
        public EnvironmentSettings environmentSettings;
        public CharacterOption[] characterOptions;
        public bool isBranch;
        public int branchEndIndex;
        public BranchOption[] branchOptions;

        public SingleAction(
            string name, string dialogue, 

        AudioClip audio, Sprite backGround, AudioClip bg_Music, 

        EnvironmentSettings environmentSettings, CharacterOption[] characterOptions, 
        bool isBranch, int branchEndJumpIndex, BranchOption[] branchOptions
        )
        {
            this.name = name;
            this.dialogue = dialogue;
            this.audio = audio;
            this.backGround = backGround;
            this.bg_Music = bg_Music;
            this.environmentSettings = environmentSettings;
            this.characterOptions = characterOptions;
            this.isBranch = isBranch;
            this.branchEndIndex = branchEndJumpIndex;
            this.branchOptions = branchOptions;
        }
    }
    //
    /// <summary>
    /// will change to struct after build.(?
    /// </summary>
    [Serializable]
    public class CharacterOption
    {
        public int characterDefindID;
        public Sprite characterImage;
    #if VNG_EXPRESSION
        public Sprite characterExpression;
    #endif
        public CharacterPresenceStatus presenceStatus = CharacterPresenceStatus.None;
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

    #if !VNG_EXPRESSION
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
    #else
        public CharacterOption(
            int characterDefindID, Sprite characterImage, Sprite characterExpression,

            MotionPresents motionMode,

            float action_Duration, Ease action_Ease, 

            bool useOrigin, Vector2 origin, Vector2 appointedPosition, 

            bool ArrangeByListOrder, int CustomOrder
        )
        {
            this.characterDefindID = characterDefindID;
            this.characterImage = characterImage;
            this.characterExpression = characterExpression;
            this.motionMode = motionMode;
            this.action_Duration = action_Duration;
            this.action_Ease = action_Ease;
            this.useOrigin = useOrigin;
            this.origin = origin;
            this.appointedPosition = appointedPosition;
            this.ArrangeByListOrder = ArrangeByListOrder;
            this.CustomOrder = CustomOrder;
        }
    #endif
    }
    //
    [Serializable]
    public struct BranchOption
    {
        public string BranchText;
        public int jumpIndex;
        public int endIndex;

        public BranchOption(string BranchText, int jumpIndex, int endIndex)
        {
            this.BranchText = BranchText;
            this.jumpIndex = jumpIndex;
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
        Enter,
        Exit
    };
#endregion



    [CreateAssetMenu(fileName = "DialogueProfile", menuName = "VNG/DialogueProfile", order = 30000)]
    public sealed class DialogueProfile : ScriptableObject
    {
        public SingleAction[] actions = Array.Empty<SingleAction>();
        public SingleAction this[int index]
        {
            get
            {
                return actions[index];
            }

            set
            {
                actions[index] = value;
            }
        }
    
    
    }
}