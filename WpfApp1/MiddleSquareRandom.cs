namespace WpfApp1;

public class MiddleSquareRandom:IRandom
{
    private int seed;
    private int maxValue;

    public MiddleSquareRandom(int seed, int maxValue)
    {
        this.seed = seed;
        this.maxValue = maxValue;
    }

    public int Generate()
    {
        int square = seed * seed;
        string temp = square.ToString("D8");
        string middleDigits = temp.Substring(2, 4);
        seed = int.Parse(middleDigits);
        return seed%maxValue;
    }
}