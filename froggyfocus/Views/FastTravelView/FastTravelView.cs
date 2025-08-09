using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class FastTravelView : PanelView
{
    public static FastTravelView Instance => Get<FastTravelView>();
    protected override bool IgnoreCreate => false;

    [Export]
    public Button BackButton;

    [Export]
    public AnimatedOverlay Overlay;

    [Export]
    public ImageButton LocationButtonTemplate;

    private List<ImageButton> buttons = new();

    private List<string> scenes = new()
    {
        nameof(SwampScene),
        nameof(CaveScene)
    };

    public override void _Ready()
    {
        base._Ready();
        BackButton.Pressed += Back_Pressed;
        InitializeButtons();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "TRAVEL";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Show view",
            Action = ShowView
        });

        void ShowView(DebugView v)
        {
            Show();
            v.Close();
        }
    }

    private void InitializeButtons()
    {
        LocationButtonTemplate.Hide();

        foreach (var scene in scenes)
        {
            CreateLocationButton(scene);
        }
    }

    protected override void GrabFocusAfterOpen()
    {
        base.GrabFocusAfterOpen();
        buttons.FirstOrDefault().GrabFocus();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree() && !Animating)
        {
            Close();
        }
    }

    private Button CreateLocationButton(string scene_name)
    {
        var info = LocationController.Instance.GetInfo(scene_name);
        var button = LocationButtonTemplate.Duplicate() as ImageButton;
        button.SetParent(LocationButtonTemplate.GetParent());
        button.Show();
        button.SetTexture(info.PreviewImage);
        button.Text = scene_name;
        button.Pressed += () => LocationButton_Pressed(scene_name);
        buttons.Add(button);
        return button;
    }

    private void LocationButton_Pressed(string scene_name)
    {
        Transition(scene_name);
    }

    private void Back_Pressed()
    {
        Close();
    }

    protected void Transition(string scene_name)
    {
        if (Animating) return;
        Animating = true;

        Data.Game.CurrentScene = scene_name;
        Data.Game.StartingNode = "StartBoat";
        Data.Game.Save();

        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            ReleaseCurrentFocus();
            InputBlocker.Show();

            AnimatedOverlay.AnimateBehindHide();
            yield return AnimatedPanel.AnimatePopHide();

            yield return Overlay.AnimateFrontShow();
            var scene = Scene.Goto<TransitionScene>();
            yield return Overlay.AnimateFrontHide();

            yield return scene.AnimateTransition();

            yield return Overlay.AnimateFrontShow();
            Scene.Goto(scene_name);
            yield return Overlay.AnimateFrontHide();

            InputBlocker.Hide();

            Animating = false;

            Hide();
        }
    }
}
