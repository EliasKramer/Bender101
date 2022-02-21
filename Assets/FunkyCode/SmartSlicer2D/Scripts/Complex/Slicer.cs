using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Utilities2D;

namespace Slicer2D.Complex {

	public class Slicer {
		public static double precision = 0.1f; // Should not be too low (Lower does not mean more precise)

		// Complex Slice
		static public Slice2D Slice(Polygon2D polygon, List<Vector2D> slice) {
			Slice2D result = Slice2D.Create (null, slice);

			if (slice == null) {
				return(result);	
			}

			if (slice.Count < 2) {
				return(result);	
			}

			// Normalize into clockwise
			polygon.Normalize ();

			// Optimization
			// Skip slicing polygons that are not overlapping with slice
			Rect sliceRect = Math2D.GetBounds(slice);
			Rect polygonRect = polygon.GetBounds();

			if (sliceRect.Overlaps(polygonRect) == false) {
				return(result);
			}

			// If possible - slice out area
			if (Sliceable2D.complexSliceType != Sliceable2D.SliceType.Regular) {
				result = Complex.SlicerExtended.SlicePolygonInside (polygon, slice);
				if (result.GetPolygons().Count > 0) {
					return(result);
				}
			}

			result = MultipleSlice(polygon, slice);

			return(result);
		}

		static private Slice2D MultipleSlice(Polygon2D polygon, List<Vector2D> slice) {
			Slice2D result = Slice2D.Create (null, slice);

		
			List<SlicerSplit> slices = SlicerSplit.GetSplitSlices(polygon, slice);
			if (slices.Count < 1) {
				return(result);
			}

			// Adjusting split lines before performing convex split
			result.AddPolygon(polygon);

			foreach (SlicerSplit id in slices) {
				if (id.points.Count > 1) {
					foreach (Vector2D p in id.points) {
						result.AddCollision (p);
					}

					// Sclice line points generated from intersections list
					Vector2D vec0 = id.points.First();
					vec0.Push (Vector2D.Atan2 (vec0, id.points[1]), precision);

					Vector2D vec1 = id.points.Last();
					vec1.Push (Vector2D.Atan2 (vec1, id.points[id.points.Count - 2]), precision);

					// For each in polygons list attempt convex split
					List<Polygon2D> resultPolygons = new List<Polygon2D>(result.GetPolygons()); // necessary?
					foreach (Polygon2D poly in resultPolygons) {
						Slice2D resultList = SingleSlice(poly, id); 

						if (resultList.GetPolygons().Count > 0) {
							if (resultList.slices.Count > 0) {
								foreach (List<Vector2D> resultSlice in resultList.slices) {
									result.AddSlice(resultSlice);
								}
							}
							
							foreach (Polygon2D resultPoly in resultList.GetPolygons()) {
								result.AddPolygon(resultPoly);
							}

							// If it's possible to perform convex split, remove parent polygon from result list
							result.RemovePolygon(poly);
						}
					}
				}
			}

			//Debug.Log("1 " + slices[0].Count + " " + slices[0][0].ToVector2() + " " + slices[0][1].ToVector2());
			//Debug.Log("2 " + slices[1].Count + " " + slices[1][0].ToVector2() + " " + slices[1][1].ToVector2());

			// if exist then remove slice was not performed
			result.RemovePolygon (polygon);
			return(result);
		}

