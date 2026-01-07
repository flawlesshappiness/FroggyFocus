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
        var types = new List<UpgradeType>
        {
            UpgradeType.CursorRadius,
            UpgradeType.CursorSpeed,
            UpgradeType.CursorTickAmount,
            UpgradeType.InventorySize,
            UpgradeType.ShieldMax,
        };

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
        first.UpgradeButton.FocusEntered += ScrollContainer.ScrollVerticalToTop;

        var last = upgrade_controls.Last();
        last.UpgradeButton.FocusEntered += ScrollContainer.ScrollVerticalToBottom;

        UpgradeControlTemplate.Hide();
    }

    public Button GetFirstButton()
    {
        return upgrade_controls.FirstOrDefault()?.UpgradeButton;
    }
}
