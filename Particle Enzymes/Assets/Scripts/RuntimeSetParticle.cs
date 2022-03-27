using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenuAttribute(fileName = "RuntimeSetParticle", menuName = "Runtime Set Particle")]
public class RuntimeSetParticle : RuntimeSet<Particle>
{
    public GameObject particlePrefab;
    public int particlesPerFrame, numberParticles, numberParticleTypes;
    public List<Material> particleMaterials;

    public void GenerateMaterials()
    {
        particleMaterials = new List<Material>();

        for (int i = 0; i < numberParticleTypes; i++)
        {
            Material m = new Material(particlePrefab.GetComponent<MeshRenderer>().sharedMaterial.shader);

            m.color = new Color(Random.Range(.5f, 1f), Random.Range(.5f, 1f), Random.Range(.5f, 1f));

            particleMaterials.Add(m);
        }
    }
}