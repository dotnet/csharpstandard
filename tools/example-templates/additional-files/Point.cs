public class Point
{
    public int x;
    public int y;
    public int X { get { return x; } set { x = value;} }
    public int Y { get { return y; } set { y = value;} }
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
