using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;
using Utilities2D.Extensions;

namespace Slicer2D.Controller.Linear {

	[System.Serializable]
	public class Controller : Slicer2D.Controller.Base {
		// Algorhitmic
		Pair2[] linearPair = new Pair2[10];
		public bool startedSlice = false;

		// Settings
		public bool endSliceIfPossible = false;
		public bool startSliceIfPossible = false;
		public bool strippedLinear = false;
		public float minVertexDistance = 1f;
		public bool displayCollisions = false;
		public bool sliceJoints = false;

		// Autocomplete
		public bool autocomplete = false;
		public bool autocompleteDisplay = false;
		public float autocompleteDistance = 1;

		public bool addForce = true;
		public float addForceAmount = 5f;

		public void Initialize() {
			for(int id = 0; id < 10; id++) {
				linearPair[id] = Pair2.zero;
			}
		}

		public Pair2 GetPair(int id) {
			if (autocomplete) {
				return(AutoComplete.GetPair(linearPair[id], autocompleteDistance));
			}
			return(linearPair[id]);
		}

		public void Update() {
			for(int id = 0; id < 10; id++) {
				Vector2 pos = input.GetInputPosition(id);

				if (input.GetInputClicked(id)) {
					linearPair[id] = new Pair2(pos, pos);
					startedSlice = false;
				}
				
				// Start Slice If Possible
				if (startSliceIfPossible) {
					if (startedSlice == true) { 
						if (Sliceable2D.PointInSlicerComponent(pos.ToVector2D()) == null) {
							startedSlice = false;
						}
					} else if (startedSlice == false) {
						if (Sliceable2D.PointInSlicerComponent(pos.ToVector2D()) != null) {
							startedSlice = true;
						} else {
							linearPair[id].a = pos;
						}
					}
				}

				// End Slice If Possible
				if (input.GetInputHolding(id)) {
					linearPair[id].b = pos;
				
					if (endSliceIfPossible) {
						if (input.GetSlicingEnabled(id)) {
							if (LinearSlice (GetPair(id).ToPair2D())) {
								linearPair[id].a = pos;

								if (startSliceIfPossible) {
									linearPair[id] = new Pair2(pos, pos);
									startedSlice = false;
								}
							}
						}
					}
				}

				if (input.GetInputReleased(id)) {
					if (input.GetSlicingEnabled(id)) {
						LinearSlice (GetPair(id).ToPair2D());
					}
				}
			}
		}
		
		public void Draw(Transform transform) {
			if (input.GetVisualsEnabled() == false) {
				return;
			}
			
			visuals.Clear();

			for(int id = 0; id < 10; id++) {

				if (input.GetInputHolding(id)) {

					if (startSliceIfPossible == false || startedSlice == true) {
						Pair2 pair = linearPair[id];

						if (autocompleteDisplay) {
							pair = GetPair(id);
						}
						
						// If Stripped Line
						if (strippedLinear) {
							Vector2List linearPoints = GetLinearVertices(pair, minVertexDistance * visuals.visualScale);

							if (linearPoints.Count() > 1) {
								visuals.GenerateComplexMesh(linearPoints);
							}
						
						} else {
							visuals.GenerateLinearMesh(pair);
						}

						if (displayCollisions) {
							List<Slice2D> results = Slicing.LinearSliceAll (linearPair[id].ToPair2D(), sliceLayer, false);
							foreach(Slice2D slice in results) {
								foreach(Vector2D collision in slice.GetCollisions()) {
									Pair2 p = new Pair2(collision.ToVector2(), collision.ToVector2());
									visuals.GenerateLinearMesh(p);
								}
							}
						}
					}
				}
			}

			visuals.Draw();

			return;
		}

		private bool LinearSlice(Pair2D slice) {
			if (sliceJoints) {
				Slicer2D.Controller.Joints.LinearSliceJoints(slice);
			}
			
			List<Slice2D> results = Slicing.LinearSliceAll (slice, sliceLayer);
			bool result = false;

			foreach (Slice2D id in results)  {
				if (id.GetGameObjects().Count > 0) {
					result = true;
				}

				eventHandler.Perform(id);
			}

			if (addForce == true) {
				foreach (Slice2D id in results)  {
					AddForce.LinearSlice(id, addForceAmount);
				}
			}

			return(result);
		}

		static public Vector2List GetLinearVertices(Pair2 pair, float minVertexDistance) {
			Vector2 startPoint = pair.a;
			Vector2 endPoint = pair.b;

			Vector2List linearPoints = new Vector2List(true);
			int loopCount = 0;
			while ((Vector2.Distance (startPoint, endPoint) > minVertexDistance)) {
				linearPoints.Add (startPoint);

				float direction = endPoint.Atan2(startPoint);
				startPoint = startPoint.Push (direction, minVertexDistance);

				loopCount ++;
				if (loopCount > 150) {
					break;
				}
			}

			linearPoints.Add (endPoint);

			return(linearPoints);
		}
	}
}