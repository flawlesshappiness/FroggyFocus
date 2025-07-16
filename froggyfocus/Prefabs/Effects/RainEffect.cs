using Godot;

public partial class RainEffect : GpuParticles3D
{
    public override void _Ready()
    {
        base._Ready();
        SetIntensity(0);
        RainController.Instance.OnRainIntensityChanged += RainIntensityChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        RainController.Instance.OnRainIntensityChanged -= RainIntensityChanged;
    }

    public void SetIntensity(float t)
    {
        Emitting = t > 0;
        AmountRatio = t;
    }

    private void RainIntensityChanged(float t)
    {
        SetIntensity(t);
    }
}
