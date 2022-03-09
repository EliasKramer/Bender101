using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Utilities2D.Extensions;
using Utilities2D;

namespace Slicer2D.Complex {

	public class SlicerExtended {
		static double precision = 0.00001f; // For Edge Collisions
		
		// Can Be Used In Advanced SliceInside
		static public Polygon2D CreateSlice(List<Vector2D> polygonSlice) {
			if (polygonSlice.Count < 1) {
				return(null);
			}
			polygonSlice.Add(polygonSlice.First());
			if (Math2D.SliceIntersectItself(polygonSlice) == true) {
				return(null);
			}
			polygonSlice.Remove(polygonSlice.Last());

			if (polygonSlice.Count () > 2) {
				return(new Polygon2D (polygonSlice));
			}

			return(null);
		}

		static public Vector2D GetSingleStartingPoint(Polygon2D polygon, Polygon2D slicePolygon) {
			Vector2D startPoint = null;
			foreach (Vector2D id in slicePolygon.pointsList) {
				if (polygon.PointInPoly (id) == false) {
					startPoint = id;
					break;
				}
			}
			return(startPoint);
		}

		static public Vector2D GetDoubleStartingPoint(Polygon2D polygon, Polygon2D slicePolygon) {
			Vector2D startPoint = null;
			foreach (Pair2D id in Pair2D.GetList(slicePolygon.pointsList)) {
				if (polygon.PointInPoly (id.A) == false && polygon.PointInPoly (id.B) == false) {
					startPoint = id.B;
					break;
				}
			}
			return(startPoint);
		}

		// Polygon Slice - TODO: Return No Polygon if it's eaten by polygon slice
		static public Slice2D PolygonSlice(Polygon2D polygon, Polygon2D slicePolygon) {
			List<Polygon2D> holes = polygon.holesList;

			polygon = new Polygon2D(new List<Vector2D>(polygon.pointsList));
			polygon.holesList = holes;
			
			slicePolygon = new Polygon2D(new List<Vector2D>(slicePolygon.pointsList));

			Slice2D result = Slice2D.Create (null, polygon);

			Sliceable2D.SliceType tempSliceType = Sliceable2D.complexSliceType;
			Sliceable2D.complexSliceType = Sliceable2D.SliceType.SliceHole;

			slicePolygon.Normalize ();
			polygon.Normalize ();

			// Eat a polygon completely
			// Complex Slicer does not register slice in this case
			if (slicePolygon.PolyInPoly (polygon) == true) {
				result.AddPolygon (polygon);
				return(result);
			}

			// Cut a hole inside (how does this work if it collides with other hole?)
			if (polygon.PolyInPoly (slicePolygon) == true) {
				polygon.AddHole (slicePolygon);
				result.AddPolygon (polygon);

				Math2D.Distance distance = Math2D.Distance.PolygonToPolygon(polygon, slicePolygon);
				if (distance != null && distance.value < precision) {
					return(SlicePolygonFromEdge(polygon, slicePolygon));
				}

				return(result);
			}

			// Act as Regular Slice
			Vector2D startPoint = GetDoubleStartingPoint(polygon, slicePolygon);

			if (startPoint == null) {
				startPoint = GetSingleStartingPoint(polygon, slicePolygon);
			} else {
				/*
				int count = slicePolygon.pointsList.Count;
				int id = slicePolygon.pointsList.IndexOf(startPoint);
				Debug.Log("[1] " + slicePolygon.pointsList[id].ToString() + " " + slicePolygon.pointsList[(id + 1) % count].ToString() + " " + slicePolygon.pointsList[(id + 2) % count].ToString());
			

				Vector2D o = slicePolygon.pointsList[id + 1];

				Vector2D p = (startPoint + o) / 2;
				slicePolygon.pointsList.Insert(id , p);

				startPoint = p;

				Debug.Log(startPoint.ToString() + " " +  o.ToString() + " " + p.ToString());

				Debug.Log("[2] " + slicePolygon.pointsList[id].ToString() + " " + slicePolygon.pointsList[(id + 1) % count].ToString() + " " + slicePolygon.pointsList[(id + 2) % count].ToString());
			*/
			
			}

			

			if (startPoint == null) {
				if (Math2D.PolyIntersectPoly(polygon, slicePolygon)) {
					return(SlicePolygonFromEdge(polygon, slicePolygon));
				}

				Debug.LogWarning ("Starting Point Error In PolygonSlice");
				return(result);
			}

			slicePolygon.pointsList = Vector2DList.GetListStartingPoint (slicePolygon.pointsList, startPoint);
		
			/*
			List<Vector2D> s = new List<Vector2D> ();
			foreach (Pair2D pair in Pair2D.GetList(polygonSlice.pointsList, false)) {
				List<Vector2D> stackList = polygon.GetListSliceIntersectPoly(pair);
				stackList = Vector2DList.GetListSortedToPoint (stackList, pair.A);
				Vector2D old = pair.A;
				s.Add (old);

				foreach (Vector2D id in stackList) {
					s.Add (new Vector2D((old.GetX() + id.GetX()) / 2, (old.GetY() + id.GetY()) / 2));
					old = id;
				}
			}

			polygonSlice.pointsList = s;
			*/

			slicePolygon.AddPoint (startPoint);

			// Not Necessary
			if (polygon.SliceIntersectPoly (slicePolygon.pointsList) == false) {
				return(result);
			}

			// Slice More Times?
			result = Slicer.Slice (polygon, new List<Vector2D> (slicePolygon.pointsList));
		
			if (result.GetPolygons().Count < 1) {
				Debug.LogWarning ("Slicer2D: Returns Empty Polygon Slice");
			}

			Sliceable2D.complexSliceType = tempSliceType;

			return(result);
		}

