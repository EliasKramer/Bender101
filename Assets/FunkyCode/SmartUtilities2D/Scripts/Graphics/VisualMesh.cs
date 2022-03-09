using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities2D.Extensions;

namespace Utilities2D {
	public class VisualMesh {
		const float pi = Mathf.PI;
		const float pi2 = pi / 2;

		public List<Mesh> meshes = new List<Mesh>();

		public Vector3[] verticesArray = new Vector3[0];
		public Vector2[] uvArray = new Vector2[0];
		public int[] trianglesArray = new int[0];

		public List<Vector3> vertices = new List<Vector3>();
		public List<Vector2> uv = new List<Vector2>();
		public List<int> triangles = new List<int>();

		public int triCount = 0;
		public int uvCount = 0;
		public int vertCount = 0;
		public int tris = 0;
		
		public VisualMesh() {
			meshes = new List<Mesh>();
		}

		public Mesh GetMesh(int id = 0) {
			if (meshes.Count <= id) {
				Mesh m = new Mesh();
				meshes.Add(m);
			}
			return(meshes[id]);
		}

		public Mesh Export(int id = 0) {
			Mesh mesh = GetMesh(id);

			if (verticesArray.Length != vertCount) {
				verticesArray = new Vector3[vertCount];
				uvArray = new Vector2[uvCount];
				trianglesArray = new int[triCount];
			}
				
			for(int i = 0; i < vertCount; i++) {
				verticesArray[i] = vertices[i];
			}

			for(int i = 0; i < uvCount; i++) {
				uvArray[i] = uv[i];
			}

			for(int i = 0; i < triCount; i++) {
				trianglesArray[i] = triangles[i];
			}

			mesh.triangles = null;
			mesh.uv = null;
			mesh.vertices = null;
			

			mesh.vertices = verticesArray;
			mesh.uv = uvArray;
			mesh.triangles = trianglesArray;		

			Clear();
			return(mesh);
		}

		public void Clear() {	
			triCount = 0;
			uvCount = 0;
			vertCount = 0;
			tris = 0;
		}

		public void AddVertice(Vector3 v) {
			if (vertCount >= vertices.Count) {
				vertices.Add(v);
			} else {
				vertices[vertCount] = v;
			}
			vertCount++;
		}

		public void AddTriangle(int tri) {
			if (triCount >= triangles.Count) {
				triangles.Add(tri);
			} else {
				triangles[triCount] = tri;
			}
			triCount++;
		}

		public void AddUV(Vector2 uvVar) {
			if (uvCount >= uv.Count) {
				uv.Add(uvVar);
			} else {
				uv[uvCount] = uvVar;
			}
			uvCount++;
		}

		public void CreatePolygon(Transform transform, Polygon2D polygon, float lineOffset, float lineWidth, bool connectedLine) {
			int count = polygon.pointsList.Count;
			int lastID = count - 1;
			int startID = 0;
			
			if (connectedLine == false) {
				lastID = 0;
				startID = 1;
			}
			
			Pair2 p = Pair2.zero;
			p.a = polygon.pointsList[lastID].ToVector2();
			
			for(int i = startID; i < count; i++) {
				p.b = polygon.pointsList[i].ToVector2();

				CreateLine(p, transform.localScale, lineWidth, lineOffset);

				p.a = p.b;
			}
			
			foreach(Polygon2D hole in polygon.holesList) {
				count = hole.pointsList.Count;
				lastID = count - 1;
				startID = 0;
			
				if (connectedLine == false) {
					lastID = 0;
					startID = 1;
				}

				p.a = hole.pointsList[lastID].ToVector2();
				
				for(int i = startID; i < count; i++) {
					p.b = hole.pointsList[i].ToVector2();

					CreateLine(p, transform.localScale, lineWidth, lineOffset);

					p.a = p.b;
				}
			} 
		}

		///// Box /////
		public void CreateBox(float size) {
			float uv0 = 0; 
			float uv1 = 1f;

			
			AddVertice( new Vector3(-size, -size, 0) );
			AddVertice( new Vector3(size, -size, 0) );
			AddVertice( new Vector3(size, size, 0) );
			AddVertice( new Vector3(-size, size, 0) );
		
			
			AddUV( new Vector2(uv0, uv0) );
			AddUV( new Vector2(uv1, uv0) );
			AddUV( new Vector2(uv1, uv1) );
			AddUV( new Vector2(uv1, uv0) );

			AddTriangle(tris + 0);
			AddTriangle(tris + 1);
			AddTriangle(tris + 2);

			AddTriangle(tris + 2);
			AddTriangle(tris + 3);
			AddTriangle(tris + 0);

			tris += 4;
		}

