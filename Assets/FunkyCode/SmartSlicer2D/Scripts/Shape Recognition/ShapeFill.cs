using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {
	
	public class ShapeFill : MonoBehaviour {
		public int gridWidth = 10;
		public int gridHeight = 10;
		
		public List<Vector2D> pointsIn = new List<Vector2D>();
		
		private Polygon2D polygon = null;
		private Polygon2D polygon_world = null;

		public ShapeMovement movement = new ShapeMovement();

		public bool visualisation = true;
		public bool guiInfo = true;

		public void Awake() {
			Polygon2D poly = GetPolygon();
			Rect bounds = poly.GetBounds();
			
			float stepX = bounds.width / gridWidth;
			float stepY = bounds.height / gridHeight;

			for(float x = 0; x <= bounds.width; x += stepX ) {
				for(float y = 0; y <= bounds.height; y += stepY ) {
					Vector2D pos = new Vector2D(x + bounds.x, y + bounds.y);
					if (poly.PointInPoly(pos)) {
						pointsIn.Add(pos);
					}
				}
			}
		}
		
		public void Update() {
			if (visualisation == true) {
				ShapeDraw.DrawFill(this);
			}
		}

		// Debugging
		public void OnGUI() {
			if (guiInfo == true) {
				ShapeFillResult result = new ShapeFillResult(this);
				GUI.Label(new Rect(0, 0, 500, 500), "Points Count: " + pointsIn.Count);
				GUI.Label(new Rect(0, 20, 500, 500), "Points RESULT: " + result.pointsIn + " (" + (int)(100 * result.percentage) + "%)");
			}
		}

		public Polygon2D GetWorldPolygon() {
			movement.Update(transform);
			
			if (movement.update) {
				
				movement.update = false;
				polygon_world = null;
			}

			if (polygon_world == null) {
				polygon_world = GetPolygon().ToWorldSpace (transform);
			}

			return(polygon_world);
		}

		public Polygon2D GetPolygon() {
			if (polygon == null) {
				polygon = Polygon2DList.CreateFromGameObject(gameObject)[0];
			}
			return(polygon);
		}
	}
}