using DG.Tweening;
using FSF.Collection;
using UnityEngine.UI;
using UnityEngine;

namespace FSF.VNG
{
    public class TypeWriter : MonoSingleton<TypeWriter>
    {
        public Text Name;
        public Text Dialogue;
        
        public Tween typer_Tween;
        private string _currentDialogue;
        private bool _isTyping;
        private float _baseDurationPerChar;

        protected override void OnAwake()
        {
            _baseDurationPerChar = Dialogue_Configs.textTypeDuration;
        }

        public bool OutputText(string name, string dialogue)
        {
            //if (string.IsNullOrEmpty(dialogue)) return false;

            // 重用Tween对象优化性能[1,3](@ref)
            if (_isTyping)
            {
                // 安全停止动画[4](@ref)
                if (typer_Tween != null && typer_Tween.IsActive())
                {
                    typer_Tween.Kill(true);
                    Dialogue.text = _currentDialogue;
                }
                _currentDialogue = string.Empty;

                return false;
            }

            _currentDialogue = dialogue;
            Name.text = name;
            Dialogue.text = string.Empty;
            // 配置可复用的Tween[3](@ref)
            typer_Tween = Dialogue.DOText(dialogue, dialogue.Length * _baseDurationPerChar)
                .SetEase(Ease.Linear)
                .SetAutoKill(false)
                .OnStart(() => _isTyping = true)
                .OnComplete(OnTweenComplete)
                .OnKill(() => _isTyping = false);

            return true;
        }

        private void OnTweenComplete()
        {
            _isTyping = false;
            // 可选：触发音效或后续事件[3](@ref)
        }
    }
}