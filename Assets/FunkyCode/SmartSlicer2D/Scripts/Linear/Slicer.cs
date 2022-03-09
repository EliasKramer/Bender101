using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Utilities2D;

namespace Slicer2D.Linear {

	public class Slicer {
		public static double precision = 0.000001f;

		// Linear Slice
		static public Slice2D Slice(Polygon2D polygon, Pair2D slice) {
			Slice2D result = Slice2D.Create(null, slice);

			if (slice == null) {
				return(result);	
			}

			// Normalize into clockwise
			polygon.Normalize();
		
			// Optimization
			// Skip slicing polygons that are not overlapping with slice
			Rect sliceRect = Math2D.GetBounds(slice);
			Rect polygonRect = polygon.GetBounds();

			if (sliceRect.Overlaps(polygonRect) == false) {
				return(result);
			}

			result = MultipleSlice(polygon, slice);

			return(result);
		}

		static private Slice2D MultipleSlice(Polygon2D polygon, Pair2D slice) {
			Slice2D result = Slice2D.Create(null, slice);
			
			List<Pair2D> slices = GetSplitSlices(polygon, slice);

			//Debug.Log(slices.Count);

			//Debug.Log(slices[0].A.ToVector2() + " " + slices[0].B.ToVector2());
			//Debug.Log(slices[1].A.ToVector2() + " " + slices[1].B.ToVector2());

			if (slices.Count < 1) {
				return(result);
			}

			result.AddPolygon(polygon);

			// Slice line points generated from intersections list
			foreach (Pair2D id in slices) {
				result.AddCollision (id.A);
				result.AddCollision (id.B);

				Vector2D vec0 = new Vector2D(id.A);
				Vector2D vec1 = new Vector2D(id.B);

				double rot = Vector2D.Atan2 (vec0, vec1);

				// Slightly pushing slice line so it intersect in all cases
				vec0.Push (rot, precision);
				vec1.Push (rot, -precision);

				Pair2D line = new Pair2D(vec0, vec1);

				// For each in polygons list attempt convex split
				foreach (Polygon2D poly in (new List<Polygon2D>(result.GetPolygons()))) {
					Slice2D resultList = SingleSlice(poly, line);

					if (resultList.GetPolygons().Count > 0) {
						if (resultList.slices.Count > 0) {
							foreach (List<Vector2D> i in resultList.slices) {
								result.AddSlice(i);
							}
						}
						
						result.AddSlice(line);

						foreach (Polygon2D i in resultList.GetPolygons()) {
							result.AddPolygon(i);
						}

						// If it's possible to perform splice, remove currently sliced polygon from result list
						result.RemovePolygon(poly);
					}
				}
			}

			result.RemovePolygon (polygon);
			return(result);
		}

		static public List<Pair2D> GetSplitSlices(Polygon2D polygon, Pair2D slice) {
			// Getting the list of intersections
			List<Pair2D> slices = new List<Pair2D>();

			List<Vector2D> intersections = polygon.GetListLineIntersectPoly(slice);

			// Single collision cannot make a proper slice
			if (intersections.Count < 2) {
				return(slices);
			}

			// Sorting intersections from one point
			intersections = Vector2DList.GetListSortedToPoint(intersections, slice.A);

			// Dividing intersections into single slices
			// Optimize this (polygon.PointInPoly) line
			// This method doesn't look like very reliable!!!

			foreach (Pair2D p in Pair2D.GetList(intersections, false)) {
				Vector2D midPoint = new Vector2D ((p.B.x + p.A.x) / 2, (p.B.y + p.A.y) / 2);

				if (Vector2D.Distance(p.A, p.B) < 0.001f) {
					continue;
				}

				if (polygon.PointInPoly (midPoint) == false) {
					continue;
				}

				slices.Add (p);
				intersections.Remove (p.A);
				intersections.Remove (p.B);
			}

			return(slices);
		}

		static private Slice2D SingleSlice(Polygon2D polygon, Pair2D slice) {
			Slice2D result = Slice2D.Create(null, slice);

			if (Vector2D.Distance(slice.A, slice.B) < 0.001f) {
				Debug.LogWarning("Slicer2D: Slice is too short");

				return(result);
			}

			Vector2D vertexToIntersection = VertexToIntersection.Get(polygon, slice);

			if (vertexToIntersection != null) {
				return(VertexToIntersection.Slice(polygon, slice, vertexToIntersection));
			}

			if ((polygon.PointInPoly (slice.A) || polygon.PointInPoly (slice.B))) { //  && pointsInHoles == 1
				Debug.LogWarning ("Slicer2D: Incorrect Split: Code 1 (One point is inside Vertice?)" + " " + slice.ToString());

				// Slicing through hole cut-out
				return(result);
			}

	
			Polygon2D holeA = polygon.PointInHole (slice.A);
			Polygon2D holeB = polygon.PointInHole (slice.B);

			int pointsInHoles = Convert.ToInt32 (holeA != null) + Convert.ToInt32 (holeB != null);

			if (pointsInHoles == 2 && holeA == holeB) {
				pointsInHoles = 1;
			}

			switch (pointsInHoles) {
				case 0:
					return(SliceWithoutHoles.Slice(polygon, slice));

				case 1:
					return(SliceWithOneHole.Slice(polygon, slice, holeA, holeB));

				case 2:
					return(SliceWithTwoHoles.Slice(polygon, slice, holeA, holeB));

				default:
					break;
			}

			return(result);
		}

