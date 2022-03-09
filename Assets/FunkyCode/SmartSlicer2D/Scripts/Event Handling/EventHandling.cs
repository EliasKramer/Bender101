using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slicer2D {

	[System.Serializable]
	public class EventHandling {
		public delegate bool Slice2DEvent(Slice2D slice);
		public delegate void Slice2DResultEvent(Slice2D slice);

		public event Slice2DEvent sliceEvent;
		public event Slice2DResultEvent sliceResultEvent;

		public event Slice2DEvent anchorSliceEvent;
		public event Slice2DResultEvent anchorSliceResultEvent;

		static public event Slice2DEvent globalSliceEvent;
		static public event Slice2DResultEvent globalSliceResultEvent;

		// static public event Slice2DEvent anchorGlobalSliceEvent;
		static public event Slice2DResultEvent anchorGlobalSliceResultEvent;

		public void ClearEvents() {
			sliceEvent = null;
			sliceResultEvent = null;
			
			anchorSliceEvent = null;
			anchorSliceResultEvent = null;
		}

		//

		public bool SliceEvent(Slice2D slice) {
			if (sliceEvent != null && sliceEvent (slice) == false) {
				return(false);
			}
			return(true);
		}

		public bool AnchorSliceEvent(Slice2D slice) {
			if (anchorSliceEvent != null && anchorSliceEvent (slice) == false) {
				return(false);
			}
			return(true);
		}

		//

		static public bool GlobalSliceEvent(Slice2D slice) {
			if (globalSliceEvent != null && globalSliceEvent (slice) == false) {
				return(false);
			}
			return(true);
		}

		static public bool AnchorGlobalSliceEvent(Slice2D slice) {
			if (anchorGlobalSliceResultEvent != null) {
				return(false);
			}
			return(true);
		}

		public void Result(Slice2D slice) {
			if (sliceResultEvent != null) {
				sliceResultEvent (slice);
			}

			if (globalSliceResultEvent != null) {
				globalSliceResultEvent (slice);
			}

			if (anchorSliceResultEvent != null) {
				anchorSliceResultEvent (slice);
			}

			if (anchorGlobalSliceResultEvent != null) {
				anchorGlobalSliceResultEvent (slice);
			}
		}

		public void AnchorResult(Slice2D slice) {
			if ((anchorSliceResultEvent != null)) {
				anchorSliceResultEvent (slice);
			}

			if ((anchorGlobalSliceResultEvent != null)) {
				anchorGlobalSliceResultEvent (slice);
			}
		}
	}

}