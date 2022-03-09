using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities2D;
using Utilities2D.Extensions;

namespace Slicer2D.Controller.Extended {
	
	[System.Serializable]
	public class CreateController : Controller.Base {
		public enum CreateType {Slice, PolygonType};

		// Algorithm
		List<Vector2D> pointsList = new List<Vector2D>();
		Pair2D linearPair = Pair2D.Zero();

		// Settings
		public CreateType createType = CreateType.Slice;
		public Polygon2D.PolygonType polygonType = Polygon2D.PolygonType.Circle;
		public float polygonSize = 1;
		public int edgeCount = 30;
		public Material material;
		float minVertexDistance = 1f;

		public void Update(Vector2 pos, Transform transform) {
			float newPolygonSize = polygonSize + UnityEngine.Input.GetAxis("Mouse ScrollWheel");
			if (newPolygonSize > 0.05f) {
				polygonSize = newPolygonSize;
			}

			if (input.GetInputClicked()) {
				pointsList.Clear ();
				pointsList.Add (pos.ToVector2D());
			}

			if (createType == CreateType.Slice) {
				if (input.GetInputHolding()) {
					if (pointsList.Count == 0 || (Vector2D.Distance (pos.ToVector2D(), pointsList.Last ()) > minVertexDistance * visuals.visualScale)) {
						pointsList.Add (pos.ToVector2D());
					}
				}

				if (input.GetInputReleased()) {
					CreatorSlice (pointsList, transform);
				}
			} else {
				if (input.GetInputClicked()) {
					PolygonCreator (pos.ToVector2D(), transform);
				}
			}
		}

		private void CreatorSlice(List <Vector2D> slice, Transform transform) {
			Polygon2D newPolygon = Slicer2D.API.CreatorSlice (slice);
			if (newPolygon != null) {
				CreatePolygon (newPolygon, transform);
			}
		}

		private void PolygonCreator(Vector2D pos, Transform transform) {
			Polygon2D.defaultCircleVerticesCount = edgeCount;
			Polygon2D newPolygon = Polygon2D.Create (polygonType, polygonSize).ToOffset (pos);
			CreatePolygon (newPolygon, transform);
		}

		private void CreatePolygon(Polygon2D newPolygon, Transform transform) {
			GameObject newGameObject = new GameObject ();
			newGameObject.transform.parent = transform;
			newGameObject.transform.position = new Vector3(0, 0, visuals.zPosition + 0.01f);

			newGameObject.AddComponent<Rigidbody2D> ();
			newGameObject.AddComponent<ColliderLineRenderer2D> ().color = Color.black;

			Sliceable2D smartSlicer = newGameObject.AddComponent<Sliceable2D> ();
			smartSlicer.textureType = Sliceable2D.TextureType.Mesh2D;
			smartSlicer.materialSettings.material = material;

			newPolygon.CreatePolygonCollider (newGameObject);
			newPolygon.CreateMesh (newGameObject, new Vector2 (1, 1), Vector2.zero, PolygonTriangulator2D.Triangulation.Advanced);
		}

		public void Draw(Transform transform) {
			if (input.GetInputHolding()) {
				visuals.Clear();
				visuals.GenerateCreateMesh(input.GetInputPosition(), polygonType, polygonSize, createType, pointsList, linearPair);
				visuals.Draw();
			} else {
				visuals.Clear();
			}
		}
	}
}