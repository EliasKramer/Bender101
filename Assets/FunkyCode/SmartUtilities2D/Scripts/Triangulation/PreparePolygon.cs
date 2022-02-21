using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Utilities2D {
		
	public class PreparePolygon {
		static float precision = 0.001f;

		// Not finished - still has some artifacts
		static Vector2D pairA = Vector2D.Zero(), pairC = Vector2D.Zero(); 
		static Vector2D vecA = Vector2D.Zero(), vecC = Vector2D.Zero(); 
		static DoublePair2D pair = new DoublePair2D(Vector2D.Zero(), Vector2D.Zero(), Vector2D.Zero());
		static Pair2D pair0 = new Pair2D(Vector2D.Zero(), Vector2D.Zero());
		static Pair2D pair1 = new Pair2D(Vector2D.Zero(), Vector2D.Zero());

		static public List<Vector2D> Get(Polygon2D polygon, float multiplier = 1f) {
			Polygon2D newPolygon = new Polygon2D();

			polygon.Normalize();

			Vector2D result;

			double rotA, rotC;
			Vector2D pB;
			for(int i = 0; i < polygon.pointsList.Count; i++) {
				pB = polygon.pointsList[i];
				
				int indexB = polygon.pointsList.IndexOf (pB);

				int indexA = (indexB - 1);
				if (indexA < 0) {
					indexA += polygon.pointsList.Count;
				}

				int indexC = (indexB + 1);
				if (indexC >= polygon.pointsList.Count) {
					indexC -= polygon.pointsList.Count;
				}

				pair.A = polygon.pointsList[indexA];
				pair.B = pB;
				pair.C = polygon.pointsList[indexC];

				rotA = Vector2D.Atan2(pair.B, pair.A);
				rotC = Vector2D.Atan2(pair.B, pair.C);

				pairA.x = pair.A.x;
				pairA.y = pair.A.y;
				pairA.Push(rotA - Mathf.PI / 2, precision * multiplier);

				pairC.x = pair.C.x;
				pairC.y = pair.C.y;
				pairC.Push(rotC + Mathf.PI / 2, precision * multiplier);
				
				vecA.x = pair.B.x;
				vecA.y = pair.B.y;
				vecA.Push(rotA - Mathf.PI / 2, precision * multiplier);
				vecA.Push(rotA, 100f);

				vecC.x = pair.B.x;
				vecC.y = pair.B.y;
				vecC.Push(rotC + Mathf.PI / 2, precision * multiplier);
				vecC.Push(rotC, 100f);

				pair0.A = pairA;
				pair0.B = vecA;

				pair1.A = pairC;
				pair1.B = vecC;

				result = Math2D.GetPointLineIntersectLine(pair0, pair1);

				if (result != null) {
					newPolygon.AddPoint(result);
				}
			}

			return(newPolygon.pointsList);
		} 
	}
}