		static public Slice2D SlicePolygonFromEdge(Polygon2D polygon, Polygon2D slicePolygon) {
			Slice2D result = Slice2D.Create (null, polygon);

			Vector2D startA = null;
			Vector2D startB = null;

			Pair2D pairA = Pair2D.Zero();
			pairA.A = polygon.pointsList.Last();

			for(int i = 0; i < polygon.pointsList.Count; i++) {
				pairA.B = polygon.pointsList[i];

				Pair2D pairB = Pair2D.Zero();
				pairB.A = slicePolygon.pointsList.Last();

				float minDistance = float.PositiveInfinity; // inf

				for(int x = 0; x < slicePolygon.pointsList.Count; x++) {
					pairB.B = slicePolygon.pointsList[x];

					double dist = Math2D.Distance.PointToLine(pairB.A, pairA);
					
					if (dist < precision) { // precision
						float distance = (float)Vector2D.Distance(pairA.A, pairB.A);
					
						if (distance < minDistance) {
							minDistance = distance;
							
							startA = pairA.B;
							startB = pairB.B;
						}
					}

					pairB.A = pairB.B;
				}

				pairA.A = pairA.B;
			}

			if (startB == null || startB == null) {
				Debug.LogWarning("Slicer2D: PolygonSlice From Edge Error");
				return(result);
			}

			polygon.pointsList = Vector2DList.GetListStartingPoint (polygon.pointsList, startA);
			slicePolygon.pointsList = Vector2DList.GetListStartingPoint (slicePolygon.pointsList, startB);

			slicePolygon.pointsList.Reverse();

			Polygon2D poly = new Polygon2D();
			
			foreach(Vector2D vec in polygon.pointsList) {
				poly.AddPoint(vec);
			}

			foreach(Vector2D vec in slicePolygon.pointsList) {
				poly.AddPoint(vec);
			}

			result.AddPolygon(poly);

			return(result);
		}

