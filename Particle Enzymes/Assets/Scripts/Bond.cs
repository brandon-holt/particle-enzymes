public class Bond
{
    public float energy;
    public Particle parent;

    public Bond(Particle parent, float energy)
    {
        this.parent = parent;
        this.energy = energy;
    }
}