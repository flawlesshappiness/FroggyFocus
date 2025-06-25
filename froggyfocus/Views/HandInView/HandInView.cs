using Godot;
using Godot.Collections;
using System.Collections;
using System.Linq;

public partial class HandInView : View
{
    public static HandInView Instance => Get<HandInView>();

    [Export]
    public AnimationPlayer AnimationPlayer_HandIn;

    [Export]
    public AnimationPlayer AnimationPlayer_Inventory;

    [Export]
    public Button CloseHandInButton;

    [Export]
    public Button CloseInventoryButton;

    [Export]
    public InventoryContainer InventoryContainer;

    [Export]
    public Array<InventoryPreviewButton> RequestButtons;

    [Export]
    public Array<InventoryPreviewButton> RewardButtons;

    private InventoryPreviewButton selected_request_button;

    public override void _Ready()
    {
        base._Ready();

        InventoryContainer.OnButtonPressed += InventoryButton_Pressed;
        CloseHandInButton.Pressed += CloseHandInButton_Pressed;
        CloseInventoryButton.Pressed += CloseInventoryButton_Pressed;

        foreach (var button in RequestButtons)
        {
            button.Pressed += () => RequestButton_Pressed(button);
        }

        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = nameof(HandInView);

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Show",
            Action = v => { v.Close(); Show(); }
        });
    }

    protected override void OnShow()
    {
        base.OnShow();
        SetLocks(true);

        foreach (var button in RequestButtons)
        {
            button.Clear();
        }

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return AnimationPlayer_HandIn.PlayAndWaitForAnimation("show");
            RequestButtons.First().GrabFocus();
        }
    }

    protected override void OnHide()
    {
        base.OnHide();
        SetLocks(false);
    }

    private void SetLocks(bool locked)
    {
        Player.SetAllLocks(nameof(HandInView), locked);
        MouseVisibility.Instance.Lock.SetLock(nameof(HandInView), locked);
    }

    private void RequestButton_Pressed(InventoryPreviewButton button)
    {
        selected_request_button = button;
        InventoryContainer.UpdateButtons();

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            ReleaseFocus();
            AnimationPlayer_HandIn.PlayAndWaitForAnimation("shrink");
            yield return AnimationPlayer_Inventory.PlayAndWaitForAnimation("show");

            var button = InventoryContainer.GetFirstButton() ?? CloseInventoryButton;
            button?.GrabFocus();
        }
    }

    private void InventoryButton_Pressed(FocusCharacterInfo info)
    {
        selected_request_button.SetCharacter(info);
        CloseInventoryButton_Pressed();
    }

    private void CloseHandInButton_Pressed()
    {
        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return AnimationPlayer_HandIn.PlayAndWaitForAnimation("hide");
            Hide();
        }
    }

    private void CloseInventoryButton_Pressed()
    {
        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            ReleaseFocus();
            AnimationPlayer_HandIn.Play("grow");
            yield return AnimationPlayer_Inventory.PlayAndWaitForAnimation("hide");
            selected_request_button?.GrabFocus();
        }
    }
}
