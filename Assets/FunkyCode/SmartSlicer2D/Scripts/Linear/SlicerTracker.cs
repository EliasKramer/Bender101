using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities2D;

namespace Slicer2D.Linear {
	
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
							tracker.firstPosition = tracker.lastPosition;
						}

						tracker.tracking = true;

					} else if (tracker.tracking == true) {
						tracker.tracking = false;

						if (tracker.firstPosition != null) {
							tracker.lastPosition = trackedPos;

							Pair2D slicePair = new Pair2D(new Vector2D(slicer.transform.TransformPoint(tracker.firstPosition.ToVector2())), new Vector2D(slicer.transform.TransformPoint(tracker.lastPosition.ToVector2())));

							Slice2D slice = slicer.LinearSlice(slicePair);
							if (slice.GetGameObjects().Count > 0) {
								CopyTracker(slice, slicer);
							};
						}

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
			foreach(Slicer2DLinearTrackedController trackerComponent in Object.FindObjectsOfType<Slicer2DLinearTrackedController>()) {
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

							t.firstPosition = pair.Value.firstPosition;
							t.lastPosition = pair.Value.lastPosition;
							t.tracking = true;

							trackerComponent.trackerObject.trackerList.Add(newSlicer, t);
						}
					}
				}
			}
		}
	}
}