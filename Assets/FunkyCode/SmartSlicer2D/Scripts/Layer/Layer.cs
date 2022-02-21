using System.Collections.Generic;
using UnityEngine;

namespace Slicer2D {
	[System.Serializable]

	public class Layer {
		[SerializeField]
		private LayerType layer = LayerType.All;

		[SerializeField]
		private bool[] layers = new bool[8];

		static public Layer Create() {
			return(new Layer());
		}

		public void SetLayerType(LayerType type) {
			layer = type;
		}

		public void SetLayer(int id, bool value) {
			layers [id] = value;
		}

		public void DisableLayers() {
			layers = new bool[10];
		}

		public LayerType GetLayerType() {
			return(layer);
		}

		public bool GetLayerState(int id) {
			return(layers [id]);
		}

	}
}