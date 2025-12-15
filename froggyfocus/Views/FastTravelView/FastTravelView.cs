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

    [Export]
    public PurchasePopup PurchasePopup;

    private LocationInfo purchase_info;
    private List<FastTravelButton> buttons = new();

    public override void _Ready()
    {
        base._Ready();
        BackButton.Pressed += Back_Pressed;
        RegisterDebugActions();
    }

    protected override void Initialize()
    {
        base.Initialize();
        InitializeButtons();
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

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Scenes",
            Action = ListScenes
        });

        void ShowView(DebugView v)
        {
            Show();
            v.Close();
        }

        void ListScenes(DebugView v)
        {
            v.SetContent_Search();

            v.ContentSearch.AddItem(nameof(SwampScene), () => SceneActions(v, nameof(SwampScene), "StartBoat"));
            v.ContentSearch.AddItem(nameof(CaveScene), () => SceneActions(v, nameof(CaveScene), "StartBoat"));
            v.ContentSearch.AddItem(nameof(FactoryScene), () => SceneActions(v, nameof(FactoryScene), "StartBoat"));
            v.ContentSearch.AddItem(nameof(GlitchScene), () => SceneActions(v, nameof(GlitchScene)));
            v.ContentSearch.AddItem(nameof(EldritchScene), () => SceneActions(v, nameof(EldritchScene)));
            v.ContentSearch.AddItem(nameof(CrystalScene), () => SceneActions(v, nameof(CrystalScene)));
            v.ContentSearch.AddItem(nameof(PartnerHomeScene), () => SceneActions(v, nameof(PartnerHomeScene)));
            v.ContentSearch.AddItem(nameof(ScientistLabScene), () => SceneActions(v, nameof(ScientistLabScene)));
            v.ContentSearch.AddItem(nameof(LetterScene), () => SceneActions(v, nameof(LetterScene)));

            v.ContentSearch.UpdateButtons();
        }

        void SceneActions(DebugView v, string scene_name, string start_node = "")
        {
            v.SetContent_Search();

            v.ContentSearch.AddItem("Goto", () => GotoScene(v, scene_name, start_node));

            v.ContentSearch.UpdateButtons();
        }

        void GotoScene(DebugView v, string scene_name, string start_node)
        {
            v.Close();

            Data.Game.CurrentScene = scene_name;
            Data.Game.StartingNode = start_node;
            Data.Game.Save();

            Scene.Goto(scene_name);
        }
    }

    private void InitializeButtons()
    {
        LocationButtonTemplate.Hide();

        foreach (var info in LocationController.Instance.Collection.Resources)
        {
            CreateLocationButton(info);
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateButtonVisibility();
    }

    private void UpdateButtonVisibility()
    {
        var scene_name = System.IO.Path.GetFileNameWithoutExtension(Data.Game.CurrentScene);
        var current_location = LocationController.Instance.Collection.Resources.FirstOrDefault(x => x.Scene == scene_name);
        buttons.ForEach(x => x.Visible = x.LocationInfo != current_location);
    }

    protected override void GrabFocusAfterOpen()
    {
        base.GrabFocusAfterOpen();
        buttons.FirstOrDefault(x => x.Visible).GrabFocus();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree() && !Animating)
        {
            Close();
        }
    }

    private Button CreateLocationButton(LocationInfo info)
    {
        var data = Location.GetOrCreateData(info.Id);
        var button = LocationButtonTemplate.Duplicate() as FastTravelButton;
        button.SetParent(LocationButtonTemplate.GetParent());
        button.Show();
        button.SetLocation(info);
        button.SetLocked(!data.Unlocked);
        button.PressedWhenUnlocked += () => LocationButton_PressedWhenUnlocked(info.Scene);
        button.PressedWhenLocked += () => LocationButton_PressedWhenLocked(info);
        buttons.Add(button);
        return button;
    }

    private void LocationButton_PressedWhenUnlocked(string scene_name)
    {
        Transition(scene_name);
    }

    private void LocationButton_PressedWhenLocked(LocationInfo info)
    {
        purchase_info = info;
        PurchasePopup.SetLocation(info);

        this.StartCoroutine(Cr, "purchase");
        IEnumerator Cr()
        {
            yield return PurchasePopup.WaitForPopup();

            var button = buttons.FirstOrDefault(x => x.LocationInfo == info);
            button.GrabFocus();

            if (PurchasePopup.Purchased)
            {
                var data = Location.GetOrCreateData(info.Id);
                data.Unlocked = true;
                Data.Game.Save();

                button.SetLocked(false);
            }
        }
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
