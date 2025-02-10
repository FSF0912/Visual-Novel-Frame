using FSF.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public Character shower;
    public Sprite s1, s2;
    public Button set;
    bool enab;
    private void Start() {
        set.onClick.AddListener(()=>{
            shower.OutputImage(enab ? s1 : s2);
            enab = !enab;
        });
    }
}
