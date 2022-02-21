using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities2D;
using Utilities2D.Extensions;

public class ComplexCut {
	public Vector2List pointsList;
	float size = 1f;

	static public ComplexCut Create(List<Vector2D> pointsList, float size) {
		ComplexCut cut = new ComplexCut();
		cut.size = size;
		cut.pointsList = new Vector2List(pointsList);
		return(cut);
	}

	public Vector2List GetPointsList(float multiplier = 1f){
		float sizeM = size * multiplier;
		float sizeM2 = 2 * sizeM;

		Vector2List list = pointsList;

		if (list.Count() < 2) {
			return(new Vector2List(true));
		}
		
		List<Vector2> newPointsListA = new List<Vector2>();
		List<Vector2> newPointsListB = new List<Vector2>();

		Pair2D pair0 = Pair2D.Zero();
		Pair2D pair1 = Pair2D.Zero();
		
		if (list.Count() > 2) {
			List<DoublePair2> pairList = DoublePair2.GetList(list);

			foreach(DoublePair2 pair in pairList) {
				float rotA = pair.b.Atan2(pair.a);
				float rotC = pair.b.Atan2(pair.c);

				Vector2 pairA = pair.a;
				pairA = pairA.Push(rotA - Mathf.PI / 2, sizeM);

				Vector2 pairC = pair.c;
				pairC = pairC.Push(rotC + Mathf.PI / 2, sizeM);
				
				Vector2 vecA = pair.b;
				vecA = vecA.Push(rotA - Mathf.PI / 2, sizeM);
				vecA = vecA.Push(rotA, 10f);

				Vector2 vecC = pair.b;
				vecC = vecC.Push(rotC + Mathf.PI / 2, sizeM);
				vecC = vecC.Push(rotC, 10f);

				pair0.A.x = pairA.x;
				pair0.A.y = pairA.y;
				pair0.B.x = vecA.x;
				pair0.B.y = vecA.y;

				pair1.A.x = pairC.x;
				pair1.A.y = pairC.y;
				pair1.B.x = vecC.x;
				pair1.B.y = vecC.y;

				Vector2D result = Math2D.GetPointLineIntersectLine(pair0, pair1);

				if (result != null) {
					newPointsListA.Add(result.ToVector2());
				}
			}

			if (newPointsListA.Count > 2) {
				newPointsListA.Remove(newPointsListA.First());
				newPointsListA.Remove(newPointsListA.Last());
			}

			foreach(DoublePair2 pair in pairList) {
				float rotA = pair.b.Atan2(pair.a);
				float rotC = pair.b.Atan2(pair.c);

				Vector2 pairA = pair.a;
				pairA = pairA.Push(rotA - Mathf.PI / 2, -sizeM);

				Vector2 pairC = pair.c;
				pairC = pairC.Push(rotC + Mathf.PI / 2, -sizeM);
				
				Vector2 vecA = pair.b;
				vecA = vecA.Push(rotA - Mathf.PI / 2, -sizeM);
				vecA = vecA.Push(rotA, 10f);

				Vector2 vecC = pair.b;
				vecC = vecC.Push(rotC + Mathf.PI / 2, -sizeM);
				vecC = vecC.Push(rotC, 10f);

				pair0.A.x = pairA.x;
				pair0.A.y = pairA.y;
				pair0.B.x = vecA.x;
				pair0.B.y = vecA.y;

				pair1.A.x = pairC.x;
				pair1.A.y = pairC.y;
				pair1.B.x = vecC.x;
				pair1.B.y = vecC.y;

				Vector2D result = Math2D.GetPointLineIntersectLine(pair0, pair1);

				if (result != null) {
					newPointsListB.Add(result.ToVector2());
				}
			}

			if (newPointsListB.Count > 2) {
				newPointsListB.Remove(newPointsListB.First());
				newPointsListB.Remove(newPointsListB.Last());
			}
		}

		Vector2List newPointsList = new Vector2List(true);
		foreach(Vector2 p in newPointsListA) {
			newPointsList.Add(p);
		}
		
		Vector2 prevA = list.points.ElementAt(list.points.Count - 2);

		Vector2 pA = list.Last();
		pA = pA.Push(pA.Atan2(prevA) - Mathf.PI / 6, sizeM2);
		newPointsList.Add(pA);

		pA = list.Last();
		pA = pA.Push(pA.Atan2(prevA) + Mathf.PI / 6, sizeM2);
		newPointsList.Add(pA);

		newPointsListB.Reverse();

		foreach(Vector2 p in newPointsListB) {
			newPointsList.Add(p);
		}

		Vector2 prevB = list.points.ElementAt(1);

		Vector2 pB = list.First();
		pB = pB.Push(pB.Atan2(prevB) - Mathf.PI / 6, sizeM2);
		newPointsList.Add(pB);

		pB = list.First();
		pB = pB.Push(pB.Atan2(prevB) + Mathf.PI / 6, sizeM2);
		newPointsList.Add(pB);

		return(newPointsList);
	}
}
