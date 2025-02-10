using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace FSF.UI
{
	public enum GradientType
	{
		Horizontal,
		Vertical,
		LeftBank,
		RightBank
	}

	public class UIGradientByColor : BaseMeshEffect {
		public GradientType Type = GradientType.Vertical;
		public Color32 StartColor = Color.white;
		public Color32 EndColor = Color.black;

		public override void ModifyMesh(VertexHelper helper) {
			if (!IsActive() || helper.currentVertCount == 0) {
				return;
			}
			var vertices = new List<UIVertex>();
			helper.GetUIVertexStream(vertices);
			// 计算中间色，倾斜时需要使用
			var middleColor = Color.Lerp(StartColor, EndColor, 0.5f);
			// 根据不同类型设置网格每个顶点的颜色
			switch (Type)
			{
				case GradientType.Vertical:
					for (var i = 0; i < vertices.Count && vertices.Count - i >= 6;)
					{
						ChangeColor(ref vertices, i + 0, StartColor);
						ChangeColor(ref vertices, i + 1, StartColor);
						ChangeColor(ref vertices, i + 2, EndColor);
						ChangeColor(ref vertices, i + 3, EndColor);
						ChangeColor(ref vertices, i + 4, EndColor);
						ChangeColor(ref vertices, i + 5, StartColor);
						i += 6;
					}
					break;
				case GradientType.Horizontal:
					for (var i = 0; i < vertices.Count && vertices.Count - i >= 6;)
					{
						ChangeColor(ref vertices, i + 0, EndColor);
						ChangeColor(ref vertices, i + 1, StartColor);
						ChangeColor(ref vertices, i + 2, StartColor);
						ChangeColor(ref vertices, i + 3, StartColor);
						ChangeColor(ref vertices, i + 4, EndColor);
						ChangeColor(ref vertices, i + 5, EndColor);
						i += 6;
					}
					break;
				case GradientType.LeftBank:
					for (var i = 0; i < vertices.Count && vertices.Count - i >= 6;)
					{
						ChangeColor(ref vertices, i + 0, StartColor);
						ChangeColor(ref vertices, i + 1, middleColor);
						ChangeColor(ref vertices, i + 2, EndColor);
						ChangeColor(ref vertices, i + 3, EndColor);
						ChangeColor(ref vertices, i + 4, middleColor);
						ChangeColor(ref vertices, i + 5, StartColor);
						i += 6;
					}
					break;
				case GradientType.RightBank:
					for (var i = 0; i < vertices.Count && vertices.Count - i >= 6;)
					{
						ChangeColor(ref vertices, i + 0, middleColor);
						ChangeColor(ref vertices, i + 1, StartColor);
						ChangeColor(ref vertices, i + 2, middleColor);
						ChangeColor(ref vertices, i + 3, middleColor);
						ChangeColor(ref vertices, i + 4, EndColor);
						ChangeColor(ref vertices, i + 5, middleColor);
						i += 6;
					}
					break;
			}
			helper.Clear();
			helper.AddUIVertexTriangleStream(vertices);
		}

		private static void ChangeColor(ref List<UIVertex> verList, int index, Color color) {
			var temp = verList[index];
			temp.color *= color;
			verList[index] = temp;
		}
	}
}
