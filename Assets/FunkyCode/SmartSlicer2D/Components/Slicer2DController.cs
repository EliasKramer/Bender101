using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Slicer2D {

	public class Slicer2DController : MonoBehaviour {
		public enum SliceType {Linear, LinearCut, LinearTracked, LinearTrail, Complex, ComplexCut, ComplexClick, ComplexTracked, ComplexTrail, Point, Polygon, Explode, Create, MergerComplex, MergerPolygon};
		public static Color[] slicerColors = {Color.black, Color.green, Color.yellow , Color.red, new Color(1f, 0.25f, 0.125f)};

		public SliceType sliceType = SliceType.Complex;

		// Slicer2DController.Get()
		private static Slicer2DController instance;
		
		// Slicer Layer
		public Layer sliceLayer = Layer.Create();

		// Slicer Visuals
		public Visuals visuals = new Visuals();

		// Input
		public InputController input = new InputController();

		// Input Events Handler
		public ControllerEventHandling eventHandler = new ControllerEventHandling();

		// Different Slicer Type Managers
		public Controller.Linear.Controller linearControllerObject = new Controller.Linear.Controller();
		public Controller.Linear.CutController linearCutControlelrObject = new  Controller.Linear.CutController();
		public Controller.Linear.TrackerController linearTrackedControlelrObject = new Controller.Linear.TrackerController();
		public Controller.Linear.TrailController linearTrailControllerObject = new Controller.Linear.TrailController();

		public Controller.Complex.Controller complexControllerObject = new Controller.Complex.Controller();
		public Controller.Complex.CutController complexCutControllerObject = new Controller.Complex.CutController();
		public Controller.Complex.TrackerController complexTrackedControllerObject = new Controller.Complex.TrackerController();
		public Controller.Complex.TrailController complexTrailControllerObject = new Controller.Complex.TrailController();
		public Controller.Complex.ClickController complexClickControllerObject = new Controller.Complex.ClickController();
	
		public Controller.Extended.PolygonController polygonControllerObject = new Controller.Extended.PolygonController();
		public Controller.Extended.CreateController createControllerObject = new Controller.Extended.CreateController();
		public Controller.Extended.PointController pointControllerObject = new Controller.Extended.PointController();
		public Controller.Extended.ExplodeController explodeControllerObject = new Controller.Extended.ExplodeController();

		public Controller.Merge2D.ComplexController mergerComplexControllerObject = new Controller.Merge2D.ComplexController();
		public Controller.Merge2D.PolygonController mergerPolygonControllerObject = new Controller.Merge2D.PolygonController();

		public bool UIBlocking = true;

		public void AddResultEvent(ControllerEventHandling.ResultEvent e) {
			eventHandler.sliceResultEvent += e;
		}

		public void Awake() {
			instance = this;
		}
		
		public void Start() {
			visuals.Initialize(gameObject);

			linearControllerObject.SetController(gameObject, input, visuals, sliceLayer, eventHandler);
			linearControllerObject.Initialize();

			complexControllerObject.SetController(gameObject, input, visuals, sliceLayer, eventHandler);
			complexControllerObject.Initialize();

			linearTrailControllerObject.SetController(gameObject, input, visuals, sliceLayer, eventHandler);
			linearTrailControllerObject.Initialize();
			
			complexTrailControllerObject.SetController(gameObject, input, visuals, sliceLayer, eventHandler);
			complexTrailControllerObject.Initialize();

			linearCutControlelrObject.SetController(gameObject, input, visuals, sliceLayer, eventHandler);
			complexCutControllerObject.SetController(gameObject, input, visuals, sliceLayer, eventHandler);

			linearTrackedControlelrObject.SetController(gameObject, input, visuals, sliceLayer, eventHandler);
			complexTrackedControllerObject.SetController(gameObject, input, visuals, sliceLayer, eventHandler);

			polygonControllerObject.SetController(gameObject, input, visuals, sliceLayer, eventHandler);
			createControllerObject.SetController(gameObject, input, visuals, sliceLayer, eventHandler);

			complexClickControllerObject.SetController(gameObject, input, visuals, sliceLayer, eventHandler);	

			pointControllerObject.SetController(gameObject, input, visuals, sliceLayer, eventHandler);

			explodeControllerObject.SetController(gameObject, input, visuals, sliceLayer, eventHandler);
			
			mergerComplexControllerObject.SetController(gameObject, input, visuals, sliceLayer, eventHandler);
			mergerComplexControllerObject.Initialize();

			mergerPolygonControllerObject.SetController(gameObject, input, visuals, sliceLayer, eventHandler);
		}

		public bool BlockedByUI() {
			if (UIBlocking == false) {
				return(false);
			}

			if (EventSystem.current == null) {
				return(false);
			}

			if (EventSystem.current.IsPointerOverGameObject(0)) {
				return(true);
			}

			if (EventSystem.current.IsPointerOverGameObject(-1)) {
				return(true);
			}

			if (UnityEngine.Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(UnityEngine.Input.GetTouch(0).fingerId)) {
				return(true);
			}

			return(false);
		}
		
		public void LateUpdate() {
			if (BlockedByUI() == false) {
				InputController.zPosition = visuals.zPosition;
				input.Update();
			}

			Vector2 pos = input.GetInputPosition();

			switch (sliceType) {	
				case SliceType.Linear:
					linearControllerObject.Update();
					break;

				case SliceType.LinearCut:
					linearCutControlelrObject.Update(pos);
					break;

				case SliceType.ComplexCut:
					complexCutControllerObject.Update(pos);
					break;

				case SliceType.Complex:
					complexControllerObject.Update();
					break;
				
				case SliceType.LinearTracked:
					linearTrackedControlelrObject.Update(pos);
					break;

				case SliceType.ComplexTracked:
					complexTrackedControllerObject.Update(pos);
					break;

				case SliceType.Point:
					pointControllerObject.Update(pos);
					break;
					
				case SliceType.Explode:			
					explodeControllerObject.Update(pos);
					break;

				case SliceType.ComplexClick:
					complexClickControllerObject.Update(pos);
					break;

				case SliceType.LinearTrail:
					linearTrailControllerObject.Update();
					break;

				case SliceType.ComplexTrail:
					complexTrailControllerObject.Update();
					break;

				case SliceType.Create:
					createControllerObject.Update(pos, transform);
					break;

				case SliceType.Polygon:
					polygonControllerObject.Update(pos);
					break;

				case SliceType.MergerComplex:
					mergerComplexControllerObject.Update();
					break;

				case SliceType.MergerPolygon:
					mergerPolygonControllerObject.Update(pos);
					break;


				default:
					break; 
			}

			Draw();
		}
		
		public void Draw() {		
			if (visuals.drawSlicer == false) {
				return;
			}
			switch (sliceType) {
				case SliceType.Linear:
					linearControllerObject.Draw(transform);
					break;

				case SliceType.Complex:
					complexControllerObject.Draw(transform);
					break;

				case SliceType.LinearTracked:
					linearTrackedControlelrObject.Draw(transform);
					break;

				case SliceType.ComplexTracked:
					complexTrackedControllerObject.Draw(transform);
					break;

				case SliceType.LinearCut:
					linearCutControlelrObject.Draw(transform);			
					break;
					
				case SliceType.ComplexCut:
					complexCutControllerObject.Draw(transform);					
					break;
					
				case SliceType.Polygon:
					polygonControllerObject.Draw(transform);
					break;

				case SliceType.Create:
					createControllerObject.Draw(transform);
					break;

				case SliceType.ComplexTrail:
					complexTrailControllerObject.Draw(transform);
					break;

				case SliceType.LinearTrail:
					linearTrailControllerObject.Draw(transform);
					break;

				case SliceType.ComplexClick:
					complexClickControllerObject.Draw(transform);
					break;

				case SliceType.Point:
					pointControllerObject.Draw(transform);
					break;

				case SliceType.Explode:
					explodeControllerObject.Draw(transform);
					break;

				case SliceType.MergerComplex:
					mergerComplexControllerObject.Draw(transform);
					break;

				case SliceType.MergerPolygon:
					mergerPolygonControllerObject.Draw(transform);
					break;
			}
		}

		public void SetSliceType(int type) {
			sliceType = (SliceType)type;
		}

		public void SetLayerType(int type) {
			if (type == 0) {
				sliceLayer.SetLayerType((LayerType)0);
			} else {
				sliceLayer.SetLayerType((LayerType)1);
				sliceLayer.DisableLayers ();
				sliceLayer.SetLayer (type - 1, true);
			}
		}

		public void SetSlicerColor(int colorInt) {
			visuals.slicerColor = slicerColors [colorInt];
		}

		public static Slicer2DController Get() {
			return(instance);
		}
	}
}