		public class VertexToIntersection {
			static public Slice2D Slice(Polygon2D polygon, Pair2D slice, Vector2D vertex) {
				Slice2D result = Slice2D.Create(null, slice);

				polygon.pointsList = Vector2DList.GetListStartingPoint(polygon.pointsList, vertex);

				Polygon2D polyA = new Polygon2D();
				Polygon2D polyB = new Polygon2D();

				Polygon2D currentPoly = polyA;

				int collisionCount = 0;

				Pair2D id = Pair2D.Zero();
				id.A = polygon.pointsList.Last();

				for(int i = 0; i < polygon.pointsList.Count; i++) {
					id.B = polygon.pointsList[i];

					Vector2D intersection = Math2D.GetPointLineIntersectLine (id, slice);

					if (intersection != null) { // && Vector2D.Distance(intersection, vertex) < 0.001f

						if (polyA.pointsList.Count < 1 || Vector2D.Distance(intersection, polyA.pointsList.Last()) > precision) {
							polyA.AddPoint (intersection);
						}

						if (polyB.pointsList.Count < 1 || Vector2D.Distance(intersection, polyB.pointsList.Last()) > precision) {
							polyB.AddPoint (intersection);
						}
						
					
						currentPoly = (currentPoly == polyA) ? polyB : polyA;

						collisionCount++;

					}

					if (currentPoly.pointsList.Count < 1 || Vector2D.Distance(id.B, currentPoly.pointsList.Last()) > precision) {
						currentPoly.AddPoint (id.B);
					}								

					id.A = id.B;
				}

				switch(collisionCount) {
					case 1:
					case 2:
					case 3:
						if (polyA.pointsList.Count () >= 3) {
							result.AddPolygon (polyA);
						}

						if (polyB.pointsList.Count () >= 3) {
							result.AddPolygon (polyB);
						}

						foreach (Polygon2D poly in result.GetPolygons()) {
							foreach (Polygon2D hole in polygon.holesList) {
								if (poly.PolyInPoly (hole) == true) {
									poly.AddHole (hole);	
								}
							}
						}

						return(result);
						
					default:

						Debug.LogWarning("Slicer2D: Vertex Linear Slice with " + collisionCount + " collision points " + slice.ToString());

						break;
				}

				return(result);
			}

			public static Vector2D Get(Polygon2D polygon, Pair2D slice) {
				Vector2D pairA = null;
				Vector2D pairB = null;
				foreach(Vector2D p in polygon.pointsList) {
					if (Vector2D.Distance(p, slice.A) < 0.001f) {
						pairA = p;
					}
					if (Vector2D.Distance(p, slice.B) < 0.001f) {
						pairB = p;
					}
				}
				if (pairA != null && pairB == null) {
					return(pairA);
				} else if (pairB != null && pairA == null) {
					return(pairB);
				} else if (pairA != null && pairB == null) {
					//Debug.LogWarning("From Vertex To Vertex");
					return(null);
				} else {
					return(null);
				}
			}
		}

