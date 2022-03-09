using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {

	public class Settings {
		static public SettingsProfile profile = null;

		public enum InstantiationMethod {
			Default,
			Quality,
			Performance
		}

		public enum CenterOfSliceTransform {
			Default, 
			Origin,
			ColliderCenter
		};

		public enum Triangulation {
			Default,
			Advanced,
			Legacy
		}

		public enum Batching {
			Default,
			On,
			Off
		}

		public enum RenderingPipeline {
			BuiltIn,
			LightWeight
		}
			
		static public SettingsProfile GetProfile() {
			if (profile == null) {
				profile = Resources.Load("Profiles/Default") as SettingsProfile;
			}

			return(profile);
		}

		public static bool GetBatching(bool setting) {
			SettingsProfile profile = GetProfile();

			if (profile == null) {
				Debug.LogWarning("Profile Settings Are Missing");
				return(setting);
			}

			switch(profile.batching) {
				case Batching.On:
					return(true);
				case Batching.Off:
					return(false);
				default:
					return(setting);
			}
		}

		public static Sliceable2D.CenterOfSliceTransform GetCenterOfSliceTransform(Sliceable2D.CenterOfSliceTransform setting) {
			SettingsProfile profile = GetProfile();

			if (profile == null) {
				Debug.LogWarning("Profile Settings Are Missing");
				return(setting);
			}

			if (profile.centerOfSliceTransform == CenterOfSliceTransform.Default) {
				return(setting);
			} else {
				int settingID = (int)profile.centerOfSliceTransform - 1;
				return((Sliceable2D.CenterOfSliceTransform)settingID);
			}
		}

		public static PolygonTriangulator2D.Triangulation GetTriangulation(PolygonTriangulator2D.Triangulation setting) {
			SettingsProfile profile = GetProfile();

			if (profile == null) {
				Debug.LogWarning("Profile Settings Are Missing");
				return(setting);
			}

			if (profile.triangulation == Triangulation.Default) {
				return(setting);
			} else {
				int triangulationID = (int)profile.triangulation - 1;
				return((PolygonTriangulator2D.Triangulation)triangulationID);
			}
		}

		public static Sliceable2D.InstantiationMethod GetComponentsCopy(Sliceable2D.InstantiationMethod setting) {
			SettingsProfile profile = GetProfile();

			if (profile == null) {
				Debug.LogWarning("Profile Settings Are Missing");
				return(setting);
			}

			if (profile.componentsCopy == InstantiationMethod.Default) {
				return(setting);
			} else {
				int copyID = (int)profile.componentsCopy - 1;
				return((Sliceable2D.InstantiationMethod)copyID);
			}
		}

		public static float GetGarbageCollector() {
			SettingsProfile profile = GetProfile();

			if (profile == null) {
				Debug.LogWarning("Profile Settings Are Missing");
				return(-1);
			}

			if (profile.garbageCollector) {
				return(profile.garbageCollectorSize);
			} else {
				return(-1);
			}
		}

		public static RenderingPipeline GetRenderingPipeline() {
			SettingsProfile profile = GetProfile();

			if (profile == null) {
				Debug.LogWarning("Profile Settings Are Missing");
				return(RenderingPipeline.BuiltIn);
			}


			return(profile.renderingPipeline);
		}

		public static int GetExplosionSlices() {
			SettingsProfile profile = GetProfile();

			if (profile == null) {
				Debug.LogWarning("Profile Settings Are Missing");
				return(2);
			}

			return(profile.explosionPieces);
		}
	}
}