		static private Slice2D SingleSlice(Polygon2D polygon, Complex.SlicerSplit split) {
			Slice2D result = Slice2D.Create (null, split.points);

			//Debug.Log(split.points.Count + " " + split.points[0].ToString() + " " + split.points[1].ToString() + " " + split.points[2].ToString() + " " + split.points[3].ToString() + " " );

			// Change
			if (Sliceable2D.complexSliceType == Sliceable2D.SliceType.Regular) {
				if (Math2D.SliceIntersectItself(split.points)) {
					Debug.LogWarning("Slicer2D: Slice Intersect Itself In Regular Mode");
					return(result);
				}
			}

			// Start and End of slice should be outside the polygon
			if (polygon.PointInPoly (split.points.First ()) == true || polygon.PointInPoly (split.points.Last ()) == true) {
				return(result);
			}

			List<Vector2D> slice = new List<Vector2D> (split.points);

			if (split.type == Complex.SlicerSplit.Type.SingleVertexCollision) {
				Debug.Log("Single Vertex Collision");

				return(result);
			}

			Collision collisionSlice = new Collision(polygon, slice);

			if (collisionSlice.error) {
				Debug.LogWarning ("Slicer2D: Complex Collision Error"); 

				return(result);
			}

			List<Polygon2D> intersectHoles = polygon.GetListSliceIntersectHoles (slice);

			switch (intersectHoles.Count) {
				case 0:
					if (collisionSlice.collisionCount == 2) {
						return(SliceWithoutHoles.Slice(polygon, slice, collisionSlice));
					}
					break;

				case 1:
					return(SliceWithOneHole.Slice(polygon, slice, collisionSlice));

				case 2:
					return(SliceWithTwoHoles.Slice(polygon, slice, collisionSlice));

				default:
					break;
				}

			return(result);
		}

		public class SliceWithoutHoles {
			static public Slice2D Slice(Polygon2D polygon, List<Vector2D> slice, Collision collisionSlice) {
				Slice2D result = Slice2D.Create (null, slice);

				List<Vector2D> points = collisionSlice.GetPoints();

				// Simple non-hole slice
				Polygon2D polyA = new Polygon2D();
				Polygon2D polyB = new Polygon2D();

				Polygon2D currentPoly = polyA;

				List<Vector2D> slices = new List<Vector2D>(collisionSlice.GetPointsWithIntersection());

				List<Pair2D> polygonPairs = Pair2D.GetList(polygon.pointsList);

				foreach (Pair2D p in polygonPairs) {
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
							if (collisionSlice.GetPointsWithIntersection().Count > 0) {
								if (Vector2D.Distance (first, collisionSlice.Last ()) < Vector2D.Distance (first, collisionSlice.First ())) {
									collisionSlice.Reverse ();
								}

								currentPoly.AddPoints (collisionSlice.GetPointsWithIntersection());
							}
							/////

							currentPoly = polyB;

							if (collisionSlice.GetPointsWithIntersection().Count > 0) {
								currentPoly.AddPoints (collisionSlice.GetPointsWithIntersection());
							}

							currentPoly = polyA;
						}

						if (intersections.Count == 1) {
							Vector2D intersection = intersections.First ();

							///// Add Inside Points
							if (collisionSlice.GetPointsWithIntersection().Count > 0) {
								if (Vector2D.Distance (intersection, collisionSlice.Last ()) < Vector2D.Distance (intersection, collisionSlice.First ())) {
									collisionSlice.Reverse ();
								}

								currentPoly.AddPoints (collisionSlice.GetPointsWithIntersection());
							}
							/////

							currentPoly = (currentPoly == polyA) ? polyB : polyA;
						}
					}

					currentPoly.AddPoint (p.B);
				}

				result.AddPolygon (polyA);
				result.AddPolygon (polyB);

				foreach (Polygon2D poly in result.GetPolygons()) {
					foreach (Polygon2D hole in polygon.holesList) {
						if (poly.PolyInPoly (hole) == true) {
							poly.AddHole (hole);	
						}
					}
				}

