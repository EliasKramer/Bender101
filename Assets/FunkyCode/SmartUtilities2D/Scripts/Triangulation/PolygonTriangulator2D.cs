using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Utilities2D {

	public class PolygonTriangulator2D : MonoBehaviour {
		public enum Triangulation {Advanced, Legacy};

		public static Mesh Triangulate3D(Polygon2D polygon, float z, Vector2 UVScale, Vector2 UVOffset, float UVRotation, Triangulation triangulation) {
			polygon.Normalize();

			Mesh result = null;
			switch (triangulation) {
				case Triangulation.Advanced:		
					List<Vector2> uvs = new List<Vector2>();
					List<Vector3> sideVertices = new List<Vector3>();
					List<int> sideTriangles = new List<int>();

					Vector3 pointA = new Vector3(0, 0, 0);
					Vector3 pointB = new Vector3(0, 0, 0);
					Vector3 pointC = new Vector3(0, 0, z);
					Vector3 pointD = new Vector3(0, 0, z);

					Vector2 uv0 = new Vector2();
					Vector2 uv1 = new Vector2();

					float a, b;

					int vCount = 0;

					Pair2D pair = new Pair2D(new Vector2D(polygon.pointsList.Last()), null);
					foreach (Vector2D p in polygon.pointsList) {
						pair.B = p;
						
						pointA.x = (float)pair.A.x;
						pointA.y = (float)pair.A.y;
						pointB.x = (float)pair.B.x;
						pointB.y = (float)pair.B.y;
						pointC.x = (float)pair.B.x;
						pointC.y = (float)pair.B.y;
						pointD.x = (float)pair.A.x;
						pointD.y = (float)pair.A.y;

						sideVertices.Add(pointA);
						sideVertices.Add(pointB);
						sideVertices.Add(pointC);
						sideVertices.Add(pointD);

						a = ((float)pair.A.x + 25f) / 50 + UVOffset.x / 2;
						b = ((float)pair.B.x + 25f) / 50 + UVOffset.x / 2;

						uv0.x = a;
						uv0.y = a;
						uv1.x = b;
						uv1.y = b;

						uvs.Add(new Vector2(0, 0));
						uvs.Add(new Vector2(0, 1));
						uvs.Add(new Vector2(1, 1));
						uvs.Add(new Vector2(1, 0));
						
						sideTriangles.Add(vCount + 2);
						sideTriangles.Add(vCount + 1);
						sideTriangles.Add(vCount + 0);
						
						sideTriangles.Add(vCount + 0);
						sideTriangles.Add(vCount + 3);
						sideTriangles.Add(vCount + 2);

						vCount += 4;

						pair.A = pair.B;
					}

					Mesh mainMesh = new Mesh();
					mainMesh.subMeshCount = 2;

					Mesh surfaceMesh = PerformTriangulationAdvanced (polygon, UVScale, UVOffset, UVRotation);

					///// UVS  /////
					foreach(Vector2 p in surfaceMesh.uv) {
						uvs.Add(p);
					}
					foreach(Vector2 p in surfaceMesh.uv) {
						uvs.Add(p);
					}

					surfaceMesh.triangles = surfaceMesh.triangles.Reverse().ToArray();

					///// VERTICES ////
					
					List<Vector3> vertices = new List<Vector3>();
					foreach(Vector3 v in sideVertices) {
						vertices.Add(v);
					}

					foreach(Vector3 v in surfaceMesh.vertices) {
						Vector3 nV = v;
						nV.z = z;
						vertices.Add(nV);
					}

					foreach(Vector3 v in surfaceMesh.vertices) {
						vertices.Add(v);
					}

					mainMesh.SetVertices(vertices);

					///// TRIANGLES /////

					List<int> triangles = new List<int>();
					foreach(int p in sideTriangles) {
						triangles.Add(p);
					}
					mainMesh.SetTriangles(triangles, 0);

					triangles.Clear();
					int count = sideVertices.Count();
					foreach(int p in surfaceMesh.triangles) {
						triangles.Add(p + count);
					}

					int trisCount = surfaceMesh.triangles.Count() / 3;
					int[] tris = surfaceMesh.triangles;
				
					for(var i = 0; i < trisCount; i++) {
						var tmp = tris[i * 3];
						tris[i * 3] = tris[i * 3 + 1];
						tris[i * 3 + 1] = tmp;
					}
									
					count += surfaceMesh.vertices.Count();  
					foreach(int p in tris) {
						triangles.Add(p + count);
					}

					mainMesh.SetTriangles(triangles, 1);

					///// LEFTOVERS /////

					mainMesh.uv = uvs.ToArray();
					mainMesh.RecalculateNormals();
					mainMesh.RecalculateBounds();
				
					result = mainMesh;

				break;
			}

			return(result);
		}

		public static Mesh Triangulate2D(Polygon2D polygon, Vector2 UVScale, Vector2 UVOffset, Triangulation triangulation) {
			polygon.Normalize();

			Mesh result = null;
			switch (triangulation) {
				case Triangulation.Advanced:
					Slicer2D.Profiler.IncAdvancedTriangulation();

					float GC = Slicer2D.Settings.GetGarbageCollector();
					if (GC > 0 & polygon.GetArea() < GC) {
						Debug.LogWarning("Smart Utility 2D: Slice was removed because it was too small");
						
						return(null);
					}

					Polygon2D newPolygon = new Polygon2D(PreparePolygon.Get(polygon));
					
					if (newPolygon.pointsList.Count < 3) {
						Debug.LogWarning("Smart Utility 2D: Mesh is too small for advanced triangulation, using simplified triangulations instead (size: " + polygon.GetArea() +")");
						
						result = PerformTriangulation(polygon, UVScale, UVOffset);
					
						return(result);
					}
					
					foreach (Polygon2D hole in polygon.holesList) {
						newPolygon.AddHole(new Polygon2D(PreparePolygon.Get(hole, -1)));
					}

					result = PerformTriangulation(newPolygon, UVScale, UVOffset);

				break;

				case Triangulation.Legacy:
					Slicer2D.Profiler.IncLegacyTriangulation();

					List<Vector2> list = new List<Vector2>();
					foreach(Vector2D p in polygon.pointsList) {
						list.Add(p.ToVector2());
					}

					result = Triangulator.Create(list.ToArray(), UVScale, UVOffset);

					return(result);
			}

			return(result);
		}

		public static Mesh PerformTriangulation(Polygon2D polygon, Vector2 UVScale, Vector2 UVOffset) {
			Polygon2DList.RemoveClosePoints(polygon.pointsList);

			foreach(Polygon2D hole in polygon.holesList) {
				Polygon2DList.RemoveClosePoints(hole.pointsList);
			}

			TriangulationWrapper.Polygon poly = new TriangulationWrapper.Polygon();

			List<Vector2> pointsList = null;
			List<Vector2> UVpointsList = null;

			Vector3 v = Vector3.zero;

			foreach (Vector2D p in polygon.pointsList) {
				v.x = (float)p.x;
				v.y = (float)p.y;

				poly.outside.Add (v);
				poly.outsideUVs.Add (new Vector2(v.x / UVScale.x + .5f + UVOffset.x, v.y / UVScale.y + .5f + UVOffset.y));
			}

			foreach (Polygon2D hole in polygon.holesList) {
				pointsList = new List<Vector2>();
				UVpointsList = new List<Vector2>();
				
				foreach (Vector2D p in hole.pointsList) {
					v.x = (float)p.x;
					v.y = (float)p.y;

					pointsList.Add (v);

					UVpointsList.Add (new Vector2(v.x / UVScale.x + .5f, v.y / UVScale.y + .5f));
				}

				poly.holes.Add (pointsList);
				poly.holesUVs.Add (UVpointsList);
			}

			return(TriangulationWrapper.CreateMesh (poly));
		}

		public static Mesh PerformTriangulationAdvanced(Polygon2D polygon, Vector2 UVScale, Vector2 UVOffset, float UVRotation) {
			TriangulationWrapper.Polygon poly = new TriangulationWrapper.Polygon();

			List<Vector2> pointsList = null;
			List<Vector2> UVpointsList = null;

			Vector3 v = Vector3.zero;

			foreach (Vector2D p in polygon.pointsList) {
				v = p.ToVector2();
				poly.outside.Add (v);

				float distance = Mathf.Sqrt((v.x / UVScale.x) * (v.x / UVScale.x) + (v.y / UVScale.y) * (v.y / UVScale.y));
				float rotation = Mathf.Atan2(v.y / UVScale.y, v.x / UVScale.x);

				float x = Mathf.Cos(rotation + UVRotation * Mathf.Deg2Rad) * distance;
				float y = Mathf.Sin(rotation + UVRotation * Mathf.Deg2Rad) * distance;

				poly.outsideUVs.Add (new Vector2(x + .5f + UVOffset.x, y + .5f + UVOffset.y));
			}

			foreach (Polygon2D hole in polygon.holesList) {
				pointsList = new List<Vector2> ();
				UVpointsList = new List<Vector2> ();
				
				foreach (Vector2D p in hole.pointsList) {
					v = p.ToVector2();
					pointsList.Add (v);
					UVpointsList.Add (new Vector2(v.x / UVScale.x + .5f + UVOffset.x, v.y / UVScale.y + .5f + UVOffset.y));
				}

				poly.holes.Add (pointsList);
				poly.holesUVs.Add (UVpointsList);
			}

			return(TriangulationWrapper.CreateMesh (poly));
		}
	}
}