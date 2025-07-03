using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class UpgradeContainer : ControlScript
{
    [Export]
    public UpgradeControl UpgradeControlTemplate;

    [Export]
    public ScrollContainer ScrollContainer;

    [Export]
    public AudioStreamPlayer SfxPurchaseSuccess;

    [Export]
    public AudioStreamPlayer SfxPurchaseFail;

    private List<UpgradeControl> upgrade_controls = new();

    protected override void OnShow()
    {
        base.OnShow();
        InitializeUpgrades();
    }

    private void InitializeUpgrades()
    {
        ClearUpgradeControls();
        CreateUpgradeControls();
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
        var types = System.Enum.GetValues(typeof(UpgradeType)).Cast<UpgradeType>();
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

        var first = upgrade_controls.First();
        first.UpgradeButton.FocusEntered += ScrollToTop;

        var last = upgrade_controls.Last();
        last.UpgradeButton.FocusEntered += ScrollToBottom;

        UpgradeControlTemplate.Hide();
    }

    public Button GetFirstButton()
    {
        return upgrade_controls.FirstOrDefault()?.UpgradeButton;
    }

    private void ScrollToTop()
    {
        ScrollContainer.ScrollVertical = 0;
    }

    private void ScrollToBottom()
    {
        ScrollContainer.ScrollVertical = 9999;
    }
}
