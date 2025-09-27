using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class AppearanceContainer : ControlScript
{
    [Export]
    public ItemCategory ItemCategory;

    [Export]
    public ItemType NoneType;

    [Export]
    public bool ShowUnowned;

    [Export]
    public bool ShowOwned;

    [Export]
    public AppearancePreviewButton ButtonTemplate;

    [Export]
    public Label UnlockHintLabel;

    [Export]
    public GridContainer GridContainer;

    public event Action<AppearanceInfo> OnButtonPressed;
    public event Action TopButtonFocusEntered;

    private List<ButtonMap> maps = new();

    private class ButtonMap
    {
        public AppearancePreviewButton Button { get; set; }
        public AppearanceInfo Info { get; set; }
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeAppearance();
    }

    private void InitializeAppearance()
    {
        ButtonTemplate.Hide();

        var infos = AppearanceController.Instance.GetInfos(ItemCategory)
            .OrderByDescending(x => x.Type == NoneType)
            .ToList();

        foreach (var info in infos)
        {
            var button = CreateAppearanceButton(info);
            button.SetAppearance(info);
            button.Pressed += () => AppearanceButtonPressed(info);
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateButtons();
    }

    public void UpdateButtons()
    {
        var infos = AppearanceController.Instance.GetInfos(ItemCategory);
        var owns_any_item = infos.Any(x => Item.IsOwned(x.Type) && x.Type != NoneType);
        UnlockHintLabel.Visible = !owns_any_item && !ShowUnowned;

        foreach (var map in maps)
        {
            var owned = Item.IsOwned(map.Info.Type);
            var shop_info = ShopController.Instance.GetInfo(map.Info.Type);
            var show_if_unowned = (!owned && ShowUnowned) && shop_info != null;
            var show_if_owned = owned && ShowOwned;
            map.Button.Visible = show_if_unowned || show_if_owned;
        }
    }

    private AppearancePreviewButton CreateAppearanceButton(AppearanceInfo info)
    {
        var button = ButtonTemplate.Duplicate() as AppearancePreviewButton;
        button.SetParent(ButtonTemplate.GetParent());
        button.FocusEntered += () => PreviewButton_FocusEntered(button);
        maps.Add(new ButtonMap
        {
            Button = button,
            Info = info
        });
        return button;
    }

    private void AppearanceButtonPressed(AppearanceInfo info)
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

    public List<AppearancePreviewButton> GetTopButtons()
    {
        var count = GridContainer.Columns;
        return maps.Select(map => map.Button).Take(count).ToList();
    }

    public List<AppearancePreviewButton> GetBottomButtons()
    {
        var count = GridContainer.Columns;
        var reverse_list = maps.ToList();
        reverse_list.Reverse();
        return reverse_list.Select(map => map.Button).Take(count).ToList();
    }

    private void PreviewButton_FocusEntered(AppearancePreviewButton button)
    {
        var columns = GridContainer.Columns;
        var top_buttons = maps.Select(map => map.Button).Take(columns).ToList();

        if (top_buttons.Contains(button))
        {
            TopButtonFocusEntered?.Invoke();
        }
    }
}
