using Godot;
using System;

public partial class FastTravelButton : ImageButton
{
    [Export]
    public PriceControl PriceControl;

    public LocationInfo LocationInfo { get; private set; }
    public bool IsLocked { get; private set; }

    public event Action PressedWhenLocked;
    public event Action PressedWhenUnlocked;

    public override void _Ready()
    {
        base._Ready();
        Pressed += FastTravelButtonPressed;
    }

    public void SetLocation(LocationInfo info)
    {
        LocationInfo = info;
        TextureRect.Texture = info.PreviewImage;
        PriceControl.SetPrice(info.Price);
    }

    public void UpdateLocked()
    {
        var data = Location.GetOrCreateData(LocationInfo.Id);
        SetLocked(!data.Unlocked);
    }

    public void SetLocked(bool locked)
    {
        IsLocked = locked;

        var c_locked = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        var c_unlocked = Colors.White;
        TextureRect.Modulate = locked ? c_locked : c_unlocked;

        PriceControl.Visible = locked;
    }

    private void FastTravelButtonPressed()
    {
        if (IsLocked)
        {
            PressedWhenLocked?.Invoke();
        }
        else
        {
            PressedWhenUnlocked?.Invoke();
        }
    }
}
