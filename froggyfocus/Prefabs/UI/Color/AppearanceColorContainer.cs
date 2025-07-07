using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AppearanceColorContainer : ControlScript
{
    [Export]
    public bool ShowUnlocked;

    [Export]
    public bool ShowLocked;

    [Export]
    public AppearanceColorButton ButtonTemplate;

    public event Action<AppearanceColorInfo> OnColorPressed;

    private List<ButtonMap> maps = new();

    private class ButtonMap
    {
        public AppearanceColorButton Button { get; set; }
        public AppearanceColorInfo Info { get; set; }
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        ButtonTemplate.Hide();

        var parent = ButtonTemplate.GetParent();
        foreach (var info in AppearanceColorController.Instance.Collection.Resources)
        {
            var button = ButtonTemplate.Duplicate() as AppearanceColorButton;
            button.SetParent(parent);
            button.Show();
            button.SetColor(info);
            button.Pressed += () => ColorPressed(info);

            maps.Add(new ButtonMap
            {
                Button = button,
                Info = info
            });
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateButtons();
    }

    public void UpdateButtons()
    {
        foreach (var map in maps)
        {
            var unlocked = Data.Game.Appearance.PurchasedColors.Contains(map.Info.Type);
            map.Button.Visible = unlocked == ShowUnlocked || !unlocked == ShowLocked;
        }
    }

    private void ColorPressed(AppearanceColorInfo info)
    {
        OnColorPressed?.Invoke(info);
    }

    public Button GetButton(AppearanceColorInfo info)
    {
        return maps.FirstOrDefault(x => x.Info == info)?.Button;
    }
}
