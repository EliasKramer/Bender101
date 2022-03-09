using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities2D {

	public class DoublePair2D {

		public Vector2D A;
		public Vector2D B;
		public Vector2D C;

		public DoublePair2D(Vector2D pointA, Vector2D pointB, Vector2D pointC) {
			A = pointA;
			B = pointB;
			C = pointC;
		}

		static public List<DoublePair2D> GetList(List<Vector2D> list, bool connect = true) {
			List<DoublePair2D> pairsList = new List<DoublePair2D>();
			if (list.Count > 0) {
				foreach (Vector2D pB in list) {
					int indexB = list.IndexOf (pB);

					int indexA = (indexB - 1);
					if (indexA < 0) {
						indexA += list.Count;
					}

					int indexC = (indexB + 1);
					if (indexC >= list.Count) {
						indexC -= list.Count;
					}

					pairsList.Add (new DoublePair2D (list[indexA], pB, list[indexC]));
				}
			}
			return(pairsList);
		}
	}

	public struct DoublePair2 {
		public Vector2 a;
		public Vector2 b;
		public Vector2 c;

		public DoublePair2(Vector2 pointA, Vector2 pointB, Vector2 pointC) {
			a = pointA;
			b = pointB;
			c = pointC;
		}

		static public List<DoublePair2> GetList(Vector2List list, bool connect = true) {
			List<DoublePair2> pairsList = new List<DoublePair2>();
			if (list.Count() > 0) {
				int count = list.points.Count;
				for(int i = 0; i < count; i++) {
					int indexB = i;
					int indexA = ((i + count) - 1) % count;
					int indexC = (i + 1) % count;

					pairsList.Add (new DoublePair2 (list.points[indexA], list.points[indexB], list.points[indexC]));				
				}
			}
			return(pairsList);
		}
	}
	
}
