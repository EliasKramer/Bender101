using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {

	[CustomEditor(typeof(Sliceable2D))]
	public class Slicer2DEditor : Editor {
		static bool foldout = true;

		override public void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			Sliceable2D script = target as Sliceable2D;

			// Disable
			//script.shapeType = (Slicer2D.ShapeType)EditorGUILayout.EnumPopup ("Shape Type", script.shapeType);
			script.textureType = (Sliceable2D.TextureType)EditorGUILayout.EnumPopup ("Texture Type", script.textureType);
			script.colliderType = (Sliceable2D.ColliderType)EditorGUILayout.EnumPopup ("Collider Type", script.colliderType);

			SetTriangulation(script);

			SetComponentsCopy(script);

			script.centerOfSlice = (Sliceable2D.CenterOfSliceTransform)EditorGUILayout.EnumPopup ("Center of Slice", script.centerOfSlice);
			script.slicingLayer = (SlicingLayer)EditorGUILayout.EnumPopup ("Slicing Layer", script.slicingLayer);
			script.supportJoints = EditorGUILayout.Toggle("Support Joints", script.supportJoints);
			script.limit.enabled = EditorGUILayout.Toggle("Slicing Limit", script.limit.enabled);

			if (script.limit.enabled) {
				script.limit.maxSlices = EditorGUILayout.IntSlider("Max Slices", script.limit.maxSlices, 1, 10);
			}

			script.recalculateMass = EditorGUILayout.Toggle("Recalculate Mass", script.recalculateMass);
		
			script.anchor.enable = EditorGUILayout.Toggle("Anchors", script.anchor.enable);

			if (script.anchor.enable) {
				SerializedProperty anchorList = serializedObject.FindProperty ("anchor.anchorsList");

				EditorGUILayout.PropertyField (anchorList, true);
			}

			foldout = EditorGUILayout.Foldout(foldout, "Material Settings" );
			if (foldout) {
				EditorGUI.indentLevel = EditorGUI.indentLevel + 1;

				SetBatching(script);

				switch(script.textureType) {
					case Sliceable2D.TextureType.Mesh2D:
						script.materialSettings.material = (Material)EditorGUILayout.ObjectField("Material", script.materialSettings.material, typeof(Material), true);
						script.materialSettings.scale = EditorGUILayout.Vector2Field("Material Scale", script.materialSettings.scale);
						script.materialSettings.offset = EditorGUILayout.Vector2Field("Material Offset", script.materialSettings.offset);
				
						break;

					case Sliceable2D.TextureType.Sprite3D:
						script.materialSettings.depth = EditorGUILayout.Slider("Depth", script.materialSettings.depth, 0.1f, 100);
						script.materialSettings.sideMaterial = (Material)EditorGUILayout.ObjectField("Side Material", script.materialSettings.sideMaterial, typeof(Material), true);
						break;

					case Sliceable2D.TextureType.Mesh3D:
						script.materialSettings.depth = EditorGUILayout.Slider("Depth", script.materialSettings.depth, 0.1f, 100);
						script.materialSettings.material = (Material)EditorGUILayout.ObjectField("Main Material", script.materialSettings.material, typeof(Material), true);
						script.materialSettings.sideMaterial = (Material)EditorGUILayout.ObjectField("Side Material", script.materialSettings.sideMaterial, typeof(Material), true);
						script.materialSettings.scale = EditorGUILayout.Vector2Field("Material Scale", script.materialSettings.scale);
						script.materialSettings.offset = EditorGUILayout.Vector2Field("Material Offset", script.materialSettings.offset);
					
						break;

					case Sliceable2D.TextureType.Sprite:

						break;
				}

				EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
			}

			if (EditorGUI.EndChangeCheck()) {
				serializedObject.ApplyModifiedProperties();
			}

			if (GUI.changed) {
				if (EditorApplication.isPlaying == false) {

					EditorUtility.SetDirty(script);
				}
			}
		}

		public void SetTriangulation(Sliceable2D script) {
			Slicer2D.SettingsProfile profile = Slicer2D.Settings.GetProfile();

			if (profile == null || profile.triangulation == Slicer2D.Settings.Triangulation.Default) {
				script.materialSettings.triangulation = (PolygonTriangulator2D.Triangulation)EditorGUILayout.EnumPopup ("Triangulation", script.materialSettings.triangulation);
			} else {
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.EnumPopup ("Triangulation", Slicer2D.Settings.GetTriangulation(script.materialSettings.triangulation));
				EditorGUI.EndDisabledGroup();
			}
		}

		public void SetComponentsCopy(Sliceable2D script) {
			Slicer2D.SettingsProfile profile = Slicer2D.Settings.GetProfile();

			if (profile == null || profile.componentsCopy == Slicer2D.Settings.InstantiationMethod.Default) {

				script.instantiateMethod = (Sliceable2D.InstantiationMethod)EditorGUILayout.EnumPopup ("Instantiation Method", script.instantiateMethod);
			} else {
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.EnumPopup ("Instantiation", Slicer2D.Settings.GetComponentsCopy(script.instantiateMethod));
				EditorGUI.EndDisabledGroup();
			}
		}

		public void SetBatching(Sliceable2D script) {
			Slicer2D.SettingsProfile profile = Slicer2D.Settings.GetProfile();

			if (profile == null || profile.batching == Slicer2D.Settings.Batching.Default) {
				script.materialSettings.batchMaterial = EditorGUILayout.Toggle("Batch Material", script.materialSettings.batchMaterial);
			} else {
				EditorGUI.BeginDisabledGroup(true);
				EditorGUILayout.Toggle("Batch Material", Slicer2D.Settings.GetBatching(script.materialSettings.batchMaterial));
				EditorGUI.EndDisabledGroup();
			}
		}
	}
}