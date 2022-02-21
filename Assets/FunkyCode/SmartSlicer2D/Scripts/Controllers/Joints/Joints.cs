using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D.Controller {

	public class Joints {

		static public void LinearSliceJoints(Pair2D slice) {
			foreach(Utilities2D.Joint2D joint in Utilities2D.Joint2D.GetJointsConnected()) {
				Vector2 localPosA = joint.anchoredJoint2D.connectedAnchor;
				Vector2 worldPosA = joint.anchoredJoint2D.connectedBody.transform.TransformPoint(localPosA);
				Vector2 localPosB = joint.anchoredJoint2D.anchor;
				Vector2 worldPosB = joint.anchoredJoint2D.transform.TransformPoint(localPosB);

				switch (joint.jointType) {
					case Utilities2D.Joint2D.Type.HingeJoint2D:
						worldPosA = joint.anchoredJoint2D.connectedBody.transform.position;
						break;
					default:
						break;
				}
				
				Pair2D jointLine = new Pair2D(worldPosA, worldPosB);

				if (Math2D.LineIntersectLine(slice, jointLine)) {
					UnityEngine.Object.Destroy(joint.anchoredJoint2D);
				}
			}
		}

		static public void ComplexSliceJoints(List<Vector2D> slice) {
			foreach(Utilities2D.Joint2D joint in Utilities2D.Joint2D.GetJointsConnected()) {
				Vector2 localPosA = joint.anchoredJoint2D.connectedAnchor;
				Vector2 worldPosA = joint.anchoredJoint2D.connectedBody.transform.TransformPoint(localPosA);
				Vector2 localPosB = joint.anchoredJoint2D.anchor;
				Vector2 worldPosB = joint.anchoredJoint2D.transform.TransformPoint(localPosB);

				switch (joint.jointType) {
					case Utilities2D.Joint2D.Type.HingeJoint2D:
						worldPosA = joint.anchoredJoint2D.connectedBody.transform.position;
						break;
					default:
						break;
				}

				Pair2D jointLine = new Pair2D(worldPosA, worldPosB);

				foreach(Pair2D pair in Pair2D.GetList(slice, false)) {
					if (Math2D.LineIntersectLine(pair, jointLine)) {
						UnityEngine.Object.Destroy(joint.anchoredJoint2D);
					}
				}
			}	
		}
	}

}