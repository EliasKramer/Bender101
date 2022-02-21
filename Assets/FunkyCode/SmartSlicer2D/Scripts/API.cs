using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {

    public class API {

        static public Slice2D LinearSlice(Polygon2D polygon, Pair2D slice) {
            return(Linear.Slicer.Slice (polygon, slice));
        }

        static public Slice2D LinearCutSlice(Polygon2D polygon, LinearCut linearCut) {
            return(Complex.SlicerExtended.LinearCutSlice (polygon, linearCut));
        }

        static public Slice2D ComplexSlice(Polygon2D polygon, List<Vector2D> slice) {
            return(Complex.Slicer.Slice (polygon, slice));
        }

        static public Slice2D ComplexCutSlice(Polygon2D polygon, ComplexCut complexCut) {
            return(Complex.SlicerExtended.ComplexCutSlice (polygon, complexCut));
        }

        static public Slice2D PointSlice(Polygon2D polygon, Vector2D point, float rotation) {
            return(Linear.SlicerExtended.SliceFromPoint (polygon, point, rotation));
        }

        static public Slice2D PolygonSlice(Polygon2D polygon, Polygon2D polygonB) {
            return(Complex.SlicerExtended.PolygonSlice (polygon, polygonB)); 
        }

        static public Slice2D ExplodeByPoint(Polygon2D polygon, Vector2D point, int explosionSlices = 0) {
            return(Linear.SlicerExtended.ExplodeByPoint (polygon, point, explosionSlices));
        }

        static public Slice2D ExplodeInPoint(Polygon2D polygon, Vector2D point, int explosionSlices = 0) {
            return(Linear.SlicerExtended.ExplodeInPoint (polygon, point));
        }

        static public Slice2D Explode(Polygon2D polygon, int explosionSlices = 0) {
            return(Linear.SlicerExtended.Explode (polygon, explosionSlices));
        }

        static public Polygon2D CreatorSlice(List<Vector2D> slice) {
            return(Complex.SlicerExtended.CreateSlice (slice));
        }

        static public Merge.Merge2D ComplexMerge(Polygon2D polygon, List<Vector2D> slice) {
            return(Merge.Merger.Merge (polygon, slice));
        }

        static public Merge.Merge2D PolygonMerge(Polygon2D polygon, Polygon2D mergePolygon) {
            return(Merge.MergerExtended.MergePolygon (polygon, mergePolygon));
        }
        
    }
    
}