using DG.Tweening;
using FSF.Collection;
using UnityEngine.UI;

namespace FSF.DialogueSystem{
    public class TypeWriter : MonoSingleton<TypeWriter>
    {
        public Text Name, Dialogue;
        Tween typer;
        string current_Dialogue;
        bool done = true;

        public bool OutputText(string name, string dialogue)
        {
            if (done)
            {
                typer?.Kill();
                current_Dialogue = dialogue;
                Name.text = name;
                Dialogue.text = string.Empty;
                typer = Dialogue.DOText(dialogue, dialogue.Length * Dialogue_Configs.textTypeDuration).SetEase(Ease.Linear).OnComplete(()=>{
                    done = true;
                });
                done = false;
                return true;
            }
            else
            {
                typer?.Kill();
                Dialogue.text = current_Dialogue;
                current_Dialogue = dialogue;
                done = true;
                return false;
            }
        }
    }
}
