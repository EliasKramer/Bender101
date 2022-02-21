using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Utilities2D;

namespace Slicer2D.Linear {

	public class SlicerExtended {

		static public Slice2D ExplodeInPoint(Polygon2D polygon, Vector2D point, int explosionSlices = 0) {
			Slice2D result = Slice2D.Create(null, point);

			if (polygon.PointInPoly (point) == false) {
				return(result);
			}

			return(Explode(polygon));
		}

		static public Slice2D ExplodeByPoint(Polygon2D polygon, Vector2D point, int explosionSlices = 0)
		{
			if (explosionSlices < 1) {
				explosionSlices = Settings.GetExplosionSlices();
			}

			Slice2D result = Slice2D.Create(null, point);

			if (polygon.PointInPoly (point) == false) {
				return(result);
			}

			float sliceRot = UnityEngine.Random.Range(0, 360);

			Polygon2D hole = Polygon2D.Create(Polygon2D.PolygonType.Rectangle, 0.1f);
			hole = hole.ToOffset(point);
			polygon.AddHole(hole);

			result.AddPolygon (polygon);

			int tries = 0;
			while (result.GetPolygons().Count < explosionSlices) {
				foreach (Polygon2D p in new List<Polygon2D>(result.GetPolygons())) {
					sliceRot += UnityEngine.Random.Range(15, 45) * Mathf.Deg2Rad;

					Vector2D v = new Vector2D(point);
					v.Push(sliceRot, 0.4f);

					Slice2D newResult = SliceFromPoint (p, v, sliceRot);

					if (newResult.GetPolygons().Count > 0) {
						if (newResult.slices.Count > 0) {
							foreach (List<Vector2D> i in newResult.slices) {
								result.AddSlice(i);
							}
						}

						foreach (Polygon2D poly in newResult.GetPolygons()) {
							result.AddPolygon (poly);
						}
						result.RemovePolygon (p);
					}
				}
				
				tries += 1;
				if (tries > 20) {
					return(result);
				}
			}

			return(result);
		}

		static public Slice2D Explode(Polygon2D polygon, int explosionSlices = 0) {
			if (explosionSlices < 1) {
				explosionSlices = Settings.GetExplosionSlices();
			}

			Slice2D result = Slice2D.Create(null, Slice2DType.Explode);

			Rect polyRect = polygon.GetBounds ();

			result.AddPolygon (polygon);

			int tries = 0;
			while (result.GetPolygons().Count < explosionSlices) {
				foreach (Polygon2D p in new List<Polygon2D>(result.GetPolygons())) {
					Slice2D newResult = SliceFromPoint (p, new Vector2D(polyRect.x + UnityEngine.Random.Range(0, polyRect.width), polyRect.y + UnityEngine.Random.Range(0, polyRect.height)), UnityEngine.Random.Range(0, Mathf.PI * 2));
					
					/*
					Polygon2D smallest = Polygon2DList.GetSmallest(newResult.GetPolygons());
					if (smallest != null) {
						Debug.Log(smallest.GetArea());

						if (smallest.GetArea() < 1) {
							continue;
						}
					}*/
					
					if (newResult.GetPolygons().Count > 0) {
						if (newResult.slices.Count > 0) {
							foreach (List<Vector2D> i in newResult.slices) {
								result.AddSlice(i);
							}
						}

						foreach (Polygon2D poly in newResult.GetPolygons()) {
							result.AddPolygon (poly);
						}
						result.RemovePolygon (p);
					}
				}
				
				tries += 1;
				if (tries > 20) {
					return(result);
				}
			}

			return(result);
		}

		// Slice From Point
		static public Slice2D SliceFromPoint(Polygon2D polygon, Vector2D point, float rotation) {
			Slice2D result = Slice2D.Create(null, point, rotation);
		
			// Normalize into clockwise
			polygon.Normalize();

			Vector2D sliceA = new Vector2D (point);
			Vector2D sliceB = new Vector2D (point);

			sliceA.Push (rotation, 1e+10f / 2);
			sliceB.Push (rotation, -1e+10f / 2);

			if (polygon.PointInPoly (point) == false) {
				return(result);
			}
				
			// Getting the list of intersections
			List<Vector2D> intersectionsA = polygon.GetListLineIntersectPoly(new Pair2D(point, sliceA));
			List<Vector2D> intersectionsB = polygon.GetListLineIntersectPoly(new Pair2D(point, sliceB));

			// Sorting intersections from one point
			if (intersectionsA.Count > 0 && intersectionsB.Count > 0) {
				sliceA = Vector2DList.GetListSortedToPoint (intersectionsA, point) [0];
				sliceB = Vector2DList.GetListSortedToPoint (intersectionsB, point) [0];
			} else {
				return(result);
			}
				
			List<Pair2D> collisionList = new List<Pair2D>();
			collisionList.Add (new Pair2D (sliceA, sliceB));

			result.AddPolygon(polygon);

			foreach (Pair2D id in collisionList) {
				result.AddCollision (id.A);
				result.AddCollision (id.B);

				// Sclice line points generated from intersections list
				Vector2D vec0 = new Vector2D(id.A);
				Vector2D vec1 = new Vector2D(id.B);

				double rot = Vector2D.Atan2 (vec0, vec1);

				// Slightly pushing slice line so it intersect in all cases
				vec0.Push (rot, Linear.Slicer.precision);
				vec1.Push (rot, -Linear.Slicer.precision);

				// For each in polygons list attempt convex split
				List<Polygon2D> temp = new List<Polygon2D>(result.GetPolygons()); // necessary?
				foreach (Polygon2D poly in temp) {
					// NO, that's the problem
					Slice2D resultList = Linear.Slicer.Slice(poly, new Pair2D(vec0, vec1));

					if (resultList.GetPolygons().Count > 0) {
						if (resultList.slices.Count > 0) {
							foreach (List<Vector2D> i in resultList.slices) {
								result.AddSlice(i);
							}
						}

						foreach (Polygon2D i in resultList.GetPolygons()) {
							result.AddPolygon(i);
						}					

						// If it's possible to perform splice, remove parent polygon from result list
						result.RemovePolygon(poly);
					}
				}
			}
			result.RemovePolygon (polygon);
			return(result);
		}
	}
}