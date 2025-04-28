using Godot;

public partial class DebugInputString : ControlScript
{
    [Export]
    public Label Label;

    [Export]
    public LineEdit Text;

    public string Id { get; set; }
}
