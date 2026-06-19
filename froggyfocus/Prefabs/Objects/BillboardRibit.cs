using Godot;
using Godot.Collections;

public partial class BillboardRibit : Node3D
{
    [Export]
    public Array<Label3D> Labels;

    public override void _Ready()
    {
        base._Ready();
        InitializeLabels();
    }

    private void InitializeLabels()
    {
        Labels.ForEach(x => x.Hide());
        Labels.PickRandom().Show();
    }
}
