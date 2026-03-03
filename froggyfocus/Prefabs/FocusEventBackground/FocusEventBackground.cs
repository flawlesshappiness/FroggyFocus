using Godot;
using Godot.Collections;

public partial class FocusEventBackground : Node3D
{
    [Export]
    public string Id;

    [Export]
    public Array<AudioStreamPlayer> SoundEffects;

    public override void _Ready()
    {
        base._Ready();
        VisibilityChanged += _VisibilityChanged;
    }

    private void _VisibilityChanged()
    {
        var visible = IsVisibleInTree();
        SoundEffects.ForEach(x => x.Playing = visible);
    }
}
