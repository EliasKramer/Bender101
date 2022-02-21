using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities2D;
using Utilities2D.Extensions;

namespace Slicer2D.Controller.Complex {

	[System.Serializable]
	public class TrackerController : Slicer2D.Controller.Base {
		// Algorhitmic
		List<Vector2D> pointsList = new List<Vector2D>();
		Slicer2D.Complex.SlicerTracker complexTracker = new Slicer2D.Complex.SlicerTracker();

		// Settings
		public Sliceable2D.SliceType complexSliceType = Sliceable2D.SliceType.SliceHole;
		public float minVertexDistance = 1f;

		public void Update(Vector2 pos) {
			if (input.GetInputClicked()) {
				pointsList.Clear ();
				complexTracker.trackerList.Clear ();
				pointsList.Add (new Vector2D(pos));
			}
							
			if (input.GetInputPressed() && pointsList.Count > 0) {
				Vector2 posMove = pointsList.Last ().ToVector2();

				int loopCount = 0;
				while ((Vector2.Distance (posMove, pos) > minVertexDistance)) {
					float direction = (float)Vector2D.Atan2 (pos, posMove);
					posMove = posMove.Push (direction, minVertexDistance);

					Sliceable2D.complexSliceType = complexSliceType;
					
					pointsList.Add (posMove.ToVector2D());
					complexTracker.Update(posMove, 0);
				
					loopCount ++;
					if (loopCount > 150) {
						break;
					}
				}

				complexTracker.Update(posMove, minVertexDistance);
			}

			if (input.GetInputReleased()) {
				pointsList.Clear();

				complexTracker.trackerList.Clear();
			}
		}

		public void Draw(Transform transform) {
			if (pointsList.Count > 0) {
				visuals.Clear();
				visuals.GenerateComplexTrackerMesh(input.GetInputPosition(), complexTracker.trackerList);
				visuals.Draw();
			}
		}
	}
}