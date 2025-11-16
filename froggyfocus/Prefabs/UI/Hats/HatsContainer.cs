using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class HatsContainer : ControlScript
{
    [Export]
    public bool ShowUnpurchased;

    [Export]
    public AppearancePreviewButton HatButtonTemplate;

    [Export]
    public Label UnlockHintLabel;

    public event Action<AppearanceInfo> OnButtonPressed;

    private List<ButtonMap> maps = new();

    private class ButtonMap
    {
        public AppearancePreviewButton Button { get; set; }
        public AppearanceInfo Info { get; set; }
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeHats();
    }

    private void InitializeHats()
    {
        HatButtonTemplate.Hide();

        var infos = AppearanceController.Instance.GetInfos(ItemCategory.Hat)
            .OrderBy(x => Item.IsOwned(x.Type))
            .ThenBy(x => x.Type == ItemType.Hat_None)
            .ToList();

        foreach (var info in infos)
        {
            var button = CreateHatButton(info);
            button.SetAppearance(info);
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
        var infos = AppearanceController.Instance.GetInfos(ItemCategory.Hat);
        var has_unlocked_hats = infos.Any(x => x.Type != ItemType.Hat_None);
        UnlockHintLabel.Visible = !has_unlocked_hats && !ShowUnpurchased;

        foreach (var map in maps)
        {
            var owned = Item.IsOwned(map.Info.Type);
            var shop_info = ShopController.Instance.GetInfo(map.Info.Type);
            var show_if_unpurchased = (!owned && ShowUnpurchased) && shop_info != null;
            var show_if_owned = owned && !ShowUnpurchased;
            map.Button.Visible = show_if_unpurchased || show_if_owned;
        }
    }

    private AppearancePreviewButton CreateHatButton(AppearanceInfo info)
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

    private void HatButtonPressed(AppearanceInfo info)
    {
        OnButtonPressed?.Invoke(info);
    }

    public Button GetButton(AppearanceInfo info)
    {
        return maps.FirstOrDefault(x => x.Info == info)?.Button;
    }

    public Button GetFirstButton()
    {
        return maps.FirstOrDefault().Button;
    }
}
