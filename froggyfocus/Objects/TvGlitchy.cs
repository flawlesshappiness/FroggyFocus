using Godot;
using Godot.Collections;

public partial class TvGlitchy : Area3D, IInteractable
{
    [Export]
    public string GameFlagId;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public AudioStreamPlayer3D SfxOn;

    [Export]
    public Array<MatrixLabel> MatrixLabels;

    public bool IsOn => GameFlags.IsFlag(GameFlagId, 1);

    public override void _Ready()
    {
        base._Ready();

        if (IsOn)
        {
            SetOn(IsOn);
        }
    }

    public void Interact()
    {
        SetOn(!IsOn);

        if (IsOn)
        {
            DialogueController.Instance.StartDialogue("##GLITCH_TV_ON##");
        }
    }

    private void SetOn(bool on)
    {
        GameFlags.SetFlag(GameFlagId, on ? 1 : 0);

        var anim = on ? "on" : "off";
        AnimationPlayer.Play(anim);

        MatrixLabels.ForEach(x => x.SetOn(on));

        if (on)
        {
            SfxOn.Play();
        }
    }
}
