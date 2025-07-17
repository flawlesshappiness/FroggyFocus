using Godot;

public partial class WindEffect : GpuParticles3D
{
    private ParticleProcessMaterial mat;

    public override void _Ready()
    {
        base._Ready();
        mat = ProcessMaterial.Duplicate() as ParticleProcessMaterial;
        ProcessMaterial = mat;

        SetIntensity(0);

        WindController.Instance.OnWindIntensityChanged += WindIntensityChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        WindController.Instance.OnWindIntensityChanged -= WindIntensityChanged;
    }

    private void WindIntensityChanged(float t)
    {
        SetIntensity(t);
    }

    public void SetIntensity(float t)
    {
        Emitting = t > 0;

        AmountRatio = t;

        var velocity = Mathf.Lerp(20f, 60f, t);
        mat.InitialVelocityMin = velocity;
        mat.InitialVelocityMax = velocity;

        var position = Mathf.Lerp(20f, 60f, t);
        mat.EmissionShapeOffset = new Vector3(-position, 0, 0);
    }
}
