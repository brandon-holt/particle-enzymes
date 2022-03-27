using UnityEngine;

public class Particle : MonoBehaviour
{
    public ParticleParameters particleParameters;
    public int assemblyIndex;
    public int depth;
    public int type;
    public bool formBonds;
    public Particle root;
    public Transform originalParent;
    public Bond bond;
    public Rigidbody rb;
    public LineRenderer lineRenderer;
    private Coroutine updateCoroutine;

    private void Awake()
    {
        root = this;

        rb = GetComponent<Rigidbody>();

        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.useWorldSpace = false;

        originalParent = transform.parent;
    }

    private void Start()
    {
        depth = particleParameters.GetDepth(gameObject);

        assemblyIndex = particleParameters.GetAssemblyIndex(gameObject);

        particleParameters.InitializeParticle(this);

        StartMovement();
    }

    public void StartMovement()
    {
        updateCoroutine = StartCoroutine(particleParameters.UpdateParticle(this));
    }

    public void StopMovement()
    {
        if (updateCoroutine != null) StopCoroutine(updateCoroutine);
    }

    public void WaitForBondBreak(Particle a, Bond bond, float lifetime)
    {
        StartCoroutine(particleParameters.BreakBond(a, bond, lifetime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Particle") && formBonds)
        {
            if (rb.velocity.sqrMagnitude < collision.rigidbody.velocity.sqrMagnitude)
            {
                ContactPoint[] contacts = new ContactPoint[collision.contactCount];

                collision.GetContacts(contacts);

                particleParameters.MakeBond(contacts[0].thisCollider.GetComponent<Particle>(),
                contacts[0].otherCollider.GetComponent<Particle>());
            }
        }
        else if (collision.gameObject.CompareTag("World"))
        {
            particleParameters.BounceBack(this);
        }
    }
}