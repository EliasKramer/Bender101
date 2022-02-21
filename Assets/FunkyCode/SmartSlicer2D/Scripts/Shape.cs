using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {
	
	public class Shape {
		private Polygon2D polygon = null;

		private Polygon2D polygon_world = null;
		private Polygon2D polygon_world_cache = null;

		private Sliceable2D slicer = null;
		
		public Movement movement = new Movement();

		// Is It Necessary?
		public void ForceUpdate() {
			movement.ForceUpdate();
			polygon_world = null;
			polygon_world_cache = null;
		}

		public void SetSlicer2D(Sliceable2D slicerPass) {
			slicer = slicerPass;
		}

		// Shape API
		public Polygon2D GetLocal() {
			if (polygon == null) {
				List<Polygon2D> polygons = Polygon2DList.CreateFromGameObject(slicer.gameObject);

				if (polygons.Count > 0) {
					polygon = polygons[0];
				}
				
			}
			return(polygon);
		}

		public Polygon2D GetWorld() {
			movement.Update(slicer);

			if (movement.update) {
				movement.update = false;

				polygon_world = null;
			}

			Polygon2D localPoly = GetLocal();

			if (localPoly == null) {
				return(null);
			}

			if (polygon_world == null) {
				if (polygon_world_cache == null) {

					polygon_world = localPoly.ToWorldSpace (slicer.transform);
					polygon_world_cache = polygon_world;

				} else {
					Polygon2D newPolygon = polygon_world_cache;
					Polygon2D poly = localPoly;

					List<Vector2D> pointsList = poly.pointsList;
					Vector2 v;

					for(int i = 0; i < pointsList.Count; i++) {
						v = slicer.transform.TransformPoint (pointsList[i].ToVector2());
						newPolygon.pointsList[i].x = v.x;
						newPolygon.pointsList[i].y = v.y;
					}

					for(int x = 0; x < newPolygon.holesList.Count; x++) {
						pointsList = poly.holesList[x].pointsList;

						for(int i = 0; i < pointsList.Count; i++) {
							v = slicer.transform.TransformPoint (pointsList[i].ToVector2());
							newPolygon.holesList[x].pointsList[i].x = v.x;
							newPolygon.holesList[x].pointsList[i].y = v.y;
						}
					}

					polygon_world = newPolygon;
				}
				
			}

			return(polygon_world);
		}
	}
}