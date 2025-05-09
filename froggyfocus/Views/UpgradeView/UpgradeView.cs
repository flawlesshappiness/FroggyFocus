using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class UpgradeView : View
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Button BackButton;

    [Export]
    public UpgradeControl UpgradeControlTemplate;

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
            yield return AnimationPlayer.PlayAndWaitForAnimation("show");
        }
    }

    private void Close()
    {
        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
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
            control.SetUpgrade(type);

            upgrade_controls.Add(control);
        }

        UpgradeControlTemplate.Hide();
    }

    private void BackClicked()
    {
        Close();
    }
}
