using Godot;

public partial class OptionsView : View
{
    public override string Directory => Paths.ViewDirectory;
    public static OptionsView Instance => Get<OptionsView>();

    [Export]
    public OptionsControl OptionsControl;

    public View BackView { get; set; }

    public override void _Ready()
    {
        base._Ready();
        OptionsControl.OnBack += ClickBack;
    }

    private void ClickBack()
    {
        if (BackView == null) return;

        Hide();
        BackView.Show();
    }
}
