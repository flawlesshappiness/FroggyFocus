using Godot;

public partial class CollectNotificationContainer : MarginContainer
{
    [Export]
    public CollectNotificationControl NotificationTemplate;

    [Export]
    public AudioStreamPlayer SfxCollect;

    public override void _Ready()
    {
        base._Ready();
        NotificationTemplate.Hide();

        InventoryController.Instance.OnCharacterAdded += InventoryCharacter_Added;
    }

    private void InventoryCharacter_Added(InventoryCharacterData data)
    {
        var info = FocusCharacterController.Instance.GetInfoFromPath(data.InfoPath);
        if (info == null) return;

        CreateNotification(info.Name);

        if (Data.Options.CollectBugSoundEnabled)
        {
            SfxCollect.Play();
        }
    }

    private CollectNotificationControl CreateNotification(string name)
    {
        var node = NotificationTemplate.Duplicate() as CollectNotificationControl;
        node.SetParent(NotificationTemplate.GetParent());
        node.Label.Text = name;
        node.Show();
        node.AnimateShow();
        return node;
    }
}
