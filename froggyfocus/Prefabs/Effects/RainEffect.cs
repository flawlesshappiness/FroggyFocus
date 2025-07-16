using Godot;

public partial class RainEffect : GpuParticles3D
{
    public override void _Ready()
    {
        base._Ready();
        SetIntensity(0);
    }

    public void SetIntensity(float t)
    {
        Emitting = t > 0;
        AmountRatio = t;
    }
}
