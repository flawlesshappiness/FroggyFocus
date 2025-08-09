using Godot;

public partial class FastTravelBoat : Area3D, IInteractable
{
    public void Interact()
    {
        FastTravelView.Instance.Show();
    }
}
