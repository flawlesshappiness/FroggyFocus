using Godot;

public partial class AppearanceAttachment : Node3D
{
    [Export]
    public ItemType Type;

    public AppearanceInfo Info { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        InitializeInfo();
    }

    private void InitializeInfo()
    {
        Info = AppearanceController.Instance.GetInfo(Type);

        if (Info == null)
        {
            Debug.LogError($"{Name}: Failed to get AppearanceInfo for {Type}");
        }
    }

    public virtual void SetPrimaryColor(Color color)
    {
    }

    public virtual void SetSecondaryColor(Color color)
    {
    }

    public void SetDefaultColors()
    {
        SetPrimaryColor(Info.DefaultPrimaryColor);
        SetSecondaryColor(Info.DefaultSecondaryColor);
    }
}
