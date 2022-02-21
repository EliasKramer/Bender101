using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities2D.Extensions;
using Utilities2D;

namespace Slicer2D {
    public class VisualMeshExtended : Utilities2D.VisualMesh {
        
		///// POINT //////
		public void GeneratePoint(Pair2 linearPair, Transform transform, float lineWidth, float zPosition) {
			CreateLine(linearPair, transform.localScale, lineWidth, zPosition);
		}

		///// POLYGON /////
		public void GeneratePolygonMesh(Vector2 pos, Polygon2D.PolygonType polygonType, float polygonSize, float minVertexDistance, Transform transform, float lineWidth, float zPosition) {
			Polygon2D slicePolygon = Polygon2D.Create (polygonType, polygonSize).ToOffset(pos);

			Vector2 vA, vB;
			foreach(Pair2 pair in Pair2.GetList(slicePolygon.pointsList, true)) {
				vA = pair.a;
				vB = pair.b;

				vA = vA.Push (pair.a.Atan2 (pair.b), -minVertexDistance / 5);
				vB = vB.Push (pair.a.Atan2 (pair.b), minVertexDistance / 5);

				CreateLine(new Pair2(vA, vB), transform.localScale, lineWidth, zPosition);
			}
		}

		public void GeneratePolygon2DMesh(Transform transform, Polygon2D polygon, float lineOffset, float lineWidth, bool connectedLine) {
			Polygon2D poly = polygon;

			foreach(Pair2 p in Pair2.GetList(poly.pointsList, connectedLine)) {
				CreateLine(p, transform.localScale, lineWidth, lineOffset);
			}

			foreach(Polygon2D hole in poly.holesList) {
				foreach(Pair2 p in Pair2.GetList(hole.pointsList, connectedLine)) {
					CreateLine(p, transform.localScale, lineWidth, lineOffset);
				}
			}
		}

		///// COMPLEX /////
		public void Complex_GenerateMesh(Vector2List complexSlicerPointsList, Transform transform, float lineWidth, float minVertexDistance, float zPosition, float squareSize, float lineEndWidth, float vertexSpace, Slicer2D.LineEndingType endingType, int edges) {
			float size = squareSize;
			
			Vector2 vA, vB;
			float rot;

			List<Pair2> list = Pair2.GetList(complexSlicerPointsList, false);
			
			foreach(Pair2 pair in list) {
				vA = pair.a;
				vB = pair.b;

				rot = (float)Vector2D.Atan2 (pair.a, pair.b);

				vA = vA.Push (rot, -minVertexDistance * vertexSpace);
				vB = vB.Push (rot, minVertexDistance * vertexSpace);

				CreateLine(new Pair2(vA, vB), transform.localScale, lineWidth, zPosition);
			}

			Pair2 linearPair = Pair2.zero;;
			linearPair.a = complexSlicerPointsList.First();
			linearPair.b = complexSlicerPointsList.Last();

			GenerateSquare(linearPair.a, size, transform, lineEndWidth, zPosition, endingType, edges);

			GenerateSquare(linearPair.b, size, transform, lineEndWidth, zPosition, endingType, edges);
		}

		public void Complex_GenerateTrackerMesh(Dictionary<Sliceable2D, Tracker.Object> trackerList, Transform transform, float lineWidth, float zPosition) {
			foreach(KeyValuePair<Sliceable2D, Tracker.Object> trackerPair in trackerList) {
				Tracker.Object tracker = trackerPair.Value;
				if (trackerPair.Key != null && tracker.tracking) {
					if (tracker.firstPosition == null || tracker.lastPosition == null) {
						continue;
					}
					List<Vector2D> points = Vector2DList.PointsToWorldSpace(trackerPair.Key.transform, tracker.GetLinearPoints());
					foreach(Pair2 pair in Pair2.GetList(points, false)) {
						CreateLine(pair, transform.localScale, lineWidth, zPosition);
					}
				}
			}
		}

