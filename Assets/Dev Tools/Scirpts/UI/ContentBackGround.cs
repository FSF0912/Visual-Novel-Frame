using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ContentBackGround : MonoBehaviour
{
    public Text referText;
    public float SpacingX = 30f;
    RectTransform self;
    void Start()
    {
        self = this.transform as RectTransform;
    }

    void Update()
    {
        self.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, referText.preferredWidth + SpacingX);
    }
}
