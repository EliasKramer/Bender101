using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {
	
	[System.Serializable]
	public class Anchor {
		public bool enable = false;

		public Collider2D[] anchorsList = new Collider2D[1];
		public Sliceable2D.AnchorType anchorType = Sliceable2D.AnchorType.AttachRigidbody;

		public List<Polygon2D> anchorPolygons = new List<Polygon2D>();
		public List<Collider2D> anchorColliders = new List<Collider2D>();

		public Anchor Copy() {
			Anchor instance = new Anchor();
			instance.anchorsList = anchorsList;
			instance.anchorType = anchorType;
			instance.anchorPolygons = anchorPolygons;
			instance.anchorColliders = anchorColliders;

			return(instance);
		}
		
		// ANCHOR STUFF!!!! Separate
		public static Polygon2D GetPolygonInWorldSpace(Sliceable2D slicer, Polygon2D poly) {
			return(poly.ToWorldSpace(slicer.anchor.anchorColliders[slicer.anchor.anchorPolygons.IndexOf(poly)].transform));
		}

		public static bool OnAnchorSlice(Sliceable2D slicer, Slice2D sliceResult) {
			if (slicer.eventHandler.AnchorSliceEvent(sliceResult) == false) {
				return(false);
			}

			if (EventHandling.GlobalSliceEvent(sliceResult) == false) {
				return(false);
			}

			switch (slicer.anchor.anchorType) {
				case Sliceable2D.AnchorType.CancelSlice:
					foreach (Polygon2D polyA in new List<Polygon2D>(sliceResult.GetPolygons())) {
						bool perform = true;
						foreach(Polygon2D polyB in slicer.anchor.anchorPolygons) {
							if (Math2D.PolyCollidePoly (polyA, GetPolygonInWorldSpace(slicer, polyB)) ) {
								perform = false;
							}
						}
						if (perform) {
							return(false);
						}
					}
					break;
				/* 
				case AnchorType.DestroySlice:
					foreach (Polygon2D polyA in new List<Polygon2D>(sliceResult.polygons)) {
						bool perform = true;
						foreach(Polygon2D polyB in polygons) {
							if (Math2D.PolyCollidePoly (polyA.pointsList, GetPolygonInWorldSpace(polyB).pointsList) ) {
								sliceResult.polygons.Remove(polyB);
							}
						}
						
					}
					break;
				*/

				default:
					break;
			}
			return(true);
		}

		public static void OnAnchorSliceResult(Sliceable2D slicer, Slice2D sliceResult) {
			if (slicer.anchor.anchorPolygons.Count < 1) {
				return;
			}

			List<GameObject> gameObjects = new List<GameObject>();

			foreach (GameObject p in sliceResult.GetGameObjects()) {
				Polygon2D polyA = Polygon2DList.CreateFromGameObject (p)[0].ToWorldSpace (p.transform);
				bool perform = true;

				foreach(Polygon2D polyB in slicer.anchor.anchorPolygons) {
					if (Math2D.PolyCollidePoly (polyA, GetPolygonInWorldSpace(slicer, polyB))) {
						perform = false;
					}
				}

				if (perform) {
					gameObjects.Add(p);
				}
			}

			Rigidbody2D rb;

			foreach(GameObject p in gameObjects) {
				switch (slicer.anchor.anchorType) {
					case Sliceable2D.AnchorType.AttachRigidbody:
						rb = p.GetComponent<Rigidbody2D> ();
						if (rb == null) {
							rb = p.AddComponent<Rigidbody2D> ();
						}
						rb.isKinematic = false;

						break;

					case Sliceable2D.AnchorType.RemoveConstraints:
						rb = p.GetComponent<Rigidbody2D>();
						if (rb != null) {
							rb.constraints = 0;
							rb.useAutoMass = true;
						}

						break;

					default:
						break;
				}

				Sliceable2D slicerG = p.GetComponent<Sliceable2D>();
				slicerG.anchor = new Anchor();
				slicerG.anchor.enable = false;
				slicerG.anchor.anchorsList = new Collider2D[1];
				slicerG.anchor.anchorColliders = new List<Collider2D>();
				slicerG.anchor.anchorPolygons = new List<Polygon2D>();
			}

			if (gameObjects.Count > 0) {
				Slice2D newSlice = Slice2D.Create(slicer.gameObject, sliceResult.sliceType);
				newSlice.SetGameObjects(gameObjects);

				slicer.eventHandler.AnchorResult(newSlice);
			}
		}
	}
}