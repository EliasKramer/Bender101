using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {

    public class Slicing {

        static public List<Slice2D> LinearSliceAll(Pair2D slice, Layer layer = null, bool perform = true) {
            List<Slice2D> result = new List<Slice2D> ();

            if (layer == null) {
                layer = Layer.Create();
            }

            foreach (Sliceable2D id in Sliceable2D.GetListLayer(layer)) {
                Slice2D sliceResult = id.LinearSlice (slice, perform);

                if (perform) {
                    if (sliceResult.GetGameObjects().Count > 0) {
                    result.Add (sliceResult);
                    }
                } else {
                    if (sliceResult.GetPolygons().Count > 0) {
                        result.Add (sliceResult);
                    }
                }
            }

            return(result);
        }
        
        static public List<Slice2D> LinearCutSliceAll(LinearCut linearCut, Layer layer = null) {
            List<Slice2D> result = new List<Slice2D> ();

            if (layer == null) {
                layer = Layer.Create();
            }

            foreach (Sliceable2D id in Sliceable2D.GetListLayer(layer)) {
                Slice2D sliceResult = id.LinearCutSlice (linearCut);
                if (sliceResult.GetGameObjects().Count > 0) {
                    result.Add (sliceResult);
                }
            }
                    
            return(result);
        }

		static public List<Slice2D> ComplexSliceAll(List<Vector2D> slice, Layer layer = null) {
			List<Slice2D> result = new List<Slice2D> ();

			if (layer == null) {
				layer = Layer.Create();
			}

			foreach (Sliceable2D id in Sliceable2D.GetListLayer(layer)) {
				Slice2D sliceResult = id.ComplexSlice (slice);
				if (sliceResult.GetGameObjects().Count > 0) {
					result.Add (sliceResult);
				}
			}
					
			return(result);
		}

		static public List<Slice2D> ComplexCutSliceAll(ComplexCut complexCut, Layer layer = null) {
			List<Slice2D> result = new List<Slice2D> ();

			if (layer == null) {
				layer = Layer.Create();
			}

			foreach (Sliceable2D id in Sliceable2D.GetListLayer(layer)) {
				Slice2D sliceResult = id.ComplexCutSlice (complexCut);
				if (sliceResult.GetGameObjects().Count > 0) {
					result.Add (sliceResult);
				}
			}
					
			return(result);
		}

		static public List<Slice2D> PointSliceAll(Vector2D slice, float rotation, Layer layer = null) {
			List<Slice2D> result = new List<Slice2D> ();

			if (layer == null) {
				layer = Layer.Create();
			}

			foreach (Sliceable2D id in Sliceable2D.GetListLayer(layer)) {
				Slice2D sliceResult = id.PointSlice (slice, rotation);
				if (sliceResult.GetGameObjects().Count > 0) {
					result.Add (sliceResult);
				}
			}

			return(result);
		}

		// Remove Position
		static public List<Slice2D> PolygonSliceAll(Vector2D position, Polygon2D slicePolygon, bool destroy, Layer layer = null) {
			List<Slice2D> result = new List<Slice2D> ();

			if (layer == null) {
				layer = Layer.Create();
			}

			Polygon2D slicePolygonDestroy = null;
			if (destroy) {
				slicePolygonDestroy = slicePolygon.ToScale(new Vector2(1.1f, 1.1f));
				slicePolygonDestroy = slicePolygonDestroy.ToOffset (position);
			}
			
			slicePolygon = slicePolygon.ToOffset (position);
			
			foreach (Sliceable2D id in Sliceable2D.GetListLayer(layer)) {
				result.Add (id.PolygonSlice (slicePolygon, slicePolygonDestroy));
			}
			
			return(result);
		}
		
		static public List<Slice2D> ExplodeByPointAll(Vector2D point, Layer layer = null) {
			List<Slice2D> result = new List<Slice2D> ();

			if (layer == null) {
				layer = Layer.Create();
			}

			foreach (Sliceable2D id in Sliceable2D.GetListLayer(layer)) {
				Slice2D sliceResult = id.ExplodeByPoint (point);
				if (sliceResult.GetGameObjects().Count > 0) {
					result.Add (sliceResult);
				}
			}

			return(result);
		}

		static public List<Slice2D> ExplodeInPointAll(Vector2D point, Layer layer = null) {
			List<Slice2D> result = new List<Slice2D> ();

			if (layer == null) {
				layer = Layer.Create();
			}
			
			foreach (Sliceable2D id in Sliceable2D.GetListLayer(layer)) {
				Slice2D sliceResult = id.ExplodeInPoint (point);
				if (sliceResult.GetGameObjects().Count > 0) {
					result.Add (sliceResult);
				}
			}

			return(result);
		}

		static public List<Slice2D> ExplodeAll(Layer layer = null, int explosionSlices = 0) {
			List<Slice2D> result = new List<Slice2D> ();

			if (layer == null) {
				layer = Layer.Create();
			}

			foreach (Sliceable2D id in Sliceable2D.GetListLayer(layer)) {
				Slice2D sliceResult = id.Explode (explosionSlices);
				if (sliceResult.GetGameObjects().Count > 0) {
					result.Add (sliceResult);
				}
			}

			return(result);
		}

    }

}