				result.AddSlice(slices);
				return(result);
			}
		}

		public class SliceWithOneHole {
			static public Slice2D Slice(Polygon2D polygon, List<Vector2D> slice, Collision collisionSlice) {
				Slice2D result = Slice2D.Create (null, slice);
				Polygon2D holeA = polygon.PointInHole (slice.First ());
				Polygon2D holeB = polygon.PointInHole (slice.Last ());
				Polygon2D holePoly = (holeA != null) ? holeA : holeB;

				if (holePoly == null) {
					Debug.LogWarning ("Slicer2D: Slice is not in the hole (SliceWithOneHole)");

					return(result);
				}

				List<Vector2D> slices = new List<Vector2D>(collisionSlice.GetPointsWithIntersection());

				// If any slice endings is not outside polygon
				if ((polygon.PointInPoly (slice.First ()) == false || polygon.PointInPoly (slice.Last ()) == false) == false) { 
					return(result);
				}

				// Slicing Into The Same Hole
				if (holeA == holeB) { 
					if (Sliceable2D.complexSliceType == Sliceable2D.SliceType.Regular) {
						return(result);
					}

					result = SliceIntoSameHole(polygon, holePoly, slice, collisionSlice);
					result.AddSlice(slices);

				// Slicing From Outside To Hole
				} else if (holePoly != null) {
					result = SliceFromOutsideToHole(polygon, holePoly, slice, collisionSlice);
					result.AddSlice(slices);
				}
			
				return(result);
			}

			static public Slice2D SliceFromOutsideToHole(Polygon2D polygon, Polygon2D holePoly, List<Vector2D> slice, Collision collisionSlice) {
				Slice2D result = Slice2D.Create (null, slice);
			
				Polygon2D polyA = new Polygon2D ();
				Polygon2D polyB = new Polygon2D (holePoly.pointsList);
				polyB.pointsList.Reverse ();

				List<Vector2D> pointsA = Vector2DList.GetListStartingIntersectSlice (polygon.pointsList, slice);
				List<Vector2D> pointsB = Vector2DList.GetListStartingIntersectSlice (polyB.pointsList, slice);

				if (pointsA.Count < 1) {
					Debug.LogWarning ("Slicer2D: Not enough of slice intersections with polygon (SliceWithOneHole)");
				}

				polyA.AddPoints (pointsA);

				// pointsA empty
				if (collisionSlice.GetPointsInside().Count > 0) {
					if (Vector2D.Distance (pointsA.Last (), collisionSlice.Last ()) < Vector2D.Distance (pointsA.Last (), collisionSlice.First ())) {
						collisionSlice.Reverse ();
					}

					polyA.AddPoints (collisionSlice.GetPointsInside());
				}

				polyA.AddPoints (pointsB);

				if (collisionSlice.GetPointsInside().Count > 0) {
					collisionSlice.Reverse ();
					polyA.AddPoints (collisionSlice.GetPointsInside());
				}

				foreach (Polygon2D poly in polygon.holesList) { // Check for errors?
					if (poly != holePoly && polyA.PolyInPoly(poly) == true) {
						polyA.AddHole (poly);
					}
				}

				result.AddPolygon (polyA);
				return(result);
			}

			static public Slice2D SliceIntoSameHole(Polygon2D polygon, Polygon2D holePoly, List<Vector2D> slice, Collision collisionSlice) {
				Slice2D result = Slice2D.Create (null, slice);
			
				// Slice Into Same Pair
				if (collisionSlice.polygonCollisionPairs.Count == 1) {
					Polygon2D slicePoly = new Polygon2D(collisionSlice.GetPointsWithIntersection());

					Polygon2D newHole = new Polygon2D ();
					if (slicePoly.PolyInPoly(holePoly)) {
						newHole = slicePoly;
					} else {
						foreach (Pair2D pair in Pair2D.GetList (holePoly.pointsList)) {
							newHole.AddPoint(pair.A);

							if (Vector2D.Distance (pair.A, collisionSlice.Last ()) < Vector2D.Distance (pair.A, collisionSlice.First ())) {
								collisionSlice.Reverse();
							}

							if (Math2D.LineIntersectSlice (pair, slice)) {
								newHole.AddPoints(collisionSlice.GetPoints());
							}
						}
					}

					Polygon2D polygonA = new Polygon2D (polygon.pointsList);
					polygonA.AddHole(newHole);

					// Adds polygons if they are not in the hole
					foreach (Polygon2D poly in polygon.holesList) { // Check for errors?
						if (poly != holePoly && polygonA.PolyInPoly(poly) == true) {
							polygonA.AddHole (poly);
						}
					}

					if (Sliceable2D.complexSliceType == Sliceable2D.SliceType.FillSlicedHole) {
						result.AddPolygon (slicePoly);
					}

					result.AddPolygon(polygonA);

					return(result);

				// Slice Into Different Pair
				} else {
					Polygon2D polyA = new Polygon2D (polygon.pointsList);
					Polygon2D newHoleA = new Polygon2D ();
					Polygon2D newHoleB = new Polygon2D ();

					List<Pair2D> iterateList = Pair2D.GetList (holePoly.pointsList);

					bool addPoints = false;

					foreach (Pair2D pair in iterateList) {
						List<Vector2D> intersect = Math2D.GetListLineIntersectSlice (pair, slice);

						switch(addPoints) {
							case false:
								if (intersect.Count > 0) {
									addPoints = true;
								}

								break;
							case true:
								newHoleA.AddPoint(pair.A);

								if (intersect.Count > 0) {
									addPoints = false;

									if (Vector2D.Distance (intersect[0], collisionSlice.Last ()) < Vector2D.Distance (intersect[0], collisionSlice.First ())) {
										collisionSlice.Reverse();
									}
									newHoleA.AddPoints(collisionSlice.GetPointsWithIntersection());
								}
								break;
						}
					}

					addPoints = true;
					foreach (Pair2D pair in iterateList) {
						List<Vector2D> intersect = Math2D.GetListLineIntersectSlice (pair, slice);

						switch(addPoints) {
							case false:
								if (intersect.Count > 0) {
									addPoints = true;
								}

								break;
							case true:
								newHoleB.AddPoint(pair.A);
								
								if (intersect.Count > 0) {
									addPoints = false;

									if (Vector2D.Distance (intersect[0], collisionSlice.Last ()) < Vector2D.Distance (intersect[0], collisionSlice.First ())) {
										collisionSlice.Reverse();
									}
									newHoleB.AddPoints(collisionSlice.GetPointsWithIntersection());
								}
								break;
						}
					}

					if (newHoleB.GetArea() > newHoleA.GetArea()) {
						Polygon2D tempPolygon = newHoleA;
						newHoleA = newHoleB;
						newHoleB = tempPolygon;
					}

					polyA.AddHole(newHoleA);

					if (Sliceable2D.complexSliceType == Sliceable2D.SliceType.FillSlicedHole) {
						result.AddPolygon (newHoleB);
					}

					// Adds polygons if they are not in the hole
					foreach (Polygon2D poly in polygon.holesList) { // Check for errors?
						if (poly != holePoly && polyA.PolyInPoly(poly) == true) {
							polyA.AddHole (poly);
						}
					}

					result.AddPolygon (polyA);

					return(result);
				}
			}
		}

		public class SliceWithTwoHoles {
			static public Slice2D Slice(Polygon2D polygon, List<Vector2D> slice, Collision collisionSlice) {
				Slice2D result = Slice2D.Create (null, slice);

				if (Sliceable2D.complexSliceType == Sliceable2D.SliceType.Regular) {
					return(result);
				}

				Polygon2D polyA = new Polygon2D ();
				Polygon2D polyB = new Polygon2D (polygon.pointsList);

				Polygon2D holeA = polygon.PointInHole (slice.First ());
				Polygon2D holeB = polygon.PointInHole (slice.Last ());

				if (holeA == null || holeB == null) {
					// Shouldn't really happen no more

					Debug.LogWarning ("Slicer: ERROR Split"); 
					return(result);
				}

				List<Vector2D> slices = new List<Vector2D>(collisionSlice.GetPointsWithIntersection());

				List<Vector2D> pointsA = Vector2DList.GetListStartingIntersectSlice (holeA.pointsList, slice);
				List<Vector2D> pointsB = Vector2DList.GetListStartingIntersectSlice (holeB.pointsList, slice);

				polyA.AddPoints (pointsA);

				if (collisionSlice.GetPointsInside().Count > 0) {
					if (Vector2D.Distance (pointsA.Last (), collisionSlice.Last ()) < Vector2D.Distance (pointsA.Last (), collisionSlice.First ())) {
						collisionSlice.Reverse ();
					}

					polyA.AddPoints (collisionSlice.GetPointsInside());
				}

				polyA.AddPoints (pointsB);

				if (collisionSlice.GetPointsInside().Count > 0) {
					collisionSlice.Reverse ();

					polyA.AddPoints (collisionSlice.GetPointsInside());
				}

				foreach (Polygon2D poly in polygon.holesList) {
					if (poly != holeA && poly != holeB) {
						polyB.AddHole (poly);
					}
				}

				polyB.AddHole (polyA);
				result.AddPolygon (polyB);

				result.AddSlice(slices);
				return(result);
			}
		}
	}
}