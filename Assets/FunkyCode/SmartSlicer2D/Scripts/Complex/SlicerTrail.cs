using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities2D;
using Utilities2D.Extensions;

namespace Slicer2D.Complex {
	
	public class SlicerTrail {
		public Dictionary<Sliceable2D, Trail.Object> trailList = new Dictionary<Sliceable2D, Trail.Object>();

		public List<Slice2D> Update(Vector2D position, float timer, Layer layer) {
			List<Slice2D> result = new List<Slice2D>();

			foreach(Sliceable2D slicer in Sliceable2D.GetListCopy()) {
				if (slicer.MatchLayers (layer) == false) {
						
					continue;
				}

				Trail.Object trail = null;
				trailList.TryGetValue(slicer, out trail);
				
				if (trail == null) {
					trail = new Trail.Object();
					trailList.Add(slicer, trail);
				}

				if (trail.lastPosition != null) {
					if (Vector2D.Distance(trail.lastPosition, position) > 0.05f) {
						trail.pointsList.Add(new Trail.Point(position, timer));
					}
				} else {
					trail.pointsList.Add(new Trail.Point(position, timer));
				}

				if (trail.pointsList.Count > 1) {
					foreach(Trail.Point trailPoint in new List<Trail.Point>(trail.pointsList)) {
						if (trailPoint.Update() == false) {
							trail.pointsList.Remove(trailPoint);
						}
					}

					List<Vector2D> points = new List<Vector2D>();
					foreach(Trail.Point trailPoint in trail.pointsList) {
						points.Add(trailPoint.position);
					}

					Sliceable2D.complexSliceType = Sliceable2D.SliceType.Regular;
					Slice2D slice = slicer.ComplexSlice(points);
					if (slice.GetGameObjects().Count > 0) {
						trailList.Remove(slicer);

						result.Add(slice);
					};
				}

				trail.lastPosition = position;
			}

			return(result);
		}
	}
}