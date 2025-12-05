using Godot;
using System;

public partial class SaveProfilesContainer : MarginContainer
{
    [Export]
    public SaveProfileControl ProfileControl1;

    [Export]
    public SaveProfileControl ProfileControl2;

    [Export]
    public SaveProfileControl ProfileControl3;

    public event Action ProfilePressed;

    public override void _Ready()
    {
        base._Ready();
        ProfileControl1.ProfileButton.Pressed += Profile_Pressed;
        ProfileControl2.ProfileButton.Pressed += Profile_Pressed;
        ProfileControl3.ProfileButton.Pressed += Profile_Pressed;
    }

    public void LoadProfiles()
    {
        ProfileControl1.LoadData(1);
        ProfileControl2.LoadData(2);
        ProfileControl3.LoadData(3);
    }

    private void Profile_Pressed()
    {
        ProfilePressed?.Invoke();
    }
}
