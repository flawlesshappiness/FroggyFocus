using Godot;
using System.Collections;

public enum ColorButtonType
{
    Red, Yellow, Blue
}

public partial class ColorButton : Area3D
{
    [Export]
    public ColorButtonType Type;

    [Export]
    public ShaderMaterial RedMaterial;

    [Export]
    public ShaderMaterial YellowMaterial;

    [Export]
    public ShaderMaterial BlueMaterial;

    [Export]
    public MeshInstance3D MeshInstance;

    [Export]
    public AnimationPlayer AnimationPlayer;

    private bool is_down;

    public override void _Ready()
    {
        base._Ready();
        SetMaterial(GetMaterial(Type));

        BodyEntered += PlayerEntered;
        ColorButtonController.Instance.OnTypeChanged += TypeChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        ColorButtonController.Instance.OnTypeChanged -= TypeChanged;
    }

    private void PlayerEntered(GodotObject go)
    {
        if (is_down) return;

        AnimateDown();
    }

    private void TypeChanged(ColorButtonType type)
    {
        if (Type != type && is_down)
        {
            AnimateUp();
        }
    }

    private void SetMaterial(ShaderMaterial material)
    {
        MeshInstance.SetSurfaceOverrideMaterial(0, material);
    }

    public ShaderMaterial GetMaterial(ColorButtonType type) => type switch
    {
        ColorButtonType.Red => RedMaterial,
        ColorButtonType.Yellow => YellowMaterial,
        ColorButtonType.Blue => BlueMaterial,
        _ => RedMaterial
    };

    private void AnimateUp()
    {
        is_down = false;

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("up");
        }
    }

    private void AnimateDown()
    {
        is_down = true;

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("down");
            ColorButtonController.Instance.ChangeType(Type);
        }
    }
}
