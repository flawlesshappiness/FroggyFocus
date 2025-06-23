using Godot;

public partial class UpgradeLevelNode : Control
{
    [Export]
    public Control Active;

    [Export]
    public Control Inactive;

    public void SetActive(bool active)
    {
        Active.Visible = active;
        Inactive.Visible = !active;
    }
}
