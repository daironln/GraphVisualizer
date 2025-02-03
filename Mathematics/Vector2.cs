namespace GraphVisualizer.Mathematics;

public class Vector2
{
    public float X { get; set; }
    public float Y { get; set; }

    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public float Length => (float)Math.Sqrt(X * X + Y * Y);

    public Vector2 Normalize()
    {
        float length = Length;
        return length > 0 ? new Vector2(X / length, Y / length) : new Vector2(0, 0);
    }

    public static Vector2 operator *(Vector2 v, float scalar)
    {
        return new Vector2(v.X * scalar, v.Y * scalar);
    }
}