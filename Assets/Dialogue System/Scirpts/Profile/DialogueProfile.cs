using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FSF.VNG
{
    #region Data/Structures
    [Serializable]
    public class SingleAction
    {
        public string name = "Jenny";
        [TextArea(1, 15)] public string dialogue = "Hello!";
        public string voiceIndex;
        public string backGround;
        public string Music;
        public EnvironmentSettings environmentSettings;
        public CharacterOption[] characterOptions;
        public bool isBranch;
        public BranchOption[] branchOptions;

        public SingleAction(){}
    }

    [Serializable]
    public class CharacterOption
    {
        public int characterDefindID;
        public string characterImage;
        public string characterExpression;
        public CharacterPresenceStatus presenceStatus = CharacterPresenceStatus.None;
        public MotionPresents motionMode = MotionPresents.None;
        [Space(10f)]
        public float action_Duration = 0.6f;
        public Ease action_Ease = Ease.InOutSine;
        [JsonIgnore]
        public Vector2 appointedPosition = Vector2.zero;

        /// <summary>
        /// <para>
        /// 采用列表顺序决定场景中的角色顺序。
        /// </para>
        /// </summary>
        public bool SortByListOrder = true;

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
            int characterDefindID, string characterImage, string characterExpression,

            MotionPresents motionMode,

            float action_Duration, Ease action_Ease, 

             Vector2 appointedPosition, 

            bool ArrangeByListOrder, int CustomOrder
        )
        {
            this.characterDefindID = characterDefindID;
            this.characterImage = characterImage;
            this.characterExpression = characterExpression;
            this.motionMode = motionMode;
            this.action_Duration = action_Duration;
            this.action_Ease = action_Ease;
            this.appointedPosition = appointedPosition;
            this.SortByListOrder = ArrangeByListOrder;
            this.CustomOrder = CustomOrder;
        }
    }
    //
    [Serializable]
    public struct BranchOption
    {
        public string BranchText;
        public int jumpIndex;
        public int endIndex;
        public int returnIndex;

        public BranchOption(string BranchText, int jumpIndex, int endIndex, int returnIndex)
        {
            this.BranchText = BranchText;
            this.jumpIndex = jumpIndex;
            this.endIndex = endIndex;
            this.returnIndex = returnIndex;
        }
    } 

    [Serializable]
    public struct EpisodeSymbol
    {
        public int Chapter;
        public int Scene;

        public EpisodeSymbol(int Chapter, int Scene)
        {
            this.Chapter = Chapter;
            this.Scene = Scene;
        }

        public int this[int index] => index switch
        {
            0 => Chapter,
            1 => Scene,
            _ => throw new IndexOutOfRangeException()
        };
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



    //[CreateAssetMenu(fileName = "DialogueProfile", menuName = "VNG/DialogueProfile", order = 30000)]
    [Serializable]
    public sealed class DialogueProfile// : ScriptableObject
    {
        public EpisodeSymbol episode = new EpisodeSymbol(1, 1);
        public SingleAction[] actions = Array.Empty<SingleAction>();
        [JsonIgnore]
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