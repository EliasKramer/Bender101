using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities2D.Extensions;
using Utilities2D;

namespace Slicer2D {
	public class Slicer2DComplexController : MonoBehaviour {
		// Controller Visuals
		public Visuals visuals = new Visuals();

		// Physics Force
		public bool addForce = true;
		public float addForceAmount = 5f;

		// Mouse Events
		private static Vector2List[] points = new Vector2List[10];
		private float minVertexDistance = 1f;

		// Input
		public InputController input = new InputController();

		// Complex Slice Type
		public Sliceable2D.SliceType complexSliceType = Sliceable2D.SliceType.SliceHole;

		public void Start() {
			visuals.Initialize(gameObject);

			for(int id = 0; id < 10; id++) {
				points[id] = new Vector2List(true);
			}
		}

		public void Update() {
			input.Update();

			if (visuals.drawSlicer == false) {
				return;
			}

			visuals.Clear();

			for(int id = 0; id < 10; id++) {
				if (points[id].Count() < 1) {
					continue;
				}

				if (input.GetVisualsEnabled(id) == false) {
					continue;
				}

				visuals.GenerateComplexMesh(points[id]);
				
			}

			visuals.Draw();
		}

		// Checking mouse press and release events to get linear slices based on input
		public void LateUpdate() {
			for(int id = 0; id < 10; id++) {
				Vector2 pos = input.GetInputPosition(id);

				if (input.GetInputClicked(id)) {
					points[id].Clear ();
					points[id].Add (pos);
				}

				if (input.GetInputPressed(id)) {
					Vector2 posMove = points[id].Last ();
					while ((Vector2.Distance (posMove, pos) > minVertexDistance)) {
						float direction = (float)Vector2D.Atan2 (pos, posMove);
						posMove = posMove.Push (direction, minVertexDistance);
						points[id].Add (posMove);
					}
				}

				if (input.GetInputReleased(id)) {
					Sliceable2D.complexSliceType = complexSliceType;

					if (input.GetSlicingEnabled(id)) {
						ComplexSlice (points[id].ToVector2DList());
					}

					points[id].Clear();
				}

				if (input.GetInputPressed(id) == false) {
					if (points[id].Count() > 0) {
						points[id].Clear();
					}
				}
			}
		}

		private void ComplexSlice(List <Vector2D> slice) {
			List<Slice2D> results = Slicing.ComplexSliceAll (slice, null);

			if (addForce == false) {
				return;
			}

			foreach (Slice2D id in results) {
				AddForce.ComplexSlice(id, addForceAmount);
			}
		}
	}
}