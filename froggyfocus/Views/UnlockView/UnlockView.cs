using Godot;
using System;
using System.Collections;
using System.Linq;

public partial class UnlockView : View
{
    public static UnlockView Instance => Get<UnlockView>();

    [Export]
    public UnlockPopup Popup;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "UNLOCK";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Select item",
            Action = ListItems
        });

        void ListItems(DebugView v)
        {
            v.SetContent_Search();

            var items = Enum.GetValues(typeof(ItemType)).Cast<ItemType>();
            foreach (var item in items)
            {
                v.ContentSearch.AddItem(item.ToString(), () => SelectItem(item, v));
            }

            v.ContentSearch.UpdateButtons();
        }

        void SelectItem(ItemType item, DebugView v)
        {
            v.SetContent_Search();

            v.ContentSearch.AddItem("Test unlock", () => TestUnlock(item, v));

            v.ContentSearch.UpdateButtons();
        }

        void TestUnlock(ItemType item, DebugView v)
        {
            v.Close();
            Coroutine.Start(Cr);
            IEnumerator Cr()
            {
                yield return WaitForItemUnlock(item);
            }
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        Player.SetAllLocks(nameof(UnlockView), true);
    }

    protected override void OnHide()
    {
        base.OnHide();
        Player.SetAllLocks(nameof(UnlockView), false);
    }

    public Coroutine ShowItemUnlock(ItemType item)
    {
        return this.StartCoroutine(WaitForItemUnlock(item), "unlock");
    }

    public IEnumerator WaitForItemUnlock(ItemType item)
    {
        Show();
        Popup.SetAppearanceItem(item);
        Popup.SetItemUnlock();
        yield return Popup.WaitForPopup();
        Hide();
    }
}
