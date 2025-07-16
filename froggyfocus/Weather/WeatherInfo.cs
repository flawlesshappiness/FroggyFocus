using Godot;

[GlobalClass]
public partial class WeatherInfo : Resource
{
    [Export]
    public Color SunColor;

    [Export]
    public float SunShadowOpacity;

    [Export]
    public bool FogEnabled;

    [Export]
    public Color FogColor;

    [Export(PropertyHint.Range, "0,1,0.0001")]
    public float FogDensity;

    [Export]
    public float Rain;
}
