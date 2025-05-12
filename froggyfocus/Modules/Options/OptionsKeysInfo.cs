using Godot;
using Godot.Collections;

[GlobalClass]
public partial class OptionsKeysInfo : Resource
{
    private static OptionsKeysInfo _instance;
    public static OptionsKeysInfo Instance => _instance ?? (_instance = GD.Load<OptionsKeysInfo>($"{Paths.Modules}/Options/Resources/{nameof(OptionsKeysInfo)}.tres"));

    [Export]
    public Array<string> Actions;
}
