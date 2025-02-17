using DG.Tweening;
using FSF.Collection;
using UnityEngine.UI;

namespace FSF.VNG
{
    public class TypeWriter : MonoSingleton<TypeWriter>
    {
        public Text Name, Dialogue;
        public Tween typer_Tween;
        string current_Dialogue;
        bool done = true;

        public bool OutputText(string name, string dialogue)
        {
            if (done)
            {
                typer_Tween?.Kill();
                current_Dialogue = dialogue;
                Name.text = name;
                Dialogue.text = string.Empty;
                typer_Tween = Dialogue.DOText(dialogue, dialogue.Length * Dialogue_Configs.textTypeDuration).SetEase(Ease.Linear);
                typer_Tween.SetAutoKill(false);
                typer_Tween.OnComplete(()=>{
                    done = true;
                });
                done = false;
                return true;
            }
            else
            {
                typer_Tween?.Kill();
                Dialogue.text = current_Dialogue;
                current_Dialogue = dialogue;
                done = true;
                return false;
            }
        }
    }
}
