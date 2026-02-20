using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class AppearanceColorControl : VBoxContainer
{
    [Export]
    public ItemCategory ItemCategory;

    [Export]
    public ItemType NoneType;

    [Export]
    public Button PrimaryButton;

    [Export]
    public Button SecondaryButton;

    [Export]
    public Control ButtonContainer;

    [Export]
    public AppearanceContainer ColorContainer;

    [Export]
    public Array<Control> OtherControls;

    private bool is_primary;

    public event Action<AppearanceInfo> OnPrimarySelected;
    public event Action<AppearanceInfo> OnSecondarySelected;
    public event Action<AppearanceInfo> OnColorSelected;

    public override void _Ready()
    {
        base._Ready();
        PrimaryButton.Pressed += Primary_Pressed;
        SecondaryButton.Pressed += Secondary_Pressed;
        ColorContainer.OnButtonPressed += Color_Pressed;
        VisibilityChanged += OnVisibilityChanged;
    }

    private void OnVisibilityChanged()
    {
        if (!IsVisibleInTree()) return;
        SetColorsVisible(false);
    }

    private void Primary_Pressed()
    {
        is_primary = true;
        SetColorsVisible(true);
        ColorContainer.GetFirstButton().GrabFocus();
    }

    private void Secondary_Pressed()
    {
        is_primary = false;
        SetColorsVisible(true);
        ColorContainer.GetFirstButton().GrabFocus();
    }

    private void Color_Pressed(AppearanceInfo info)
    {
        var data = Data.Game.FrogAppearanceData.GetOrCreateAttachmentData(ItemCategory);
        //data.PrimaryColor = is_primary ? info.Type : data.PrimaryColor;
        //data.SecondaryColor = !is_primary ? info.Type : data.SecondaryColor;
        SetColorsVisible(false);

        OnColorSelected?.Invoke(info);
        if (is_primary)
        {
            PrimaryButton.GrabFocus();
            OnPrimarySelected?.Invoke(info);
        }
        else if (!is_primary)
        {
            SecondaryButton.GrabFocus();
            OnSecondarySelected?.Invoke(info);
        }
    }

    private void SetColorsVisible(bool visible)
    {
        var owns_color = ItemController.Instance.GetInfos(ItemCategory.Color).Any(x => x.Type != ItemType.Color_Default && Item.IsOwned(x.Type));
        var owns_item = ItemController.Instance.GetInfos(ItemCategory).Any(x => x.Type != NoneType && Item.IsOwned(x.Type));
        ButtonContainer.Visible = !visible && owns_color && owns_item;
        PrimaryButton.Visible = !visible;
        SecondaryButton.Visible = !visible;
        OtherControls.ForEach(x => x.Visible = !visible);
        ColorContainer.Visible = visible;
    }

    public void SetSecondaryEnabled(bool enabled)
    {
        SecondaryButton.Disabled = !enabled;
    }
}
