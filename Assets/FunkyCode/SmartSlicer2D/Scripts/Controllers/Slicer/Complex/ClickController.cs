using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D.Extensions;
using Utilities2D;

namespace Slicer2D.Controller.Complex {

	[System.Serializable]
	public class ClickController : Slicer2D.Controller.Base {
		// Algorithm
		List<Vector2D> pointsList = new List<Vector2D>();

		// Settings
		public Sliceable2D.SliceType complexSliceType = Sliceable2D.SliceType.SliceHole;
		public int pointsLimit = 3;
		public bool sliceJoints = false;
		public bool endSliceIfPossible = false;

		public bool addForce = true;
		public float addForceAmount = 5f;

		public void Update(Vector2 pos) {
		
			if (endSliceIfPossible) {
				bool ended = false;
				if (input.GetInputClicked()) {
					pointsList.Add(pos.ToVector2D());

					Sliceable2D.complexSliceType = complexSliceType;

					ended = ComplexSlice (pointsList);;
				}

				if (ended) {
					pointsList.Clear();
				}

			} else {
				if (input.GetInputClicked()) {
					pointsList.Add(pos.ToVector2D());
				}

				if (pointsList.Count >= pointsLimit) {
					Sliceable2D.complexSliceType = complexSliceType;

					ComplexSlice (pointsList);

					pointsList.Clear ();
				}
			}
		}
		
		bool ComplexSlice(List <Vector2D> slice) {
			if (sliceJoints) {
				Slicer2D.Controller.Joints.ComplexSliceJoints(slice);
			}

			List<Slice2D> results = Slicing.ComplexSliceAll (slice, sliceLayer);
			bool result = false;

			foreach (Slice2D id in results) {
				if (id.GetGameObjects().Count > 0) {
					result = true;
				}

				eventHandler.Perform(id);
			}

			if (addForce == true) {
				foreach (Slice2D id in results)  {
					AddForce.ComplexSlice(id, addForceAmount);
				}
			}
			return(result);
		}

		public void Draw(Transform transform) {
			if (pointsList.Count > 0) {
				Vector2List points = new Vector2List(pointsList);
				Vector2 posA = input.GetInputPosition();
				points.Add(posA);
				
				visuals.Clear();
				visuals.GenerateComplexMesh(points);
				visuals.Draw();
			} else {
				visuals.Clear();
			}
		}
	}
}