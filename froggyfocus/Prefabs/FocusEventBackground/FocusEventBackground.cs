using Godot;
using Godot.Collections;

public partial class FocusEventBackground : Node3D
{
    [Export]
    public string Id;

    [Export]
    public Array<string> Aliases;

    [Export]
    public Array<AudioStreamPlayer> SoundEffects;

    public override void _Ready()
    {
        base._Ready();
        SetSoundEnabled(false);
        VisibilityChanged += _VisibilityChanged;
    }

    private void _VisibilityChanged()
    {
        SetSoundEnabled(IsVisibleInTree());
    }

    private void SetSoundEnabled(bool enabled)
    {
        SoundEffects.ForEach(x => x.Playing = enabled);
    }
}
