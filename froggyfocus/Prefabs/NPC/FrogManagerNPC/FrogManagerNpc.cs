using Godot;

public partial class FrogManagerNpc : Area3D, IInteractable
{
    [Export]
    public FrogCharacter FrogCharacter;

    public override void _Ready()
    {
        base._Ready();
        FrogCharacter.HatGroup.SetHat(AppearanceHatType.TopHat);
    }

    public void Interact()
    {
    }
}
