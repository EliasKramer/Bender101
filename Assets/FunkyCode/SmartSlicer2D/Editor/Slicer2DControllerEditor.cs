using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Utilities2D;

namespace Slicer2D {
	
	[CustomEditor(typeof(Slicer2DController))]
	public class Slicer2DControllerEditor : Editor {
		static bool visualsFoldout = true;
		static bool foldout = true;
		static bool lineEndFoldout = true;

		override public void OnInspectorGUI() {
			Slicer2DController script = target as Slicer2DController;

			script.sliceType = (Slicer2DController.SliceType)EditorGUILayout.EnumPopup ("Slicer Type", script.sliceType);
			script.sliceLayer.SetLayerType((LayerType)EditorGUILayout.EnumPopup ("Slicer Layer", script.sliceLayer.GetLayerType()));

			EditorGUI.indentLevel = EditorGUI.indentLevel + 2;

			if (script.sliceLayer.GetLayerType() == LayerType.Selected) {
				for (int i = 0; i < 8; i++) {
					script.sliceLayer.SetLayer(i, EditorGUILayout.Toggle ("Layer " + (i + 1), script.sliceLayer.GetLayerState(i)));
				}
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 2;

			visualsFoldout = EditorGUILayout.Foldout(visualsFoldout, "Visuals" );
			if (visualsFoldout) {
				EditorVisuals(script.visuals);
			}
				
			SliceTypesUpdate (script);

			DrawUIBlocking(script);

			script.input.multiTouch = EditorGUILayout.Toggle("Multi Touch", script.input.multiTouch);

			if (GUI.changed) {
				if (EditorApplication.isPlaying == false) {

					EditorUtility.SetDirty(script);
				}
			}
		}


		void DrawUIBlocking(Slicer2DController script) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			script.UIBlocking = EditorGUILayout.Toggle("UI Blocking", script.UIBlocking);
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		void SliceTypesUpdate(Slicer2DController script) {
			switch (script.sliceType) {

				case Slicer2DController.SliceType.Linear:
					foldout = EditorGUILayout.Foldout(foldout, "Linear Slicer" );
					if (foldout) {
						EditorLinear(script.linearControllerObject);
					}
					break;

				case Slicer2DController.SliceType.LinearCut:
					foldout = EditorGUILayout.Foldout(foldout, "Linear Cut Slicer" );
					if (foldout) {
						EditorLinearCut(script.linearCutControlelrObject);
					}
					break;

				case Slicer2DController.SliceType.Complex:
					foldout = EditorGUILayout.Foldout (foldout, "Complex Slicer");
					if (foldout) {
						EditorComplex(script.complexControllerObject);
					}
					break;

				case Slicer2DController.SliceType.ComplexCut:
					foldout = EditorGUILayout.Foldout(foldout, "Complex Cut Slicer" );
					if (foldout) {
						EditorComplexCut(script.complexCutControllerObject);
					}
					break;

				case Slicer2DController.SliceType.ComplexClick:
					foldout = EditorGUILayout.Foldout (foldout, "Complex Click");
					if (foldout) {
						EditorComplexClick(script.complexClickControllerObject);
					}
					break;

				case Slicer2DController.SliceType.Point:
					foldout = EditorGUILayout.Foldout (foldout, "Point Slicer");
					if (foldout) {
						EditorPoint(script.pointControllerObject);
					}
					break;

				case Slicer2DController.SliceType.Polygon:
					foldout = EditorGUILayout.Foldout(foldout, "Polygon Slicer");
					if (foldout) {
						EditorPolygon(script.polygonControllerObject);
					}
					break;
				
				case Slicer2DController.SliceType.ComplexTrail:
					foldout = EditorGUILayout.Foldout (foldout, "Complex Trail Slicer");
					if (foldout) {
						EditorComplexTrail(script.complexTrailControllerObject);
					}
					break;

				case Slicer2DController.SliceType.LinearTrail:
					foldout = EditorGUILayout.Foldout (foldout, "Linear Trail Slicer");
					if (foldout) {
						EditorLinearTrail(script.linearTrailControllerObject);
					}
					break;

				case Slicer2DController.SliceType.ComplexTracked:
					foldout = EditorGUILayout.Foldout (foldout, "Complex Tracked Slicer");
					if (foldout) {
						EditorComplexTracked(script.complexTrackedControllerObject);
					}
					break;

				case Slicer2DController.SliceType.Explode:
					foldout = EditorGUILayout.Foldout (foldout, "Explode By Point");
					if (foldout) {
						EditorExplode(script.explodeControllerObject) ;
					}
					break;

				case Slicer2DController.SliceType.Create:
					foldout = EditorGUILayout.Foldout (foldout, "Polygon Creator");
					if (foldout) {
						EditorCreate(script.createControllerObject);
					}
					break;
			}
		}

		public void EditorLinear(Controller.Linear.Controller id) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			id.addForce = EditorGUILayout.Foldout (id.addForce, "Add Force");
			if (id.addForce) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				id.addForceAmount = EditorGUILayout.FloatField ("Force Amount", id.addForceAmount);

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}


