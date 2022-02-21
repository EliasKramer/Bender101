using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {
	public class Slicer2DSound : MonoBehaviour {
		public AudioClip clip;

		static public TimerHelper timer = null;

		void Start () {
			Sliceable2D slicer = GetComponent<Sliceable2D>();
			slicer.AddResultEvent(SlicerEvent);

			if (timer == null) {
				timer = TimerHelper.Create();
			}
		}

		void SlicerEvent (Slice2D slice) {
			if (timer.GetMillisecs() < 15) {
				return;
			}

			if (clip == null) {
				return;
			}

			timer.Reset();

			GameObject sound = new GameObject();
			sound.name = "Audio Clip '" + clip.name + "'";

			sound.transform.parent = Slicer2DSoundManager.Get().transform;

			AudioSource audio = sound.AddComponent<AudioSource>();
			audio.clip = clip;
			audio.enabled = false;
			audio.enabled = true;
		}
	}
}