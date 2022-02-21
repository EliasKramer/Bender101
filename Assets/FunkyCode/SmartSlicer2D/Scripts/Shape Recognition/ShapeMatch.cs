using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slicer2D {

	public class ShapeMatch : MonoBehaviour {
		public ShapeMatchType type = ShapeMatchType.World;

		public ShapeObject shapeA;
		public ShapeObject shapeB;

		public bool visualisation = true;
		public bool guiInfo = true;

		public static ShapeMatchResult GetMatch(ShapeObject shapeA, ShapeObject shapeB, ShapeMatchType type = ShapeMatchType.World) {
			return(new ShapeMatchResult(shapeA, shapeB, type));
		}

		public void Update() {
			if (ShapeMatchType.World == type) {
				if (visualisation == true) {
					ShapeDraw.DrawMatch(shapeA, shapeB);
				}
			}
		}

		// Debugging
		public void OnGUI() {
			if (guiInfo == true) {
				ShapeMatchResult result = GetMatch(shapeA, shapeB, type);
				GUI.Label(new Rect(0, 0, 500, 500), "Points Count: " + shapeA.pointsIn.Count + " + " + shapeB.pointsIn.Count + " = " + result.allPoints);
				GUI.Label(new Rect(0, 20, 500, 500), "Points Similarity: " + result.pointsIn + " (" + (int)(result.percentage * 100) + "%)");
			}
		}
	}
	
}