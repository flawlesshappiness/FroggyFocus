using Godot;

[GlobalClass]
public partial class WeatherInfo : Resource
{
    [Export]
    public Environment.BGMode BackgroundMode = Environment.BGMode.Sky;

    [Export]
    public Color BackgroundColor;

    [Export(PropertyHint.Range, "0,1,0.0001")]
    public float BackgroundEnergyMultiplier = 1.0f;

    [Export]
    public Color SunColor;

    [Export]
    public float SunShadowOpacity;

    [Export]
    public bool FogEnabled;

    [Export]
    public FogType FogType;

    [Export]
    public Color FogColor;

    [Export(PropertyHint.Range, "0,1,0.0001")]
    public float FogDensity;

    [Export(PropertyHint.Range, "0,1,0.0001")]
    public float FogAerialPerspective;

    [Export]
    public float Rain;

    [Export]
    public bool Thunder;

    [Export]
    public bool Wind;

    [Export]
    public Vector2 WindRange;
}

public enum FogType { Fog, Volumetric }