		public void Complex_GenerateTrackerMesh(Vector2 pos, Dictionary<Sliceable2D, Tracker.Object> trackerList, Transform transform, float lineWidth, float zPosition, float squareSize, LineEndingType endingType, int edges) {
			float size = squareSize;

			GenerateSquare(pos, size, transform, lineWidth, zPosition, endingType, edges);

			CreateLine(new Pair2(pos, pos), transform.localScale, lineWidth, zPosition);

			foreach(KeyValuePair<Sliceable2D, Tracker.Object> trackerPair in trackerList) {
				Tracker.Object tracker = trackerPair.Value;
				if (trackerPair.Key != null && tracker.tracking) {
					List<Vector2D> pointListWorld = Vector2DList.PointsToWorldSpace(trackerPair.Key.transform, tracker.pointsList);

					pointListWorld.Add(new Vector2D(pos));

					List<Pair2> pairList = Pair2.GetList(pointListWorld, false);

					foreach(Pair2 pair in pairList) {
						CreateLine(pair, transform.localScale, lineWidth, zPosition);
					}
				}
			}
		}

		public void Complex_GenerateCutMesh(List<Vector2D> complexSlicerPointsList, float cutSize, Transform transform, float lineWidth, float zPosition) {
			ComplexCut complexCutLine = ComplexCut.Create(complexSlicerPointsList, cutSize);
			foreach(Pair2 pair in Pair2.GetList(complexCutLine.GetPointsList(), true)) {
				CreateLine(pair, transform.localScale, lineWidth, zPosition);
			}
		}

		//// LINEAR /////
		public void Linear_GenerateMesh(Pair2 linearPair, Transform transform, float lineWidth, float zPosition, float size, float lineEndWidth, LineEndingType endingType, int edges) {
			CreateLine(linearPair, transform.localScale, lineWidth, zPosition);

			GenerateSquare(linearPair.a, size, transform, lineEndWidth, zPosition, endingType, edges);

			GenerateSquare(linearPair.b, size, transform, lineEndWidth, zPosition, endingType, edges);
		}

		public void Linear_GenerateCutMesh(Pair2 linearPair, float cutSize, Transform transform, float lineWidth, float zPosition) {
			LinearCut linearCutLine = LinearCut.Create(linearPair, cutSize);
			
			foreach(Pair2 pair in Pair2.GetList(linearCutLine.GetPointsList(), true)) {
				CreateLine(pair, transform.localScale, lineWidth, zPosition);
			}
		}

		public void Linear_GenerateTrackerMesh(Vector2 pos, Dictionary<Sliceable2D, Tracker.Object> trackerList, Transform transform, float lineWidth, float zPosition, float size, LineEndingType endingType, int edges) {
			GenerateSquare(pos, size, transform, lineWidth, zPosition, endingType, edges);

			CreateLine(new Pair2(pos, pos), transform.localScale, lineWidth, zPosition);

			foreach(KeyValuePair<Sliceable2D, Tracker.Object> trackerPair in trackerList) {
				Tracker.Object tracker = trackerPair.Value;
				if (trackerPair.Key != null && tracker.tracking) {
					foreach(Pair2 pair in Pair2.GetList(Vector2DList.PointsToWorldSpace(trackerPair.Key.transform, tracker.GetLinearPoints()), false)) {
						CreateLine(pair, transform.localScale, lineWidth, zPosition);
					}
				}
			}
		}