			id.autocomplete = EditorGUILayout.Toggle ("Autocomplete", id.autocomplete);
			if (id.autocomplete) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				id.autocompleteDisplay = EditorGUILayout.Toggle ("Autocomplete Display", id.autocompleteDisplay);
				id.autocompleteDistance = EditorGUILayout.FloatField ("Aatocomplete Distance", id.autocompleteDistance);
				
				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}

			id.startSliceIfPossible = EditorGUILayout.Toggle ("Start Slice If Possible", id.startSliceIfPossible);
			id.endSliceIfPossible = EditorGUILayout.Toggle ("End Slice If Possible", id.endSliceIfPossible);

			id.strippedLinear = EditorGUILayout.Toggle ("Stripped", id.strippedLinear);

			id.sliceJoints = EditorGUILayout.Toggle ("Slice Joints", id.sliceJoints);

			id.displayCollisions = EditorGUILayout.Toggle ("Display Collisions", id.displayCollisions);

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		public void EditorLinearCut(Controller.Linear.CutController id) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				//complexSliceType = (Slicer2D.SliceType)EditorGUILayout.EnumPopup ("Slice Mode", complexSliceType);
				
				id.cutSize = EditorGUILayout.FloatField ("Linear Cut Size", id.cutSize);
				if (id.cutSize < 0.01f) {
					id.cutSize = 0.01f;
				}

				//sliceJoints = EditorGUILayout.Toggle ("Slice Joints", sliceJoints);

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		public void EditorComplex(Slicer2D.Controller.Complex.Controller id) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			id.complexSliceType = (Sliceable2D.SliceType)EditorGUILayout.EnumPopup ("Slice Mode", id.complexSliceType);
			
