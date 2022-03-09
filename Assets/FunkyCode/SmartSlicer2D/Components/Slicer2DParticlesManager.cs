using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities2D;

namespace Slicer2D {
	public class Slicer2DParticlesManager : MonoBehaviour {
		static public Slicer2DParticlesManager instance;

        ParticleSystem particleSystem2D;
        public ParticleSystem.Particle[] particleArray = new ParticleSystem.Particle[500];

        public static Material material = null;

        public SortingLayer sortingLayer = new SortingLayer();
        
		static public Material GetMaterial() {
			if (material == null) {
				material = MaterialManager.GetAdditiveCopy().material;
				material.mainTexture = Resources.Load<Texture>("Sprites/Flare");
			}
			return(material);
		}

        public ParticleSystem GetParticleSystem() {
            if (particleSystem2D == null) {
                particleSystem2D = instance.gameObject.AddComponent<ParticleSystem>();

                ParticleSystem.EmissionModule emission = particleSystem2D.emission;
                emission.enabled = false;

                ParticleSystemRenderer renderer = GetComponent<ParticleSystemRenderer>();

                renderer.material = GetMaterial();

                renderer.sortingOrder = sortingLayer.Order;
                renderer.sortingLayerName = sortingLayer.Name;
            }

            return(particleSystem2D);
        }

		static public void Instantiate() {
			if (instance != null) {
				return;
			}

			GameObject manager = new GameObject();
			manager.name = "Slicer2D Particles";

			instance = manager.AddComponent<Slicer2DParticlesManager>();
		}

		void Start () {
			if (instance == null) {
				instance = this;
			} else if (instance != this) {
				Debug.LogWarning("Slicer2D: Multiple Particle Managers Detected!");

				Destroy(this);
			}
		}

		void Update() {
            GetParticleSystem();

            int particlesAlive = particleSystem2D.GetParticles (particleArray);

            for (int p = 0; p < particlesAlive; p++) {
                ParticleSystem.Particle particle = particleArray [p];

                if (particle.remainingLifetime < 0.01f || particle.GetCurrentSize(particleSystem2D) < 0.01f) {
                    continue;
                }

                particle.size *= (1f - 5f * Time.deltaTime);
    
                particleArray[p] = particle;
            }

            particleSystem2D.SetParticles(particleArray, particlesAlive);

        }
	}
}


[System.Serializable]
public class SortingLayer {
    public string Name;
    public int Order;

    public void ApplyToMeshRenderer(MeshRenderer meshRenderer) {
        if (meshRenderer == null) {
            return;
        }
        
        if (meshRenderer.sortingLayerName != Name) {
            meshRenderer.sortingLayerName = Name;
        }

        if (meshRenderer.sortingOrder != Order) {
            meshRenderer.sortingOrder = Order;
        }
    }
}