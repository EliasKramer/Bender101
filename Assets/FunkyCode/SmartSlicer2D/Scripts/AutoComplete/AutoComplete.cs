using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D.Extensions;
using Utilities2D;

namespace Slicer2D {

	public class AutoComplete {
		static public Pair2 GetPair(Pair2 pair, float distance) {
			float direction = pair.a.Atan2(pair.b);
			Vector2 resultA = pair.a;
			Vector2 resultB = pair.b;
			
			Vector2 pointA = pair.a;
			Vector2 pointB = pair.b;

			pointA = pointA.Push(direction, distance);
			pointB = pointB.Push(direction, -distance);

			Sliceable2D slicerA = Sliceable2D.PointInSlicerComponent(pair.a.ToVector2D());
			Sliceable2D slicerB = Sliceable2D.PointInSlicerComponent(pair.b.ToVector2D());

			Pair2 thresholdPairA = new Pair2(pair.a, pointA);
			Pair2 thresholdPairB = new Pair2(pair.b, pointB);

			if (slicerA != null) {
				List<Vector2D> pointsA = slicerA.shape.GetWorld().GetListLineIntersectPoly(thresholdPairA.ToPair2D());

				if (pointsA.Count > 0) {
					pointsA = Vector2DList.GetListSortedToPoint(pointsA, pointA.ToVector2D());

					resultA = pointsA[pointsA.Count - 1].ToVector2();
					resultA.Push(direction, 0.05f);
				}
			}

			if (slicerB != null) {
				List<Vector2D> pointsB = slicerB.shape.GetWorld().GetListLineIntersectPoly(thresholdPairB.ToPair2D());

				if (pointsB.Count > 0) {
					pointsB = Vector2DList.GetListSortedToPoint(pointsB, pointB.ToVector2D());

					resultB = pointsB[pointsB.Count - 1].ToVector2();
					resultB.Push(direction, -0.05f);
				}
			}
			return(new Pair2(resultA, resultB));
		}

		static public Vector2List GetPoints(Vector2List list, float distance) {
			if (list.Count() < 2) {
				return(list);
			}

			Vector2List result = list.Copy();

			float directionA = list.points[0].Atan2(list.points[1]);
			float directionB = list.points[list.points.Count - 2].Atan2(list.points[list.points.Count - 1]);
			
			Vector2 pointA = list.points[0];
			Vector2 pointB = list.points[list.points.Count - 1];

			pointA.Push(directionA, distance);
			pointB.Push(directionB, -distance);

			Sliceable2D slicerA = Sliceable2D.PointInSlicerComponent(list.points[0].ToVector2D());
			Sliceable2D slicerB = Sliceable2D.PointInSlicerComponent(list.points[list.points.Count - 1].ToVector2D());

			Pair2 thresholdPairA = new Pair2(list.points[0], pointA);
			Pair2 thresholdPairB = new Pair2(list.points[list.points.Count - 1], pointB);

			Vector2D resultA = null;
			Vector2D resultB = null;

			if (slicerA != null) {
				List<Vector2D> pointsA = slicerA.shape.GetWorld().GetListLineIntersectPoly(thresholdPairA.ToPair2D());

				if (pointsA.Count > 0) {
					pointsA = Vector2DList.GetListSortedToPoint(pointsA, pointA.ToVector2D());

					resultA = pointsA[pointsA.Count - 1];
					resultA.Push(directionA, 0.05f);
				}
			}

			if (slicerB != null) {
				List<Vector2D> pointsB = slicerB.shape.GetWorld().GetListLineIntersectPoly(thresholdPairB.ToPair2D());

				if (pointsB.Count > 0) {
					pointsB = Vector2DList.GetListSortedToPoint(pointsB, pointB.ToVector2D());

					resultB = pointsB[pointsB.Count - 1];
					resultB.Push(directionB, -0.05f);
				}
			}

			if (resultA != null) {
				result.Insert(0, resultA.ToVector2());
			}

			if (resultB != null) {
				result.Add(resultB.ToVector2());
			}

			return(result);
		}
	}
}