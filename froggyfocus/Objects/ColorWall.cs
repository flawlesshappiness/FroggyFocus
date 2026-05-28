using Godot;
using System.Collections;

public partial class ColorWall : Node3D
{
    [Export(PropertyHint.Range, "0,2,0.1")]
    public float Delay = 0.0f;

    [Export]
    public ColorButtonType Type;

    [Export]
    public ShaderMaterial SolidMaterial;

    [Export]
    public ShaderMaterial ClearMaterial;

    [Export]
    public MeshInstance3D MeshInstance;

    [Export]
    public CollisionShape3D Collider;

    [Export]
    public AudioStreamPlayer3D SfxSolid;

    public override void _Ready()
    {
        base._Ready();
        SetSolid(false);
        ColorButtonController.Instance.OnTypeChanged += TypeChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        ColorButtonController.Instance.OnTypeChanged -= TypeChanged;
    }

    private void TypeChanged(ColorButtonType type)
    {
        AnimateSetSolid(type == Type);
    }

    private void SetSolid(bool solid)
    {
        Collider.SetDeferred("disabled", !solid);
        MeshInstance.SetSurfaceOverrideMaterial(0, solid ? SolidMaterial : ClearMaterial);
    }

    private void AnimateSetSolid(bool solid)
    {
        this.StartCoroutine(Cr, "solid");
        IEnumerator Cr()
        {
            yield return new WaitForSeconds(Delay);
            SfxSolid.Play();
            SetSolid(solid);
        }
    }
}
