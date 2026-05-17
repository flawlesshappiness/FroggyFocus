using Godot;
using Godot.Collections;

[GlobalClass, Tool]
public partial class GPUParticles3DParent : Node3D
{
    [Export(PropertyHint.ToolButton, "Update Particles")]
    public Callable UpdateParticlesCallable;

    [Export]
    public bool Emitting
    {
        get => _emitting;
        set
        {
            _emitting = value;
            UpdateEmitting();
        }
    }

    [Export]
    public Array<GpuParticles3D> Particles;

    private bool IsEditor => Engine.IsEditorHint();

    private bool _emitting;

    public override void _EnterTree()
    {
        base._EnterTree();

        if (IsEditor)
        {
            UpdateParticlesCallable = new Callable(this, nameof(UpdateParticles));
            UpdateParticles();
        }

        ChildOrderChanged += ChildOrder_Changed;
    }

    private void ChildOrder_Changed()
    {
        UpdateParticles();
    }

    public void UpdateParticles()
    {
        Particles = new Array<GpuParticles3D>(this.GetNodesInChildren<GpuParticles3D>().ToArray());
    }

    private void UpdateEmitting()
    {
        if (Particles == null) return;
        foreach (var ps in Particles)
        {
            if (ps == null) continue;
            ps.Emitting = Emitting;
        }
    }
}
