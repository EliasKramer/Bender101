using UnityEditor;
using UnityEngine;

namespace Utilities2D {

	[CanEditMultipleObjects]
	[CustomEditor(typeof(ColliderLineRenderer2D))]
	public class ColliderLineRenderer2DEditor : Editor {

		override public void OnInspectorGUI() {
			ColliderLineRenderer2D script = target as ColliderLineRenderer2D;

			script.customColor = EditorGUILayout.Toggle("Custom Color", script.customColor);

			script.color = EditorGUILayout.ColorField("Color", script.color);

			script.lineWidth = EditorGUILayout.FloatField("Line Width", script.lineWidth);

			script.drawEdgeCollider = EditorGUILayout.Toggle("Edge Collider", script.drawEdgeCollider);

			if (GUILayout.Button("Update Renderer")) {
				script.Initialize();
			}
		}
	}
}