		///// GENERAL /////
		public void GenerateSquare(Vector2 point, float size, Transform transform, float width, float z, LineEndingType endingType, int edges) {
			if (endingType == LineEndingType.Square) {
				
				CreateLine(new Pair2(new Vector2(point.x - size, point.y - size), new Vector2(point.x + size, point.y - size)), transform.localScale, width, z);
				CreateLine(new Pair2(new Vector2(point.x - size, point.y - size), new Vector2(point.x - size, point.y + size)), transform.localScale, width, z);
				CreateLine(new Pair2(new Vector2(point.x + size, point.y + size), new Vector2(point.x + size, point.y - size)), transform.localScale, width, z);
				CreateLine(new Pair2(new Vector2(point.x + size, point.y + size), new Vector2(point.x - size, point.y + size)), transform.localScale, width, z);
			
			} else {
				float step = 360f / edges;

				for(int i = 0; i < edges; i++) {
					float x0 = Mathf.Cos((i - 1) * step * Mathf.Deg2Rad) * size;
					float y0 =  Mathf.Sin((i - 1) * step * Mathf.Deg2Rad) * size;
					float x1 = Mathf.Cos(i * step * Mathf.Deg2Rad) * size;
					float y1 =  Mathf.Sin(i * step * Mathf.Deg2Rad) * size;

					CreateLine(new Pair2(new Vector2(point.x + x0, point.y + y0), new Vector2(point.x + x1, point.y + y1)), transform.localScale, width, z);
				}	
			}
		}

        ///// Create /////
		public void GenerateCreateMesh(Vector2 pos, Polygon2D.PolygonType polygonType, float polygonSize, Controller.Extended.CreateController.CreateType createType, List<Vector2D> complexSlicerPointsList, Pair2D linearPair, float minVertexDistance, Transform transform, float lineWidth, float zPosition, float squareSize, LineEndingType endingType, int edges) {
			float size = squareSize;

			if (createType == Controller.Extended.CreateController.CreateType.Slice) {
				if (complexSlicerPointsList.Count > 0) {
					linearPair.A = new Vector2D(complexSlicerPointsList.First());
					linearPair.B = new Vector2D(complexSlicerPointsList.Last());

					GenerateSquare(linearPair.A.ToVector2(), size, transform, lineWidth, zPosition, endingType, edges);

					GenerateSquare(linearPair.B.ToVector2(), size, transform, lineWidth, zPosition, endingType, edges);

					Vector2D vA, vB;
					foreach(Pair2 pair in Pair2.GetList(complexSlicerPointsList, true)) {
						vA = new Vector2D (pair.a);
						vB = new Vector2D (pair.b);

						vA.Push (Vector2D.Atan2 (pair.a, pair.b), -minVertexDistance / 5);
						vB.Push (Vector2D.Atan2 (pair.a, pair.b), minVertexDistance / 5);

						CreateLine(new Pair2(vA.ToVector2(), vB.ToVector2()), transform.localScale, lineWidth, zPosition);
					}
				}
			} else {
				Polygon2D poly = Polygon2D.Create(polygonType, polygonSize).ToOffset(pos);

				Vector2D vA, vB;
				foreach(Pair2 pair in Pair2.GetList(poly.pointsList, true)) {
					vA = new Vector2D (pair.a);
					vB = new Vector2D (pair.b);

					vA.Push (Vector2D.Atan2 (pair.a, pair.b), -minVertexDistance / 5);
					vB.Push (Vector2D.Atan2 (pair.a, pair.b), minVertexDistance / 5);

					CreateLine(new Pair2(vA.ToVector2(), vB.ToVector2()), transform.localScale, lineWidth, zPosition);
				}
			}
		}

		
		public void GenerateTrailMesh(Dictionary<Sliceable2D, Trail.Object> trailList, Transform transform, float lineWidth, float zPosition, float squareSize) {
			foreach(KeyValuePair<Sliceable2D, Trail.Object> s in trailList) {
				if (s.Key != null) {

					List<Vector2D> points = new List<Vector2D>();
					foreach(Trail.Point trailPoint in s.Value.pointsList) {

						points.Add(trailPoint.position);

					}

					foreach(Pair2 pair in Pair2.GetList(points, false)) {
				
						CreateLine(pair, new Vector3(1, 1, 1), lineWidth, zPosition);

					}
				}
			}
		}

    }
}
