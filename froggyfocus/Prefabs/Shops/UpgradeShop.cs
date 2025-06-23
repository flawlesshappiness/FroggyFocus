using Godot;

public partial class UpgradeShop : Area3D, IInteractable
{
    public void Interact()
    {
        ShopView.Instance.Show();
    }
}
