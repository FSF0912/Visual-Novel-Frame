using System;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace FSF.Collection
{
    public enum PopupMode
    {
        Cover,
        Await
    };
    public class PopupMenu : MonoSingleton<PopupMenu>
    {
        public Text TopNoticeText;
        public Animation Top_Animation;
        [Space]
        public Text central_Title;
        public Text central_Text;
        public Animation Central_Animation;
        public string animName;
        
        public GameObject button_Prefab;
        public RectTransform central_ButtonsHolder;

       //central
        protected override void OnAwake()
        {
            if (Top_Animation.GetClipCount() > 1 || Central_Animation.GetClipCount() != 2)
            {
                Debug.LogWarning("Multiple animation clip included.\nAnimations may not be played correctly.");
            }
        }

        public void TopPanel(string message)
        {
            Top_Animation.Stop();
            TopNoticeText.text = message;
            Top_Animation.Play();
        }

        public async UniTask TopPanelAsync(string message)
        {
            Top_Animation.Stop();
            TopNoticeText.text = message;
            Top_Animation.Play();
            await UniTask.WaitForSeconds(Top_Animation.clip.length);
        }

        public void CentralPanel(string title, string message, params ValueTuple<string, Action>[] actions)
        {
            if (central_ButtonsHolder.childCount > 0)
            {
                for (int i = central_ButtonsHolder.childCount - 1; i >= 0; i--)
                {
                    Destroy(central_ButtonsHolder.GetChild(i).gameObject);
                }
            }

            foreach (var action in actions)
            {
                var current = Instantiate(button_Prefab, central_ButtonsHolder).GetComponent<PopupButtonSingle>();
                current.text.text = action.Item1;
                current.button.onClick.AddListener(()=> {
                action.Item2.Invoke();
                Central_Animation.Stop();
                Central_Animation[animName].time = Central_Animation[animName].length;
                Central_Animation[animName].speed = -1;
                Central_Animation.Play();
                });
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(central_ButtonsHolder);

            central_Title.text = title;
            central_Text.text = message;
            Central_Animation.Stop();
            Central_Animation[animName].speed = 1;
            Central_Animation.Play();
        }
    }
}
