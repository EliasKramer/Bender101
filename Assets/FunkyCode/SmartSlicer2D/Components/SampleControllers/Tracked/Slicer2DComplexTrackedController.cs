using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {
	
	public class Slicer2DComplexTrackedController : MonoBehaviour {
		public Complex.SlicerTracker trackerObject = new Complex.SlicerTracker();
		public Color color = Color.black;
		public float lineWidth = 0.25f;
		public float zPosition = 0;
		public float verticesDistance = 0.25f;

		private Mesh mesh;
		static private SmartMaterial material;

		public Visuals visuals = new Visuals();

		public Material GetStaticMaterial() {
			if (material == null) {
				material = MaterialManager.GetVertexLitCopy();
				material.SetColor(Color.black);
			}
			return(material.material);
		}

		void Awake() {
			visuals.SetGameObject(gameObject);

			visuals.lineEndSize = 0;
		}

		void Update () {
			trackerObject.Update(transform.position, verticesDistance);

			visuals.lineWidth = lineWidth;
			visuals.zPosition = zPosition;
			visuals.slicerColor = color;

			visuals.Clear();

			visuals.GenerateComplexTrackerMesh(transform.position, trackerObject.trackerList); //, transform, lineWidth, transform.position.z + 0.001f
		
			visuals.Draw();
		}

		void OnEnable() {
			trackerObject = new Complex.SlicerTracker();
		}
	}
}