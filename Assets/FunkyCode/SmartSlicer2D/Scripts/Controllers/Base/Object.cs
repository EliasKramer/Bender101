using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slicer2D.Controller {

	public class Base {
		public InputController input = new InputController();
		public ControllerEventHandling eventHandler = new ControllerEventHandling();
		public Visuals visuals = new Visuals();
		public Layer sliceLayer = Layer.Create();

		public void SetController(GameObject gameObject, InputController inputController, Visuals visualsSettings, Layer layerObject, ControllerEventHandling eventHandling) {
			input = inputController;
			visuals = visualsSettings;
			sliceLayer = layerObject;
			eventHandler = eventHandling;
			visuals.SetGameObject(gameObject);
		}
	}

}