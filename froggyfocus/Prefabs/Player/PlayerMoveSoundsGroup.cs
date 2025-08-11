using Godot;
using Godot.Collections;
using System.Linq;

public partial class PlayerMoveSoundsGroup : Node3D
{
    [Export]
    public Array<PlayerMoveSounds> MoveSounds;

    private PlayerMoveSounds current;

    public override void _Ready()
    {
        base._Ready();
        current = MoveSounds.FirstOrDefault();
    }

    public void SetSounds(string id)
    {
        current = MoveSounds.FirstOrDefault(x => x.Id == id) ?? MoveSounds.FirstOrDefault();
    }

    public void PlayMove()
    {
        current.PlayMove();
    }

    public void PlayJump()
    {
        current.PlayJump();
    }

    public void PlayLand()
    {
        current.PlayLand();
    }
}
