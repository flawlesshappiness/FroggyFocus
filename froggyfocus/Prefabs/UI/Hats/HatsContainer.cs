using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class HatsContainer : ControlScript
{
    [Export]
    public bool ShowLocked;

    [Export]
    public bool ShowUnlocked;

    [Export]
    public bool ObscureLocked;

    [Export]
    public bool ShowEmpty;

    [Export]
    public AppearancePreviewButton HatButtonTemplate;

    [Export]
    public Label UnlockHintLabel;

    public event Action<AppearanceHatInfo> OnButtonPressed;

    private List<ButtonMap> maps = new();

    private class ButtonMap
    {
        public AppearancePreviewButton Button { get; set; }
        public AppearanceHatInfo Info { get; set; }
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeHats();
    }

    private void InitializeHats()
    {
        HatButtonTemplate.Hide();

        var infos = AppearanceHatController.Instance.Collection.Resources.ToList()
            .OrderBy(IsHatUnlocked)
            .ToList();

        var empty_button = CreateHatButton(null);
        empty_button.Pressed += () => HatButtonPressed(null);

        foreach (var info in AppearanceHatController.Instance.Collection.Resources)
        {
            var button = CreateHatButton(info);
            button.SetPrefab(info.Prefab);
            button.Pressed += () => HatButtonPressed(info);
        }

        UpdateButtons();
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateButtons();
    }

    public void UpdateButtons()
    {
        var infos = AppearanceHatController.Instance.Collection.Resources.ToList();

        var has_unlocked_hats = infos.Any(IsHatUnlocked);
        UnlockHintLabel.Visible = !has_unlocked_hats && !ShowLocked;

        foreach (var map in maps)
        {
            if (map.Info == null)
            {
                map.Button.Visible = has_unlocked_hats && ShowEmpty;
            }
            else
            {
                var locked = !IsHatUnlocked(map.Info);
                map.Button.SetLocked(locked && ObscureLocked);
                map.Button.Visible = locked == ShowLocked || !locked == ShowUnlocked;
            }
        }
    }

    private bool IsHatUnlocked(AppearanceHatInfo info)
    {
        return Data.Game.Appearance.UnlockedHats.Contains(info.Type);
    }

    private AppearancePreviewButton CreateHatButton(AppearanceHatInfo info)
    {
        var button = HatButtonTemplate.Duplicate() as AppearancePreviewButton;
        button.SetParent(HatButtonTemplate.GetParent());
        button.Show();
        maps.Add(new ButtonMap
        {
            Button = button,
            Info = info
        });
        return button;
    }

    private void HatButtonPressed(AppearanceHatInfo info)
    {
        OnButtonPressed?.Invoke(info);
    }

    public Button GetButton(AppearanceHatInfo info)
    {
        return maps.FirstOrDefault(x => x.Info == info)?.Button;
    }
}
