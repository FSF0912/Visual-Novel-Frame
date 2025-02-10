using UnityEngine;
using UnityEngine.UI;
namespace FSF.UI{

    [RequireComponent(typeof(Button))]
    public class Popup_OpenButton : MonoBehaviour
    {
        public PopupInterface popupInterface;
        private void Start(){
            GetComponent<Button>().onClick.AddListener(()=>{
                popupInterface.OperatePanel(this.transform.position,true);
            });
        }
    }
}
