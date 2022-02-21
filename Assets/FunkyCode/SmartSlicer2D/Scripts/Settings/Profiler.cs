using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slicer2D {
	
	public class Profiler {
		static int counter_AdvancedTriangulation = 0;
		static int counter_LegacyTriangulation = 0;
		static int counter_BatchingApplied = 0;

		static int counter_ObjectsCreated = 0;
		static int counter_SlicesCreatedWithPerformance = 0;
		static int counter_SlicesCreatedWithQuality = 0;

		public static void IncAdvancedTriangulation() {
			counter_AdvancedTriangulation ++;
		}

		public static void IncLegacyTriangulation() {
			counter_LegacyTriangulation ++;
		}

		public static void IncBatchingApplied() {
			counter_BatchingApplied ++;
		}

		public static void IncObjectsCreated() {
			counter_ObjectsCreated ++;
		}

		public static void IncSlicesCreatedWithPerformance() {
			counter_SlicesCreatedWithPerformance ++;
		}

		public static void IncSlicesCreatedWithQuality() {
			counter_SlicesCreatedWithQuality ++;
		}

		public static int GetAdvancedTriangulation() {
			return(counter_AdvancedTriangulation);
		}

		public static int GetLegacyTriangulation() {
			return(counter_LegacyTriangulation);
		}

		public static int GetBatchingApplied() {
			return(counter_BatchingApplied);
		}

		public static int GetObjectsCreated() {
			return(counter_ObjectsCreated);
		}

		public static int GetSlicesCreatedWithPeroformance() {
			return(counter_SlicesCreatedWithPerformance);
		}

		public static int GetSlicesCreatedWithQuality() {
			return(counter_SlicesCreatedWithQuality);
		}

	}
}