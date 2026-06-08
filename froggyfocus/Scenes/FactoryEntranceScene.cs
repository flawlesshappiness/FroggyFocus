using Godot;
using Godot.Collections;

public partial class FactoryEntranceScene : GameScene
{
    [Export]
    public AudioStreamPlayer AmbVents;

    [Export]
    public AudioStreamPlayer AmbRoom;

    [Export]
    public Array<Node3D> VentsWorld;

    [Export]
    public Array<Node3D> LabWorld;

    [Export]
    public Array<VentDoor> VentDoorEnter;

    [Export]
    public Array<VentDoor> VentDoorExit;

    public override void _Ready()
    {
        base._Ready();
        VentDoorEnter.ForEach(x => x.OnTransition += EnterVent);
        VentDoorExit.ForEach(x => x.OnTransition += ExitVent);

        AmbRoom.FadeIn(2f, 1f);
        SetVentsVisible(false);
    }

    private void EnterVent()
    {
        AmbVents.FadeIn(2f, 1f);
        AmbRoom.FadeOut(1f);
        SetVentsVisible(true);
    }

    private void ExitVent()
    {
        AmbVents.FadeOut(1f);
        AmbRoom.FadeIn(2f, 1f);
        SetVentsVisible(false);
    }

    private void SetVentsVisible(bool visible)
    {
        VentsWorld.ForEach(x => x.Visible = visible);
        LabWorld.ForEach(x => x.Visible = !visible);
    }
}
