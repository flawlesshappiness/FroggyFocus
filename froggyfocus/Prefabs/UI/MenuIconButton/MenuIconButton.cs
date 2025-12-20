using Godot;

public partial class MenuIconButton : ButtonScript
{
    [Export]
    public string LabelText;

    [Export]
    public Label FocusLabel;

    public override void _Ready()
    {
        base._Ready();
        FocusLabel.Text = LabelText;
        FocusLabel.Hide();
    }

    protected override void Button_FocusEnter()
    {
        base.Button_FocusEnter();
        FocusLabel.Show();
    }

    protected override void Button_FocusExit()
    {
        base.Button_FocusExit();
        FocusLabel.Hide();
    }
}
