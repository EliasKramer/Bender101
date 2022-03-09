using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities2D.Extensions;
using Utilities2D;

namespace Slicer2D {
	public enum LineEndingType {Square, Circle}

	[System.Serializable]
	public class Visuals {

		public LineEndingType lineEndingType = LineEndingType.Square;
		public int lineEndingEdgeCount = 6;

		public bool drawSlicer = true;
		public float visualScale = 1f;
		public float lineWidth = 1.0f;
		public float lineEndWidth = 1.0f;
		public float zPosition = 0f;
		public Color slicerColor = Color.white;
		public bool lineBorder = true;
		public float lineEndSize = 0.5f;
		public float vertexSpace = 0.25f;
		public float borderScale = 2f;
		public float minVertexDistance = 1f;

		public bool customMaterial = false;
		public Material customFillMaterial;
		public Material customBoarderMaterial;

		public SortingLayer sortingLayer = new SortingLayer();

		//public bool lineEndRotation = false;

		public bool customEndingsImage = false;
		public Material customEndingImageMaterial = null;
		public List<Pair2> customEndingsPosition = new List<Pair2>();

		// Mesh & Material
		private List<Mesh> mesh = new List<Mesh>();
		private List<Mesh> meshBorder = new List<Mesh>();

		private SmartMaterial fillMaterial;
		private SmartMaterial boarderMaterial;

		public List<RendererObject> rendererObjects = new List<RendererObject>();
		private GameObject gameObject;
		private Transform transform;

		public VisualMeshExtended visualMesh = new VisualMeshExtended();
		public VisualMeshExtended visualMeshBorder = new VisualMeshExtended();


		public RendererObject GetFreeRenderObject() {
			foreach(RendererObject renderObject in rendererObjects) {
				if (renderObject.drawn == false) {
					return(renderObject);
				}
			}
			
			RendererObject newRenderGameObject = new RendererObject(gameObject);
			
			newRenderGameObject.meshRenderer.sortingOrder = sortingLayer.Order;
			newRenderGameObject.meshRenderer.sortingLayerName = sortingLayer.Name;
			

			rendererObjects.Add(newRenderGameObject);

			return(newRenderGameObject);
		}

		public void SetGameObject(GameObject setGameObject) {
			gameObject = setGameObject;
			transform = gameObject.transform;
		}

		// Visuals
		//GameObject visualsGameObject;
		//MeshFilter visualMeshFilter;
		//MeshRenderer visualMeshRenderer;

		public void Draw() {
			if (lineBorder && meshBorder.Count > 0) {
				if (meshBorder.Count > 0) {
					foreach(Mesh m in meshBorder) {
						RendererObject renderObject = GetFreeRenderObject();
						renderObject.drawn = true;
						renderObject.meshRenderer.sharedMaterial = GetBorderMaterial();
						renderObject.meshFilter.sharedMesh = m;
						renderObject.transform.position = Vector3.zero;

						Vector3 lScale = renderObject.transform.parent.lossyScale;
						lScale.x = 1f / lScale.x;
						lScale.y = 1f / lScale.y;
						lScale.z = 1f / lScale.z;
						renderObject.transform.localScale = lScale;
					}
				}
			}

			if (mesh.Count > 0) {
				foreach(Mesh m in mesh) {
					RendererObject renderObject = GetFreeRenderObject();
					renderObject.drawn = true;
					renderObject.meshRenderer.sharedMaterial = GetFillMaterial();
					renderObject.meshFilter.sharedMesh = m;
					renderObject.transform.position = Vector3.zero;

					Vector3 lScale = renderObject.transform.parent.lossyScale;
					lScale.x = 1f / lScale.x;
					lScale.y = 1f / lScale.y;
					lScale.z = 1f / lScale.z;
					renderObject.transform.localScale = lScale;
				}
			}

			// Does not use Mesh Renderer Object
			if (customEndingsPosition.Count > 0) {
				Matrix4x4 matrix;		
				foreach(Pair2 pair in customEndingsPosition) {
					Polygon2D polyA = Polygon2D.CreateFromRect(new Vector2(1, 1));
					//polyA.ToOffset(pair.A);
					Mesh mA = polyA.CreateMesh(new Vector2(2, 2), Vector2.zero);
					
					matrix = Matrix4x4.TRS( pair.a.ToVector3(zPosition), Quaternion.Euler(0, 0, 0),  new Vector3(1, 1, 1));

					Graphics.DrawMesh(mA, matrix, customEndingImageMaterial, 0);

					matrix = Matrix4x4.TRS( pair.b.ToVector3(zPosition), Quaternion.Euler(0, 0, 0),  new Vector3(1, 1, 1));

					Graphics.DrawMesh(mA, matrix, customEndingImageMaterial, 0);
				}	
			}
		}

