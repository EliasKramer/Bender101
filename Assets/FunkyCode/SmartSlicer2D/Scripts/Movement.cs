using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slicer2D {

	public class Movement {
		public bool update = true;

		public Vector3 updatePosition = Vector3.zero;
		public float updateRotation = 0f;

		public Sprite sprite;
		public bool spriteflipX = false;
		public bool spriteflipY = false;

		public void ForceUpdate() {
			update = true;
		}

		public void Update(Sliceable2D source) {
			Transform transform = source.transform;
			
			if (updatePosition != transform.position) {
				updatePosition = transform.position;

				update = true;
			}

			if (updateRotation != transform.rotation.eulerAngles.z) {
				updateRotation = transform.rotation.eulerAngles.z;

				update = true;
			}

			if (source.spriteRenderer != null) {	
				if (sprite != source.spriteRenderer.sprite) {
					sprite = source.spriteRenderer.sprite;

					update = true;
				}

				if (spriteflipX != source.spriteRenderer.flipX) {
					spriteflipX = source.spriteRenderer.flipX;

					update = true;
				}

				if (spriteflipY != source.spriteRenderer.flipY) {
					spriteflipY = source.spriteRenderer.flipY;

					update = true;
				}
			}
		}
	}
}