using Godot;

public partial class CrystalEnergyContainer : Area3D, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public CollisionShape3D Collider;

    [Export]
    public Node3D Crystal;

    [Export]
    public MeshInstance3D ButtonMesh;

    [Export]
    public Material UnpoweredMaterial;

    [Export]
    public Material RedMaterial;

    [Export]
    public Material GreenMaterial;

    [Export]
    public AudioStreamPlayer3D SfxPlace;

    public bool IsCompleted => GameFlags.IsFlag(HandInInfo.Id, 1);

    private const string DialoguePowerSource = "CRYSTAL_POWER_SOURCE";

    public override void _Ready()
    {
        base._Ready();
        InitializeCrystal();
        InitializeMaterials();
        SetInteractive(!IsCompleted);

        HandInController.Instance.OnHandInClaimed += HandIn_Claimed;
        DialogueController.Instance.OnDialogueEnded += Dialogue_Ended;
        GameFlagsController.Instance.OnFlagChanged += Flag_Changed;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        HandInController.Instance.OnHandInClaimed -= HandIn_Claimed;
        DialogueController.Instance.OnDialogueEnded -= Dialogue_Ended;
        GameFlagsController.Instance.OnFlagChanged -= Flag_Changed;
    }

    private void InitializeCrystal()
    {
        SetCrystalEnabled(IsCompleted);
    }

    private void InitializeMaterials()
    {
        SetPowered(IsCompleted);
    }

    public void Interact()
    {
        if (IsCompleted)
        {

        }
        else
        {
            DialogueController.Instance.StartDialogue(DialoguePowerSource);
        }
    }

    private void HandIn_Claimed(string id)
    {
        if (id == HandInInfo.Id)
        {
            GameFlags.SetFlag(id, 1);
        }
    }

    private void Flag_Changed(string id, int i)
    {
        if (id == HandInInfo.Id)
        {
            var is_on = i == 1;
            SetInteractive(!is_on);
            SetCrystalEnabled(is_on);
            SetPowered(is_on);
        }
    }

    private void Dialogue_Ended(string id)
    {
        if (id == DialoguePowerSource)
        {
            HandInView.Instance.ShowPopup(HandInInfo.Id);
        }
    }

    private void SetCrystalEnabled(bool enabled)
    {
        Crystal.Visible = enabled;

        if (enabled)
        {
            SfxPlace.Play();
        }
    }

    private void SetPowered(bool powered)
    {
        ButtonMesh.SetSurfaceOverrideMaterial(1, powered ? UnpoweredMaterial : RedMaterial);
        ButtonMesh.SetSurfaceOverrideMaterial(2, powered ? GreenMaterial : UnpoweredMaterial);
    }

    private void SetInteractive(bool interactive)
    {
        Collider.Disabled = !interactive;
    }
}
