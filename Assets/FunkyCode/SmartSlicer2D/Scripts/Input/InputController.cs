using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Utilities2D;

namespace Slicer2D {

	[System.Serializable]
	public class InputController {
		public Input[] input = new Input[10];

		public bool multiTouch = true;

		public static float zPosition = 0;

		private static bool useTouch = false;

		public void AddCompletedEvents(Input.InputCompleted controllerEvent, int id = 0) {
			input[id].controllerEvents += controllerEvent;
		}

		public bool GetVisualsEnabled(int id = 0) {
			return(input[id].visualsEnabled);
		}

		public void SetVisualsState(bool state, int id = 0) {
			input[id].visualsEnabled = state;
		}

		public bool GetSlicingEnabled(int id = 0) {
			return(input[id].slicingEnabled);
		}

		public void SetSlicingState(bool state, int id = 0) {
			input[id].slicingEnabled = state;
		}
		
		public InputController() {
			multiTouch = true;

			for(int id = 0; id < 10; id++) {
				input[id] = new Input();
			}
		}

		///// Get Functions /////
		public Vector2 GetInputPosition(int id = 0) {
			return(input[id].position);
		}

		public bool GetInputClicked(int id = 0) {
			return(input[id].clicked);
		}

		public bool GetInputPressed(int id = 0) {
			return(input[id].pressed);
		}

		public bool GetInputHolding(int id = 0) {
			return(input[id].holding);
		}

		public bool GetInputReleased(int id = 0) {
			return(input[id].released);
		}

		public void ClearActions(int id = 0) {
			input[id].eventsBank.Clear();
		}

		public bool Playing(int id = 0) {
			return(input[id].playing);
		}

		///// Event Actions /////
		public void SetMouse(Vector2 position, float time, int id = 0) {
			InputEvent e = new InputEvent();
			e.eventType = InputEvent.EventType.SetPosition;
			e.position = position;
			e.time = time;

			input[id].eventsBank.Add(e);
		}

		public void MoveMouse(Vector2 position, float time, int id = 0) {
			InputEvent e = new InputEvent();
			e.eventType = InputEvent.EventType.Move;
			e.position = position;
			e.time = time;

			input[id].eventsBank.Add(e);
		}

		public void PressMouse(float time, int id = 0) {
			InputEvent e = new InputEvent();
			e.eventType = InputEvent.EventType.Press;
			e.time = time;

			input[id].eventsBank.Add(e);
		}

		public void ReleaseMouse(float time, int id = 0) {
			InputEvent e = new InputEvent();
			e.eventType = InputEvent.EventType.Release;
			e.time = time;

			input[id].eventsBank.Add(e);
		}

		public void OnGUI() {
			//GUI.Label(new Rect(0, Screen.height - 30, 100, 100), "Touch Count: " + Input.touchCount);
			//for(int i = 0; i < Input.touchCount; i++) {
			//	Touch touch = Input.GetTouch(i);
			//	GUI.Label(new Rect(0, Screen.height - 60 - i * 30, 300, 100), "Pos: " + touch.position + " " + touch.phase);
			//}

			//for(int i = 0; i < 5; i++) {
			//	GUI.Label(new Rect(0, Screen.height - 60 - i * 30, 300, 100), "Pos: " + GetInputPosition(i).ToVector2());
			//}
		}

