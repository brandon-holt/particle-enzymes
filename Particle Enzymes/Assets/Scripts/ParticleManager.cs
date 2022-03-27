using System.Collections;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public RuntimeSetParticle particleSet;

    private void Start()
    {
        StartCoroutine(InstantiateParticles());
    }

    private IEnumerator InstantiateParticles()
    {
        particleSet.GenerateMaterials();

        particleSet.Clear();

        while (particleSet.items.Count < particleSet.numberParticles)
        {
            for (int i = 0; i < particleSet.particlesPerFrame; i++)
            {
                Particle p = Instantiate(particleSet.particlePrefab, transform).GetComponent<Particle>();

                particleSet.Add(p);

                p.type = particleSet.items.Count % particleSet.numberParticleTypes;

                p.gameObject.name = "P" + p.type;

                p.GetComponent<MeshRenderer>().material = particleSet.particleMaterials[p.type];

                p.GetComponent<LineRenderer>().material = particleSet.particleMaterials[p.type];

                if (particleSet.items.Count >= particleSet.numberParticles) break;
            }

            yield return null;
        }

        yield return new WaitForSeconds(10f);

        foreach (Particle p in particleSet.items) p.formBonds = true;
    }
}