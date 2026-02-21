using Godot;

public class ColorData
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    public Color Color
    {
        get => new Color(R, G, B);
        set
        {
            R = value.R;
            G = value.G;
            B = value.B;
        }
    }
}
