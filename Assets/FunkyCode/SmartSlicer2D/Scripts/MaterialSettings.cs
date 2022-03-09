using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {

	[System.Serializable]
	public class MaterialSettings {
		public PolygonTriangulator2D.Triangulation triangulation = PolygonTriangulator2D.Triangulation.Advanced;
		public Material material;
		public Material sideMaterial;
		public Vector2 scale = new Vector2(1, 1);
		public Vector2 offset = Vector2.zero;
		public float depth = 1;
		public bool batchMaterial = false;

		public MaterialSettings Copy() {
			MaterialSettings instance = new MaterialSettings();
			instance.offset = offset;
			instance.scale = scale;
			instance.material = material;
			instance.sideMaterial = sideMaterial;
			instance.triangulation = triangulation;
			instance.batchMaterial = batchMaterial;

			return(instance);
		}

		public void CreateMesh(GameObject gameObject, Polygon2D polygon) {
			polygon.CreateMesh (gameObject, scale, offset, GetTriangulation());

			if (material) {
				MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer> ();
				if (Settings.GetBatching(batchMaterial)) {
					Slicer2D.Profiler.IncBatchingApplied();
					meshRenderer.sharedMaterial = material;
				} else {
					meshRenderer.sharedMaterial = new Material(material);
				}
			}
		}

		public PolygonTriangulator2D.Triangulation GetTriangulation() {
			return(Settings.GetTriangulation(triangulation));
		}
	}
}