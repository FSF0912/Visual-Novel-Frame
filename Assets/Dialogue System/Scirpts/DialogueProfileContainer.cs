// DialogueProfileContainer.cs （必须放在Assets目录下，不需要Editor文件夹）
#if UNITY_EDITOR
using UnityEngine;

namespace FSF.VNG
{
    // 容器类定义
    public class DialogueProfileContainer : ScriptableObject
    {
        public DialogueProfile profile = new DialogueProfile();
    }
}
#endif