using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

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

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();

        ShopContainer.BackButton.Pressed += BackClicked;

        ShopContainer.HatsContainer.OnButtonPressed += HatButton_Pressed;
        ShopContainer.ColorContainer.OnColorPressed += ColorButton_Pressed;
    }

    protected override void OnShow()
    {
        base.OnShow();

        Open();

        Player.SetAllLocks(nameof(ShopView), true);
        PauseView.ToggleLock.SetLock(nameof(ShopView), true);
        MouseVisibility.Show(nameof(ShopView));
    }

    protected override void OnHide()
    {
        base.OnHide();
        Player.SetAllLocks(nameof(ShopView), false);
        PauseView.ToggleLock.SetLock(nameof(ShopView), false);
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
            Text = "Items",
            Action = v => UnlockItems(v)
        });

        void UnlockItems(DebugView v)
        {
            v.SetContent_Search();
            v.ContentSearch.AddItem("Colors", () => SelectColor(v));
            v.ContentSearch.AddItem("Hats", () => SelectHat(v));
            v.ContentSearch.UpdateButtons();
        }

        void SelectColor(DebugView v)
        {
            v.SetContent_Search();

            var infos = AppearanceColorController.Instance.Collection.Resources;
            foreach (var info in infos)
            {
                var unlocked = Data.Game.Appearance.PurchasedColors.Contains(info.Type) ? "> " : string.Empty;
                v.ContentSearch.AddItem($"{unlocked}{info.Name}", () => ColorActions(info, v));
            }

            v.ContentSearch.UpdateButtons();
        }

        void ColorActions(AppearanceColorInfo info, DebugView v)
        {
            v.SetContent_Search();
            AddListToggleButtons(v, Data.Game.Appearance.PurchasedColors, info.Type, "purchased", () => ColorActions(info, v));

            if (info.Locked)
            {
                AddListToggleButtons(v, Data.Game.Appearance.UnlockedColors, info.Type, "unlocked", () => ColorActions(info, v));
            }

            v.ContentSearch.AddItem("back", () => SelectColor(v));
            v.ContentSearch.UpdateButtons();
        }

        void SelectHat(DebugView v)
        {
            v.SetContent_Search();

            var infos = AppearanceHatController.Instance.Collection.Resources;
            foreach (var info in infos)
            {
                v.ContentSearch.AddItem($"{info.Name}", () => HatActions(info, v));
            }

            v.ContentSearch.UpdateButtons();
        }

        void HatActions(AppearanceHatInfo info, DebugView v)
        {
            v.SetContent_Search();
            AddListToggleButtons(v, Data.Game.Appearance.PurchasedHats, info.Type, "purchased", () => HatActions(info, v));

            if (info.Locked)
            {
                AddListToggleButtons(v, Data.Game.Appearance.UnlockedHats, info.Type, "unlocked", () => HatActions(info, v));
            }

            v.ContentSearch.AddItem("back", () => SelectHat(v));
            v.ContentSearch.UpdateButtons();
        }

        void AddListToggleButtons<T>(DebugView v, List<T> list, T value, string text, Action pressed)
        {
            if (list.Contains(value))
            {
                v.ContentSearch.AddItem(text, () =>
                {
                    list.Remove(value);
                    Data.Game.Save();
                    pressed?.Invoke();
                });
            }
            else
            {
                v.ContentSearch.AddItem($"not {text}", () =>
                {
                    list.Add(value);
                    Data.Game.Save();
                    pressed?.Invoke();
                });
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree())
        {
            if (PurchasePopup.Active)
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
        var focus_button = ShopContainer.HatsContainer.GetButton(info);
        PurchasePopup.SetHat(info);

        this.StartCoroutine(Cr, "popup");
        IEnumerator Cr()
        {
            yield return PurchasePopup.WaitForPopup();

            if (PurchasePopup.Purchased)
            {
                AppearanceHatController.Instance.Purchase(info.Type);
                ShopContainer.HatsContainer.UpdateButtons();
                Data.Game.Save();

                ShopContainer.TabContainer.GetTabBar().GrabFocus();
            }
            else if (PurchasePopup.Cancelled)
            {
                ShopContainer.HatsContainer.GetButton(info).GrabFocus();
            }
        }
    }

    private void ColorButton_Pressed(AppearanceColorInfo info)
    {
        var focus_button = ShopContainer.ColorContainer.GetButton(info);
        PurchasePopup.SetColor(info);

        this.StartCoroutine(Cr, "popup");
        IEnumerator Cr()
        {
            yield return PurchasePopup.WaitForPopup();

            if (PurchasePopup.Purchased)
            {
                AppearanceColorController.Instance.Purchase(info.Type);
                ShopContainer.ColorContainer.UpdateButtons();
                Data.Game.Save();

                ShopContainer.TabContainer.GetTabBar().GrabFocus();
            }
            else if (PurchasePopup.Cancelled)
            {
                ShopContainer.ColorContainer.GetButton(info).GrabFocus();
            }
        }
    }
}
