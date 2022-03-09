using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D.Merge {

	public class Merge2D {
		// List<Vector2D> slice = new List<Vector2D>();

		public List<Polygon2D> polygons =new List<Polygon2D>();
		public List<Vector2D> collisions = new List<Vector2D>();
		public List<List<Vector2D>> slices = new List<List<Vector2D>>();

		public static Merge2D Create(List<Vector2D> slice) {
			Merge2D merge2D = new Merge2D ();
			//merge2D.slice = new List<Vector2D>(slice);

			return(merge2D);
		}

			public void AddCollision(Vector2D point) {
			if (collisions == null) {
				collisions = new List<Vector2D>();
			}
			collisions.Add (point);
		}

		
		public void AddPolygon(Polygon2D polygon) {
			if (polygons == null) {
				polygons = new List<Polygon2D>();
			}
			polygons.Add (polygon);
		}

		public void AddSlice(List<Vector2D> list) {
			slices.Add(list);
		}
	}
}

