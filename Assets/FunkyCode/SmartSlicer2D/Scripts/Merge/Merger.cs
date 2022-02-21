using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities2D;

namespace Slicer2D.Merge {

	public class Merger {
		public static float precision = 0.0001f;

		// Complex Merge
		static public Merge2D Merge(Polygon2D polygon, List<Vector2D> slice) {
			Merge2D result = Merge2D.Create (slice);

			if (slice.Count < 2) {
				return(result);	
			}

			// Normalize into clockwise
			polygon.Normalize ();

			Rect sliceRect = Math2D.GetBounds(slice);
			Rect polygonRect = polygon.GetBounds();

			if (sliceRect.Overlaps(polygonRect) == false) {
				return(result);
			}

			List<List<Vector2D>> slices = GetSplitSlices(polygon, slice);

			// Adjusting split lines before performing convex split
			
			result.AddPolygon(polygon);

			foreach (List<Vector2D> id in slices) {
				if (id.Count < 1) {
					continue;
				}

				foreach (Vector2D p in id) {
					result.AddCollision (p);
				}
			
				// Sclice line points generated from intersections list
				Vector2D vec0 = id.First();
				vec0.Push (Vector2D.Atan2 (vec0, id[1]), precision); 	// ERROR

				Vector2D vec1 = id.Last();
				vec1.Push (Vector2D.Atan2 (vec1, id[id.Count - 2]), precision);

				// For each in polygons list attempt convex split
				List<Polygon2D> temp = new List<Polygon2D>(result.polygons); // necessary?
				foreach (Polygon2D poly in temp) {
					Merge2D resultList = SingleMerge(poly, id); 

					if (resultList.slices.Count > 0) {
						foreach (List<Vector2D> i in resultList.slices) {
							result.AddSlice(i);
						}
					}

					if (resultList.polygons.Count > 0) {
						foreach (Polygon2D i in resultList.polygons) {
							result.AddPolygon(i);
						}

						// If it's possible to perform convex split, remove parent polygon from result list
						result.polygons.Remove(poly);
					}
				}
			}
		
			return(result);
		}

		static private Merge2D SingleMerge(Polygon2D polygon, List<Vector2D> slice) {
			Merge2D result = Merge2D.Create(slice);

			if (polygon.PointInPoly (slice.First ()) == false || polygon.PointInPoly (slice.Last ()) == false) {
				//Debug.Log("incorrect");
				return(result);
			}

			slice = new List<Vector2D> (slice);

			Merge.Collision collisionMerge = new Merge.Collision(polygon, slice);

			if (collisionMerge.error) {
				// When this happens?
				
					Debug.LogWarning ("Merger2D: Unexpected Error 2"); 
				
				return(result);
			}

			List<Polygon2D> intersectHoles = polygon.GetListSliceIntersectHoles (slice);

			switch (intersectHoles.Count) {
				case 0:
					if (collisionMerge.collisionCount == 2) {
						return(SliceWithoutHoles (polygon, slice, collisionMerge));
					}
					break;

				case 1:
					break;

				case 2:
					break;

				default:
					break;
			}

			return(result);
		}

		static private Merge2D SliceWithoutHoles(Polygon2D polygon, List<Vector2D> slice, Merge.Collision mergeCollision) {
			Merge2D result = Merge2D.Create(slice);

			// Simple non-hole slice
			Polygon2D polyA = new Polygon2D();
			Polygon2D polyB = new Polygon2D();

			Polygon2D currentPoly = polyA;

			List<Vector2D> slices = new List<Vector2D>(mergeCollision.GetPoints());


			foreach (Pair2D p in Pair2D.GetList(polygon.pointsList)) {
				List<Vector2D> intersections = Math2D.GetListLineIntersectSlice (p, slice);

				if (intersections.Count () > 0) {
					
					if (intersections.Count == 2) {
						Vector2D first = intersections.First ();
						Vector2D last = intersections.Last ();

						if (Vector2D.Distance (last, p.A) < Vector2D.Distance (first, p.A)) {
							first = intersections.Last ();
							last = intersections.First ();
						}
						
						// Add Inside Points
						if (mergeCollision.GetPoints().Count > 0) { // InsidePlus
							if (Vector2D.Distance (first, mergeCollision.Last ()) < Vector2D.Distance (first, mergeCollision.First ())) {
								mergeCollision.Reverse ();
							}

							currentPoly.AddPoints (mergeCollision.GetPoints()); // InsidePlus
						}
						/////

						currentPoly = polyB;

						if (mergeCollision.GetPoints().Count > 0) { // InsidePlus(
							currentPoly.AddPoints (mergeCollision.GetPoints()); // InsidePlus
						}

						currentPoly = polyA;
					}

					if (intersections.Count == 1) {
						Vector2D intersection = intersections.First ();

						///// Add Inside Points
						if (mergeCollision.GetPoints().Count > 0) { //InsidePlus
							if (Vector2D.Distance (intersection, mergeCollision.Last ()) < Vector2D.Distance (intersection, mergeCollision.First ())) {
								mergeCollision.Reverse ();
							}

							currentPoly.AddPoints (mergeCollision.GetPoints()); // InsidePlus
						}
						/////

						currentPoly = (currentPoly == polyA) ? polyB : polyA;
					}
				}

				currentPoly.AddPoint (p.B);
			}
			
			Polygon2D mainPoly = polyA;
			if (polyB != null && polyB.GetArea() > polyA.GetArea()) {
				mainPoly = polyB;
			}

			result.AddPolygon (mainPoly);
		
			foreach (Polygon2D hole in polygon.holesList) {
				mainPoly.AddHole (hole);	
			}
		
			result.AddSlice(slices);
			
			return(result);
		}


		static public List<List<Vector2D>> GetSplitSlices(Polygon2D polygon, List<Vector2D> slice) {
			bool inside = polygon.PointInPoly (slice.First ());

			// Generate Slices
			List<List<Vector2D>> slices = new List<List<Vector2D>>();
			List<Vector2D> currentSlice = new List<Vector2D> ();

			Pair2D pair = Pair2D.Zero();
			for(int i = 0; i < slice.Count - 1; i++) {
				pair.A = slice[i];
				pair.B = slice[i + 1];

				List<Vector2D> stackList = polygon.GetListLineIntersectPoly(pair);
				stackList = Vector2DList.GetListSortedToPoint (stackList, pair.A);

				foreach (Vector2D id in stackList) {
					if (inside == true) {
						currentSlice = new List<Vector2D> ();
						currentSlice.Add (id);
					} else {
						currentSlice.Add (id);
						slices.Add (currentSlice);
					}
					inside = !inside;
				}

				if (inside == false) {
					currentSlice.Add (pair.B);
				}
			}
			
			return(slices);
		}
	}
}

