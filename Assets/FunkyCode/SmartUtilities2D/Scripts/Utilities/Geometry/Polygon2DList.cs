using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Utilities2D {

	public class Polygon2DList : Polygon2D {

		static public Polygon2D GetBiggest(List<Polygon2D> list) {
			double area = -1e+10f;
			Polygon2D CutObject = null;
			foreach(Polygon2D poly in list) {
				if (poly.GetArea() > area) {
					CutObject = poly;
					area = poly.GetArea();
				}
			}
			return(CutObject);
		}

		static public Polygon2D GetSmallest(List<Polygon2D> list) {
			double area = 1e+10f;
			Polygon2D CutObject = null;
			foreach(Polygon2D poly in list) {
				if (poly.GetArea() < area) {
					CutObject = poly;
					area = poly.GetArea();
				}
			}
			return(CutObject);
		}

		static public void RemoveClosePoints(List<Vector2D> list, float closePrecision = 0.005f) {
			List<Vector2D> points = new List<Vector2D>(list);
			Pair2D pair = new Pair2D(list.Last(), null);

			//Vector2D lastPos = null;

			for(int i = 0; i < points.Count; i++) {
				pair.B = points[i];

				if (points.Count < 4) {
					break;
				}

				if (Vector2D.Distance(pair.A, pair.B) < closePrecision) {
					//Debug.LogWarning("Smart Utility 2D: Polygon points are too close");
					list.Remove(pair.A);
				} else {
					pair.A = pair.B;
				}
			}
		}

		// Get List Of Polygons from Collider (Usually Used Before Creating Slicer2D Object)
		static public List<Polygon2D> CreateFromPolygonColliderToWorldSpace(PolygonCollider2D collider) {
			List<Polygon2D> result = new List<Polygon2D> ();

			if (collider != null && collider.pathCount > 0) {
				Polygon2D newPolygon = new Polygon2D ();

				foreach (Vector2 p in collider.GetPath (0)) {
					newPolygon.AddPoint (p + collider.offset);
				}
				
				newPolygon = newPolygon.ToWorldSpace(collider.transform);

				result.Add (newPolygon);

				for (int i = 1; i < collider.pathCount; i++) {
					Polygon2D hole = new Polygon2D ();
					foreach (Vector2 p in collider.GetPath (i)) {
						hole.AddPoint (p + collider.offset);
					}

					hole = hole.ToWorldSpace(collider.transform);

					if (newPolygon.PolyInPoly (hole) == true) {
						newPolygon.AddHole(hole);
					} else {
						result.Add(hole);
					}
				}
			}
			return(result);
		}

		static public List<Polygon2D> CreateFromPolygonColliderToLocalSpace(PolygonCollider2D collider) {
			List<Polygon2D> result = new List<Polygon2D>();

			if (collider != null && collider.pathCount > 0) {
				Polygon2D newPolygon = new Polygon2D ();

				Vector2[] pointList = collider.GetPath (0);
				
				for(int x = 0; x < pointList.Length; x++) {
					newPolygon.AddPoint (pointList[x] + collider.offset);
				}

				result.Add(newPolygon);

				for (int i = 1; i < collider.pathCount; i++) {
					Polygon2D hole = new Polygon2D ();

					pointList = collider.GetPath (i);

					for(int x = 0; x < pointList.Length; x++) {
						hole.AddPoint (pointList[x] + collider.offset);
					}

					if (newPolygon.PolyInPoly (hole) == true) {
						newPolygon.AddHole (hole);
					} else {
						result.Add(hole);
					}
				}
			}
			return(result);
		}

		// Slower CreateFromCollider
		public static List<Polygon2D> CreateFromGameObject(GameObject gameObject) {
			ColliderType colliderType = GetColliderType(gameObject);

			return(CreateFromGameObject(gameObject, colliderType));
		}

		// Faster CreateFromCollider
		public static List<Polygon2D> CreateFromGameObject(GameObject gameObject, ColliderType colliderType) {
			List<Polygon2D> result = new List<Polygon2D>();
			switch (colliderType) {
				case ColliderType.Edge:
					result.Add(CreateFromEdgeCollider (gameObject.GetComponent<EdgeCollider2D> ()));
					break;
				case ColliderType.Polygon:
					result = CreateFromPolygonColliderToLocalSpace(gameObject.GetComponent<PolygonCollider2D> ());
					break;
				case ColliderType.Box:
					result.Add(CreateFromBoxCollider (gameObject.GetComponent<BoxCollider2D> ()));
					break;
				case ColliderType.Circle:
					result.Add(CreateFromCircleCollider (gameObject.GetComponent<CircleCollider2D> ()));
					break;
				case ColliderType.Capsule:
					result.Add(CreateFromCapsuleCollider (gameObject.GetComponent<CapsuleCollider2D> ()));
					break;
				default:
					break;
			}
			return(result);
		}
	}
}