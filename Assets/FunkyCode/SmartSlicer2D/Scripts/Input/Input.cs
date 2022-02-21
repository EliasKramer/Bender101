using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {

	public class Input {
		public bool visualsEnabled = true;
		public bool slicingEnabled = true;
		
		public bool released = false;
		public bool pressed = false;
		public bool holding = false;
		public bool clicked = false;
		public Vector2 position = Vector2.zero;

		public bool playing = false;
		public bool loop = false;

		public bool rawInput = true;

		public List<InputEvent> eventsPlaying = new List<InputEvent>();
		public List<InputEvent> eventsBank = new List<InputEvent>();
		public InputEvent currentEvent;
			
		public TimerHelper timer = null;
		
		// Event Handling
		public delegate void InputCompleted();
		public event InputCompleted controllerEvents;

		public void EventsCompleted() {
			if (controllerEvents != null) {
				controllerEvents();
			}
		}
	}
	
}