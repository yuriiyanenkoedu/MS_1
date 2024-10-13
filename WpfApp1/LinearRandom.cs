namespace WpfApp1;

public class LinearRandom:IRandom
{
    private const long k = 1664525;
    private const long m = 4294967296;
    private const long shift = 1013904223;

    private long seed;
    private long maxValue;

    public LinearRandom(long seed, long maxValue)
    {
        this.seed = seed;
        this.maxValue = maxValue;
    }
    
    public int Generate()
    {
        seed = (k * seed + shift) % m;
        return Convert.ToInt32(seed % maxValue);
    }
}