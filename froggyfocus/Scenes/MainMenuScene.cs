using Godot;
using System.Collections.Generic;

public partial class MainMenuScene : GameScene
{
    [Export]
    public Camera3D Camera;

    private List<FrogCharacter> frogs = new();

    public override void _Ready()
    {
        base._Ready();
        Camera.Current = true;

        InitializeFrogs();

        GameProfileController.Instance.OnGameProfileSelected += ProfileSelected;
        MainMenuView.Instance.Show();
    }

    protected override void Initialize()
    {
        base.Initialize();
        MainMenuView.Instance.Overlay.AnimateFrontHide();

        var master = AudioBus.Get(AudioBusNames.Master);
        master.SetMuted(false);
    }

    private void InitializeFrogs()
    {
        frogs = this.GetNodesInChildren<FrogCharacter>();
    }

    private void LoadFrogs()
    {
        frogs.ForEach(x => x.LoadAppearance());
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GameProfileController.Instance.OnGameProfileSelected -= ProfileSelected;
    }

    private void ProfileSelected(int profile)
    {
        LoadFrogs();
    }
}
