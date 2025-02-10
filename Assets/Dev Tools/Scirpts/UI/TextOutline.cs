using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FSF.UI{
    public class TextOutline : BaseMeshEffect
    {
        List<UIVertex> _uiVertices = new List<UIVertex>();
        [Range(0, 6)]
        public float outLineWidth = 1;
        public Color EdgeColor = Color.red;
        public Color TextColor = Color.white;
        public override void ModifyMesh(VertexHelper vh)
        {
            vh.GetUIVertexStream(_uiVertices);
            ModifyUIVertexs(_uiVertices);
            vh.Clear();
            vh.AddUIVertexTriangleStream(_uiVertices);
        }
        protected override void Start()
        {
            UseUVChannels();
        }
        private void UseUVChannels()
        {
            var shader = Shader.Find("Hidden/UITextOutline");
            base.graphic.material = new Material(shader);
            AdditionalCanvasShaderChannels v1 = base.graphic.canvas.additionalShaderChannels;
            var v2 = AdditionalCanvasShaderChannels.TexCoord1;
            if ((v1 & v2) != v2)
            {
                base.graphic.canvas.additionalShaderChannels |= v2;
            }
            v2 = AdditionalCanvasShaderChannels.TexCoord2;
            if ((v1 & v2) != v2)
            {
                base.graphic.canvas.additionalShaderChannels |= v2;
            }
        }
        void ModifyUIVertexs(List<UIVertex> uiVertices)
        {
            for (int i = 0; i <= uiVertices.Count - 3; i += 3)
            {
                UIVertex uiVertex1 = uiVertices[i];
                UIVertex uiVertex2 = uiVertices[i + 1];
                UIVertex uiVertex3 = uiVertices[i + 2];
                Vector3 pos1 = uiVertex1.position;
                Vector3 pos2 = uiVertex2.position;
                Vector3 pos3 = uiVertex3.position;

                Vector2 uv1 = uiVertex1.uv0;
                Vector2 uv2 = uiVertex2.uv0;
                Vector2 uv3 = uiVertex3.uv0;

                Vector3 pos_center = (pos1 + pos2 + pos3) / 3;
                Vector2 uv_min = new Vector2(Mathf.Min(uv1.x, uv2.x, uv3.x), Mathf.Min(uv1.y, uv2.y, uv3.y));
                Vector2 uv_max = new Vector2(Mathf.Max(uv1.x, uv2.x, uv3.x), Mathf.Max(uv1.y, uv2.y, uv3.y));
                Vector4 uv_border = new Vector4(uv_min.x, uv_min.y, uv_max.x, uv_max.y);
                
                Vector2 pos_base1 = pos1 - pos2;
                Vector2 pos_base2 = pos3 - pos2;

                Vector2 uv_base1 = uv1 - uv2;
                Vector2 uv_base2 = uv3 - uv2;

                uiVertices[i] = ModifyPosUV(uiVertex1, pos_center, pos_base1, pos_base2, uv_base1, uv_base2, uv_border);
                uiVertices[i + 1] = ModifyPosUV(uiVertex2, pos_center, pos_base1, pos_base2, uv_base1, uv_base2, uv_border);
                uiVertices[i + 2] = ModifyPosUV(uiVertex3, pos_center, pos_base1, pos_base2, uv_base1, uv_base2, uv_border);
            }
        }
        UIVertex ModifyPosUV(UIVertex uiVertex, Vector3 pos_centor,
            Vector2 pos_base1, Vector2 pos_base2,
            Vector2 uv_base1, Vector2 uv_base2, Vector4 uv_border)
        {
            Vector3 pos = uiVertex.position;
            float offsetX = pos.x > pos_centor.x ? outLineWidth : -outLineWidth;
            float offsetY = pos.y > pos_centor.y ? outLineWidth : -outLineWidth;
            pos.x += offsetX;
            pos.y += offsetY;
            uiVertex.position = pos;
            Vector2 offset = new Vector2(offsetX, offsetY);
            Vector2 uv = uiVertex.uv0;
            Matrix2x2 pos_m = new Matrix2x2(pos_base1.x,pos_base2.x,pos_base1.y,pos_base2.y);
            pos_m=pos_m.Inverse();
            Matrix2x2 uv_m = new Matrix2x2(uv_base1.x, uv_base2.x, uv_base1.y, uv_base2.y);
            Vector2 uv_offset = uv_m * pos_m * offset;
            uv += uv_offset;
            uiVertex.uv0 = new Vector4(uv.x, uv.y, outLineWidth, 0);
            uiVertex.uv1 = uv_border;
            uiVertex.uv2 = new Vector4(EdgeColor.r,EdgeColor.g,EdgeColor.b,EdgeColor.a);
            Color32 color32 = (Color32)TextColor;
            uiVertex.color =color32;
            return uiVertex;
        }


    }

    public class Matrix2x2
    {
        private float[,] matrix = new float[2, 2];

        public Matrix2x2(float a, float b, float c, float d)
        {
            matrix[0, 0] = a;
            matrix[0, 1] = b;
            matrix[1, 0] = c;
            matrix[1, 1] = d;
        }

        public float Determinant()
        {
            return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
        }

        public Matrix2x2 Inverse()
        {
            float det = Determinant();
            float invDet = 1 / det;

            float a = matrix[1, 1] * invDet;
            float b = -matrix[0, 1] * invDet;
            float c = -matrix[1, 0] * invDet;
            float d = matrix[0, 0] * invDet;

            return new Matrix2x2(a, b, c, d);
        }

        public static Matrix2x2 operator *(Matrix2x2 m1, Matrix2x2 m2)
        {
            float a = m1.matrix[0, 0] * m2.matrix[0, 0] + m1.matrix[0, 1] * m2.matrix[1, 0];
            float b = m1.matrix[0, 0] * m2.matrix[0, 1] + m1.matrix[0, 1] * m2.matrix[1, 1];
            float c = m1.matrix[1, 0] * m2.matrix[0, 0] + m1.matrix[1, 1] * m2.matrix[1, 0];
            float d = m1.matrix[1, 0] * m2.matrix[0, 1] + m1.matrix[1, 1] * m2.matrix[1, 1];

            return new Matrix2x2(a, b, c, d);
        }

        public static Vector2 operator *(Matrix2x2 m, Vector2 v)
        {
            float x = m.matrix[0, 0] * v.x + m.matrix[0, 1] * v.y;
            float y = m.matrix[1, 0] * v.x + m.matrix[1, 1] * v.y;

            return new Vector2(x, y);
        }
    }
}