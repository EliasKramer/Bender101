using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities2D;

namespace Slicer2D.Complex {

	public class SlicerTracker {
		public Dictionary<Sliceable2D, Tracker.Object> trackerList = new Dictionary<Sliceable2D, Tracker.Object>();

		public void Update(Vector2 position, float minVertexDistance = 1f) {
			List<Sliceable2D> slicer2DList = Sliceable2D.GetListCopy();

			Vector2D trackedPos;

			foreach(Sliceable2D slicer in slicer2DList) {

				Tracker.Object tracker = null;
				trackerList.TryGetValue(slicer, out tracker);
				
				if (tracker == null) {
					tracker = new Tracker.Object();
					trackerList.Add(slicer, tracker);
				}

				trackedPos = new Vector2D(slicer.transform.transform.InverseTransformPoint(position));

				if (tracker.lastPosition != null) {
					if (slicer.shape.GetLocal().PointInPoly(trackedPos)) {
						if (tracker.tracking == false) {
							tracker.pointsList.Add(tracker.lastPosition);
						}

						tracker.tracking = true;

						if (tracker.pointsList.Count < 1 || (Vector2D.Distance (trackedPos, tracker.pointsList.Last ()) > minVertexDistance)) {
							tracker.pointsList.Add(trackedPos);
						}

					} else if (tracker.tracking == true) {
						tracker.tracking = false;

						/*

						Vector2D posA = new Vector2D(tracker.pointsList.Last());
						Vector2D posB = new Vector2D(trackedPos);

						float rot = (floatVector2D.Atan2(posB, posA);

						posB.Push(rot, 1);

						tracker.pointsList.Add(posB);
						*/

						tracker.pointsList.Add(trackedPos);


						

						// Test
						

						List<Vector2D> slicePoints = new List<Vector2D>();
						foreach(Vector2D point in tracker.pointsList) {
							slicePoints.Add(new Vector2D(slicer.transform.TransformPoint(point.ToVector2())));
						}

						Slice2D slice = slicer.ComplexSlice(slicePoints);
						if (slice.GetGameObjects().Count > 0) {
							CopyTracker(slice, slicer);
						};

						trackerList.Remove(slicer);
					} else {
						if (tracker.tracking == false && tracker.lastPosition != null) {
							if (trackedPos.x != tracker.lastPosition.x && trackedPos.y != tracker.lastPosition.y) {
								bool collision = Math2D.LineIntersectPoly(new Pair2D(trackedPos, tracker.lastPosition), slicer.shape.GetLocal());

								if (collision) {
									tracker.firstPosition = tracker.lastPosition;

									tracker.tracking = true;
								}
							}
						}
					}
				}

				if (tracker != null) {
					tracker.lastPosition = trackedPos;
				}
			}
		}

		public void CopyTracker(Slice2D slice, Sliceable2D slicer) {
			foreach(Slicer2DComplexTrackedController trackerComponent in Object.FindObjectsOfType<Slicer2DComplexTrackedController>()) {
				if (trackerComponent.trackerObject == this) {
					continue;
				}

				Dictionary<Sliceable2D, Tracker.Object> list = new Dictionary<Sliceable2D, Tracker.Object>(trackerComponent.trackerObject.trackerList);
				foreach(KeyValuePair<Sliceable2D, Tracker.Object> pair in list) {
					if (pair.Key != slicer) {
						continue;
					}
					foreach(GameObject g in slice.GetGameObjects()){
						Sliceable2D newSlicer = g.GetComponent<Sliceable2D>();
						
						Tracker.Object t = null;
						trackerList.TryGetValue(newSlicer, out t);

						if (t == null) {
							t = new Tracker.Object();
							
							t.pointsList = new List<Vector2D>(pair.Value.pointsList);
							t.tracking = true;

							trackerComponent.trackerObject.trackerList.Add(newSlicer, t);
						}
					}
				}
			}
		}
	}
}