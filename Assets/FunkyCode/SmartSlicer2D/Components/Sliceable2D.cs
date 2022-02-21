using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {

	[ExecuteInEditMode]
	public class Sliceable2D : MonoBehaviour {
		public enum ShapeType {Collider, SpriteCustomShape};
		public enum SliceType {Regular, SliceHole, FillSlicedHole}; // Customize More!
		public enum TextureType {Sprite, Sprite3D, Mesh2D, Mesh3D, Collider2D, SpriteAnimation, ImageUI, None};
		public enum CenterOfSliceTransform {Origin, ColliderCenter};
		public enum AnchorType {AttachRigidbody, RemoveConstraints, CancelSlice, Nothing}; // DestroySlice
		public enum ColliderType {PolygonCollider2D, EdgeCollider2D};
		public enum InstantiationMethod {Quality, Performance};

		// Complex Slicer Options
		public static SliceType complexSliceType = SliceType.Regular;

		//[Tooltip("Type of texture to generate")]
		public TextureType textureType = TextureType.Sprite;

		public SlicingLayer slicingLayer = SlicingLayer.Layer1;

		public ColliderType colliderType = ColliderType.PolygonCollider2D;

		// Mesh Fields
		public MaterialSettings materialSettings = new MaterialSettings();

		public ShapeType shapeType = ShapeType.Collider;

		// Slicing Limit
		public Limit limit = new Limit();

		// Event Handling
		public EventHandling eventHandler = new EventHandling();

		public InstantiationMethod instantiateMethod = InstantiationMethod.Quality;
		
		// Mass
		public CenterOfSliceTransform centerOfSlice = CenterOfSliceTransform.Origin;
		public bool recalculateMass = false;

		// Sprite Information
		public VirtualSpriteRenderer spriteRenderer = null;

		// Joints Support
		public bool supportJoints = false;
		private Rigidbody2D body = null;
		private List<Utilities2D.Joint2D> joints = new List<Utilities2D.Joint2D>();

		static private List<Sliceable2D> slicer2DList = new List<Sliceable2D>();
		static private List<Sliceable2D> getLayerList = new List<Sliceable2D>();

		// Shape API
		public Shape shape = new Shape();

		private MeshFilter meshFilter;
		private MeshRenderer meshRenderer;
		private SpriteRenderer spriteRendererComponent;

		public Anchor anchor = new Anchor();

		private bool reinitialize = false;

		// Why we need this option?
		// Instantiate Performane will cause issues because colliders and sprite renderer is removed
		public bool afterSliceRemoveOrigin = true;

		public void OnDestroy() {
			if (Application.isPlaying) {
				if (meshFilter != null) {
					if (meshFilter.sharedMesh != null) {
						//UnityEngine.Debug.Log("destroy");
						Destroy(meshFilter.sharedMesh);
					}
				}
			}
			eventHandler.ClearEvents();
		}

		private Polygon2D GetPolygonToSlice() {
			if (limit.counter >= limit.maxSlices && limit.enabled) {
				return(null);
			}

			return(shape.GetWorld());
		}

		public Rigidbody2D GetRigibody() {
			if (body == null) {
				body = GetComponent<Rigidbody2D>();
			}
			return(body); 
		}

		// Component Events
		public void AddAnchorEvent(EventHandling.Slice2DEvent slicerEvent) {
			eventHandler.anchorSliceEvent += slicerEvent;
		}
		
		public void AddAnchorResultEvent(EventHandling.Slice2DResultEvent slicerEvent) {
			eventHandler.anchorSliceResultEvent += slicerEvent;
		}

		public void AddEvent(EventHandling.Slice2DEvent slicerEvent) {
			eventHandler.sliceEvent += slicerEvent;
		}
		
		public void AddResultEvent(EventHandling.Slice2DResultEvent slicerEvent) {
			eventHandler.sliceResultEvent += slicerEvent;
		}

		// Static Events
		// static public void AddGlobalAnchorEvent(Slicer2DEventHandling.Slice2DEvent slicerEvent) {
		// 	Slicer2DEventHandling.anchorGlobalSliceEvent += slicerEvent;
		// }
		
		static public void AddGlobalResultAnchorEvent(EventHandling.Slice2DResultEvent slicerEvent) {
			EventHandling.anchorGlobalSliceResultEvent += slicerEvent;
		}

		static public void AddGlobalEvent(EventHandling.Slice2DEvent slicerEvent) {
			EventHandling.globalSliceEvent += slicerEvent;
		}
		static public void AddGlobalResultEvent(EventHandling.Slice2DResultEvent slicerEvent) {
			EventHandling.globalSliceResultEvent += slicerEvent;
		}

		static public int GetListCount() {
			return(slicer2DList.Count);
		}

		static public List<Sliceable2D> GetList() {
			return(slicer2DList);
		}

		static public List<Sliceable2D> GetListCopy() {
			return(new List<Sliceable2D>(slicer2DList));
		}

		static public List<Sliceable2D> GetListLayer(Layer layer) {
			getLayerList.Clear();

			foreach (Sliceable2D id in slicer2DList) {
				if (id.MatchLayers (layer)) {
					getLayerList.Add(id);
				}
			}
			
			return(getLayerList);
		}
			
		public int GetLayerID() {
			return((int)slicingLayer);
		}

		public bool MatchLayers(Layer sliceLayer) {
			return((sliceLayer == null || sliceLayer.GetLayerType() == LayerType.All) || sliceLayer.GetLayerState(GetLayerID ()));
		}

		void OnEnable() {
			slicer2DList.Add (this);
		}

		void OnDisable() {
			slicer2DList.Remove (this);
		}

		// ANCHOR!!!!!!! ////////////////////

		////////////////////////

		void Awake() {
			Slicer2D.Profiler.IncObjectsCreated();

			shape.SetSlicer2D(this);
		}

		void Start() {
			Initialize ();

			if (supportJoints == true) {
				RecalculateJoints();
			}

			StartAnchor ();
		}

		void Update() {
			if (Application.isPlaying) {
				if (reinitialize) {
					reinitialize = false;

					switch(textureType) {
						case TextureType.Sprite3D:

							Vector2D uvOffset = new Vector2D(materialSettings.offset);
							Polygon2D.SpriteToMesh3D(gameObject, spriteRenderer, materialSettings.depth, materialSettings.GetTriangulation(), uvOffset);

							MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

							Material[] sharedMaterials = new Material[2];
							sharedMaterials[1] = spriteRenderer.material;
							sharedMaterials[0] = materialSettings.sideMaterial;

							meshRenderer.sharedMaterials = sharedMaterials;		
						
						break;
					}
				}
			}
		}

		// Check Before Each Function - Then This Could Be Private
		public void Initialize() {
			shape.ForceUpdate();

			List<Polygon2D> result = Polygon2DList.CreateFromGameObject (gameObject);

			// Split collider if there are more polygons than 1
			if (result.Count > 1) {
				PerformResult(result, new Slice2D());
			}

			switch (textureType) {
				case TextureType.Mesh2D:
					if (Settings.GetBatching(materialSettings.batchMaterial) && spriteRenderer != null) {
						materialSettings.material = spriteRenderer.material;
					}

					// Needs Mesh UV Options
					materialSettings.CreateMesh(gameObject, shape.GetLocal());

					meshRenderer = GetComponent<MeshRenderer> ();

					meshFilter = GetComponent<MeshFilter>();
					
					break;

				case TextureType.Mesh3D:
					shape.GetLocal().CreateMesh3D (gameObject, materialSettings.depth, materialSettings.scale, materialSettings.offset, 0, materialSettings.GetTriangulation());

					meshRenderer = GetComponent<MeshRenderer> ();
					meshFilter = GetComponent<MeshFilter>();

					Material[] sharedMaterials = new Material[2];
					sharedMaterials[1] = materialSettings.material;
					sharedMaterials[0] = materialSettings.sideMaterial;

					meshRenderer.sharedMaterials = sharedMaterials;

					break;

				case TextureType.Sprite:
					if (spriteRenderer == null) {
						spriteRendererComponent = GetComponent<SpriteRenderer>();

						spriteRenderer = new VirtualSpriteRenderer(spriteRendererComponent);
					} else {
						if (Settings.GetBatching(materialSettings.batchMaterial) == false) {
							spriteRenderer.material = new Material(spriteRenderer.material);
						}
					}
					break;

				case TextureType.Sprite3D:
					if (spriteRenderer == null) {
						spriteRendererComponent = GetComponent<SpriteRenderer>();

						spriteRenderer = new VirtualSpriteRenderer(spriteRendererComponent);

						reinitialize = true;

						if (Application.IsPlaying(this.gameObject)) {
							Destroy(spriteRendererComponent);
						}
						
						spriteRendererComponent = null;
					} else {
						if (Settings.GetBatching(materialSettings.batchMaterial) == false) {
							spriteRenderer.material = new Material(spriteRenderer.material);
						}
					}
					break;
				
				case TextureType.SpriteAnimation:
					if (spriteRenderer == null) {
						spriteRenderer = new VirtualSpriteRenderer(GetComponent<SpriteRenderer>());
					}
					break;

				default:
					break;
				}
		}

		public List<GameObject> PerformResult(List<Polygon2D> result, Slice2D slice) {
			List<GameObject> resultGameObjects = new List<GameObject> ();

			if (result.Count < 1) {
				return(resultGameObjects);
			}

			slice.SetPolygons(result);

			slice.originGameObject = gameObject;

			if (eventHandler.SliceEvent(slice) == false) {
				return(resultGameObjects);
			}

			if (EventHandling.GlobalSliceEvent(slice) == false) {
				return(resultGameObjects);
			}

			double originArea = 1f;
			if (recalculateMass) {
				originArea = shape.GetLocal().GetArea();
			}

			Rigidbody2D originalRigidBody = gameObject.GetComponent<Rigidbody2D>();

			Collider2D collider2D = gameObject.GetComponent<Collider2D>();
			PhysicsMaterial2D material = collider2D.sharedMaterial;
			bool isTrigger = collider2D.isTrigger;	

			switch(Settings.GetComponentsCopy(instantiateMethod)) {
				case InstantiationMethod.Performance: 

					switch (textureType) {
						case TextureType.Sprite:
						case TextureType.Sprite3D:
							if (spriteRendererComponent) {
								DestroyImmediate(spriteRendererComponent);
								spriteRendererComponent = null;
							}
							break;
						}

					BoxCollider2D c1 = gameObject.GetComponent<BoxCollider2D>();
					if (c1 != null) {
						DestroyImmediate(c1);
					}

					CircleCollider2D c2 = gameObject.GetComponent<CircleCollider2D>();
					if (c2 != null) {
						DestroyImmediate(c2);
					}

					CapsuleCollider2D c3 = gameObject.GetComponent<CapsuleCollider2D>();
					if (c3 != null) {
						DestroyImmediate(c3);
					}
					break;
			}

			int name_id = 1;
			foreach (Polygon2D id in result) {
				GameObject gObject = null;

				switch(Settings.GetComponentsCopy(instantiateMethod)) {
					case InstantiationMethod.Performance: 
						Slicer2D.Profiler.IncSlicesCreatedWithPerformance();

						gObject = Instantiate(gameObject, transform.position, transform.rotation, transform.parent);
					
						break;

					case InstantiationMethod.Quality: 
						Slicer2D.Profiler.IncSlicesCreatedWithQuality();

						gObject = new GameObject();
						gObject.transform.parent = transform.parent;
						gObject.transform.position = transform.position;
						gObject.transform.rotation = transform.rotation;
					
						break;
				}
				
				gObject.name = name + " (" + name_id + ")";
				gObject.transform.localScale = transform.localScale;
				
				gObject.layer = gameObject.layer;
				gObject.tag = gameObject.tag;
				
				resultGameObjects.Add (gObject);

				switch(Settings.GetComponentsCopy(instantiateMethod)) {
					case InstantiationMethod.Quality: 
						Components.Copy(this, gObject);
						break;
				}

				Sliceable2D slicer = gObject.GetComponent<Sliceable2D> ();

				slicer.limit = new Limit();
				slicer.limit.counter = limit.counter + 1;
				slicer.limit.maxSlices = limit.maxSlices;
				slicer.limit.enabled = limit.enabled;

				slicer.eventHandler = new EventHandling();

				slicer.shape = new Shape();
				slicer.shape.SetSlicer2D(slicer);
				slicer.shape.ForceUpdate();

				slicer.materialSettings = materialSettings.Copy();

				slicer.anchor = anchor.Copy();

				Components.CopyRigidbody2D(originalRigidBody, slicer, id, originArea);

				if (Settings.GetCenterOfSliceTransform(centerOfSlice) == CenterOfSliceTransform.ColliderCenter) {
					Polygon2D localPoly = id.ToLocalSpace (gObject.transform);
					Rect bounds = localPoly.GetBounds();
					Vector2 center = bounds.center;
			
					Vector2 centerWorld = new Vector2(center.x * transform.lossyScale.x, center.y * transform.lossyScale.y);
					gObject.transform.Translate(centerWorld);
					
					slicer.materialSettings.offset += center;					
				}

				Collider2D collider = null;
				switch (colliderType) {
					case ColliderType.PolygonCollider2D:
						collider = (Collider2D)id.ToLocalSpace (gObject.transform).CreatePolygonCollider (gObject);
						break;
					case ColliderType.EdgeCollider2D:
						collider = (Collider2D)id.ToLocalSpace (gObject.transform).CreateEdgeCollider (gObject);
						break;
				}

				collider.sharedMaterial = material;
				collider.isTrigger = isTrigger;

				Vector2D uvOffset;

				switch (textureType) {
					case TextureType.Sprite:
						slicer.spriteRenderer = spriteRenderer;
						uvOffset = new Vector2D(slicer.materialSettings.offset);
						Polygon2D.SpriteToMesh(gObject, spriteRenderer, materialSettings.GetTriangulation(), uvOffset);
						break;

					case TextureType.Sprite3D:
						slicer.spriteRenderer = spriteRenderer;
						uvOffset = new Vector2D(slicer.materialSettings.offset);
						Polygon2D.SpriteToMesh3D(gObject, spriteRenderer, materialSettings.depth, materialSettings.GetTriangulation(), uvOffset);

						MeshRenderer meshRenderer = slicer.GetComponent<MeshRenderer>();

						Material[] sharedMaterials = new Material[2];
						sharedMaterials[1] = spriteRenderer.material;
						sharedMaterials[0] = materialSettings.sideMaterial;

						meshRenderer.sharedMaterials = sharedMaterials;				

						break;

					case TextureType.SpriteAnimation:
						slicer.textureType = TextureType.Sprite;
						Polygon2D.SpriteToMesh(gObject, spriteRenderer, materialSettings.GetTriangulation());
						break;
						
					default:
						break;
					}

				name_id += 1;
			}
				
			if (afterSliceRemoveOrigin) {	
				Destroy (gameObject);
			}

			if (resultGameObjects.Count > 0) {
				slice.originGameObject = gameObject;
				
				slice.SetGameObjects(resultGameObjects);

				if (supportJoints == true) {
					SliceJointEvent (slice);
				}

				eventHandler.Result(slice);
			}

			return(resultGameObjects);
		}

		///// Merger 2D
		public Merge.Merge2D PolygonMerge(Polygon2D slice, bool perform = true) {
			Merge.Merge2D merge2D = Merge.Merge2D.Create (slice.pointsList);

			if (isActiveAndEnabled == false) {
				return(merge2D);
			}

			Polygon2D colliderPolygon = shape.GetWorld();
			if (colliderPolygon != null) {
				Merge.Merge2D mergeResult = API.PolygonMerge (colliderPolygon, slice);

				if (mergeResult.polygons.Count > 0) {
					if (perform) {
						PerformMergeResult (mergeResult.polygons, mergeResult);
					}
				}

				return(mergeResult);
			}
				
			return(merge2D);
		}

		public Merge.Merge2D ComplexMerge(List<Vector2D> slice, bool perform = true) {
			Merge.Merge2D merge2D = Merge.Merge2D.Create (slice);

			if (isActiveAndEnabled == false) {
				return(merge2D);
			}

			Polygon2D colliderPolygon = shape.GetWorld();
			if (colliderPolygon != null) {
				Merge.Merge2D mergeResult = API.ComplexMerge (colliderPolygon, slice);

				if (mergeResult.polygons.Count > 0) {
					if (perform) {
						PerformMergeResult (mergeResult.polygons, mergeResult);
					}
				}

				return(mergeResult);
			}
				
			return(merge2D);
		}

		///// Merger2D Perform
		public void PerformMergeResult(List<Polygon2D> polygons, Merge.Merge2D mergeResult) {
			if (mergeResult.polygons.Count > 0) {
				Polygon2D merged =  mergeResult.polygons[0];
				if (merged != null) {
					merged.ToLocalSpace(transform).CreatePolygonCollider(gameObject);

					shape = new Shape();

					shape.SetSlicer2D(this);

					Initialize();

					GetComponent<ColliderLineRenderer2D>().Initialize();
				}
			}
		}

		///// Slicer 2D
		public Slice2D LinearSlice(Pair2D slice, bool perform = true) {
			Slice2D slice2D = Slice2D.Create (gameObject, slice);

			if (isActiveAndEnabled == false) {
				return(slice2D);
			}

			Polygon2D colliderPolygon = GetPolygonToSlice ();
			if (colliderPolygon != null) {
				Slice2D sliceResult = Slicer2D.API.LinearSlice (colliderPolygon, slice);

				if (perform) {
					PerformResult (sliceResult.GetPolygons(), sliceResult);
				}

				return(sliceResult);
			}
				
			return(slice2D);
		}

		public Slice2D LinearCutSlice(LinearCut slice) {
			Slice2D slice2D = Slice2D.Create (gameObject, slice);

			if (isActiveAndEnabled == false) {
				return(slice2D);
			}

			Polygon2D colliderPolygon = GetPolygonToSlice ();
			if (colliderPolygon != null) {
				Polygon2D slicePoly = new Polygon2D(slice.GetPointsList(1.01f));
				
				if (Math2D.PolyInPoly(slicePoly, colliderPolygon) == true) {
					Destroy (gameObject);
					return(slice2D);

				} else {
					Slice2D sliceResult = Slicer2D.API.LinearCutSlice (colliderPolygon, slice);
					
					foreach(Polygon2D poly in new List<Polygon2D> (sliceResult.GetPolygons())) {
						if (Math2D.PolyInPoly(slicePoly, poly)) {
							sliceResult.RemovePolygon(poly);
						}
					}

					PerformResult (sliceResult.GetPolygons(), sliceResult);

					return(sliceResult);
				}
			}
			
			return(Slice2D.Create (gameObject, slice));
		}
				
		public Slice2D ComplexSlice(List<Vector2D> slice) {
			Slice2D slice2D = Slice2D.Create (gameObject, slice);

			if (isActiveAndEnabled == false) {
				return(slice2D);
			}

			Polygon2D colliderPolygon = GetPolygonToSlice ();
			if (colliderPolygon != null) {
				Slice2D sliceResult = Slicer2D.API.ComplexSlice (colliderPolygon, slice);
				PerformResult (sliceResult.GetPolygons(), sliceResult);

				return(sliceResult);
			}
			
			return(slice2D);
		}

		public Slice2D ComplexCutSlice(ComplexCut slice) {
			Slice2D slice2D = Slice2D.Create (gameObject, slice);

			if (isActiveAndEnabled == false) {
				return(slice2D);
			}

			Polygon2D colliderPolygon = GetPolygonToSlice ();
			if (colliderPolygon != null) {
				Polygon2D slicePoly = new Polygon2D(slice.GetPointsList(1.01f));
				
				if (Math2D.PolyInPoly(slicePoly, colliderPolygon) == true) {
					Destroy (gameObject);
					return(slice2D);

				} else {
					Slice2D sliceResult = Slicer2D.API.ComplexCutSlice (colliderPolygon, slice);
					
					foreach(Polygon2D poly in new List<Polygon2D> (sliceResult.GetPolygons())) {
						if (Math2D.PolyInPoly(slicePoly, poly)) {
							sliceResult.RemovePolygon(poly);
						}
					}

					PerformResult (sliceResult.GetPolygons(), sliceResult);

					return(sliceResult);
				}
			}
			
			return(Slice2D.Create (gameObject, slice));
		}

		public Slice2D PointSlice(Vector2D point, float rotation) {
			Slice2D slice2D = Slice2D.Create (gameObject, point, rotation);

			if (isActiveAndEnabled == false) {
				return(slice2D);
			}

			Polygon2D colliderPolygon = GetPolygonToSlice ();
			if (colliderPolygon != null) {
				Slice2D sliceResult = Slicer2D.API.PointSlice (colliderPolygon, point, rotation);

				PerformResult (sliceResult.GetPolygons(), sliceResult);
				
				return(sliceResult);
			}

			return(slice2D);
		}

		public Slice2D PolygonSlice(Polygon2D slice, Polygon2D slicePolygonDestroy) {
			Slice2D slice2D = Slice2D.Create (gameObject, slice);

			if (isActiveAndEnabled == false) {
				return(slice2D);
			}
			
			Polygon2D colliderPolygon = GetPolygonToSlice ();
			if (colliderPolygon != null) {
				Slice2D sliceResult = Slicer2D.API.PolygonSlice (colliderPolygon, slice);

				if (sliceResult.GetPolygons().Count > 0) {
					if (slicePolygonDestroy != null) {
						foreach (Polygon2D p in new List<Polygon2D>(sliceResult.GetPolygons())) {
							if (slicePolygonDestroy.PolyInPoly (p) == true) {
								sliceResult.RemovePolygon (p);
							}
						}
					}
					// Check If Slice Result Is Correct
					if (sliceResult.GetPolygons().Count > 0) {
						sliceResult.AddGameObjects (PerformResult (sliceResult.GetPolygons(), slice2D));
					} else if (slicePolygonDestroy != null) {
						Destroy (gameObject);
					}
		
					return(sliceResult);
				}
			}

			return(slice2D);
		}

		public Slice2D ExplodeByPoint(Vector2D point, int explosionSlices = 0) {
			Slice2D slice2D = Slice2D.Create (gameObject, point);
			
			if (isActiveAndEnabled == false) {
				return(slice2D);
			}

			Polygon2D colliderPolygon = GetPolygonToSlice ();
			if (colliderPolygon != null) {
				Slice2D sliceResult = Slicer2D.API.ExplodeByPoint (colliderPolygon, point, explosionSlices);
				PerformResult (sliceResult.GetPolygons(), sliceResult);
				
				return(sliceResult);
			}

			return(slice2D);
		}

		public Slice2D ExplodeInPoint(Vector2D point, int explosionSlices = 0) {
			Slice2D slice2D = Slice2D.Create (gameObject, point);
			
			if (isActiveAndEnabled == false) {
				return(slice2D);
			}

			Polygon2D colliderPolygon = GetPolygonToSlice ();
			if (colliderPolygon != null) {
				Slice2D sliceResult = Slicer2D.API.ExplodeInPoint (colliderPolygon, point, explosionSlices);
				PerformResult (sliceResult.GetPolygons(), sliceResult);
				
				return(sliceResult);
			}

			return(slice2D);
		}

		public Slice2D Explode(int explosionSlices = 0) {
			Slice2D slice2D = Slice2D.Create (gameObject, Slice2DType.Explode);

			if (isActiveAndEnabled == false) {
				return(slice2D);
			}

			Polygon2D colliderPolygon = GetPolygonToSlice ();
			if (colliderPolygon != null) {
				Slice2D sliceResult = Slicer2D.API.Explode (colliderPolygon, explosionSlices);
				PerformResult (sliceResult.GetPolygons(), sliceResult);
				
				return(sliceResult);
			}

			return(slice2D);
		}
			
		public void RecalculateJoints() {
			Rigidbody2D body = GetRigibody();
			if (body) {
				joints = Utilities2D.Joint2D.GetJointsConnected (body);
			}
		}

		void SliceJointEvent(Slice2D sliceResult) {
			RecalculateJoints() ;

			// Remove Slicer Component Duplicated From Sliced Components
			foreach (GameObject g in sliceResult.GetGameObjects()) {
				List<Utilities2D.Joint2D> joints = Utilities2D.Joint2D.GetJoints(g);
				foreach(Utilities2D.Joint2D joint in joints) {
					if (Polygon2DList.CreateFromGameObject (g)[0].PointInPoly (new Vector2D (joint.anchoredJoint2D.anchor)) == false) {

						Destroy (joint.anchoredJoint2D);
					} else {

						if (joint.anchoredJoint2D != null && joint.anchoredJoint2D.connectedBody != null) {

							Sliceable2D slicer2D = joint.anchoredJoint2D.connectedBody.gameObject.GetComponent<Sliceable2D>();
							if (slicer2D != null) {
								slicer2D.RecalculateJoints();
							}
						}
					}
				}
			}
		
			if (GetRigibody() == null) {
				return;
			}

			// Reconnect Joints To Sliced Bodies
			foreach(Utilities2D.Joint2D joint in joints) {
				if (joint.anchoredJoint2D == null) {
					continue;
				}
				
				foreach (GameObject g in sliceResult.GetGameObjects()) {
					Sliceable2D slicer = g.GetComponent<Sliceable2D>();

					//Polygon2D poly = slicer.shape.GetLocal();
					Polygon2D polyWorld = slicer.shape.GetWorld();
					
					Vector2D connectedAnchor2D = new Vector2D (joint.anchoredJoint2D.connectedAnchor);
					Vector2 connectedAnchorWorld = joint.anchoredJoint2D.connectedBody.transform.TransformPoint(connectedAnchor2D.ToVector2());
					Vector2D connectedAnchorWorld2D =  new Vector2D (connectedAnchorWorld);

					switch (joint.jointType) {
						case Utilities2D.Joint2D.Type.HingeJoint2D:
							if (polyWorld.PointInPoly (connectedAnchorWorld2D)) {
								joint.anchoredJoint2D.connectedBody = g.GetComponent<Rigidbody2D> ();
							}
							break;

						default:
							if (polyWorld.PointInPoly (connectedAnchorWorld2D)) {
								joint.anchoredJoint2D.connectedBody = g.GetComponent<Rigidbody2D> ();
							} else {
								
							}
							break;
					}
				}
			}
		}
			
		void StartAnchor () {
			bool addEvents = false;

			foreach(Collider2D collider in anchor.anchorsList) {
				if (collider != null) {
					addEvents = true;
				}
			}

			if (addEvents == false) {
				return;
			}

			Sliceable2D slicer = GetComponent<Sliceable2D> ();
			if (slicer != null) {
				slicer.AddResultEvent (OnAnchorSliceResult);
				slicer.AddEvent (OnAnchorSlice);
			}

			anchor.anchorPolygons = new List<Polygon2D>();
			anchor.anchorColliders = new List<Collider2D>();

			foreach(Collider2D collider in anchor.anchorsList) {
				anchor.anchorPolygons.Add(Polygon2DList.CreateFromGameObject (collider.gameObject)[0]);
				anchor.anchorColliders.Add(collider);
			}
		}

		bool OnAnchorSlice(Slice2D sliceResult) {
			return(Anchor.OnAnchorSlice(this, sliceResult));
		}

		void OnAnchorSliceResult(Slice2D sliceResult) {
			Anchor.OnAnchorSliceResult(this, sliceResult);
		}
		
		static public Sliceable2D PointInSlicerComponent(Vector2D point) {
			foreach(Sliceable2D slicer in Sliceable2D.GetList()) {
				Polygon2D poly = slicer.shape.GetWorld();
				if (poly.PointInPoly(point)) {
					return(slicer);
				}
			}
			return(null);
		}
	}
}