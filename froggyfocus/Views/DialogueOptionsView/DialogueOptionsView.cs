using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class DialogueOptionsView : View
{
    [Export]
    public AnimatedOverlay Overlay;

    [Export]
    public Control InputBlocker;

    [Export]
    public Array<AnimatedPanel> AnimatedPanels;

    [Export]
    public Array<Button> OptionButtons;

    public static DialogueOptionsView Instance => Get<DialogueOptionsView>();

    private bool HasSettings => current_settings != null;

    private int selected_index;
    private Settings current_settings;
    private List<ButtonMap> maps = new();

    private class ButtonMap
    {
        public int Index { get; set; }
        public AnimatedPanel Panel { get; set; }
        public Button Button { get; set; }
    }

    public class Settings
    {
        public List<Option> Options { get; set; } = new();
    }

    public class Option
    {
        public string Text { get; set; }
        public Action Action { get; set; }
    }

    public override void _Ready()
    {
        base._Ready();
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < OptionButtons.Count; i++)
        {
            var panel = AnimatedPanels[i];
            var button = OptionButtons[i];

            var map = new ButtonMap
            {
                Index = i,
                Panel = panel,
                Button = button,
            };

            button.Pressed += () => Button_Pressed(map.Index);

            maps.Add(map);
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        Player.SetAllLocks(nameof(DialogueOptionsView), true);
        MouseVisibility.Show(nameof(DialogueOptionsView));
    }

    protected override void OnHide()
    {
        base.OnHide();
        Player.SetAllLocks(nameof(DialogueOptionsView), false);
        MouseVisibility.Hide(nameof(DialogueOptionsView));
    }

    private void Button_Pressed(int i)
    {
        selected_index = i;
        HideDialogueOptions();
    }

    private void UpdateButtons(Settings settings)
    {
        for (int i = 0; i < maps.Count; i++)
        {
            var map = maps[i];
            var valid = i < settings.Options.Count;
            map.Button.Visible = valid;

            if (!valid) continue;

            var option = settings.Options[i];
            map.Button.Text = option.Text;
        }
    }

    private IEnumerator WaitForShowButtons()
    {
        var visible_maps = maps
            .Where(x => x.Index < current_settings.Options.Count);

        foreach (var map in visible_maps)
        {
            map.Panel.AnimatePopShow();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator WaitForHideButtons()
    {
        var visible_maps = maps
            .Where(x => x.Index < current_settings.Options.Count)
            .OrderByDescending(x => x.Index == selected_index);

        foreach (var map in visible_maps)
        {
            map.Panel.AnimatePopHide();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void ShowDialogueOptions(Settings settings)
    {
        current_settings = settings;
        UpdateButtons(settings);

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            Show();
            InputBlocker.Show();
            ReleaseCurrentFocus();

            UpdateButtons(settings);

            yield return Overlay.AnimateBehindShow();

            yield return WaitForShowButtons();

            InputBlocker.Hide();

            maps.First().Button.GrabFocus();
        }
    }

    private void HideDialogueOptions()
    {
        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            InputBlocker.Show();
            ReleaseCurrentFocus();

            yield return WaitForHideButtons();

            yield return Overlay.AnimateBehindHide();

            InputBlocker.Hide();
            Hide();

            DoSelectedAction();
        }
    }

    private void DoSelectedAction()
    {
        var option = current_settings.Options[selected_index];
        option.Action?.Invoke();
    }
}
