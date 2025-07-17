using Godot;

[GlobalClass]
public partial class WindInfo : Resource
{
    [Export]
    public PackedScene EffectScene;

    [Export]
    public SoundInfo SoundInfo;

    [Export]
    public Noise Noise;
}
