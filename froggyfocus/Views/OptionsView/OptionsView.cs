using Godot;

public partial class OptionsView : View
{
    public static OptionsView Instance => Get<OptionsView>();

    [Export]
    public OptionsControl OptionsControl;
}
