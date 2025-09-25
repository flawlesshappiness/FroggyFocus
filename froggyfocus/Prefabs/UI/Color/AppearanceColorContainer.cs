using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AppearanceColorContainer : ControlScript
{
    [Export]
    public bool ShowOwned;

    [Export]
    public bool ShowUnowned;

    [Export]
    public bool ShowDefault;

    [Export]
    public AppearanceColorButton ButtonTemplate;

    public event Action<AppearanceColorInfo> OnColorPressed;
    public event Action OnDefaultColorPressed;

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

        foreach (var info in AppearanceColorController.Instance.Collection.Resources)
        {
            var button = CreateButton(info);
            button.SetColor(info);
            button.Pressed += () => ColorPressed(info);
        }
    }

    private AppearanceColorButton CreateButton(AppearanceColorInfo info)
    {
        var button = ButtonTemplate.Duplicate() as AppearanceColorButton;
        button.SetParent(ButtonTemplate.GetParent());
        button.Show();
        maps.Add(new ButtonMap
        {
            Button = button,
            Info = info
        });
        return button;
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
            var owned = Item.IsOwned(map.Info.Type);
            map.Button.Visible = owned == ShowOwned || !owned == ShowUnowned;
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

    public Button GetFirstButton()
    {
        return maps.FirstOrDefault().Button;
    }
}
