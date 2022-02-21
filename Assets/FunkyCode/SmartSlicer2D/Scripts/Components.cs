using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {
	
	public class Components {

		static public void CopyRigidbody2D(Rigidbody2D originalRigidBody, Sliceable2D slicer, Polygon2D id, double originArea) {
			if (originalRigidBody) {
				Rigidbody2D newRigidBody = slicer.GetRigibody();
				
				newRigidBody.position = originalRigidBody.position;
				newRigidBody.isKinematic = originalRigidBody.isKinematic;
				newRigidBody.velocity = originalRigidBody.velocity;
				newRigidBody.drag = originalRigidBody.drag;
				newRigidBody.angularVelocity = originalRigidBody.angularVelocity;
				newRigidBody.angularDrag = originalRigidBody.angularDrag;
				newRigidBody.constraints = originalRigidBody.constraints;
				newRigidBody.gravityScale = originalRigidBody.gravityScale;
				newRigidBody.collisionDetectionMode = originalRigidBody.collisionDetectionMode;
				newRigidBody.sharedMaterial = originalRigidBody.sharedMaterial;
				//newRigidBody.sleepMode = originalRigidBody.sleepMode;
				//newRigidBody.inertia = originalRigidBody.inertia;

				// Center of Mass : Auto / Center
				//if (slicer.centerOfMass == Slicer2D.CenterOfMass.RigidbodyOnly) {
				///	newRigidBody.centerOfMass = Vector2.zero;
				//}
				
				if (slicer.recalculateMass) {
					float newArea = (float)id.ToLocalSpace(slicer.transform).GetArea ();
					newRigidBody.mass = originalRigidBody.mass * (float) (newArea / originArea);
				} else {
					newRigidBody.mass = originalRigidBody.mass;
				}
			}
		}

		static public void Copy(Sliceable2D slicer, GameObject gObject) {
			Component[] scriptList = slicer.gameObject.GetComponents<Component>();	
			Component script;

			System.Reflection.FieldInfo[] fields;
			System.Reflection.FieldInfo field;

			System.Type objectType;
			string objectString;

			for(int i = 0; i < scriptList.Length; i++) {
				script = scriptList[i];

				if (script == null) {
					continue;
				}

				objectType = script.GetType();
				objectString = objectType.ToString();

				// Do not copy Colliders
				switch(objectString) {
					case "UnityEngine.Transform":

					case "UnityEngine.PolygonCollider2D":
					case "UnityEngine.EdgeCollider2D":
					case "UnityEngine.BoxCollider2D":
					case "UnityEngine.CircleCollider2D":
					case "UnityEngine.CapsuleCollider2D":
					continue;
				}

				switch (slicer.textureType) {
					case Sliceable2D.TextureType.SpriteAnimation:
						if (objectString == "UnityEngine.SpriteRenderer" || objectString == "UnityEngine.Animator") {
							continue;
						}
						break;
					
					case Sliceable2D.TextureType.Sprite:
					case Sliceable2D.TextureType.Sprite3D:
						if (objectString == "UnityEngine.SpriteRenderer") {
							continue;
						}
						break;

					default:
						break;
				}

			
				switch(objectString) {
					case "UnityEngine.SpringJoint2D":
					SpringJoint2D orgingSpringJoint2D = (SpringJoint2D)script;
					SpringJoint2D springJoint2D = gObject.AddComponent<SpringJoint2D>();

					springJoint2D.connectedBody = orgingSpringJoint2D.connectedBody;
					continue;
				}

				gObject.AddComponent(objectType);

				fields = objectType.GetFields();

				for(int x = 0; x < fields.Length; x++) {
					field = fields[x];
		
					field.SetValue(gObject.GetComponent(objectType), field.GetValue(script));
				}
			}

			Behaviour[] children = gObject.GetComponentsInChildren<Behaviour>();
			Behaviour[] slicerChildren = slicer.GetComponentsInChildren<Behaviour>();
			System.Type componentType;

			foreach (Behaviour childCompnent in children) {
				componentType = childCompnent.GetType();

				foreach (Behaviour child in slicerChildren) {
					
					if (child.GetType() == componentType) {
						childCompnent.enabled = child.enabled;
						break;
					}
				}
			}
		}
	}
}