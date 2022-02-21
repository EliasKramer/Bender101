using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {
	[ExecuteInEditMode]
	public class JointLineRenderer2D : MonoBehaviour {
		public bool customColor = false;
		public Color color = Color.white;
		public float lineWidth = 1;

		private List<Utilities2D.Joint2D> joints = new List<Utilities2D.Joint2D>();

		private SmartMaterial material = null;
		private static SmartMaterial staticMaterial = null;

		private VisualMesh visualMesh = new VisualMesh();

		const float lineOffset = -0.001f;

		public SmartMaterial GetMaterial() {
			if (material == null || material.material == null) {
				material = MaterialManager.GetVertexLitCopy();
			}

			return(material);
		}

		public SmartMaterial GetStaticMaterial() {
			if (staticMaterial == null || staticMaterial.material == null)   {
				staticMaterial = MaterialManager.GetVertexLitCopy();
				staticMaterial.SetColor(Color.black);
			}
			
			return(staticMaterial);
		}

		public void Start() {
			joints = Utilities2D.Joint2D.GetJoints(gameObject);
		}

		public void Update() {
			foreach(Utilities2D.Joint2D joint in joints) {
				if (joint.gameObject == null) {
					continue;
				}
				if (joint.anchoredJoint2D == null) {
					continue;
				}
				if (joint.anchoredJoint2D.isActiveAndEnabled == false) {
					continue;
				}

				Vector2 originPoint = transform.TransformPoint (joint.anchoredJoint2D.anchor);
				Vector2 connectedPoint = Vector2.zero;
				if (joint.anchoredJoint2D.connectedBody != null) {
					switch (joint.jointType) {
						case Utilities2D.Joint2D.Type.HingeJoint2D:
							connectedPoint = joint.anchoredJoint2D.connectedBody.transform.TransformPoint (Vector2.zero);
							break;

						default:
							connectedPoint = joint.anchoredJoint2D.connectedBody.transform.TransformPoint (joint.anchoredJoint2D.connectedAnchor);
							break;
					}
				}

				Pair2 pair = new Pair2(originPoint, connectedPoint);
				Draw(pair);
			}
		}

		public void Draw(Pair2 pair) {		
			visualMesh.CreateLine(pair, new Vector3(1, 1, 1), lineWidth, transform.position.z + lineOffset);
			visualMesh.Export();

			if (customColor) {
				material.SetColor(color);
			
				visualMesh.Draw(GetMaterial().material);
			} else {
				visualMesh.Draw(GetStaticMaterial().material);
			}
		}
	}
}