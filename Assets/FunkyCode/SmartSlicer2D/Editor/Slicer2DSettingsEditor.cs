using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Slicer2D {
	
	public class Slicer2DSettingsEditor : EditorWindow {
		int tab = 0;

		[MenuItem("Tools/Slicer 2D")]
		public static void ShowWindow() {
			GetWindow<Slicer2DSettingsEditor>(false, "Slicer 2D", true);
		}

		void OnGUI() {
			tab = GUILayout.Toolbar (tab, new string[] {"Preferences", "Profiler"});
			switch (tab) {
				case 0:
					Preferences();
					break;

				case 1:
					Profiler();
					break;
			}
		}

		void Preferences() {
			Slicer2D.SettingsProfile profile = Slicer2D.Settings.GetProfile();

			if (profile) {
				profile.renderingPipeline= (Slicer2D.Settings.RenderingPipeline)EditorGUILayout.EnumPopup("Rendering Pipeline", profile.renderingPipeline);

				profile.garbageCollector = EditorGUILayout.Toggle("Garbage Collector", profile.garbageCollector);
				if (profile.garbageCollector) {
					profile.garbageCollectorSize = EditorGUILayout.FloatField("Garbage Collector Size", profile.garbageCollectorSize);
				}

				profile.explosionPieces = (int)EditorGUILayout.Slider("Explosion Pieces", profile.explosionPieces, 1, 30);

				profile.componentsCopy = (Slicer2D.Settings.InstantiationMethod)EditorGUILayout.EnumPopup("Instatiation Method", profile.componentsCopy);

				profile.triangulation = (Slicer2D.Settings.Triangulation)EditorGUILayout.EnumPopup("Triangulation", profile.triangulation);

				profile.batching = (Slicer2D.Settings.Batching)EditorGUILayout.EnumPopup("Batching", profile.batching);

				profile.centerOfSliceTransform = (Slicer2D.Settings.CenterOfSliceTransform)EditorGUILayout.EnumPopup("Center Of Slice", profile.centerOfSliceTransform);
				
				if (GUILayout.Button("Default Settings")) {
					profile.garbageCollector = true;
					profile.garbageCollectorSize = 0.005f;

					profile.componentsCopy = Slicer2D.Settings.InstantiationMethod.Default;
					profile.triangulation = Slicer2D.Settings.Triangulation.Default;
					profile.batching = Slicer2D.Settings.Batching.Default;
				}

				if (GUI.changed && EditorApplication.isPlaying == false) {
					EditorUtility.SetDirty(profile);
				}

				EditorGUILayout.HelpBox("Settings marked as 'default' will use local component setting", MessageType.Info);

				EditorGUILayout.HelpBox("Garbage Collector: When enabled, very small unuseful slices are removed", MessageType.None);
				EditorGUILayout.HelpBox("Instatiation Method: Performance mode would increase performance about 25%, however cannot be used in certain cases", MessageType.None);
				EditorGUILayout.HelpBox("Triangulation: The more reliable triangulation method, the slower most likely it performs. Simple shapes could use less complicated triangulation", MessageType.None);
				EditorGUILayout.HelpBox("Batching: when enabled, sliced parts of the object will use same material instance as it's origin (Improves Performance)", MessageType.None);

			} else {
				EditorGUILayout.HelpBox("Slicer2D Settings Profile Not Found!", MessageType.Error);
			}	
		}

		void Profiler() {
			EditorGUILayout.HelpBox("Advanced Triangulation: " + Slicer2D.Profiler.GetAdvancedTriangulation(), MessageType.None);
			EditorGUILayout.HelpBox("Legacy Triangulation: " + Slicer2D.Profiler.GetLegacyTriangulation(), MessageType.None);
			EditorGUILayout.HelpBox("Batched Objects: " + Slicer2D.Profiler.GetBatchingApplied(), MessageType.None);
			EditorGUILayout.HelpBox("Objects Created: " + Slicer2D.Profiler.GetObjectsCreated(), MessageType.None);
			EditorGUILayout.HelpBox("Objects Slices Created With Performance: " + Slicer2D.Profiler.GetSlicesCreatedWithPeroformance(), MessageType.None);
			EditorGUILayout.HelpBox("Objects Slices Created With Quality: " + Slicer2D.Profiler.GetSlicesCreatedWithQuality(), MessageType.None);
		}

	}
}