			id.addForce = EditorGUILayout.Foldout (id.addForce, "Add Force");
			if (id.addForce) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				id.addForceAmount = EditorGUILayout.FloatField ("Force Amount", id.addForceAmount);

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}

			id.autocomplete = EditorGUILayout.Toggle ("Autocomplete", id.autocomplete);
			if (id.autocomplete) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				id.autocompleteDisplay = EditorGUILayout.Toggle ("Autocomplete Display", id.autocompleteDisplay);
				id.autocompleteDistance = EditorGUILayout.FloatField ("Aatocomplete Distance", id.autocompleteDistance);
				
				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}

			id.startSliceIfPossible = EditorGUILayout.Toggle ("Start Slice If Possible", id.startSliceIfPossible);
			id.endSliceIfPossible = EditorGUILayout.Toggle ("End Slice If Possible", id.endSliceIfPossible);

			id.minVertexDistance = EditorGUILayout.Slider("Min Vertex Distance", id.minVertexDistance, 0.05f, 5f);
			
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		public void EditorComplexCut(Controller.Complex.CutController id) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			//complexSliceType = (Slicer2D.SliceType)EditorGUILayout.EnumPopup ("Slice Mode", complexSliceType);
			
			id.cutSize = EditorGUILayout.FloatField ("Complex Cut Size", id.cutSize);
			if (id.cutSize < 0.01f) {
				id.cutSize = 0.01f;
			}

			id.minVertexDistance = EditorGUILayout.Slider("Min Vertex Distance", id.minVertexDistance, 0.05f, 5f);

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		public void EditorLinearTrail(Controller.Linear.TrailController id) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			id.addForce = EditorGUILayout.Foldout (id.addForce, "Add Force");
			if (id.addForce) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				id.addForceAmount = EditorGUILayout.FloatField ("Force Amount", id.addForceAmount);

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}

			int length = id.trailRendererCount;
			id.trailRendererCount = EditorGUILayout.IntField ("Trail Renderer Count", id.trailRendererCount);

			if (length != id.trailRendererCount) {
				id.trailRenderer = new TrailRenderer[id.trailRendererCount];
			} else {
				if (length > id.trailRenderer.Length) {
					id.trailRenderer = new TrailRenderer[length];
				}
			}

			for(int i = 0; i < length; i++) {
				id.trailRenderer[i] = (TrailRenderer)EditorGUILayout.ObjectField ("Trail Renderer", id.trailRenderer[i], typeof(TrailRenderer), true);		
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		public void EditorComplexTrail(Controller.Complex.TrailController id) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			id.addForce = EditorGUILayout.Foldout (id.addForce, "Add Force");
			if (id.addForce) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				id.addForceAmount = EditorGUILayout.FloatField ("Force Amount", id.addForceAmount);

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}

			int length = id.trailRendererCount;
			id.trailRendererCount = EditorGUILayout.IntField ("Trail Renderer Count", id.trailRendererCount);

			if (length != id.trailRendererCount) {
				id.trailRenderer = new TrailRenderer[id.trailRendererCount];
			} else {
				if (length > id.trailRenderer.Length) {
					id.trailRenderer = new TrailRenderer[length];
				}
			}

			for(int i = 0; i < length; i++) {
				id.trailRenderer[i] = (TrailRenderer)EditorGUILayout.ObjectField ("Trail Renderer", id.trailRenderer[i], typeof(TrailRenderer), true);		
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		public void EditorPolygon(Controller.Extended.PolygonController id) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			//complexSliceType = (Slicer2D.SliceType)EditorGUILayout.EnumPopup ("Slice Mode", complexSliceType);

			id.polygonType = (Polygon2D.PolygonType)EditorGUILayout.EnumPopup ("Type", id.polygonType);
			if (id.polygonType == Polygon2D.PolygonType.Circle) {
				id.edgeCount = (int)EditorGUILayout.Slider("Edge Count", id.edgeCount, 3, 100);
			}

			id.polygonSize = EditorGUILayout.FloatField ("Size", id.polygonSize);
			id.polygonDestroy = EditorGUILayout.Toggle ("Destroy Result", id.polygonDestroy);

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		public void EditorComplexClick(Controller.Complex.ClickController id) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			id.complexSliceType = (Sliceable2D.SliceType)EditorGUILayout.EnumPopup ("Slice Mode", id.complexSliceType);

			id.pointsLimit = EditorGUILayout.IntField ("Points Limit", id.pointsLimit);

			id.endSliceIfPossible = EditorGUILayout.Toggle ("End Slice If Possible", id.endSliceIfPossible);

			id.sliceJoints = EditorGUILayout.Toggle ("Slice Joints", id.sliceJoints);

			id.addForce = EditorGUILayout.Foldout (id.addForce, "Add Force");
			if (id.addForce) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				id.addForceAmount = EditorGUILayout.FloatField ("Force Amount", id.addForceAmount);

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}
		
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		public void EditorPoint(Controller.Extended.PointController id) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			id.sliceRotation = (Controller.Extended.PointController.SliceRotation)EditorGUILayout.EnumPopup ("Slice Rotation", id.sliceRotation);

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		public void EditorComplexTracked(Controller.Complex.TrackerController id) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			//???
			id.complexSliceType = (Sliceable2D.SliceType)EditorGUILayout.EnumPopup ("Slice Mode", id.complexSliceType);

			id.minVertexDistance = EditorGUILayout.Slider("Min Vertex Distance", id.minVertexDistance, 0.05f, 5f);

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		public void EditorExplode(Controller.Extended.ExplodeController id) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			id.explodeFromPoint = EditorGUILayout.Toggle ("Explode From Point", id.explodeFromPoint);

			id.addForce = EditorGUILayout.Foldout (id.addForce, "Add Force");
			if (id.addForce) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				id.addForceAmount = EditorGUILayout.FloatField ("Force Amount", id.addForceAmount);

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		public void EditorCreate(Controller.Extended.CreateController id) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

			id.createType = (Controller.Extended.CreateController.CreateType)EditorGUILayout.EnumPopup ("Creation Type",id.createType);
			if (id.createType == Controller.Extended.CreateController.CreateType.PolygonType) {
				id.polygonType = (Polygon2D.PolygonType)EditorGUILayout.EnumPopup ("Type", id.polygonType);
				id.polygonSize = EditorGUILayout.FloatField ("Size", id.polygonSize);
				
				if (id.polygonType == Polygon2D.PolygonType.Circle) {
					id.edgeCount = (int)EditorGUILayout.Slider("Edge Count", id.edgeCount, 3, 100);
				}
			}

			id.material = (Material)EditorGUILayout.ObjectField ("Material", id.material, typeof(Material), true);

			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}

		public void EditorVisuals(Slicer2D.Visuals id) {
			EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
			id.drawSlicer = EditorGUILayout.Toggle ("Enable Visuals", id.drawSlicer);

			if (id.drawSlicer == true) {
				GUISortingLayer.Draw(id.sortingLayer);
				
				id.zPosition = EditorGUILayout.FloatField ("Slicer Z", id.zPosition);
				id.slicerColor = (Color)EditorGUILayout.ColorField ("Slicer Color", id.slicerColor);
				id.visualScale = EditorGUILayout.Slider("Slicer Scale", id.visualScale, 0.5f, 50f);

				id.lineBorder = EditorGUILayout.Toggle ("Border", id.lineBorder);

				if (id.lineBorder == true) { // Disable?
					id.borderScale = EditorGUILayout.Slider("Border Scale", id.borderScale, 1f, 5f);
				}

				id.lineWidth = EditorGUILayout.Slider("Line Width", id.lineWidth, 0.05f, 5f);

				id.minVertexDistance = EditorGUILayout.Slider("Min Vertex Distance", id.minVertexDistance, 0.05f, 5f);
			
				id.vertexSpace = EditorGUILayout.Slider("Vertex Space", id.vertexSpace, 0f, 1f);
		
				id.customMaterial = EditorGUILayout.Toggle("Custom Material", id.customMaterial);

				lineEndFoldout = EditorGUILayout.Foldout(lineEndFoldout, "Line Ending" );
				if (lineEndFoldout) {
					EditorGUI.indentLevel = EditorGUI.indentLevel + 2;

					id.lineEndingType = (Slicer2D.LineEndingType)EditorGUILayout.EnumPopup ("Type", id.lineEndingType);
					if (id.lineEndingType == Slicer2D.LineEndingType.Circle) {
						id.lineEndingEdgeCount = (int)EditorGUILayout.Slider("Edges", id.lineEndingEdgeCount, 3, 30);
					}

					id.lineEndWidth = EditorGUILayout.Slider("Width", id.lineEndWidth, 0.05f, 5f);
					id.lineEndSize = EditorGUILayout.Slider("Size", id.lineEndSize, 0.05f, 5f);

					id.customEndingsImage = EditorGUILayout.Toggle("Custom Image", id.customEndingsImage);
				
					if (id.customEndingsImage) {
						id.customEndingImageMaterial = (Material)EditorGUILayout.ObjectField("Ending Material", id.customEndingImageMaterial, typeof(Material), true);
					}

					EditorGUI.indentLevel = EditorGUI.indentLevel - 2;
				}

				if (id.customMaterial) {
					id.customFillMaterial = (Material)EditorGUILayout.ObjectField("Fill", id.customFillMaterial, typeof(Material), true);
					id.customBoarderMaterial = (Material)EditorGUILayout.ObjectField("Boarder", id.customBoarderMaterial, typeof(Material), true);
				}

				if (id.lineWidth < 0.01f) {
					id.lineWidth = 0.01f;
				}
				
				if (id.lineEndSize < 0.05f) {
					id.lineEndSize = 0.05f;
				}

				if (id.minVertexDistance < 0.05f) {
					id.minVertexDistance = 0.05f;
				}
			}
			
			EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
		}
	}
}