		public void CreateLine(Pair2 pair, Vector3 transformScale, float lineWidth, float z = 0f) {
			float xuv0 = 0; 
			float xuv1 = 1f - xuv0;
			float yuv0 = 0;
			float yuv1 = 1f - yuv0;

			float size = lineWidth / 6;
			float rot =  pair.a.Atan2(pair.b);

			Vector2 A1 = pair.a;
			Vector2 A2 = pair.a;
			Vector2 B1 = pair.b;
			Vector2 B2 = pair.b;

			Vector2 scale = new Vector2(1f / transformScale.x, 1f / transformScale.y);

			A1 = A1.Push (rot + pi2, size, scale);
			A2 = A2.Push (rot - pi2, size, scale);
			B1 = B1.Push (rot + pi2, size, scale);
			B2 = B2.Push (rot - pi2, size, scale);

			AddVertice(	new Vector3(B1.x, B1.y, z)		);
			AddVertice(	new Vector3(A1.x, A1.y, z)		);
			AddVertice(	new Vector3(A2.x, A2.y, z)		);
			AddVertice(	new Vector3(B2.x, B2.y, z)		);
		
			AddUV(		new Vector2(xuv1 / 3, yuv1)		); 
			AddUV( 		new Vector2(1 - xuv1 / 3, yuv1)	);
			AddUV( 		new Vector2(1 - xuv1 / 3, yuv0)	);
			AddUV(		new Vector2(yuv1 / 3, xuv0)		);

			Vector2 A3 = A1;
			Vector2 A4 = A1;

			A3 = A1;
			A4 = A2;

			A1 = A1.Push (rot, size, scale);
			A2 = A2.Push (rot, size, scale);

			AddVertice(	new Vector3(A3.x, A3.y, z)		);
			AddVertice(	new Vector3(A1.x, A1.y, z)		);
			AddVertice(	new Vector3(A2.x, A2.y, z)		);
			AddVertice(	new Vector3(A4.x, A4.y, z)		);

			AddUV( 	new Vector2(xuv1 / 3, yuv1)			); 
			AddUV(	new Vector2(xuv0, yuv1)				);
			AddUV( 	new Vector2(xuv0, yuv0)				);
			AddUV( 	new Vector2(yuv1 / 3, xuv0)			);


			A1 = B1;
			A2 = B2;

			B1 = B1.Push (rot - Mathf.PI, size, scale);
			B2 = B2.Push (rot - Mathf.PI, size, scale);
			
			AddVertice(	 new Vector3(B1.x, B1.y, z)	);
			AddVertice(	 new Vector3(A1.x, A1.y, z)	);
			AddVertice(	 new Vector3(A2.x, A2.y, z)	);
			AddVertice(	 new Vector3(B2.x, B2.y, z)	);

			AddUV(		new Vector2(xuv0, yuv1)			); 
			AddUV(		new Vector2(xuv1 / 3, yuv1)		);
			AddUV(		new Vector2(xuv1 / 3, yuv0)		);
			AddUV( 		new Vector2(yuv0, xuv0)			);

			AddTriangle(tris + 0);
			AddTriangle(tris + 1);
			AddTriangle(tris + 2);

			AddTriangle(tris + 2);
			AddTriangle(tris + 3);
			AddTriangle(tris + 0);


			AddTriangle(tris + 4);
			AddTriangle(tris + 5);
			AddTriangle(tris + 6);

			AddTriangle(tris + 6);
			AddTriangle(tris + 7);
			AddTriangle(tris + 4);


			
			AddTriangle(tris + 8);
			AddTriangle(tris + 9);
			AddTriangle(tris + 10);

			AddTriangle(tris + 10);
			AddTriangle(tris + 11);
			AddTriangle(tris + 8);

			tris += 12;
		}

		
		public void Draw(Transform transform, Material material, int id = 0) {
			Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

			Graphics.DrawMesh(GetMesh(id), matrix, material, 0);
		}

		public void Draw(Material material, int id = 0) {
			Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, 0),  new Vector3(1, 1, 1));

			Graphics.DrawMesh(GetMesh(id), matrix, material, 0);
		}

	}
}