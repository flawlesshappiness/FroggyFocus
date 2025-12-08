using Godot;
using System;

public partial class BestiaryControl : Control
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

    public event Action OnBack;

    private bool entry_active;

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
                EntryBack_Pressed();
            }
            else
            {
                BackButton_Pressed();
            }
        }
    }

    private void CharacterPressed(FocusCharacterInfo info)
    {
        EntryControl.Load(info);

        AnimatedPanel_Container.AnimateShrink();
        AnimatedPanel_EntryControl.AnimatePopShow();

        entry_active = true;
    }

    private void EntryBack_Pressed()
    {
        entry_active = false;
        AnimatedPanel_EntryControl.AnimatePopHide();
        AnimatedPanel_Container.AnimateGrow();
    }
}
