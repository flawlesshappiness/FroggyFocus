using Godot;
using System.Collections;

public partial class InputPromptControl : ControlScript
{
    [Export]
    public InputPromptTexture Texture;

    [Export]
    public Label ActionLabel;

    [Export]
    public AnimationPlayer AnimationPlayer;

    private string target_action;
    private string current_action;

    public override void _Ready()
    {
        base._Ready();
        this.StartCoroutine(UpdateCr, "update");
    }

    public void ShowInteract()
    {
        target_action = "Interact";
    }

    public void HidePrompt()
    {
        target_action = string.Empty;
    }

    private IEnumerator UpdateCr()
    {
        var is_visible = false;
        while (true)
        {
            yield return null;

            if (target_action == current_action) continue;

            if (is_visible)
            {
                yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
                current_action = string.Empty;
                is_visible = false;
            }

            if (target_action != string.Empty)
            {
                var success = Texture.UpdateIcon(target_action);
                ActionLabel.Text = target_action;
                current_action = target_action;

                yield return AnimationPlayer.PlayAndWaitForAnimation("show");
                is_visible = true;
            }
        }
    }
}
