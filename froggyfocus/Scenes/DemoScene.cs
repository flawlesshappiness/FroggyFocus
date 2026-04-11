using Godot;
using System.Collections;
using System.Collections.Generic;

public partial class DemoScene : GameScene
{
    [Export]
    public bool PlayIntroWhenInternal;

    [Export]
    public AnimatedPathFollow3D IntroCameraPath;

    public override void _Ready()
    {
        base._Ready();
        InitializeItems();
        InitializeUpgrades();

        if (ApplicationInfo.Instance.Type != ApplicationType.Internal || PlayIntroWhenInternal)
        {
            AnimateIntroCamera();
        }
    }

    private void InitializeItems()
    {
        var items = new List<ItemType>()
        {
            ItemType.BodyTop_Gradient,
            ItemType.BodyTop_GradientLong,
            ItemType.BodyTop_GradientShort,
            ItemType.BodyPattern_Desert,
            ItemType.BodyEye_Cute,
            ItemType.BodyEye_Derp,
        };

        foreach (var type in items)
        {
            var item = Item.GetOrCreateData(type);
            item.Owned = true;
        }
    }

    private void InitializeUpgrades()
    {
        var speed = UpgradeController.Instance.GetOrCreateData(UpgradeType.CursorSpeed);
        speed.Level = 0;

        var tick = UpgradeController.Instance.GetOrCreateData(UpgradeType.TickAmount);
        tick.Level = 0;

        var time = UpgradeController.Instance.GetOrCreateData(UpgradeType.FocusTime);
        time.Level = 2;
    }

    private Coroutine AnimateIntroCamera()
    {
        var id = "intro";
        return this.StartCoroutine(Cr, "intro_camera");
        IEnumerator Cr()
        {
            Player.SetAllLocks(id, true);

            IntroCameraPath.Camera.Current = true;
            yield return IntroCameraPath.Animate();

            TransitionView.Instance.StartTransition(new TransitionSettings
            {
                Type = TransitionType.Color,
                Color = Colors.Black,
                Duration = 2f,
                OnTransition = OnTransition
            });
        }

        void OnTransition()
        {
            Player.SetAllLocks(id, false);
            Player.Instance.SetCameraTarget();
        }
    }
}
