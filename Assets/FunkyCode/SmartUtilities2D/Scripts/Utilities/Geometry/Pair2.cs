using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Utilities2D {
	
	public struct Pair2 {
		public Vector2 a;
		public Vector2 b;

		public static Pair2 zero = new Pair2(Vector2.zero, Vector2.zero);

		public Pair2(Vector2 a, Vector2 b) {
			this.a = a;
			this.b = b;
		}

		public Pair2D ToPair2D() {
			return(new Pair2D(new Vector2D(a), new Vector2D(b)));
		}
		
		static public List<Pair2> GetList(List<Vector2D> list, bool connect = true){
			List<Pair2> pairsList = new List<Pair2>();
			if (list.Count > 0) {
				Vector2D p0 = null;
				
				if (connect == true) {
					p0 = list.Last ();
				}
				
				foreach (Vector2D p1 in list) {
					if (p0 != null) {
						pairsList.Add (new Pair2 (p0.ToVector2(), p1.ToVector2()));
					}

					p0 = p1;
				}
			}
			return(pairsList);
		}

		static public List<Pair2> GetList(Vector2List list, bool connect = true){
			List<Pair2> pairsList = new List<Pair2>();
			if (list.points.Count > 0) {
				Vector2? p0 = null;

				if (connect == true) {
					p0 = list.points.Last();
				}
				
				foreach (Vector2 p1 in list.points) {
					if (p0 != null) {
						pairsList.Add (new Pair2 (p0.Value, p1));
					}

					p0 = p1;
				}
			}
			return(pairsList);
		}
	}







	namespace Extensions {

		public static class Vector2Extension {

			public static Vector2D ToVector2D(this Vector2 v) {
				return(new Vector2D(v));
			}

			public static Vector3 ToVector3(this Vector2 v, float z) {
				return(new Vector3(v.x, v.y, z));
			}


			public static Vector2 Push(this Vector2 v, float rot, float distance) {
				v.x += Mathf.Cos(rot) * distance;
				v.y += Mathf.Sin(rot) * distance;
				return(v);
			}

			public static float Atan2(this Vector2 a, Vector2 b) {
				return(Mathf.Atan2 (a.y - b.y, a.x - b.x));
			}

			public static Vector2 Push(this Vector2 v, float rot, float distance, Vector2 scale) {
				v.x += Mathf.Cos(rot) * distance * scale.x;
				v.y += Mathf.Sin(rot) * distance * scale.y;
				return(v);
			}
		}
	}
}