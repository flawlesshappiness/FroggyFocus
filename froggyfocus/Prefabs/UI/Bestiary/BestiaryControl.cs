using Godot;
using System;
using System.Collections;

public partial class BestiaryControl : ControlScript
{
    [Export]
    public Button BackButton;

    [Export]
    public BestiaryContainer Container;

    [Export]
    public BestiaryEntryControl EntryControl;

    [Export]
    public AnimatedPanel AnimatedPanel_Container;

    [Export]
    public AnimatedPanel AnimatedPanel_EntryControl;

    [Export]
    public Control InputBlocker;

    public event Action OnBack;

    private bool entry_active;
    private FocusCharacterInfo selection;

    public override void _Ready()
    {
        base._Ready();
        BackButton.Pressed += BackButton_Pressed;
        Container.CharacterPressed += CharacterPressed;
        EntryControl.BackPressed += EntryBack_Pressed;

        AnimatedPanel_Container.AnimateGrow();
    }

    private void BackButton_Pressed()
    {
        OnBack?.Invoke();
    }

    public Control GetFocusControl()
    {
        return Container.GetFirstButton();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (!IsVisibleInTree()) return;

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree())
        {
            if (entry_active)
            {
                HideEntry();
            }
            else
            {
                BackButton_Pressed();
            }
        }
    }

    private void CharacterPressed(FocusCharacterInfo info)
    {
        selection = info;
        EntryControl.Load(info);
        ShowEntry();
    }

    private void EntryBack_Pressed()
    {
        HideEntry();
    }

    private void ShowEntry()
    {
        entry_active = true;

        this.StartCoroutine(Cr, "entry");
        IEnumerator Cr()
        {
            ReleaseCurrentFocus();
            InputBlocker.Show();
            AnimatedPanel_Container.AnimateShrink();
            yield return AnimatedPanel_EntryControl.AnimatePopShow();
            InputBlocker.Hide();

            EntryControl.BackButton.GrabFocus();
        }
    }

    private void HideEntry()
    {
        this.StartCoroutine(Cr, "entry");
        IEnumerator Cr()
        {
            ReleaseCurrentFocus();
            InputBlocker.Show();
            AnimatedPanel_Container.AnimateGrow();
            yield return AnimatedPanel_EntryControl.AnimatePopHide();
            InputBlocker.Hide();

            var focus_control = Container.GetButton(selection) ?? BackButton;
            focus_control.GrabFocus();
            entry_active = false;
        }
    }
}