		public class SliceWithoutHoles {
			static public Slice2D Slice(Polygon2D polygon, Pair2D slice) {
				Slice2D result = Slice2D.Create(null, slice);

				if (polygon.LineIntersectHoles (slice).Count > 0) {
					// When does this happen? - Only when using point slicer!!!

					Debug.LogWarning ("Slicer2D: Slice Intersect Holes (Point Slicer?)"); 

					return(result);
				}

				Polygon2D polyA = new Polygon2D();
				Polygon2D polyB = new Polygon2D();

				Polygon2D currentPoly = polyA;

				int collisionCount = 0;

				Pair2D id = Pair2D.Zero();
				id.A = polygon.pointsList.Last();

				int vertexCollisionCount = 0;
				
				for(int i = 0; i < polygon.pointsList.Count; i++) {
					id.B = polygon.pointsList[i];

					Vector2D intersection = Math2D.GetPointLineIntersectLine (id, slice);

					if (intersection != null) {
						//float distance = 1;

						bool vertexCollision = false;
						
						// Why vertex code when there is separate class for it?
						
						// Last Point
						if (polyA.pointsList.Count > 0 && Vector2D.Distance(intersection, polyA.pointsList.Last()) < precision ) {
							vertexCollision = true;
						}
						
						if (polyB.pointsList.Count > 0 && Vector2D.Distance(intersection, polyB.pointsList.Last()) < precision ) {
							vertexCollision = true;
						}

						// First Point
						if (polyA.pointsList.Count > 0 && Vector2D.Distance(intersection, polyA.pointsList.First()) < precision ) {
							vertexCollision = true;
						}
						
						if (polyB.pointsList.Count > 0 && Vector2D.Distance(intersection, polyB.pointsList.First()) < precision ) {
							vertexCollision = true;
						}

						if (vertexCollision == false) {
					
							polyA.AddPoint (intersection);
							polyB.AddPoint (intersection);

							currentPoly = (currentPoly == polyA) ? polyB : polyA;

							collisionCount++;
						} else {
							Debug.Log("vertex collision");

							vertexCollisionCount += 1;
						}
					}
										

					currentPoly.AddPoint (id.B);

					id.A = id.B;
				}

				//Debug.Log(vertexCollisionCount + " " + collisionCount);

				switch(collisionCount) {
					case 2:
						if (polyA.pointsList.Count () >= 3) {
							result.AddPolygon (polyA);
						}

						if (polyB.pointsList.Count () >= 3) {
							result.AddPolygon (polyB);
						}

						foreach (Polygon2D poly in result.GetPolygons()) {
							foreach (Polygon2D hole in polygon.holesList) {
								if (poly.PolyInPoly (hole) == true) {
									poly.AddHole (hole);	
								}
							}
						}

						return(result);
						
					default:

						Debug.LogWarning("Slicer2D: Linear Slice with " + collisionCount + " collision points");

						break;
				}

				/*
				if (collisionCount == 333) {  // Complicated Slices With Holes
					if (Slicer2D.Debug.enabled) {
						Debug.LogWarning("Slicer2D: Slice " + collisionCount);
					}

					if (polyA.pointsList.Count () >= 3) {
						result.AddPolygon (polyA);
					}

					if (polyB.pointsList.Count () >= 3) {
						result.AddPolygon (polyB);
					}

					foreach (Polygon2D poly in result.GetPolygons()) {
						foreach (Polygon2D hole in polygon.holesList) {
							if (poly.PolyInPoly (hole) == true) {
								poly.AddHole (hole);	
							}
						}
					}	

					return(result);
				}*/

				return(result);
			}
		}

		public class SliceWithTwoHoles {
			static public Slice2D Slice(Polygon2D polygon, Pair2D slice, Polygon2D holeA, Polygon2D holeB) {
				Slice2D result = Slice2D.Create(null, slice);

				if (holeA == holeB) {
					Debug.LogWarning ("Slicer2D: Incorrect Split 2: Cannot Split Into Same Hole");

					return(result);
				}

				Polygon2D polyA = new Polygon2D ();
				Polygon2D polyB = new Polygon2D (polygon.pointsList);

				polyA.AddPoints (Vector2DList.GetListStartingIntersectLine (holeA.pointsList, slice));
				polyA.AddPoints (Vector2DList.GetListStartingIntersectLine (holeB.pointsList, slice));

				foreach (Polygon2D poly in polygon.holesList) {
					if (poly != holeA && poly != holeB) {
						polyB.AddHole (poly);
					}
				}

				polyB.AddHole (polyA);

				result.AddPolygon (polyB);
				return(result);
			}
		}

		public class SliceWithOneHole {
			static public Slice2D Slice(Polygon2D polygon, Pair2D slice, Polygon2D holeA, Polygon2D holeB) {
				Slice2D result = Slice2D.Create(null, slice);

				if (holeA == holeB) {
					Polygon2D polyA = new Polygon2D (polygon.pointsList);
					Polygon2D polyB = new Polygon2D ();
					Polygon2D polyC = new Polygon2D ();

					Polygon2D currentPoly = polyB;

					foreach (Pair2D pair in Pair2D.GetList (holeA.pointsList)) {
						Vector2D point = Math2D.GetPointLineIntersectLine(slice, pair);
						if (point != null) { 
							polyB.AddPoint (point);
							polyC.AddPoint (point);
							currentPoly = (currentPoly == polyB) ? polyC : polyB;
						}
						currentPoly.AddPoint (pair.B);
					}

					if (polyB.pointsList.Count > 2 && polyC.pointsList.Count > 2) {
						if (polyB.GetArea() > polyC.GetArea()) {
							polyA.AddHole (polyB);
							result.AddPolygon (polyC);
						} else {
							result.AddPolygon (polyB);
							polyA.AddHole (polyC);
						}

						result.AddPolygon (polyA);
					}

					return(result);
					// Cross From Side To Polygon
				} else if (polygon.PointInPoly (slice.A) == false || polygon.PointInPoly (slice.B) == false) {
					Polygon2D holePoly = (holeA != null) ? holeA : holeB;

					if (holePoly != null) {
						Polygon2D polyA = new Polygon2D ();
						Polygon2D polyB = new Polygon2D (holePoly.pointsList);

						polyB.pointsList.Reverse ();

						polyA.AddPoints (Vector2DList.GetListStartingIntersectLine (polygon.pointsList, slice));
						polyA.AddPoints (Vector2DList.GetListStartingIntersectLine (polyB.pointsList, slice));

						foreach (Polygon2D poly in polygon.holesList) {
							if (poly != holePoly) {
								polyA.AddHole (poly);
							}
						}

						result.AddPolygon (polyA);
						return(result);
					}
				}
				return(result);
			}
		}
	}
}