		public void Clear() {
			foreach(RendererObject renderObject in rendererObjects) {
				renderObject.drawn = false;

				renderObject.meshFilter.sharedMesh = null;
			}

			customEndingsPosition.Clear();

			if (meshBorder.Count > 0) {
				meshBorder.Clear();
			}

			if (mesh.Count > 0) {
				mesh.Clear();
			}
		}

		public void GeneratePointMesh(Pair2 pair) {
			visualMeshBorder.GeneratePoint(pair, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f);
			meshBorder.Add( visualMeshBorder.Export(0) );
			
			visualMesh.GeneratePoint(pair, transform, lineWidth * visualScale, zPosition);
			mesh.Add( visualMesh.Export(0) );
		}

		public void GenerateLinearMesh(Pair2 linearPair) {
			if (customEndingsImage) {
				customEndingsPosition.Add(linearPair);
			}

			visualMeshBorder.Linear_GenerateMesh(linearPair, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f, lineEndSize * visualScale, lineEndWidth * visualScale * borderScale, lineEndingType, lineEndingEdgeCount);
			meshBorder.Add( visualMeshBorder.Export(0) );
		
			visualMesh.Linear_GenerateMesh(linearPair, transform, lineWidth * visualScale, zPosition, lineEndSize * visualScale, lineEndWidth * visualScale, lineEndingType, lineEndingEdgeCount);
			mesh.Add( visualMesh.Export(0) );
		}

		public void GenerateComplexMesh(Vector2List points) {
			visualMeshBorder.Complex_GenerateMesh(points, transform, lineWidth * visualScale * borderScale, minVertexDistance, zPosition + 0.001f, lineEndSize * visualScale,  lineEndWidth * visualScale * borderScale, vertexSpace, lineEndingType, lineEndingEdgeCount);
			meshBorder.Add( visualMeshBorder.Export(0) );
		
			visualMesh.Complex_GenerateMesh(points, transform, lineWidth * visualScale, minVertexDistance, zPosition, lineEndSize * visualScale, lineEndWidth * visualScale, vertexSpace, lineEndingType, lineEndingEdgeCount);
			mesh.Add( visualMesh.Export(0) );
		}

		public void GenerateLinearCutMesh(Pair2 linearPair, float cutSize) {
			visualMeshBorder.Linear_GenerateCutMesh (linearPair, cutSize * visualScale, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f);
			meshBorder.Add( visualMeshBorder.Export(0) );
		
			visualMesh.Linear_GenerateCutMesh(linearPair, cutSize * visualScale, transform, lineWidth * visualScale, zPosition);
			mesh.Add( visualMesh.Export(0) );
		}

		public void GenerateLinearTrackerMesh(Vector2 pos, Dictionary<Sliceable2D, Tracker.Object> trackerList) {
			visualMeshBorder.Linear_GenerateTrackerMesh(pos, trackerList, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f, lineEndSize * visualScale, lineEndingType, lineEndingEdgeCount);		
			meshBorder.Add( visualMeshBorder.Export(0) );
		
			visualMesh.Linear_GenerateTrackerMesh(pos, trackerList, transform, lineWidth * visualScale, zPosition, lineEndSize * visualScale, lineEndingType, lineEndingEdgeCount);
			mesh.Add( visualMesh.Export(0) );
		}

		public void GenerateComplexCutMesh(List<Vector2D> pointsList, float cutSize) {
			visualMeshBorder.Complex_GenerateCutMesh(pointsList, cutSize * visualScale, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f);		
			meshBorder.Add( visualMeshBorder.Export(0) );
		
			visualMesh.Complex_GenerateCutMesh(pointsList, cutSize * visualScale, transform, lineWidth * visualScale, zPosition);
			mesh.Add( visualMesh.Export(0) );
		}
		
