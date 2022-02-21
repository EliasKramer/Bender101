using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using System.Reflection;
using System;
using Slicer2D;


[CustomEditor(typeof(Slicer2DParticlesManager))]
public class Slicer2DParticlesManagerEditor : Editor {
    static bool foldout = true;

    override public void OnInspectorGUI() {
        Slicer2DParticlesManager script = target as Slicer2DParticlesManager;
        GUISortingLayer.Draw(script.sortingLayer);

        if (GUI.changed) {
			if (EditorApplication.isPlaying == false) {

				EditorUtility.SetDirty(script);
			}
		}
    }
}


public class GUISortingLayer {
	
	static public string[] GetSortingLayerNames() {
         System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
         PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
         return (string[])sortingLayersProperty.GetValue(null, new object[0]);
     }
 
     static public int[] GetSortingLayerUniqueIDs() {
         System.Type internalEditorUtilityType = typeof(InternalEditorUtility);
         PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
         return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
     }

	 static public void Draw(SortingLayer sortingLayer) {
		bool value = GUIFoldout.Draw("Sorting Layer", sortingLayer);
        
		if (value == false) {
			return;
		}

		EditorGUI.indentLevel++;

			string[] sortingLayerNames = GetSortingLayerNames();
			int id = Array.IndexOf(sortingLayerNames, sortingLayer.Name);
			int newId = EditorGUILayout.Popup("Name", id, sortingLayerNames);

            if (newId > -1 && newId < sortingLayerNames.Length) {
                string newName = sortingLayerNames[newId];

                if (newName != sortingLayer.Name)
                {
                    sortingLayer.Name = newName;
                }

            }

			sortingLayer.Order = EditorGUILayout.IntField("Order", sortingLayer.Order);

		EditorGUI.indentLevel--;
	 }
}


public class GUIFoldout {
	static Dictionary<object, bool> dictionary = new Dictionary<object, bool>();

	static public bool GetValue(object Object) {
		bool value = false;
		bool exist = dictionary.TryGetValue(Object, out value);

		if (exist == false) {
			dictionary.Add(Object, value);
		}

		return(value);
	}

	static public void SetValue(object Object, bool value) {
		bool resultVal;
		bool exist = dictionary.TryGetValue(Object, out resultVal);

		if (exist) {
			dictionary.Remove(Object);
			dictionary.Add(Object, value);
		}
	}

	static public bool Draw(string name, object Object) {
		bool value = EditorGUILayout.Foldout(GetValue(Object), name );
        SetValue(Object, value);
		return(value);
	}
}