using Godot;
using System.Collections;

public partial class WorldBug : Node3D
{
    [Export]
    public FocusEventInfo Info;

    [Export]
    public Node3D CharacterParent;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public AnimationPlayer AnimationPlayer_Float;

    public bool IsRunning { get; protected set; }
    private float DistanceToPlayer => GlobalPosition.DistanceTo(Player.Instance.GlobalPosition);

    protected FocusCharacter current_character;

    public const float MIN_DIST_TO_PLAYER = 6.0f;

    protected void Clear()
    {
        if (current_character != null)
        {
            current_character.QueueFree();
            current_character = null;
        }
    }

    protected FocusCharacterInfo GetRandomCharacterInfo()
    {
        return Info.Characters.PickRandom();
    }

    protected FocusCharacter CreateRandomCharacter()
    {
        Clear();
        var info = GetRandomCharacterInfo();
        current_character = info.Scene.Instantiate<FocusCharacter>();
        current_character.SetParent(CharacterParent);
        current_character.Initialize(info);
        return current_character;
    }

    public void Animate()
    {
        IsRunning = true;

        CreateRandomCharacter();

        var anim_show = current_character.Info.Tags.Contains(FocusCharacterTag.Flying) ? "show_flying" :
            current_character.Info.Tags.Contains(FocusCharacterTag.Water) ? "show_water" :
            "show";

        var anim_hide = current_character.Info.Tags.Contains(FocusCharacterTag.Flying) ? "hide_flying" :
            current_character.Info.Tags.Contains(FocusCharacterTag.Water) ? "hide_water" :
            "hide";

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            current_character.SetMoving(true);
            yield return AnimationPlayer.PlayAndWaitForAnimation(anim_show);
            current_character.SetMoving(false);

            AnimationPlayer.Play("look_around");

            var end = GameTime.Time + AnimationPlayer.GetAnimation("look_around").Length;
            while (GameTime.Time < end && DistanceToPlayer > MIN_DIST_TO_PLAYER)
            {
                yield return null;
            }

            current_character.SetMoving(true);
            yield return AnimationPlayer.PlayAndWaitForAnimation(anim_hide);
            current_character.SetMoving(false);

            Clear();
            IsRunning = false;
        }
    }
}
