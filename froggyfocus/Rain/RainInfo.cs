using Godot;

[GlobalClass]
public partial class RainInfo : Resource
{
    [Export]
    public RainType Type;

    [Export]
    public SoundInfo SoundInfo;
}
