using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities2D;

namespace Slicer2D.Complex {

    public class SlicerSplit {
        public List<Vector2D> points = new List<Vector2D>();
        public Type type = Type.Normal;

        public enum Type {Normal, SingleVertexCollision}

        static public List<SlicerSplit> GetSplitSlices(Polygon2D polygon, List<Vector2D> slice) {
            bool entered = polygon.PointInPoly (slice.First ());

            List<SlicerSplit> slices = new List<SlicerSplit>();
            SlicerSplit currentSlice = new SlicerSplit ();

            Pair2D pair = Pair2D.Zero();
            for(int i = 0; i < slice.Count - 1; i++) {
                pair.A = slice[i];
                pair.B = slice[i + 1];

                List<Vector2D> stackList = polygon.GetListLineIntersectPoly(pair);
                stackList = Vector2DList.GetListSortedToPoint (stackList, pair.A);

                foreach (Vector2D id in stackList) {
                    if (entered == true) {
                        Vector2D last = null;
                        
                        if (currentSlice.points.Count > 0) {
                            last = (Vector2D)currentSlice.points.Last();
                        }
                        
                        // Goes through a same point
                        if (last != null && last.ToVector2().Equals(id.ToVector2()) == true) {
                            Debug.LogWarning ("Slicer2D: Slicing through the point"); 
                            currentSlice.type = Type.SingleVertexCollision;
                            continue;
                        }

                        currentSlice.points.Add (id);
                        slices.Add (currentSlice);	
                        
                    } else {
                        currentSlice = new SlicerSplit ();
                        currentSlice.points.Add (id);
                    }
                    entered = !entered;
                }

                if (entered == true) {
                    currentSlice.points.Add (pair.B);
                }
            }

            return(slices);
        }
    }
}
