using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slicer2DSoundManager : MonoBehaviour {

	static public Slicer2DSoundManager instance;

	static public Slicer2DSoundManager Get() {
		if (instance != null) {
			return(instance);
		}

		foreach(Slicer2DSoundManager manager in Object.FindObjectsOfType(typeof(Slicer2DSoundManager))) {
			instance = manager;
			return(instance);
		}

		// Create New Light Manager
		GameObject gameObject = new GameObject();
		gameObject.name = "Slicer2D Sound Manager";

		instance = gameObject.AddComponent<Slicer2DSoundManager>();

		return(instance);
	}
}
