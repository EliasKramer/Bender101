using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities2D.Extensions;
using Utilities2D;

namespace Slicer2D.Controller.Complex {

	[System.Serializable]
	public class CutController : Slicer2D.Controller.Base {
		// Algorhitmic
		List<Vector2D> pointsList = new List<Vector2D>();

		// Settings
		public float cutSize = 0.5f;
		public float minVertexDistance = 1f;

		public List<Vector2D> GetList() {
			List<Vector2D> list = new List<Vector2D>(pointsList);
			
			if (list.Count > 0) {
				Vector2D pos = new Vector2D(input.GetInputPosition());
				if (Vector2D.Distance (list.Last(), pos) > 0.01f) {
					list.Add(pos);
				}
			}

			return(list);
		}


		public void Update(Vector2 pos) {

			float scroll = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
			float newCutSize = cutSize + scroll;
			if (newCutSize > 0.05f) {
				cutSize = newCutSize;
			}

			if (input.GetInputClicked()) {
				pointsList.Clear ();
				pointsList.Add (new Vector2D(pos));
			}

			if (pointsList.Count < 1) {
				return;
			}
			
			if (input.GetInputHolding()) {
				Vector2 posMove = pointsList.Last ().ToVector2();
				int loopCount = 0;
				while ((Vector2.Distance (posMove, pos) > minVertexDistance * visuals.visualScale)) {
					float direction = (float)Vector2D.Atan2 (pos, posMove);
					posMove = posMove.Push (direction, minVertexDistance * visuals.visualScale);

					pointsList.Add (new Vector2D(posMove));

					loopCount ++;
					if (loopCount > 150) {
						break;
					}
				}
			}

			if (input.GetInputReleased()) {
				ComplexCut complexCutLine = ComplexCut.Create(GetList(), cutSize * visuals.visualScale);
				
				Slicing.ComplexCutSliceAll (complexCutLine, sliceLayer);

				pointsList.Clear ();
			}
		}

		public void Draw(Transform transform) {
			if (input.GetInputHolding()) {
				visuals.Clear();
				visuals.GenerateComplexCutMesh(GetList(), cutSize);
				visuals.Draw();
			} else {
				visuals.Clear();
			}
		}
	}
}