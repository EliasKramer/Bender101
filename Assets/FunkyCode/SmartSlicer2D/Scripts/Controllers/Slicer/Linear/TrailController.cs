using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D.Controller.Linear {

	[System.Serializable]
	public class TrailController : Slicer2D.Controller.Base {
		public Slicer2D.Linear.SlicerTrail[] linearTrail = new Slicer2D.Linear.SlicerTrail[10];
		public Vector3[][] trailPositions = new Vector3[10][];

		public TrailRenderer[] trailRenderer = new TrailRenderer[10];
		public int trailRendererCount = 1;
		
		public bool addForce = true;
		public float addForceAmount = 5f;

		public void Initialize() {
			for(int id = 0; id < 10; id++) {
				trailPositions[id] = new Vector3[500];
				linearTrail[id] = new Slicer2D.Linear.SlicerTrail();
			}
		}

		public void Update() {
			for(int id = 0; id < trailRenderer.Length; id++) {

				if (trailRenderer[id] == null) {
					if (id == 0) {
						Debug.LogWarning("Slicer2D: Trail Renderer is not attached to the controller");
					}
					return;
				}

				SetTrailPosition(trailRenderer[id], id);

				int pointsCount = trailRenderer[id].GetPositions(trailPositions[id]);
				if (pointsCount < 1) {
					return;
				}

				Vector2D pos = new Vector2D(trailPositions[id][pointsCount - 1]);

				List<Slice2D> results = linearTrail[id].Update(pos, trailRenderer[id].time, sliceLayer);

				if (addForce) {
					foreach (Slice2D p in results)  {
						AddForce.LinearTrail(p, addForceAmount);
					}
				}
			}
		}

		public void Draw(Transform transform) {
			visuals.Clear();

			for(int id = 0; id < trailRenderer.Length; id++) {
				visuals.GenerateTrailMesh(linearTrail[id].trailList);
			}
			
			visuals.Draw();
		}

		public void SetTrailPosition(TrailRenderer trail, int id) {
			Vector3 pos = input.GetInputPosition(id); 
			pos.z = trail.transform.position.z;

			trail.transform.position = pos;
		}
	}
}