using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities2D.Extensions;
using Utilities2D;

namespace Slicer2D.Controller.Merge2D {
	
	[System.Serializable]
	public class ComplexController : Controller.Base {
		// Algorhitmic
		public Vector2List[] pointsList = new Vector2List[10];
		bool startedSlice = false;

		// Settings
		public Sliceable2D.SliceType complexSliceType = Sliceable2D.SliceType.SliceHole;

		// Autocomplete
		public bool autocomplete = false;
		public bool autocompleteDisplay = false;
		public float autocompleteDistance = 1;

		public float minVertexDistance = 1f;
		public bool endSliceIfPossible = false;
		public bool startSliceIfPossible = false;

		public void Initialize() {
			for(int id = 0; id < 10; id++) {
				pointsList[id] = new Vector2List(true);
			}
		}

		public Vector2List GetList(int id) {
			Vector2List list = pointsList[id].Copy();

			if (list.Count() > 0) {
				Vector2 pos = input.GetInputPosition(id);
				if (Vector2.Distance (list.Last(), pos) > 0.01f) {
					list.Add(pos);
				}
			}
				
			return(list);
		}

		public Vector2List GetPoints(int id) {
			if (autocomplete) {
				return(AutoComplete.GetPoints(GetList(id), autocompleteDistance));
			}
			return(GetList(id));
		}

		public void Update() {
			for(int id = 0; id < 10; id++) {
				Vector2 pos = input.GetInputPosition(id);
					
				if (input.GetInputClicked(id)) {
					pointsList[id].Clear ();
					pointsList[id].Add (pos);
					startedSlice = false;
				}

				if (pointsList[id].Count() < 1) {
					return;
				}
				
				if (input.GetInputHolding(id)) {
					Vector2 posMove = pointsList[id].Last ();
					bool added = false;
					int loopCount = 0;

					while ((Vector2.Distance (posMove, pos) > minVertexDistance * visuals.visualScale)) {
						float direction = (float)Vector2D.Atan2 (pos, posMove);
						posMove = posMove.Push (direction, minVertexDistance * visuals.visualScale);

						if (startSliceIfPossible == true && startedSlice == false) {
							if (Sliceable2D.PointInSlicerComponent(posMove.ToVector2D()) != null) {
								while (pointsList[id].Count() > 2) {
									pointsList[id].RemoveAt(0);
								}

								startedSlice = true;
							}
						}

						pointsList[id].Add (posMove);

						added = true;

						loopCount ++;
						if (loopCount > 150) {
							break;
						}
					}

					if (endSliceIfPossible == true && added) {
						if (ComplexMerge (GetPoints(id).ToVector2DList()) == true) {
							pointsList[id].Clear ();

							if (startSliceIfPossible) {
								pointsList[id].Add (pos);
								startedSlice = false;
							}
						}
					}
				}

				if (input.GetInputReleased(id)) {
					startedSlice = false;
					Sliceable2D.complexSliceType = complexSliceType;
					ComplexMerge (GetPoints(id).ToVector2DList());
					pointsList[id].Clear ();
				}
			}
		}

		public void Draw(Transform transform) {
			visuals.Clear();

			for(int id = 0; id < 10; id++) {
				if (input.GetInputHolding(id) ) {
					if (pointsList[id].Count() > 0) {
						if (startSliceIfPossible == false || startedSlice == true) {
							Vector2List points = GetList(id);

							if (autocompleteDisplay) {
								points = GetPoints(id);
							}
							
							visuals.GenerateComplexMesh(points);
						}
					}
				}
			}

			visuals.Draw();
		}

		bool ComplexMerge(List <Vector2D> slice) {
			List<Merge.Merge2D> results = Merge.Merging.ComplexMergeAll (slice, sliceLayer);
			bool result = false;

			foreach (Merge.Merge2D id in results) {
				if (id.polygons.Count > 0) {
					result = true;
				}

				///eventHandler.Perform(id);
			}

			return(result);
		}
	}
}