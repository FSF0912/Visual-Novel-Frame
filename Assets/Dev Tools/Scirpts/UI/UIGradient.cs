using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FSF.UI{
    public class UIGradient : BaseMeshEffect{
        public enum Type
    {
        Vertical,
        Horizontal
    }
    public Type GradientType = Type.Vertical;
    [Range(-1f, 1f)]
    public float Offset = 0f;
    public Gradient gradient;
    private List<UIVertex> vertexList = new List<UIVertex>();

    public override void ModifyMesh(VertexHelper helper)
    {
        if (!IsActive() || helper.currentVertCount == 0 || gradient == null) { return;}

        vertexList.Clear();
        helper.GetUIVertexStream(vertexList);

        int nCount = vertexList.Count;
        switch (GradientType)
        {
            case Type.Vertical:
                {
                    float fBottomY = vertexList[0].position.y;
                    float fTopY = vertexList[0].position.y;
                    float fYPos = 0f;

                    for (int i = nCount - 1; i >= 1; --i)
                    {
                        fYPos = vertexList[i].position.y;
                        if (fYPos > fTopY)
                            fTopY = fYPos;
                        else if (fYPos < fBottomY)
                            fBottomY = fYPos;
                    }

                    float fUIElementHeight = 1f / (fTopY - fBottomY);
                    UIVertex v = new UIVertex();

                    for (int i = 0; i < helper.currentVertCount; i++)
                    {
                        helper.PopulateUIVertex(ref v, i);
                        v.color = gradient.Evaluate((v.position.y - fBottomY) *
                        fUIElementHeight - Offset);
                        helper.SetUIVertex(v, i);
                    }
                }
                break;
            case Type.Horizontal:
                {
                    float fLeftX = vertexList[0].position.x;
                    float fRightX = vertexList[0].position.x;
                    float fXPos = 0f;

                    for (int i = nCount - 1; i >= 1; --i)
                    {
                        fXPos = vertexList[i].position.x;
                        if (fXPos > fRightX)
                            fRightX = fXPos;
                        else if (fXPos < fLeftX)
                            fLeftX = fXPos;
                    }

                    float fUIElementWidth = 1f / (fRightX - fLeftX);
                    UIVertex v = new UIVertex();

                    for (int i = 0; i < helper.currentVertCount; i++)
                    {
                        helper.PopulateUIVertex(ref v, i);
                        v.color = gradient.Evaluate((v.position.x - fLeftX) *
                        fUIElementWidth - Offset);
                        helper.SetUIVertex(v, i);
                    }
                }
                break;
            default:
                break;
        }
    }
    }
}
