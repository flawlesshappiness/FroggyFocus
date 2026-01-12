using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class ObjectiveView : PanelView
{
    public static ObjectiveView Instance => Get<ObjectiveView>();
    protected override bool IgnoreCreate => false;

    [Export]
    public ObjectiveControl ObjectiveControlTemplate;

    [Export]
    public Button CloseButton;

    [Export]
    public ScrollContainer ScrollContainer;

    [Export]
    public UnlockPopup UnlockPopup;

    private bool animating;
    private List<ObjectiveControl> objective_controls = new();

    private class ObjectiveMap
    {
        public ObjectiveInfo Info { get; set; }
        public ObjectiveData Data { get; set; }
    }

    public override void _Ready()
    {
        base._Ready();
        CloseButton.Pressed += CloseButton_Pressed;

        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "OBJECTIVE";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Show view",
            Action = v => { v.Close(); Show(); }
        });
    }

    protected override void OnShow()
    {
        base.OnShow();
        UpdateControls();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (Input.IsActionJustReleased("ui_cancel") && IsVisibleInTree() && !animating)
        {
            Close();
        }
    }

    protected override void GrabFocusAfterOpen()
    {
        base.GrabFocusAfterOpen();
        objective_controls.FirstOrDefault()?.ClaimButton.GrabFocus();
    }

    private void Clear()
    {
        ObjectiveControlTemplate.Hide();
        objective_controls.ForEach(x => x.QueueFree());
        objective_controls.Clear();
    }

    private void UpdateControls()
    {
        Clear();

        var infos = ObjectiveController.Instance.Collection.Resources;
        var maps = infos.Select(x => new ObjectiveMap { Info = x, Data = Objective.GetOrCreateData(x) })
            .OrderByDescending(x => Objective.GetPercentage(x.Info));

        foreach (var map in maps)
        {
            var control = CreateObjectiveControl(map.Info);
        }

        var first = objective_controls.First();
        first.ClaimButton.FocusEntered += ScrollToTop;

        var last = objective_controls.Last();
        last.ClaimButton.FocusEntered += ScrollToBottom;
    }

    private ObjectiveControl CreateObjectiveControl(ObjectiveInfo info)
    {
        var control = ObjectiveControlTemplate.Duplicate() as ObjectiveControl;
        control.SetParent(ObjectiveControlTemplate.GetParent());
        control.SetInfo(info);
        control.Show();
        control.OnItemUnlocked += ItemUnlocked;
        objective_controls.Add(control);
        return control;
    }

    private void CloseButton_Pressed()
    {
        Close();
    }

    private void ScrollToTop()
    {
        ScrollContainer.ScrollVertical = 0;
    }

    private void ScrollToBottom()
    {
        ScrollContainer.ScrollVertical = 9999;
    }

    private void ItemUnlocked(ItemType type)
    {
        animating = true;
        this.StartCoroutine(Cr, "unlock");
        IEnumerator Cr()
        {
            UnlockPopup.SetAppearanceItem(type);
            AnimatedPanel.AnimateShrink();
            yield return UnlockPopup.WaitForPopup();
            yield return AnimatedPanel.AnimateMoveUp();
            GrabFocusAfterOpen();
            animating = false;
        }
    }
}
