using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {

	public class Slicer2DParticles : MonoBehaviour {
		public string sortingLayer = "";

		float posZ = 0;

		void Start () {
			Sliceable2D slicer = GetComponent<Sliceable2D>();
			if (slicer != null) {
				slicer.AddResultEvent(SliceEvent);
			}

			posZ = transform.position.z - 0.1f;
		}

		void Emit(Vector3 startPosition) {
			startPosition.z = -5;
			Slicer2DParticlesManager manager = Slicer2DParticlesManager.instance;
			ParticleSystem particleSystem = manager.GetParticleSystem();
				
			ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
			emitParams.startColor = Color.white;
			emitParams.startSize = 4;
			emitParams.position = startPosition;
			emitParams.startLifetime = 2;

			float direction = Random.Range(0, 360) * Mathf.Deg2Rad;
			float speed = 2;
			emitParams.rotation = direction* Mathf.Rad2Deg;
			emitParams.velocity = new Vector2(Mathf.Cos(direction) * speed, Mathf.Sin(direction) * speed);
			//emitParams.startLifetime = particle.timer;

			particleSystem.Emit(emitParams, 1);
			particleSystem.Play();
		}
		
		void SliceEvent(Slice2D slice) {
			Slicer2DParticlesManager.Instantiate();

			foreach(List<Vector2D> pointList in slice.slices) {
				foreach(Pair2D p in Pair2D.GetList(pointList)) {
					Vector3 startPosition = new Vector3((float)p.A.x, (float)p.A.y, posZ);

					Emit(startPosition);
					Emit(startPosition);

					Vector3 pos = p.A.ToVector2();
					pos.z = posZ;
					while (Vector2.Distance(pos, p.B.ToVector2()) > 0.5f) {
						pos = Vector2.MoveTowards(pos, p.B.ToVector2(), 0.35f);
						
						Emit(pos);

						//Particle2D particle = Particle2D.Create(Random.Range(0, 360), pos);
						// Slicer2DParticlesManager.particlesList.Add(particle);
					}
				}
			} 
		}
	}

}