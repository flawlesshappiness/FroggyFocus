using Godot;
using System.Collections;

public partial class ShopView : View
{
    public static ShopView Instance => Get<ShopView>();

    [Export]
    public AnimatedPanel AnimatedPanel;

    [Export]
    public AnimatedOverlay AnimatedOverlay;

    [Export]
    public ShopContainer ShopContainer;

    [Export]
    public PurchasePopup PurchasePopup;

    [Export]
    public Control InputBlocker;

    private bool animating;
    private bool popup_active;
    private Control popup_back_focus;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();

        ShopContainer.BackButton.Pressed += BackClicked;
        PurchasePopup.OnCancel += PurchasePopup_OnCancel;

        ShopContainer.HatsContainer.OnButtonPressed += HatButton_Pressed;
        ShopContainer.ColorContainer.OnColorPressed += ColorButton_Pressed;
    }

    protected override void OnShow()
    {
        base.OnShow();

        Open();

        Player.SetAllLocks(nameof(ShopView), true);
        MouseVisibility.Show(nameof(ShopView));
    }

    protected override void OnHide()
    {
        base.OnHide();
        Player.SetAllLocks(nameof(ShopView), false);
        MouseVisibility.Hide(nameof(ShopView));
    }

    private void RegisterDebugActions()
    {
        var category = "SHOP";

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

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Unlock items",
            Action = v => UnlockItems(v)
        });

        void UnlockItems(DebugView v)
        {
            v.SetContent_Search();
            v.ContentSearch.AddItem("Colors", () => UnlockColors(v));
            v.ContentSearch.AddItem("Hats", () => UnlockHats(v));
            v.ContentSearch.UpdateButtons();
        }

        void UnlockColors(DebugView v)
        {
            v.SetContent_Search();

            var infos = AppearanceColorController.Instance.Collection.Resources;
            foreach (var info in infos)
            {
                var unlocked = Data.Game.Appearance.UnlockedColors.Contains(info.Type) ? "> " : string.Empty;
                v.ContentSearch.AddItem($"{unlocked}{info.Name}", () => ToggleColor(info.Type, v));
            }

            v.ContentSearch.UpdateButtons();
        }

        void ToggleColor(AppearanceColorType type, DebugView v)
        {
            if (Data.Game.Appearance.UnlockedColors.Contains(type))
            {
                Data.Game.Appearance.UnlockedColors.Remove(type);
            }
            else
            {
                Data.Game.Appearance.UnlockedColors.Add(type);
            }

            Data.Game.Save();
            UnlockColors(v);
        }

        void UnlockHats(DebugView v)
        {
            v.SetContent_Search();

            var infos = AppearanceHatController.Instance.Collection.Resources;
            foreach (var info in infos)
            {
                var unlocked = Data.Game.Appearance.UnlockedHats.Contains(info.Type) ? "> " : string.Empty;
                v.ContentSearch.AddItem($"{unlocked}{info.Name}", () => ToggleHat(info.Type, v));
            }

            v.ContentSearch.UpdateButtons();
        }

        void ToggleHat(AppearanceHatType type, DebugView v)
        {
            if (Data.Game.Appearance.UnlockedHats.Contains(type))
            {
                Data.Game.Appearance.UnlockedHats.Remove(type);
            }
            else
            {
                Data.Game.Appearance.UnlockedHats.Add(type);
            }

            Data.Game.Save();
            UnlockHats(v);
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree())
        {
            if (popup_active)
            {
                PurchasePopup.Cancel();
            }
            else
            {
                Close();
            }
        }
    }

    private void Open()
    {
        if (animating) return;
        animating = true;

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            AnimatedOverlay.AnimateBehindShow();
            yield return AnimatedPanel.AnimatePopShow();
            InputBlocker.Hide();

            ShopContainer.TabContainer.GetTabBar().GrabFocus();

            animating = false;
        }
    }

    private void Close()
    {
        if (animating) return;
        animating = true;

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            AnimatedOverlay.AnimateBehindHide();
            yield return AnimatedPanel.AnimatePopHide();
            InputBlocker.Hide();
            Hide();
            animating = false;
        }
    }

    private void BackClicked()
    {
        Close();
    }

    private void HatButton_Pressed(AppearanceHatInfo info)
    {
        popup_active = true;

        popup_back_focus = ShopContainer.HatsContainer.GetButton(info);
        PurchasePopup.SetHat(info);
        PurchasePopup.ShowPopup();

        PurchasePopup.OnPurchase = () =>
        {
            Data.Game.Appearance.UnlockedHats.Add(info.Type);
            ShopContainer.HatsContainer.UpdateButtons();
            ShopContainer.TabContainer.GetTabBar().GrabFocus();

            popup_active = false;

            Data.Game.Save();
        };
    }

    private void ColorButton_Pressed(AppearanceColorInfo info)
    {
        popup_active = true;

        popup_back_focus = ShopContainer.ColorContainer.GetButton(info);
        PurchasePopup.SetColor(info);
        PurchasePopup.ShowPopup();

        PurchasePopup.OnPurchase = () =>
        {
            Data.Game.Appearance.UnlockedColors.Add(info.Type);
            ShopContainer.ColorContainer.UpdateButtons();
            ShopContainer.TabContainer.GetTabBar().GrabFocus();

            popup_active = false;

            Data.Game.Save();
        };
    }

    private void PurchasePopup_OnCancel()
    {
        popup_back_focus.GrabFocus();
        popup_active = false;
    }
}
