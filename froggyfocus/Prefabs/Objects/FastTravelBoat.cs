using Godot;
using System.Collections.Generic;

public partial class FastTravelBoat : Area3D, IInteractable
{
    public override void _Ready()
    {
        base._Ready();
        RaceController.Instance.OnCountdownStarted += Race_Start;
        RaceController.Instance.OnRaceEnd += Race_End;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        RaceController.Instance.OnCountdownStarted -= Race_Start;
        RaceController.Instance.OnRaceEnd -= Race_End;
    }

    public void Interact()
    {
        if (HasAnyLocationUnlocked())
        {
            FastTravelView.Instance.Show();
        }
        else
        {
            DialogueController.Instance.StartDialogue("##LOCATION_NONE##");
        }
    }

    private bool HasAnyLocationUnlocked()
    {
        var location_ids = new List<string> { "cave", "factory" };
        foreach (var id in location_ids)
        {
            var data = Location.GetOrCreateData(id);
            if (data.Unlocked) return true;
        }

        return false;
    }

    private void Race_Start()
    {
        this.Disable();
    }

    private void Race_End(RaceResult result)
    {
        this.Enable();
    }
}
