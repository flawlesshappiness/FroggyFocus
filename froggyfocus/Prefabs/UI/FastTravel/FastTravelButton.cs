using Godot;
using System;
using System.Linq;

public partial class FastTravelButton : ImageButton
{
    [Export]
    public Label NameLabel;

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
        NameLabel.Text = info.Name;
    }

    public void UpdateVisible()
    {
        var scene_name = System.IO.Path.GetFileNameWithoutExtension(Data.Game.CurrentScene);
        var current_location = LocationController.Instance.Collection.Resources.FirstOrDefault(x => x.Scene == scene_name);

        var not_current_scene = LocationInfo != current_location;
        var data = Location.GetOrCreateData(LocationInfo.Id);
        Visible = data.Unlocked && not_current_scene;
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
