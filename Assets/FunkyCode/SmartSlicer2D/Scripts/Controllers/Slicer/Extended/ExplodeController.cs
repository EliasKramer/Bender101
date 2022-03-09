using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;
using Utilities2D.Extensions;

namespace Slicer2D.Controller.Extended {
	
	[System.Serializable]
	public class ExplodeController : Controller.Base {
		// Settings
		public bool addForce = true;
		public float addForceAmount = 5f;

		public bool explodeFromPoint = false;

		public void Update(Vector2 pos) {
			if (input.GetInputClicked()) {
				if (explodeFromPoint) {
					ExplodeFromPoint(pos.ToVector2D());
				} else {
					Explode(pos.ToVector2D());
				}
			}
		}

		void ExplodeFromPoint(Vector2D pos) {
			List<Slice2D> results =	Slicing.ExplodeByPointAll (pos, sliceLayer);

			foreach (Slice2D id in results) {
				eventHandler.Perform(id);
			}

			if (addForce == true) {
				foreach (Slice2D id in results) {
					AddForce.ExplodeByPoint(id, addForceAmount, pos);
				}
			}
		}

		void Explode(Vector2D pos) {
			List<Slice2D> results =	Slicing.ExplodeInPointAll (pos, sliceLayer);

			foreach (Slice2D id in results) {
				eventHandler.Perform(id);
			}

			if (addForce == true) {
				foreach (Slice2D id in results) {
					AddForce.ExplodeInPoint(id, addForceAmount, pos);
				}
			}
		}

		public void Draw(Transform transform) {
			Vector2 pos = input.GetInputPosition();
			
			visuals.Clear();
			visuals.GeneratePointMesh(new Pair2(pos, pos));
			visuals.Draw();
		}
	}
}