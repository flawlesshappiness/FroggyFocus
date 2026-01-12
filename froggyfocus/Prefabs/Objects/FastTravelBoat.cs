using Godot;
using System.Collections.Generic;

public partial class FastTravelBoat : Area3D, IInteractable
{
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
}
