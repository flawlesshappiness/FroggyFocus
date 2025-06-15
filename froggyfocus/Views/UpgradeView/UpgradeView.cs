using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class UpgradeView : View
{
    public static UpgradeView Instance => Get<UpgradeView>();

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Button BackButton;

    [Export]
    public UpgradeControl UpgradeControlTemplate;

    [Export]
    public AudioStreamPlayer SfxPurchaseSuccess;

    [Export]
    public AudioStreamPlayer SfxPurchaseFail;

    [Export]
    public Control InputBlocker;

    private bool animating;
    private List<UpgradeControl> upgrade_controls = new();

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();

        BackButton.Pressed += BackClicked;
    }

    protected override void OnShow()
    {
        base.OnShow();

        ClearUpgradeControls();
        CreateUpgradeControls();

        Open();

        Player.SetAllLocks(nameof(UpgradeView), true);
        MouseVisibility.Show(nameof(UpgradeView));
    }

    protected override void OnHide()
    {
        base.OnHide();
        Player.SetAllLocks(nameof(UpgradeView), false);
        MouseVisibility.Hide(nameof(UpgradeView));
    }

    private void RegisterDebugActions()
    {
        var category = nameof(UpgradeView);

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Show",
            Action = v => { v.Close(); Show(); }
        });

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Hide",
            Action = v => { v.Close(); Close(); }
        });
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree())
        {
            Close();
        }
    }

    private void Open()
    {
        if (animating) return;
        animating = true;

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("show");
            InputBlocker.Hide();

            upgrade_controls.First().UpgradeButton.GrabFocus();
            animating = false;
        }
    }

    private void Close()
    {
        if (animating) return;
        animating = true;

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
            InputBlocker.Hide();
            Hide();
            animating = false;
        }
    }

    private void ClearUpgradeControls()
    {
        foreach (var control in upgrade_controls)
        {
            control.QueueFree();
        }

        upgrade_controls.Clear();
    }

    private void CreateUpgradeControls()
    {
        var parent = UpgradeControlTemplate.GetParent();
        var types = System.Enum.GetValues(typeof(StatsType)).Cast<StatsType>();
        foreach (var type in types)
        {
            var control = UpgradeControlTemplate.Duplicate() as UpgradeControl;
            control.SetParent(parent);
            control.Show();
            control.Initialize(type);
            control.Update();

            upgrade_controls.Add(control);

            control.UpgradeButton.Pressed += () =>
            {
                if (animating) return;

                if (UpgradeController.Instance.TryPurchaseUpgrade(type))
                {
                    // Purchase success
                    SfxPurchaseSuccess.Play();
                }
                else
                {
                    // Purchase fail
                    SfxPurchaseFail.Play();
                }

                control.Update();
            };
        }

        UpgradeControlTemplate.Hide();
    }

    private void BackClicked()
    {
        Close();
    }
}
