using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D.Merge {

    public class Merging {

		static public List<Merge2D> ComplexMergeAll(List<Vector2D> slice, Layer layer = null) {
			List<Merge2D> result = new List<Merge2D> ();

			if (layer == null) {
				layer = Layer.Create();
			}

			foreach (Sliceable2D id in Sliceable2D.GetListLayer(layer)) {
				Merge2D sliceResult = id.ComplexMerge (slice);
				if (sliceResult.polygons.Count > 0) {
					result.Add (sliceResult);
				}
			}
					
			return(result);
		}

		static public List<Merge2D> PolygonMergeAll(Polygon2D slicePolygon, Layer layer = null) {
			List<Merge2D> result = new List<Merge2D> ();

			if (layer == null) {
				layer = Layer.Create();
			}

			foreach (Sliceable2D id in Sliceable2D.GetListLayer(layer)) {
				result.Add (id.PolygonMerge(slicePolygon));
			}
			
			return(result);
		}
    }
    
}