using UnityEngine;
using UnityEngine.UI;

namespace FSF.VNG
{
    public class SingleBranch : MonoBehaviour
    {
        public Button button;
        public Text text;
        private void Start()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (text == null)
            {
                text = GetComponentInChildren<Text>();
            }
        }

        public void SetText(string text)
        {
            this.text.text = text;
        }
    }
}