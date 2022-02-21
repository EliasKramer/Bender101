using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;
using Utilities2D.Extensions;

namespace Slicer2D.Controller.Extended {
	
	[System.Serializable]
	public class PolygonController : Controller.Base {
		// Settings
		public Polygon2D.PolygonType polygonType = Polygon2D.PolygonType.Circle;
		public float polygonSize = 1;
		public bool polygonDestroy = true;	
		public int edgeCount = 30;

		public void Update(Vector2 pos) {
			float newPolygonSize = polygonSize + UnityEngine.Input.GetAxis("Mouse ScrollWheel");
			if (newPolygonSize > 0.05f) {
				polygonSize = newPolygonSize;
			}

			if (input.GetInputClicked()) {
				PolygonSlice (pos);
			}
		}

		private void PolygonSlice(Vector2 pos) {
			Polygon2D.defaultCircleVerticesCount = edgeCount;

			Polygon2D slicePolygon = Polygon2D.Create (polygonType, polygonSize * visuals.visualScale);
			
			Slicing.PolygonSliceAll(pos.ToVector2D(), slicePolygon, polygonDestroy, sliceLayer);
		}

		public void Draw(Transform transform) {
			visuals.Clear();
			visuals.GeneratePolygonMesh(input.GetInputPosition(), polygonType, polygonSize);
			visuals.Draw();
		}
	}
}