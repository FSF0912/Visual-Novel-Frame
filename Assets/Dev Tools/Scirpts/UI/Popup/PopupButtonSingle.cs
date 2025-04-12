using UnityEngine;
using UnityEngine.UI;

namespace FSF.Collection
{
    public class PopupButtonSingle : MonoBehaviour
    {
        public Text text;
        public Button button;
        private void Start()
        {
            if (text == null) text = GetComponentInChildren<Text>();
            if (button == null) button = GetComponent<Button>();
        }
    }
}