using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {
	
	public enum ShapeMatchType {World, Local};

	public class ShapeMatchResult {
		public int allPoints;
		public int pointsIn;
		public float percentage;

		public ShapeMatchResult(ShapeObject shapeA, ShapeObject shapeB, ShapeMatchType type) {
			allPoints = shapeA.pointsIn.Count + shapeB.pointsIn.Count;
			pointsIn = 0;

			switch(type) {
				case ShapeMatchType.Local:
					CalculateLocal(shapeA, shapeB, type);
					break;

				case ShapeMatchType.World:
					CalculateWorld(shapeA, shapeB, type);
				break;
			}
			percentage = (float)pointsIn / allPoints;
		}

		void CalculateLocal(ShapeObject shapeA, ShapeObject shapeB, ShapeMatchType type) {
			Vector2 pointInWorld;
			Vector2D pointInWorld2D = Vector2D.Zero();
			Polygon2D polyInWorld;

			foreach(Vector2D point in shapeA.pointsIn) {
				pointInWorld = point.ToVector2();

				pointInWorld2D.x = pointInWorld.x;
				pointInWorld2D.y = pointInWorld.y;
			
				polyInWorld = shapeB.GetPolygon();

				if (polyInWorld.PointInPoly(pointInWorld2D)) {
					pointsIn ++;
				}
			}

			foreach(Vector2D point in shapeB.pointsIn) {
				pointInWorld = point.ToVector2();
				
				pointInWorld2D.x = pointInWorld.x;
				pointInWorld2D.y = pointInWorld.y;
				
				polyInWorld = shapeA.GetPolygon();

				if (polyInWorld.PointInPoly(pointInWorld2D)) {
					pointsIn ++;
				}
			}
		}

		void CalculateWorld(ShapeObject shapeA, ShapeObject shapeB, ShapeMatchType type) {
			Vector2 pointInWorld;
			Vector2D pointInWorld2D = Vector2D.Zero();
			Polygon2D polyInWorld;

			foreach(Vector2D point in shapeA.pointsIn) {
				pointInWorld = shapeA.transform.TransformPoint(point.ToVector2());
				pointInWorld2D.x = pointInWorld.x;
				pointInWorld2D.y = pointInWorld.y;
			
				polyInWorld = shapeB.GetWorldPolygon();

				if (polyInWorld.PointInPoly(pointInWorld2D)) {
					pointsIn ++;
				}
			}

			foreach(Vector2D point in shapeB.pointsIn) {
				pointInWorld = shapeB.transform.TransformPoint(point.ToVector2());
				pointInWorld2D.x = pointInWorld.x;
				pointInWorld2D.y = pointInWorld.y;
				
				polyInWorld = shapeA.GetWorldPolygon();

				if (polyInWorld.PointInPoly(pointInWorld2D)) {
					pointsIn ++;
				}
			}
		}
	}

}