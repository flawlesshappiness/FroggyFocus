using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class FrogCharacter : Character
{
    [Export]
    public bool DisableAnimationStates;

    [Export]
    public bool DisableAppearanceUpdates;

    [Export]
    public FrogTongue Tongue;

    [Export]
    public AnimationStateMachine Animation;

    [Export]
    public PlayerMoveSoundsGroup MoveSounds;

    [Export]
    public MeshInstance3D BodyMesh;

    private List<AppearanceAttachmentGroup> attachment_groups = new();

    public override void _Ready()
    {
        base._Ready();
        InitializeBody();
        InitializeAttachments();
        InitializeAnimations();

        if (!DisableAppearanceUpdates)
        {
            ClearAppearance();
            LoadAppearance();
        }

        CustomizeAppearanceControl.OnAppearanceChanged += AppearanceChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        CustomizeAppearanceControl.OnAppearanceChanged -= AppearanceChanged;
    }

    protected virtual void InitializeAnimations() { }
    protected virtual void InitializeBody() { }

    private void InitializeAttachments()
    {
        attachment_groups = this.GetNodesInChildren<AppearanceAttachmentGroup>();
    }

    public void ClearAppearance()
    {
        attachment_groups.ForEach(x => x.Clear());
    }

    public void LoadAppearance()
    {
        if (Data.Game == null) return;
        LoadAppearance(Data.Game);
    }

    public void LoadAppearance(GameSaveData data)
    {
        LoadBody(data);
        LoadAttachments(data);
    }

    private void AppearanceChanged()
    {
        if (DisableAppearanceUpdates) return;
        LoadAppearance();
    }

    protected virtual void LoadBody(GameSaveData game_data) { }

    private void LoadAttachments(GameSaveData game_data)
    {
        foreach (var att_data in game_data.FrogAppearanceData.Attachments)
        {
            LoadAppearanceAttachment(att_data);
        }
    }

    private AppearanceAttachmentGroup GetAttachmentGroup(ItemCategory category)
    {
        return attachment_groups.FirstOrDefault(x => x.Category == category);
    }

    private void LoadAppearanceAttachment(FrogAppearanceAttachmentData data)
    {
        SetAppearanceAttachment(data.Category, data.Type, data.PrimaryColor, data.SecondaryColor);
    }

    public void SetAppearanceAttachment(ItemCategory category, ItemType type, Color primary, Color? secondary = null)
    {
        var group = GetAttachmentGroup(category);
        group?.SetAttachment(type, primary, secondary ?? Colors.Black);
    }

    public virtual void SetMoving(bool moving) { }
    public virtual void SetJumping(bool jumping) { }
    public virtual void SetCharging(bool charging) { }
    public virtual void SetTongueOut(bool open) { }
    public virtual void SetFalling() { }
    public virtual void SetSearching(bool searching) { }
    public virtual void SetCoveringEyes(bool cover_eyes) { }

    public Coroutine AnimateEatTarget(Node3D target)
    {
        return this.StartCoroutine(Cr, nameof(AnimateEatTarget));
        IEnumerator Cr()
        {
            SetTongueOut(true);
            yield return Tongue.AnimateTongueTowards(target.GlobalPosition);
            Tongue.AttachToTongue(target);
            SetTongueOut(false);
            yield return new WaitForSeconds(0.05f);
            yield return Tongue.AnimateTongueBack();
        }
    }
}
