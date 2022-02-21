using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slicer2D {

	public class InputEvent {
		public enum EventType {None, Press, Release, Move, SetPosition};

		public EventType eventType = EventType.None;
		public Vector2 position;
		public float time;
	}
	
}