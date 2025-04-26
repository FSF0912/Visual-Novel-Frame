using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FSF.VNG
{
    #region structures
    [Serializable]
    public struct SpriteDefinition
    {
        public string DefinitionName;
        public Sprite ImageSprite;
    }

    [Serializable]
    public class CharacterDefinition
    {
        public int CharacterId = 0;
        public List<SpriteDefinition> BasePortraits = new();
        public List<SpriteDefinition> Expressions = new();
    }

    [Serializable]
    public struct AudioDefinition
    {
        public string DefinitionName;
        public AudioClip Audio;
    }
    #endregion

    [CreateAssetMenu(fileName = "Game Definition Profile", menuName = "Visual Novel/GameDefinitionAsset", order = 120)]
    public sealed class GameDefinitionAsset : ScriptableObject
    {
        public List<CharacterDefinition> CharacterDefinitions = new ();
        public List<SpriteDefinition> BackGroundDefinitions = new ();
        public List<AudioDefinition> AudioDefinitions = new ();

        public Sprite GetPortrait(int characterDefindID, string PortraitName)
        {
            return CharacterDefinitions.FirstOrDefault(x => x.CharacterId == characterDefindID)
            .BasePortraits.FirstOrDefault(a => a.DefinitionName == PortraitName).ImageSprite;
        }

        public Sprite GetExpression(int characterDefindID, string ExpressionName)
        {
            return CharacterDefinitions.FirstOrDefault(x => x.CharacterId == characterDefindID)
            .Expressions.FirstOrDefault(a => a.DefinitionName == ExpressionName).ImageSprite;
        }

        public (Sprite, Sprite) GetPortraitAndExpression(int characterDefindID, string PortraitName, string ExpressionName)
        {
            var charDef = CharacterDefinitions.FirstOrDefault(x => x.CharacterId == characterDefindID);
            (Sprite, Sprite) temp;
            temp.Item1 = charDef.BasePortraits.FirstOrDefault(a => a.DefinitionName == PortraitName).ImageSprite;
            temp.Item2 = charDef.Expressions.FirstOrDefault(b => b.DefinitionName == ExpressionName).ImageSprite;
            return temp;
        }

        public Sprite GetBackGround(string name)
        {
            return BackGroundDefinitions.FirstOrDefault(x => x.DefinitionName == name).ImageSprite;
        }

        public AudioClip GetAudio(string name)
        {
            return AudioDefinitions.FirstOrDefault(x => x.DefinitionName == name).Audio;
        }
    }
}