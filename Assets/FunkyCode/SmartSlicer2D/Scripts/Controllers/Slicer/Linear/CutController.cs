using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D.Controller.Linear {

	[System.Serializable]
	public class CutController : Slicer2D.Controller.Base {
		// Algorhitmic
		Pair2 linearPair = Pair2.zero;

		// Settings
		public float cutSize = 0.5f;

		public void Update(Vector2 pos) {

			float scroll = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
			float newCutSize = cutSize + scroll;
			if (newCutSize > 0.05f) {
				cutSize = newCutSize;
			}

			if (input.GetInputClicked()) {
				linearPair.a = pos;
			}

			if (input.GetInputHolding()) {
				linearPair.b = pos;
			}

			if (input.GetInputReleased()) {
				LinearCut linearCutLine = LinearCut.Create(linearPair, cutSize * visuals.visualScale);
				Slicing.LinearCutSliceAll (linearCutLine, sliceLayer);
			}
		}

		public void Draw(Transform transform) {
			if (input.GetInputHolding()) {
				visuals.Clear();
				visuals.GenerateLinearCutMesh(linearPair, cutSize);
				visuals.Draw();
			} else {
				visuals.Clear();
			}
		}
	}
}