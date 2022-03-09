using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {
	
	public class ShapeDraw {
		
		private static SmartMaterial material = null;
		private static Mesh mesh = null;

		static public void Draw(Vector2 p, Transform transform) {
			Vector3 pos = new Vector3(p.x, p.y, transform.position.z);

			Matrix4x4 matrix = Matrix4x4.TRS(pos, transform.rotation, new Vector3(1, 1, 1));

			Graphics.DrawMesh(GetMesh(), matrix, GetMaterial().material, 0);
		}

		static public SmartMaterial GetMaterial() {
			if (material == null) {
				material = MaterialManager.GetAlphaCopy();
				material.SetTexture(Resources.Load<Texture>("Pattern/1"));
			}
			return(material);
		}

		static public Mesh GetMesh() {
			if (mesh == null) {
				//Mesh2DMesh triangles = new Mesh2DMesh();
				//triangles.Add(Max2DMesh.Legacy.CreateBox(0.125f));
				//mesh = Max2DMesh.Export(triangles); 
			}
			return(mesh);
		}

		public static void DrawMatch (ShapeObject shapeA, ShapeObject shapeB) {
			Vector2 pointInWorld;
			Vector2D pointInWorld2D = Vector2D.Zero();
			Polygon2D polyInWorld;

			foreach(Vector2D point in shapeA.pointsIn) {
				pointInWorld = shapeA.transform.TransformPoint(point.ToVector2());

				pointInWorld2D.x = pointInWorld.x;
				pointInWorld2D.y = pointInWorld.y;
				
				polyInWorld = shapeB.GetWorldPolygon();

				if (polyInWorld.PointInPoly(pointInWorld2D)) {
					ShapeDraw.Draw(pointInWorld, shapeA.transform);
				}
			}

			foreach(Vector2D point in shapeB.pointsIn) {
				pointInWorld = shapeB.transform.TransformPoint(point.ToVector2());

				pointInWorld2D.x = pointInWorld.x;
				pointInWorld2D.y = pointInWorld.y;
				
				polyInWorld = shapeA.GetWorldPolygon();

				if (polyInWorld.PointInPoly(pointInWorld2D)) {
					ShapeDraw.Draw(pointInWorld, shapeB.transform);
				}
			}
		}

		static public void DrawFill(ShapeFill shape) {
			Vector2 pointInWorld;
			Vector2D pointInWorld2D = Vector2D.Zero();

			foreach(Vector2D point in shape.pointsIn) {
				pointInWorld = shape.transform.TransformPoint(point.ToVector2());

				pointInWorld2D.x = pointInWorld.x;
				pointInWorld2D.y = pointInWorld.y;

				if (ShapeObject.PointInShapes(pointInWorld2D) == false) {
					ShapeDraw.Draw(pointInWorld, shape.transform);
				} 			
			}
		}
	}
}