		static public Slice2D LinearCutSlice(Polygon2D polygon, LinearCut linearCut) {
			List<Vector2D> slice = linearCut.GetPointsList().ToVector2DList();
			Slice2D result = Slice2D.Create (null, linearCut);

			if (slice.Count < 1) {
				return(result);
			}

			Vector2D startPoint = null;
			foreach (Vector2D id in slice) {
				if (polygon.PointInPoly (id) == false) {
					startPoint = id;
					break;
				}
			}

			Polygon2D newPolygon = new Polygon2D(slice);

			slice = Vector2DList.GetListStartingPoint (slice, startPoint);
			slice.Add(startPoint);

			if (polygon.PolyInPoly(newPolygon)) {
				polygon.AddHole(newPolygon);
				result.AddPolygon(polygon);
				return(result);
			}

			result = Slicer.Slice (polygon, slice);
		
			return(result);
		}

		static public Slice2D ComplexCutSlice(Polygon2D polygon, ComplexCut complexCut) {
			List<Vector2D> slice = complexCut.GetPointsList().ToVector2DList();
			Slice2D result = Slice2D.Create (null, complexCut);

			if (slice.Count < 1) {
				return(result);
			}

			if (Math2D.SliceIntersectItself(slice)) {
				Debug.LogWarning("Slicer2D: Complex Cut Slicer intersected with itself!");

				return(result);
			}

			Vector2D startPoint = null;
			foreach (Vector2D id in slice) {
				if (polygon.PointInPoly (id) == false) {
					startPoint = id;
					break;
				}
			}

			Polygon2D newPolygon = new Polygon2D(slice);

			slice = Vector2DList.GetListStartingPoint (slice, startPoint);
			slice.Add(startPoint);

			if (polygon.PolyInPoly(newPolygon)) {
				polygon.AddHole(newPolygon);
				result.AddPolygon(polygon);
				return(result);
			}

			result = Slicer.Slice (polygon, slice);
		
			return(result);
		}

		// Create Polygon Inside? Extended Method?
		public static Slice2D SlicePolygonInside(Polygon2D polygon, List<Vector2D> slice) {
			Slice2D result = Slice2D.Create (null, slice);

			if (Sliceable2D.complexSliceType != Sliceable2D.SliceType.SliceHole) {
				return(result);
			}

			List<List<Vector2D>> checkSlices = new List<List<Vector2D>>();
			List<Vector2D> curSlice = null;
			
			foreach(Vector2D p in slice) {
				if (polygon.PointInPoly(p)) {
					if (curSlice == null) {
						curSlice = new List<Vector2D>();
						checkSlices.Add(curSlice);
					}
					curSlice.Add(p);
				} else {
					curSlice = null;
				}
			}

			//bool createPoly = false;
			List<Polygon2D> newPolygons = new List<Polygon2D>();
			Polygon2D newPoly = null;

			foreach(List<Vector2D> checkSlice in checkSlices) {
				foreach (Pair2D pairA in Pair2D.GetList(checkSlice, false)) {
					foreach (Pair2D pairB in Pair2D.GetList(checkSlice, false)) {
						Vector2D intersection = Math2D.GetPointLineIntersectLine (pairA, pairB);
						if (intersection != null && (pairA.A != pairB.A && pairA.B != pairB.B && pairA.A != pairB.B && pairA.B != pairB.A)) {
							if (newPoly == null) {
								
								newPoly =  new Polygon2D ();
								newPolygons.Add(newPoly);
								newPoly.AddPoint (intersection);

							} else {

							//newPoly.AddPoint (intersection);
								newPoly = null;

							}
						}
					}
					if (newPoly != null) {
						newPoly.AddPoint (pairA.B);
					}
				}
			}
			
			foreach(Polygon2D poly in new List<Polygon2D>(newPolygons)) {
				if (poly.pointsList.Count < 3) {
					newPolygons.Remove(poly);
					continue;
				}
			}

			if (newPolygons.Count > 0) {
				result.AddPolygon (polygon);

				foreach(Polygon2D poly in newPolygons) {
										
					List <Polygon2D> polys = new List<Polygon2D> (polygon.holesList);
					foreach (Polygon2D hole in polys) {
						if (poly.PolyInPoly (hole) == true) {
							polygon.holesList.Remove (hole);
							poly.AddHole (hole);
						}
					}

					polygon.AddHole (poly);
				}
				
				return(result);
			}

			return(result);
		}
	}
}