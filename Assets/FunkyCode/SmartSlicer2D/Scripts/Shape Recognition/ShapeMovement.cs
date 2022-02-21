using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slicer2D {
		
	public class ShapeMovement {
		public bool update = true;

		public Vector3 updatePosition = Vector3.zero;
		public float updateRotation = 0f;

		public void ForceUpdate() {
			update = true;
		}

		public void Update(Transform transform) {
			if (updatePosition != transform.position) {
				updatePosition = transform.position;

				update = true;
			}

			if (updateRotation != transform.rotation.eulerAngles.z) {
				updateRotation = transform.rotation.eulerAngles.z;

				update = true;
			}
		}
	}
}