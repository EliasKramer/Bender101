using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Utilities2D {
	public class Math2D {
		static Pair2D a = Pair2D.Zero();
		static Pair2D b = Pair2D.Zero();
		
		static Pair2D pair2D = Pair2D.Zero();

		public static Vector3 GetPitchYawRollRad(Quaternion rotation) {
			float roll = Mathf.Atan2(2*rotation.y*rotation.w - 2*rotation.x*rotation.z, 1 - 2*rotation.y*rotation.y - 2*rotation.z*rotation.z);
			float pitch = Mathf.Atan2(2*rotation.x*rotation.w - 2*rotation.y*rotation.z, 1 - 2*rotation.x*rotation.x - 2*rotation.z*rotation.z);
			float yaw = Mathf.Asin(2*rotation.x*rotation.y + 2*rotation.z*rotation.w);
			
			return new Vector3(pitch, roll, yaw);
		}

		public static Vector3 GetPitchYawRollDeg(Quaternion rotation) {
			Vector3 radResult = GetPitchYawRollRad(rotation);
			return new Vector3(radResult.x * Mathf.Rad2Deg, radResult.y * Mathf.Rad2Deg, radResult.z * Mathf.Rad2Deg);
		}


		public static Rect GetBounds(List<Vector2D> pointsList) {
			double rMinX = 1e+10f;
			double rMinY = 1e+10f;
			double rMaxX = -1e+10f;
			double rMaxY = -1e+10f;

			Vector2D id;

			for(int i = 0; i < pointsList.Count; i++) {
				id = pointsList[i];

				rMinX = System.Math.Min (rMinX, id.x);
				rMinY = System.Math.Min (rMinY, id.y);
				rMaxX = System.Math.Max (rMaxX, id.x);
				rMaxY = System.Math.Max (rMaxY, id.y);
			}

			return(new Rect((float)rMinX, (float)rMinY, (float)System.Math.Abs(rMinX - rMaxX), (float)System.Math.Abs(rMinY - rMaxY))); 
		}

		public static Rect GetBounds(Pair2D pair) {
			double rMinX = 1e+10f;
			double rMinY = 1e+10f;
			double rMaxX = -1e+10f;
			double rMaxY = -1e+10f;

			Vector2D id = pair.A;
			rMinX = System.Math.Min (rMinX, id.x);
			rMinY = System.Math.Min (rMinY, id.y);
			rMaxX = System.Math.Max (rMaxX, id.x);
			rMaxY = System.Math.Max (rMaxY, id.y);
		
			id = pair.B;
			rMinX = System.Math.Min (rMinX, id.x);
			rMinY = System.Math.Min (rMinY, id.y);
			rMaxX = System.Math.Max (rMaxX, id.x);
			rMaxY = System.Math.Max (rMaxY, id.y);

			return(new Rect((float)rMinX, (float)rMinY, (float)System.Math.Abs(rMinX - rMaxX), (float)System.Math.Abs(rMinY - rMaxY))); 
		}


		public static bool PolyInPoly(Polygon2D polyA, Polygon2D polyB) {
			for(int i = 0; i < polyB.pointsList.Count; i++) {
				if (PointInPoly (polyB.pointsList[i], polyA) == false) {
					return(false);
				}
			}

			if (PolyIntersectPoly (polyA, polyB) == true) {
				return(false);
			}
			
			return(true);
		}

		// Is it not finished?
		public static bool PolyCollidePoly(Polygon2D polyA, Polygon2D polyB) {
			if (PolyIntersectPoly (polyA, polyB) == true) {
				return(true);
			}

			if (PolyInPoly (polyA, polyB) == true) {
				return(true);
			}

			if (PolyInPoly (polyB, polyA) == true) {
				return(true);
			}
			
			return(false);
		}

		public static bool PolyIntersectPoly(Polygon2D polyA, Polygon2D polyB) {
			if (polyB.pointsList.Count < 2) {
				return(false);
			}

			a.B = polyA.pointsList.Last();

			for(int x = 0; x < polyA.pointsList.Count; x++) {
				a.A = polyA.pointsList[x];

				b.B = polyB.pointsList.Last();

				for(int y = 0; y < polyB.pointsList.Count; y++) {
					b.A = polyB.pointsList[y];

					if (LineIntersectLine (a, b)) {
						return(true);
					}

					b.B = b.A;
				}

				a.B = a.A;
			}
			
			return(false);
		}

		public static bool SliceIntersectPoly(List <Vector2D> slice, Polygon2D poly) {
			Pair2D pairA = new Pair2D(null,  null);

			foreach (Vector2D pointA in slice) {
				pairA.B = pointA;
				
				if (pairA.A != null && pairA.B != null) {

					Pair2D pairB = new Pair2D(new Vector2D(poly.pointsList.Last()),  null);

					foreach (Vector2D pointB in poly.pointsList) {
						pairB.B = pointB;

						if (LineIntersectLine (pairA, pairB)) {
							return(true);
						}

						pairB.A = pointB;
					}
				}

				pairA.A = pointA;
			}

			return(false);
		}

		public static bool LineIntersectSlice(Pair2D pairA, List <Vector2D> slice) {
			Pair2D pairB = new Pair2D(null,  null);

			for(int i = 0; i < slice.Count - 1; i++) {
				pairB.A = slice[i];
				pairB.B = slice[i + 1];

				if (LineIntersectLine (pairA, pairB)) {
					return(true);
				}			
			}

			return(false);
		}
			
		static Pair2D line_intersect_poly = new Pair2D(null, null);

		public static bool LineIntersectPoly(Pair2D line, Polygon2D poly) {
			Pair2D pair = line_intersect_poly;

			pair.A = new Vector2D(poly.pointsList.Last());
			pair.B = new Vector2D(Vector2.zero);

			for(int i = 0; i < poly.pointsList.Count; i++) {
				pair.B = poly.pointsList[i];

				if (LineIntersectLine (line, pair)) {
					return(true);
				}
				
				pair.A = pair.B;
			}
			
			return(false);
		}

		public static bool LineIntersectLine(Pair2D lineA, Pair2D lineB) {
			if (GetBoolLineIntersectLine (lineA, lineB)) {
				return(true);
			}

			return(false);
		}

		public static bool SliceIntersectItself(List<Vector2D> slice) {
			Pair2D pairA = new Pair2D(null,  null);
			Pair2D pairB = new Pair2D(null,  null);

			for(int i = 0; i < slice.Count - 1; i++) {
				pairA.A = slice[i];
				pairA.B = slice[i + 1];

				for(int x = 0; x < slice.Count - 1; x++) {
					pairB.A = slice[x];
					pairB.B = slice[x + 1];

					if (GetBoolLineIntersectLine (pairA, pairB)) {
						if (pairA.A != pairB.A && pairA.B != pairB.B && pairA.A != pairB.B && pairA.B != pairB.A) {
							return(true);
						}
					}	
				}			
			}
			
			return(false);
		}

		static double tor = 1E-10;

		public static Vector2D GetPointLineIntersectLine(Pair2D lineA, Pair2D lineB) {
			double dx_cx = lineB.B.x - lineB.A.x;
			double dy_cy = lineB.B.y - lineB.A.y;
			double bx_ax = lineA.B.x - lineA.A.x;
			double by_ay = lineA.B.y - lineA.A.y;
			double de = bx_ax * dy_cy - by_ay * dx_cx;

			if (System.Math.Abs(de) < 0.0001d) {
				return(null);
			}

			if (de > - tor && de < tor) {
				return(null);
			}

			double ax_cx = lineA.A.x - lineB.A.x;
			double ay_cy = lineA.A.y - lineB.A.y;

			double r = (ay_cy * dx_cx - ax_cx * dy_cy) / de;
			double s = (ay_cy * bx_ax - ax_cx * by_ay) / de;

			if ((r < 0) || (r > 1) || (s < 0)|| (s > 1)) {
				return(null);
			}

			return(new Vector2D (lineA.A.x + r * bx_ax, lineA.A.y + r * by_ay));
		}

		public static bool GetBoolLineIntersectLine(Pair2D lineA, Pair2D lineB) {
			double dx_cx = lineB.B.x - lineB.A.x;
			double dy_cy = lineB.B.y - lineB.A.y;
			double bx_ax = lineA.B.x - lineA.A.x;
			double by_ay = lineA.B.y - lineA.A.y;
			double de = bx_ax * dy_cy - by_ay * dx_cx;

			if (System.Math.Abs(de) < 0.0001d) {
				return(false);
			}

			if (de > - tor && de < tor) {
				return(false);
			}

			double ax_cx = lineA.A.x - lineB.A.x;
			double ay_cy = lineA.A.y - lineB.A.y;

			double r = (ay_cy * dx_cx - ax_cx * dy_cy) / de;
			double s = (ay_cy * bx_ax - ax_cx * by_ay) / de;

			if ((r < 0) || (r > 1) || (s < 0)|| (s > 1)) {
				return(false);
			}

			return(true);
		}

		public static bool PointInPoly(Vector2D point, Polygon2D poly) {
			if (poly.pointsList.Count < 3) {
				return(false);
			}

			int total = 0;
			int diff = 0;

			Pair2D id = pair2D;
			id.A = poly.pointsList[poly.pointsList.Count - 1];
			
			for(int i = 0; i < poly.pointsList.Count; i++) {
				id.B = poly.pointsList[i];

				diff = (GetQuad (point, id.A) - GetQuad (point, id.B));

				switch (diff) {
					case -2: case 2:
						if ((id.B.x - (((id.B.y - point.y) * (id.A.x - id.B.x)) / (id.A.y - id.B.y))) < point.x)
							diff = -diff;

						break;

					case 3:
						diff = -1;
						break;

					case -3:
						diff = 1;
						break;

					default:
						break;   
				}

				total += diff;

				id.A = id.B;
			}

			return(Mathf.Abs(total) == 4);
		}

		private static int GetQuad(Vector2D axis, Vector2D vert) {
			if (vert.x < axis.x) {
				if (vert.y < axis.y) {
					return(1);
				}
				return(4);
			}
			if (vert.y < axis.y) {
				return(2);
			}
			return(3);
		}
			
		// Getting List is Slower
		public static List <Vector2D> GetListLineIntersectPoly(Pair2D line, Polygon2D poly) {
			List <Vector2D> result = new List <Vector2D>() ;

			Vector2D intersection;

			Pair2D pair = new Pair2D(new Vector2D(poly.pointsList.Last()),  null);
			
			for(int i = 0; i < poly.pointsList.Count; i++) {
				pair.B = poly.pointsList[i];

				intersection = GetPointLineIntersectLine (line, pair);
				if (intersection != null) {
					result.Add(intersection);
				}

				pair.A = pair.B;
			}
			return(result);
		}

		public static List<Vector2D> GetListLineIntersectSlice(Pair2D pair, List<Vector2D> slice) {
			List<Vector2D> resultList = new List<Vector2D> ();
			
			Pair2D id = new Pair2D(null,  null);
			Vector2D result;

			for(int i = 0; i < slice.Count - 1; i++) {
				id.A = slice[i];
				id.B = slice[i + 1];

				result = GetPointLineIntersectLine(id, pair);
				if (result != null) {
					resultList.Add(result);
				}
			}
			return(resultList);
		}

		public class Angle {
			public static float FindAngle(Vector2 p0, Vector2 p1, Vector2 p2) {
				float b = Mathf.Pow(p1.x-p0.x,2) + Mathf.Pow(p1.y-p0.y,2);
				float a = Mathf.Pow(p1.x-p2.x,2) + Mathf.Pow(p1.y-p2.y,2);
				float c = Mathf.Pow(p2.x-p0.x,2) + Mathf.Pow(p2.y-p0.y,2);
				return Mathf.Acos( (a + b - c) / Mathf.Sqrt(4 * a * b) );
			}

			public static Vector2 ReflectAngle(Vector2 v, float wallAngle) {
				//normal vector to the wall
				Vector2 n = new Vector2(Mathf.Cos(wallAngle + Mathf.PI / 2), Mathf.Sin(wallAngle + Mathf.PI / 2));

				// p is the projection of V onto the normal
				float dotproduct = v.x * n.x + v.y * n.y;

				// the velocity after hitting the wall is V - 2p, so just subtract 2*p from V
				return(new Vector2(v.x - 2f * (dotproduct * n.x), v.y - 2f * (dotproduct * n.y)));
			}
		}

		public class Circle {
			static public bool IntersectPolygon(Polygon2D poly, Vector2D circle, float radius) {
				foreach (Pair2D id in Pair2D.GetList(poly.pointsList)) { // Remove GetList()
					if (IntersectLine(id, circle, radius) == true) {
						return(true);
					}
				}
				return(false);
			}
			
			static public bool IntersectSlice(List<Vector2D> points, Vector2D circle, float radius) {
				foreach (Pair2D id in Pair2D.GetList(points, false)) { // Remove GetList()
					if (IntersectLine(id, circle, radius) == true) {
						return(true);
					}
				}
				return(false);
			}

			static public bool IntersectLine(Pair2D line, Vector2D circle, float radius) {
				double sx = line.B.x - line.A.x;
				double sy = line.B.y - line.A.y;

				double q = ((circle.x - line.A.x) * (line.B.x - line.A.x) + (circle.y - line.A.y) * (line.B.y - line.A.y)) / (sx * sx + sy * sy);
					
				if (q < 0.0f) {
					q = 0.0f;
				} else if (q > 1.0) {
					q = 1.0f;
				}

				double dx = circle.x - ((1.0f - q) * line.A.x + q * line.B.x);
				double dy = circle.y - ((1.0f - q) * line.A.y + q * line.B.y);

				if (dx * dx + dy * dy < radius * radius) {
					return(true);
				} else {
					return(false);
				}
			}
		}

		public class Distance {
			public double value;

			public Distance(double val) {
				value = val;
			}
			
			public static Distance PolygonToPolygon(Polygon2D polyA, Polygon2D polyB) {
				Distance result = null;

				double minDistance = 99999f;
				a.B = polyA.pointsList.Last();

				for(int x = 0; x < polyA.pointsList.Count; x++) {
					a.A = polyA.pointsList[x];

					for(int y = 0; y < polyB.pointsList.Count; y++) {
						Vector2D v = polyB.pointsList[y];

						double distance = PointToLine(v, a);

						if (distance < minDistance) {
							minDistance = distance;

							result = new Distance(distance);
						}
					}

					a.B = a.A;
				}
				
				return(result);	
			}

			public static double PointToLine(Vector2D p, Pair2D pair) {
				double A = p.x - pair.A.x;
				double B = p.y - pair.A.y;
				double C = pair.B.x - pair.A.x;
				double D = pair.B.y - pair.A.y;

				double dot = A * C + B * D;
				double len_sq = C * C + D * D;

				double param = -1;

				// in case of 0 length line
				if (len_sq != 0)  {
					param = dot / len_sq;
				}
					
				double xx, yy;

				if (param < 0) {
					xx = pair.A.x;
					yy = pair.A.y;
				} else if (param > 1) {
					xx = pair.B.x;
					yy = pair.B.y;
				} else {
					xx = pair.A.x + param * C;
					yy = pair.A.y + param * D;
				}

				double dx = p.x - xx;
				double dy = p.y - yy;

				return System.Math.Sqrt(dx * dx + dy * dy);
			}
		}



		// Lighting 2D
		
		public static List<Vector2D> GetConvexHull(List<Vector2D> points) {
			//If we have just 3 points, then they are the convex hull, so return those
			if (points.Count == 3)
			{
				//These might not be ccw, and they may also be colinear
				return points;
			}

			//If fewer points, then we cant create a convex hull
			if (points.Count < 3)
			{
				return null;
			}
				
			//The list with points on the convex hull
			List<Vector2D> convexHull = new List<Vector2D>();

			//Step 1. Find the vertex with the smallest x coordinate
			//If several have the same x coordinate, find the one with the smallest z
			Vector2D startVertex = points[0];

			Vector2 startPos = startVertex.ToVector2() ;

			for (int i = 1; i < points.Count; i++)
			{
				Vector2 testPos = points[i].ToVector2() ;

				//Because of precision issues, we use Mathf.Approximately to test if the x positions are the same
				if (testPos.x < startPos.x || (Mathf.Approximately(testPos.x, startPos.x) && testPos.y < startPos.y))
				{
					startVertex = points[i];

					startPos = startVertex.ToVector2() ;
				}
			}

			//This vertex is always on the convex hull
			convexHull.Add(startVertex);

			points.Remove(startVertex);

			//Step 2. Loop to generate the convex hull
			Vector2D currentPoint = convexHull[0];

			//Store colinear points here - better to create this list once than each loop
			List<Vector2D> colinearPoints = new List<Vector2D>();

			int counter = 0;

			while (true)
			{
				//After 2 iterations we have to add the start position again so we can terminate the algorithm
				//Cant use convexhull.count because of colinear points, so we need a counter
				if (counter == 2)
				{            
					points.Add(convexHull[0]);
				}

				//Pick next point randomly
				Vector2D nextPoint = points[Random.Range(0, points.Count)];

				//To 2d space so we can see if a point is to the left is the vector ab
				Vector2 a = currentPoint.ToVector2();

				Vector2 b = nextPoint.ToVector2();

				//Test if there's a point to the right of ab, if so then it's the new b
				for (int i = 0; i < points.Count; i++)
				{
					//Dont test the point we picked randomly
					if (points[i].Equals(nextPoint))
					{
						continue;
					}

					Vector2 c = points[i].ToVector2() ;

					//Where is c in relation to a-b
					// < 0 -> to the right
					// = 0 -> on the line
					// > 0 -> to the left
					float relation = IsAPointLeftOfVectorOrOnTheLine(a, b, c);

					//Colinear points
					//Cant use exactly 0 because of floating point precision issues
					//This accuracy is smallest possible, if smaller points will be missed if we are testing with a plane
					float accuracy = 0.00001f;

					if (relation < accuracy && relation > -accuracy)
					{
						colinearPoints.Add(points[i]);
					}
					//To the right = better point, so pick it as next point on the convex hull
					else if (relation < 0f)
					{
						nextPoint = points[i];

						b = nextPoint.ToVector2();

						//Clear colinear points
						colinearPoints.Clear();
					}
					//To the left = worse point so do nothing
				}



				//If we have colinear points
				if (colinearPoints.Count > 0)
				{
					colinearPoints.Add(nextPoint);

					//Sort this list, so we can add the colinear points in correct order
					colinearPoints = colinearPoints.OrderBy(n => Vector3.SqrMagnitude(n.ToVector2() - currentPoint.ToVector2())).ToList();

					convexHull.AddRange(colinearPoints);

					currentPoint = colinearPoints[colinearPoints.Count - 1];

					//Remove the points that are now on the convex hull
					for (int i = 0; i < colinearPoints.Count; i++)
					{
						points.Remove(colinearPoints[i]);
					}

					colinearPoints.Clear();
				}
				else
				{
					convexHull.Add(nextPoint);

					points.Remove(nextPoint);

					currentPoint = nextPoint;
				}

				//Have we found the first point on the hull? If so we have completed the hull
				if (currentPoint.Equals(convexHull[0]))
				{
					//Then remove it because it is the same as the first point, and we want a convex hull with no duplicates
					convexHull.RemoveAt(convexHull.Count - 1);

					break;
				}

				counter += 1;
			}

			return convexHull;
		}

		public static float IsAPointLeftOfVectorOrOnTheLine(Vector2 a, Vector2 b, Vector2 p) {
			float determinant = (a.x - p.x) * (b.y - p.y) - (a.y - p.y) * (b.x - p.x);

			return determinant;
		}


		public static Vector2 ReflectAngle(Vector2 v, float wallAngle) {
			//normal vector to the wall
			Vector2 n = new Vector2(Mathf.Cos(wallAngle + Mathf.PI / 2), Mathf.Sin(wallAngle + Mathf.PI / 2));

			// p is the projection of V onto the normal
			float dotproduct = v.x * n.x + v.y * n.y;

			// the velocity after hitting the wall is V - 2p, so just subtract 2*p from V
			return(new Vector2(v.x - 2f * (dotproduct * n.x), v.y - 2f * (dotproduct * n.y)));
		}
		
		public static float FindAngle(Vector2 p0, Vector2 p1, Vector2 p2) {
			float b = Mathf.Pow(p1.x-p0.x,2) + Mathf.Pow(p1.y-p0.y,2);
			float a = Mathf.Pow(p1.x-p2.x,2) + Mathf.Pow(p1.y-p2.y,2);
			float c = Mathf.Pow(p2.x-p0.x,2) + Mathf.Pow(p2.y-p0.y,2);
			return Mathf.Acos( (a + b - c) / Mathf.Sqrt(4 * a * b) );
		}
	}

	/*
		public static bool SliceIntersectSlice(List <Vector2D> sliceA, List <Vector2D> sliceB) {
			Pair2D pairA = new Pair2D(null,  null);

			foreach (Vector2D pointA in sliceA) {
				pairA.B = pointA;

				if (pairA.A != null && pairA.B != null) {

					Pair2D pairB = new Pair2D(null,  null);

					foreach (Vector2D pointB in sliceB) {
						pairB.B = pointB;

						if (pairB.A != null && pairB.B != null) {
							if (LineIntersectLine (pairA, pairB)) {
								return(true);
							}
						}

						pairB.A = pointB;
					}
				}

				pairA.A = pointA;
			}

			return(false);
		}

	*/
}