using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities2D {
	
	public class Vector2DList {
		static Pair2D pair2D = Pair2D.Zero();

		static public List<Vector2D> PointsToWorldSpace(Transform transform, List<Vector2D> pointsList) {
			List<Vector2D> resultList = new List<Vector2D>(); 
			foreach (Vector2D id in pointsList) {
				resultList.Add (new Vector2D(transform.TransformPoint (id.ToVector2())));
			}

			return(resultList);
		}
		
		/// <summary>
		/// Return a sorted list by distance to a given 2D point reference
		/// ------------ Looks like decending instead of assending!!!
		/// </summary>
		static public List<Vector2D> GetListSortedToPoint(List<Vector2D> pointsList, Vector2D point) {
			List<Vector2D> resultList = new List<Vector2D>();
			List<Vector2D> listCopy = new List<Vector2D> (pointsList);

			Vector2D p;
			double d;
			double dist;
			Vector2D obj;

			while (listCopy.Count > 0) {
				
				dist = 1e+10f;
				obj = null;

				for(int i = 0; i < listCopy.Count; i++) {
					p = listCopy[i];

					d = System.Math.Sqrt(System.Math.Pow(p.x - point.x, 2) + System.Math.Pow(p.y - point.y, 2)); 
					if (d < dist) {
						obj = p;
						dist = d;
					}
				}

				if (obj != null) {
					resultList.Add(obj);
					listCopy.Remove(obj);
				}
			}

			return(resultList);
		}

		/// <summary>
		/// Return a list which starts with given 2D vector
		/// </summary>
		static public List<Vector2D> GetListStartingPoint(List<Vector2D> pointsList, Vector2D point) {
			// What if pointList does not contain point? 
			List<Vector2D> result = new List<Vector2D> ();
			bool start = false;

			Vector2D p;

			for(int i = 0; i < pointsList.Count; i++) {
				p = pointsList[i];

				if (p == point || start == true) {
					result.Add (p);
					start = true;
				}
			}

			for(int i = 0; i < pointsList.Count; i++) {
				p = pointsList[i];
				
				if (p == point) {
					start = false;
				}

				if (start == true) {
					result.Add (p);
				}
			}
			return(result);
		}

		/// <summary>
		/// Return a list which starts with first interesction with given line
		/// </summary>
		public static List<Vector2D> GetListStartingIntersectLine(List<Vector2D> pointsList, Pair2D line) {
			List<Vector2D> result = new List<Vector2D> ();
			bool start = false;

			Pair2D id = pair2D;
			Vector2D r;

			id.B = pointsList.Last();

			for(int x = 0; x < pointsList.Count; x++) {
				id.A = pointsList[x];

				r = Math2D.GetPointLineIntersectLine (id, line);
				if (start == true) {
					result.Add (id.B);
				}

				if (r != null) {
					result.Add (r);
					start = true;
				}

				id.B = id.A;
			}

			id.B = pointsList.Last();

			for(int x = 0; x < pointsList.Count; x++) {
				id.A = pointsList[x];

				r = Math2D.GetPointLineIntersectLine (id, line);
				if (start == true) {
					result.Add (id.B);
				}
					
				if (r != null) {
					result.Add (r);
					start = false;
				}

				id.B = id.A;
			}
			return(result);
		}

		// Might Break (Only for 2 collisions)
		/// <summary>
		/// Return a list which starts with first interesction with given slice
		/// </summary>
		public static List<Vector2D> GetListStartingIntersectSlice(List<Vector2D> pointsList, List<Vector2D> slice)
		{
			List<Vector2D> result = new List<Vector2D> ();
			bool start = false;

			List<Pair2D> pointsPairList = Pair2D.GetList(pointsList);

			foreach (Pair2D p in pointsPairList) {
				List<Vector2D> r = Math2D.GetListLineIntersectSlice (p, slice);
				if (start == true) {
					result.Add (p.A);
				}

				if (r.Count > 0) {
					result.Add (r.First());
					start = true;
				}
			}

			foreach (Pair2D p in pointsPairList) {
				List<Vector2D> r = Math2D.GetListLineIntersectSlice (p, slice);
				if (start == true) {
					result.Add (p.A);
				}

				if (r.Count > 0) {
					result.Add (r.First());
					start = false;
				}
			}
			return(result);
		}
	}
}