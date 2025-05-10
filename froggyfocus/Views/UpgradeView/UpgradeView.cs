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

    private List<Control> upgrade_controls = new();

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
    }

    protected override void OnHide()
    {
        base.OnHide();
        Player.SetAllLocks(nameof(UpgradeView), false);
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

    private void Open()
    {
        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("show");
            InputBlocker.Hide();
        }
    }

    private void Close()
    {
        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
            InputBlocker.Hide();
            Hide();
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
