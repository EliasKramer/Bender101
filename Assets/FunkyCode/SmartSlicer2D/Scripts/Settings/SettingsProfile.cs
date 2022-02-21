using UnityEngine;

namespace Slicer2D {
	[CreateAssetMenu(fileName = "Data", menuName = "Slicer2D/Settings Profile", order = 1)]

	public class SettingsProfile : ScriptableObject {
		public bool garbageCollector = true;
		public float garbageCollectorSize = 0.005f;

		public int explosionPieces = 15;

		public Settings.Batching batching = Settings.Batching.Default;
		
		public Settings.Triangulation triangulation = Settings.Triangulation.Default;
		
		public Settings.InstantiationMethod componentsCopy = Settings.InstantiationMethod.Default;

		public Settings.RenderingPipeline renderingPipeline = Settings.RenderingPipeline.BuiltIn;

		public Settings.CenterOfSliceTransform centerOfSliceTransform = Settings.CenterOfSliceTransform.Default;
		
	}
}