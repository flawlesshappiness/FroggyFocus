using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerMoveSoundsGroup : Node3D
{
    [Export]
    public Array<PlayerMoveSounds> MoveSounds;

    private PlayerMoveSounds current;
    private List<string> ids = new();

    public override void _Ready()
    {
        base._Ready();
        current = MoveSounds.FirstOrDefault();
    }

    public void AddId(string id)
    {
        ids.Add(id);
        IdsChanged();
    }

    public void RemoveId(string id)
    {
        ids.Remove(id);
        IdsChanged();
    }

    private void IdsChanged()
    {
        var id = ids.FirstOrDefault();
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
