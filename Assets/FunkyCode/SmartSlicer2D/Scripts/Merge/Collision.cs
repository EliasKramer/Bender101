using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities2D;

namespace Slicer2D.Merge {

	public class Collision {
		public bool error = false;

		public int collisionCount = 0; // Change?

		public List<Point> collisionSlice = new List<Point>();

		public Vector2D First() {
			return(collisionSlice.First().vector);
		}

		public Vector2D Last() {
			return(collisionSlice.Last().vector);
		}

		public List<Vector2D> GetPoints() {
			List<Vector2D> points = new List<Vector2D>();
			foreach(Point point in collisionSlice) {
				points.Add(point.vector);
			}
			return(points);
		}

		public void Reverse() {
			collisionSlice.Reverse();
		}

		
		public Collision(Polygon2D polygon, List<Vector2D> slice) {
			bool inside = true;

			Pair2D pair = Pair2D.Zero();

			List<Vector2D> intersections;
			
			for(int i = 0; i < slice.Count - 1; i++) {
				pair.A = slice[i];
				pair.B = slice[i + 1];

				intersections = polygon.GetListLineIntersectPoly(pair);

				switch(intersections.Count) {
					case 1:
						collisionCount += 1;

						collisionSlice.Add (new Point(intersections[0], Point.Type.Intersection));

						inside = !inside;

						break;

					case 2:
						collisionCount += intersections.Count; // Check if it's okay

						if (Vector2D.Distance (intersections[0], pair.A) < Vector2D.Distance (intersections[1], pair.A)) {
							collisionSlice.Add (new Point(intersections[0], Point.Type.Intersection));
							collisionSlice.Add (new Point(intersections[1], Point.Type.Intersection));
						} else {
							collisionSlice.Add (new Point(intersections[1], Point.Type.Intersection));
							collisionSlice.Add (new Point(intersections[0], Point.Type.Intersection));
						}
						break;

					case 0:
						break;

					default:
						error = true;

						break;
				}

				if (inside == false) {
					collisionSlice.Add (new Point(pair.B, Point.Type.Outside));
				}
			}
		}

		public class Point {
			public enum Type {Intersection,  Outside};
			public Vector2D vector;
			public Type collision;
			
			public Point(Vector2D pos, Type col) {
				vector = pos;
				collision = col;
			}
		}
	}
}