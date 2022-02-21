using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {
	public class Slicer2DLinearController : MonoBehaviour {
		// Controller Visuals
		public Visuals visuals = new Visuals();

		// Physics Force
		public bool addForce = true;
		public float addForceAmount = 5f;

		// Mouse Events
		Pair2[] linearPair = new Pair2[10];

		// Input
		public InputController input = new InputController();

		public void Start() {
			visuals.Initialize(gameObject);

			visuals.SetGameObject(gameObject);

			for(int id = 0; id < 10; id++) {
				linearPair[id] = Pair2.zero;
			}
		}

		public void Update() {
			input.Update();

			if (visuals.drawSlicer == false) {
				return;
			}

			visuals.Clear();

			for(int id = 0; id < 10; id++) {
				if (linearPair[id].a == Vector2.zero && linearPair[id].b == Vector2.zero) {
					continue;
				}

				if (input.GetVisualsEnabled(id) == false) {
					continue;
				}

				visuals.GenerateLinearMesh(linearPair[id]);
			}
			
			visuals.Draw();
		}

		public void OnGUI() {
			input.OnGUI();
		}

		// Checking mouse press and release events to get linear slices based on input
		public void LateUpdate()  {
			for(int id = 0; id < 10; id++) {
				Vector2 pos = input.GetInputPosition(id);

				if (input.GetInputClicked(id)) {
					linearPair[id].a = pos;
				}
				
				if (input.GetInputPressed(id)) {
					linearPair[id].b = pos;
				}

				if (input.GetInputReleased(id)) {
					if (input.GetSlicingEnabled(id)) {
						LinearSlice (linearPair[id].ToPair2D());
					}
				}

				if (input.GetInputPressed(id) == false) {
					linearPair[id].a = pos;
					linearPair[id].b = pos;
				}
			}
		}

		private void LinearSlice(Pair2D slice) {
			List<Slice2D> results = Slicing.LinearSliceAll (slice, null);

			if (addForce == false) {
				return;
			}

			// Adding Physics Forces
			foreach (Slice2D id in results) {
				AddForce.LinearSlice(id, addForceAmount);
			}
		}
	}
}