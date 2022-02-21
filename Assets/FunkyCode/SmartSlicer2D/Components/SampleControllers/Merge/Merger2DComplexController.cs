using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities2D.Extensions;
using Utilities2D;

namespace Slicer2D {
	
	public class Merger2DComplexController : MonoBehaviour {
		// Controller Visuals
		public Visuals visuals = new Visuals();

		// Mouse Events
		private static Vector2List points = new Vector2List(true);
		private float minVertexDistance = 1f;

		public List<List<Vector2D>> slices = new List<List<Vector2D>>();

		// Input
		public InputController input = new InputController();

		public void Start() {
			visuals.Initialize(gameObject);
			visuals.SetGameObject(gameObject);
		}

		public void Update() {
			input.Update();

			visuals.Clear();

			if (points.Count() > 0) {
				visuals.GenerateComplexMesh(points);
			}

			visuals.Draw();
		}

		public void LateUpdate() {
			Vector2 pos = input.GetInputPosition();

			if (input.GetInputClicked()) {
				points.Clear ();
				points.Add (pos);
			}

			if (input.GetInputPressed()) {
				Vector2 posMove = points.Last ();
				while ((Vector2.Distance (posMove, pos) > minVertexDistance)) {
					float direction = pos.Atan2 (posMove);
					posMove = posMove.Push (direction, minVertexDistance);
					points.Add (posMove);
				}
			}

			if (input.GetInputReleased()) {
				if (input.GetSlicingEnabled()) {
					ComplexMerge(points.ToVector2DList());
				}

				points.Clear();
			}

			if (input.GetInputPressed() == false) {
				if (points.Count() > 0) {
					points.Clear();
				}
			}
		}

		private void ComplexMerge(List <Vector2D> mergeSlice) {
			foreach(Sliceable2D slicer in Sliceable2D.GetList()) {
				slicer.ComplexMerge(mergeSlice);
			}
		}
	}
}