		///// Default Input Update /////
		public void Update() {
			if (multiTouch) {
				if (UnityEngine.Input.touchCount > 0) {
					useTouch = true;
				}
			}

			int inputCount = 10;

			if (multiTouch == false) {
				inputCount = 1;
			}
			
			for(int id = 0; id < inputCount; id++) {
				input[id].clicked = false;
				input[id].holding = input[id].pressed;

				if (input[id].rawInput) {
					int touchCount = UnityEngine.Input.touchCount;
					
					if (id < touchCount) {
						
						bool down = input[id].pressed;
						Touch touch = UnityEngine.Input.GetTouch(id);
						input[id].clicked = (down == false);
						input[id].pressed = (touch.phase == TouchPhase.Moved) || (touch.phase == TouchPhase.Stationary);

						if (input[id].pressed) {
							input[id].position = GetTouchPosition(touch.position);
						}

						if (down && input[id].pressed == false) {
							input[id].released = true;
						} else {
							input[id].released = false;
						}
						
					} else {
						if (useTouch == false) {
							switch(id) {
								case 0:
									bool down = input[id].pressed;
									bool getMouseButton = UnityEngine.Input.GetMouseButton(0);

									if (getMouseButton == true) {
										if (down == false) {
											input[id].clicked = true;
										}
									}
									
									//input[id].clicked = Input.GetMouseButtonDown(0);

									input[id].position = GetMousePosition();

								
									input[id].pressed = getMouseButton;
									
									
									if (down && input[id].pressed == false) {
										input[id].released = true;
									} else {
										input[id].released = false;
									}
									
									break;
							}
						} else {
							input[id].released = false;
							input[id].pressed = false;
						}
					}
					
				} else {
					// For all 10 inputs
					Update_AI(id);
				}
			}
		}

		public static Vector2 GetMousePosition() {
			Vector3 pos = UnityEngine.Input.mousePosition;
			
			pos.z = -Camera.main.transform.position.z + zPosition;
			return(Camera.main.ScreenToWorldPoint ( pos ));
		}

		public static Vector2 GetTouchPosition(Vector2 touch) {
			Vector3 pos = touch;
			pos.z = -Camera.main.transform.position.z + zPosition;
			return(Camera.main.ScreenToWorldPoint (pos));
		}

		public void Update_AI(int id = 0) {
			Input inp = input[id];
			if (inp.playing == false) {
				return;
			}

			inp.released = false;

			if (inp.currentEvent == null) {
				if (inp.eventsPlaying.Count > 0) {
					InputEvent e = inp.eventsPlaying.First();

					switch(e.eventType) {
						case InputEvent.EventType.SetPosition:
							inp.position = e.position;
							break;

						case InputEvent.EventType.Move:
							break;

						case InputEvent.EventType.Press:
							inp.pressed = true;
							inp.clicked = true;
							break;

						case InputEvent.EventType.Release:
							inp.pressed = false;
							inp.released = true;
							break;
					}

					inp.timer = TimerHelper.Create();

					inp.currentEvent = e;

					inp.eventsPlaying.Remove(e);
				}
			} else {
				switch(inp.currentEvent.eventType) {
					case InputEvent.EventType.Move:
						inp.position.x = Mathf.Lerp((float)inp.position.x, inp.currentEvent.position.x, inp.timer.Get() / inp.currentEvent.time);
						inp.position.y = Mathf.Lerp((float)inp.position.y, inp.currentEvent.position.y, inp.timer.Get() / inp.currentEvent.time);
						break;
					}

				if (inp.timer.Get() > inp.currentEvent.time) {
					inp.currentEvent = null;

					if (inp.eventsPlaying.Count < 1) {
						inp.EventsCompleted();
					}
				}
			}

			if (inp.eventsPlaying.Count < 1) {
				if (inp.currentEvent == null) {
					if (inp.loop) {
						Play();
					}

					inp.playing = false;
				}
			}
		}

		public void Play(int id = 0) {
			Input inp = input[id];
			inp.pressed = false;
			inp.clicked = false;
			inp.position = Vector2.zero;

			inp.playing = true;

			inp.eventsPlaying = new List<InputEvent>(inp.eventsBank);
		}

		public void Stop(int id = 0) {
			input[id].playing = false;
		}

		public void Resume(int id = 0) {
			input[id].playing = true;
		}

		public void SetLoop(bool l, int id = 0) {
			input[id].loop = l;
		}

		public void SetRawInput(bool inp, int id = 0) {
			input[id].rawInput = inp;
		}
	}
}