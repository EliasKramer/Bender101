using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Utilities2D {

	public struct Vector2List {
		public List<Vector2> points;

		public int Count() {
			return(points.Count);
		}

		public void Insert(int id, Vector2 vec) {
			points.Insert(id, vec);
		}

		public Vector2List Copy() {
			Vector2List list = new Vector2List(true);
			foreach(Vector2 p in points) {
				list.Add(p);
			}
			return(list);
		}

		public void RemoveAt(int id) {
			points.RemoveAt(id);
		}

		public int IndexOf(Vector2 v) {
			return(points.IndexOf(v));
		}

		public void Clear() {
			points.Clear();
		}

		public Vector2List(bool use) {
			points = new List<Vector2>();
		}

		public Vector2List(List<Vector2D> list) {
			points = new List<Vector2>();
			foreach(Vector2D p in list) {
				Add(p.ToVector2());
			}
		}

		public void Add(Vector2 v) {
			points.Add(v);
		}

		public List<Vector2D> ToVector2DList() {
			List<Vector2D> points2D = new List<Vector2D>();
			foreach(Vector2 p in points) {
				points2D.Add(new Vector2D(p));
			}
			return(points2D);
		}

		public Vector2 First() {
			return(points.First());
		}

		public Vector2 Last() {
			return(points.Last());
		}
	}
}