		public void GenerateCreateMesh(Vector2 pos, Polygon2D.PolygonType polygonType, float polygonSize, Controller.Extended.CreateController.CreateType createType, List<Vector2D> pointsList, Pair2D linearPair) {
			visualMeshBorder.GenerateCreateMesh(pos, polygonType, polygonSize, createType, pointsList, linearPair, minVertexDistance, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f, lineEndSize, lineEndingType, lineEndingEdgeCount);		
			meshBorder.Add( visualMeshBorder.Export(0) );
		
			visualMesh.GenerateCreateMesh(pos, polygonType, polygonSize, createType, pointsList, linearPair, minVertexDistance, transform, lineWidth * visualScale, zPosition, lineEndSize, lineEndingType, lineEndingEdgeCount);
			mesh.Add( visualMesh.Export(0) );
		}

		public void GenerateTrailMesh(Dictionary<Sliceable2D, Trail.Object> trailList) {
			visualMeshBorder.GenerateTrailMesh( trailList, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f, lineEndSize * visualScale);
			meshBorder.Add( visualMeshBorder.Export(0) );
		
			visualMesh.GenerateTrailMesh(trailList, transform, lineWidth * visualScale, zPosition, lineEndSize * visualScale);
			mesh.Add( visualMesh.Export(0) );
		}

		public void GeneratePolygonMesh(Vector2 pos, Polygon2D.PolygonType polygonType, float polygonSize) {
			visualMeshBorder.GeneratePolygonMesh(pos, polygonType, polygonSize * visualScale, minVertexDistance, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f);
			meshBorder.Add( visualMeshBorder.Export(0) );
		
			visualMesh.GeneratePolygonMesh(pos, polygonType, polygonSize * visualScale, minVertexDistance, transform, lineWidth * visualScale, zPosition);
			mesh.Add( visualMesh.Export(0) );
		}

		public void GenerateComplexTrackerMesh(Vector2 pos, Dictionary<Sliceable2D, Tracker.Object> trackerList) {
			visualMeshBorder.Complex_GenerateTrackerMesh(pos, trackerList, transform, lineWidth * visualScale * borderScale, zPosition + 0.001f, lineEndSize * visualScale, lineEndingType, lineEndingEdgeCount);
			meshBorder.Add( visualMeshBorder.Export(0) );
		
			visualMesh.Complex_GenerateTrackerMesh(pos, trackerList, transform, lineWidth * visualScale, zPosition, lineEndSize * visualScale, lineEndingType, lineEndingEdgeCount);
			mesh.Add( visualMesh.Export(0) );
		}

		public void Initialize(GameObject gameObject) {
			//GameObject visualsGameObject = new GameObject("Visuals");
			//visualsGameObject.transform.parent = gameObject.transform;

			//visualMeshFilter = visualsGameObject.AddComponent<MeshFilter>();
			//visualMeshRenderer = visualsGameObject.AddComponent<MeshRenderer>();
		}

		public Material GetBorderMaterial() {
			if (customMaterial) {
				return(customBoarderMaterial);
			} else {
				if (boarderMaterial == null) {
					boarderMaterial = MaterialManager.GetVertexLitCopy();
				}

				boarderMaterial.SetColor(Color.black);

				return(boarderMaterial.material);
			}
		}

		public Material GetFillMaterial() {
			if (customMaterial) {
				return(customFillMaterial);
			} else {
				if (fillMaterial == null) {
					fillMaterial = MaterialManager.GetVertexLitCopy();
				}

				fillMaterial.SetColor(slicerColor);

				return(fillMaterial.material);
			}
		}

		public class RendererObject {
			public MeshRenderer meshRenderer = new MeshRenderer();
			public MeshFilter meshFilter = new MeshFilter();
			public Transform transform;
			public bool drawn = false;

			public RendererObject(GameObject gameObject) {
				GameObject newRenderGameObject = new GameObject("Slicer Visuals Render Object");
				newRenderGameObject.transform.parent = gameObject.transform;
				transform = newRenderGameObject.transform;

				meshRenderer = newRenderGameObject.AddComponent<MeshRenderer>();
				meshFilter = newRenderGameObject.AddComponent<MeshFilter>();
			}
		}
	}
}