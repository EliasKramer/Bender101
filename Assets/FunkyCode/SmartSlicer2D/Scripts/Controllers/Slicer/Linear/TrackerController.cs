using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;
using Utilities2D.Extensions;

namespace Slicer2D.Controller.Linear {
	
	[System.Serializable]
	public class TrackerController : Slicer2D.Controller.Base {
		// Algorhitmic
		List<Vector2D> pointsList = new List<Vector2D>();

		public static Slicer2D.Linear.SlicerTracker linearTracker = new Slicer2D.Linear.SlicerTracker();

		// Settings
		float minVertexDistance = 1f;

		public void Update(Vector2 pos) {
			if (input.GetInputClicked()) {
				pointsList.Clear();
				linearTracker.trackerList.Clear ();
				pointsList.Add(pos.ToVector2D());
			}
							
			if (input.GetInputPressed() && pointsList.Count > 0) {
				linearTracker.Update(pos, minVertexDistance);
			}

			if (input.GetInputReleased()) {
				pointsList.Clear();

				linearTracker.trackerList.Clear();
			}
		}

		public void Draw(Transform transform) {
			if (pointsList.Count > 0) {
				visuals.Clear();
				visuals.GenerateLinearTrackerMesh(input.GetInputPosition(), linearTracker.trackerList);
				visuals.Draw();
			} else {
				visuals.Clear();
			}
		}
	}
}