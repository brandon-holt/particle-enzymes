using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// scriptable object called ParticleData
[CreateAssetMenu(fileName = "ParticleParameters", menuName = "Particle Parameters")]
public class ParticleParameters : ScriptableObject
{
    public float mass, drag, angularDrag; // rigidbody properties
    public float forceMagnitude; // force applied to particles
    public float centralForceFraction; // fraction of forceMagnitude pulling particles toward center
    public float worldLimitBounceForceMagnitude; // force applied to particles when they hit the world limit
    public float meanUpdateRate; // mean rate of particle update
    public float bondLength; // distance between particles
    public float meanBondDecayRate; // mean rate of bond decay

    public void InitializeParticle(Particle p)
    {
        AddRigidbody(p);

        p.rb.position = new Vector3(0, 0, 0);

        p.rb.velocity = 10f * new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }

    public void AddRigidbody(Particle p)
    {
        if (p.rb == null) p.rb = p.gameObject.AddComponent<Rigidbody>();

        p.rb.mass = mass;

        p.rb.drag = drag;

        p.rb.angularDrag = angularDrag;

        p.rb.useGravity = false;
    }

    public void RemoveRigidbody(Particle p)
    {
        Destroy(p.rb);
    }

    public void AddForce(Particle p)
    {
        Vector3 force = Vector3.zero;

        force.x = Random.Range(-1f, 1f);

        force.y = Random.Range(-1f, 1f);

        force.z = Random.Range(-1f, 1f);

        force += centralForceFraction * -p.transform.position.normalized;

        p.rb.AddForce(forceMagnitude * force, ForceMode.Impulse);

        // add torque
    }

    public void BounceBack(Particle p)
    {
        Vector3 force = worldLimitBounceForceMagnitude * -p.transform.position.normalized;

        p.rb.AddForce(force, ForceMode.Impulse);
    }

    public int GetAssemblyIndex(GameObject g, int currentAssemblyIndex = 0, int currentDepth = 0, List<string> pairs = null)
    {
        int assemblyIndex = currentAssemblyIndex;

        if (pairs == null) pairs = new List<string>();

        currentDepth++;

        for (int i = 0; i < g.transform.childCount; i++)
        {
            if (currentDepth > 0)
            {
                string pair = g.name + "-" + g.transform.GetChild(i).name;

                if (!pairs.Contains(pair))
                {
                    assemblyIndex++;

                    pairs.Add(pair);
                }
            }

            assemblyIndex = GetAssemblyIndex(g.transform.GetChild(i).gameObject, assemblyIndex, currentDepth, pairs);
        }

        return assemblyIndex;
    }

    public int GetDepth(GameObject g, int currentDepth = 0)
    {
        int depth = currentDepth;

        for (int i = 0; i < g.transform.childCount; i++)
        {
            depth = Mathf.Max(depth, GetDepth(g.transform.GetChild(i).gameObject, currentDepth + 1));
        }

        return depth;
    }

    public IEnumerator UpdateParticle(Particle p)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(.5f * meanUpdateRate, 2f * meanUpdateRate));

            if (p.rb != null) AddForce(p);
        }
    }

    public void MakeBond(Particle a, Particle b)
    {
        Random.InitState(a.type * b.type);

        float bondEnergy = Random.Range(0f, 1f);

        if (a.bond != null && bondEnergy < a.bond.energy) return;

        a.StopMovement();

        a.bond = new Bond(b, bondEnergy);

        SetRoot(a, b.root);

        RemoveRigidbody(a);

        a.transform.SetParent(b.transform);

        Random.InitState(a.type);

        float theta = Random.Range(0f, 2f * Mathf.PI);

        Random.InitState(b.type);

        float phi = Random.Range(0f, 2f * Mathf.PI);

        a.transform.rotation = Quaternion.identity;

        Vector3 localPosition = Vector3.zero;

        localPosition.x = bondLength * Mathf.Cos(theta) * Mathf.Sin(phi);

        localPosition.y = bondLength * Mathf.Sin(theta) * Mathf.Sin(phi);

        localPosition.z = bondLength * Mathf.Cos(phi);

        a.transform.localPosition = localPosition;

        a.lineRenderer.SetPositions(new Vector3[2] { Vector3.zero, b.transform.position - a.transform.position });

        b.root.assemblyIndex = GetAssemblyIndex(b.root.gameObject);

        b.root.depth = GetDepth(b.root.gameObject);

        Random.InitState(a.type * b.type);

        float bondLifetime = Random.Range(.1f * meanBondDecayRate, 10f * meanBondDecayRate);

        a.WaitForBondBreak(a, a.bond, bondLifetime);

        Random.InitState(Mathf.RoundToInt(1000f * Time.time));
    }

    public IEnumerator BreakBond(Particle a, Bond bond, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        if (a.bond != bond) yield break;

        a.transform.SetParent(a.originalParent);

        AddRigidbody(a);

        a.lineRenderer.SetPositions(new Vector3[2] { Vector3.zero, Vector3.zero });

        a.StartMovement();
    }

    private void SetRoot(Particle p, Particle root)
    {
        p.root = root;

        for (int i = 0; i < p.transform.childCount; i++)
            SetRoot(p.transform.GetChild(i).GetComponent<Particle>(), root);
    }
}