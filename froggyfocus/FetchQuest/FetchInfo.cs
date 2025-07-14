using Godot;

[GlobalClass]
public partial class FetchInfo : Resource
{
    [Export]
    public string Id;

    [Export]
    public Vector2I CountRange;

    [Export]
    public int MoneyRewardBase;

    [Export]
    public float CooldownInSeconds;
}
