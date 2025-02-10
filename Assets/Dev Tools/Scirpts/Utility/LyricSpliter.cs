using System.Collections.Generic;
using UnityEngine;

namespace FSF.Collection.Utilities{
    [System.Serializable]
    public class LyricValueKey{
        [TextArea(5,int.MaxValue)] public string Lyric;
        public float Time;
        public LyricValueKey(string Lyric, float Time){
            this.Lyric = Lyric;
            this.Time = Time;
        }
    }
    
    public static class LyricSpliter
    {
        /// <summary>
        /// <para>Here's a sample lyric:</para>
        /// <para>[00:00.01] song~~~~</para>
        /// <para>[00"00.01] is here~~~</para>
        /// <para>if "reverseTranslate" is true, outout result will like this:</para>
        /// <para>song~~~ \n is here~~~</para>
        /// <para>else: is here~~~ \n song~~~</para>
        /// 
        /// </summary>
        /// <param name="lrc"></param>
        /// <param name="reverseTranslate"></param>
        /// <returns></returns>
        public static List<LyricValueKey> Split(TextAsset lrc, bool reverseTranslate = false){
            var texts = lrc.text.Split('\n');
            List<LyricValueKey> value = new List<LyricValueKey>();
            LyricValueKey temp = null;
            for(int i = 0; i < texts.Length; i++){
                #region Time processing region
                var timeStr = texts[i].Remove(texts[i].IndexOf(']') + 1);
                timeStr = timeStr.Replace("[","");
                timeStr = timeStr.Replace("]","");
                var times = timeStr.Split(':');
                string Time1Str = times[0], Time2Str = times[1];
                if (!float.TryParse(Time1Str, out float Time1)){return null;}
                if (!float.TryParse(Time2Str, out float Time2)){return null;}
                float timeResult = Time1 * 60 + Time2;
                #endregion
                #region Lyric processing region
                string Lyric = texts[i].Remove(0, texts[i].IndexOf(']') + 1);
                #endregion
                var singleKey = new LyricValueKey(Lyric, timeResult);
                if(temp != null){
                    if(temp.Time == singleKey.Time){
                        temp.Lyric = reverseTranslate ? 
                        $"{temp.Lyric}\n{singleKey.Lyric}" : $"{singleKey.Lyric}\n{temp.Lyric}";
                    }
                    else{
                        value.Add(singleKey);
                        temp = singleKey;
                    }
                }
                else{
                    value.Add(singleKey);
                    temp = singleKey;
                }
            }
            return value;
        }
    }
}
