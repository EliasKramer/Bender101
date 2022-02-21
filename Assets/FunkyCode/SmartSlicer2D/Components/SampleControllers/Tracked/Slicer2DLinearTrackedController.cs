using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {

	public class Slicer2DLinearTrackedController : MonoBehaviour {
		public Linear.SlicerTracker trackerObject = new Linear.SlicerTracker();
		public float lineWidth = 0.25f;

		private Mesh mesh;
		static private SmartMaterial material;

		public Material GetStaticMaterial() {
			if (material == null) {
				material = MaterialManager.GetVertexLitCopy();
				material.SetColor(Color.black);
			}
			return(material.material);
		}

		void Update () {
			trackerObject.Update(transform.position);

			//mesh = Linear.GenerateTrackerMesh(trackerObject.trackerList, transform, lineWidth, transform.position.z + 0.001f);

			//Max2D.SetColor (Color.black);
			//Max2DMesh.Draw(mesh, GetStaticMaterial());
		}

		void OnEnable() {
			trackerObject = new Linear.SlicerTracker();
		}
	}
}