using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {
	
	public class ShapeFillResult {
		public int allPoints;
		public int pointsIn;
		public float percentage;

		public ShapeFillResult(ShapeFill shapeFill) {
			allPoints = shapeFill.pointsIn.Count;

			pointsIn = 0;

			Vector2 pointInWorld;
			Vector2D pointInWorld2D = Vector2D.Zero();

			foreach(Vector2D point in shapeFill.pointsIn) {
				pointInWorld = shapeFill.transform.TransformPoint(point.ToVector2());

				pointInWorld2D.x = pointInWorld.x;
				pointInWorld2D.y = pointInWorld.y;

				if (ShapeObject.PointInShapes(pointInWorld2D) == false) {
					pointsIn ++;
				} 			
			}

			percentage = (float)pointsIn / allPoints;
		}
	}
}