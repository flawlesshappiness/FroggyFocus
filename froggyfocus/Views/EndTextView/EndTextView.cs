using Godot;
using Godot.Collections;
using System.Collections;

public partial class EndTextView : View
{
    public static EndTextView Instance => Get<EndTextView>();

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public AudioStreamPlayer SfxText;

    [Export]
    public Array<Label> Labels;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        string category = "ENDING";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Play ending text",
            Action = DebugShow
        });

        void DebugShow(DebugView v)
        {
            v.Close();
            Show();
        }
    }

    protected override void OnShow()
    {
        base.OnShow();
        Player.SetInputDisabled(nameof(EndTextView), true);
        AnimationPlayer.Play("show");
        InitializeLabels();
        AnimateLabels();
    }

    protected override void OnHide()
    {
        base.OnHide();
        Player.SetInputDisabled(nameof(EndTextView), false);
    }

    private void InitializeLabels()
    {
        Labels.ForEach(x => x.Hide());
    }

    private void AnimateLabels()
    {
        GameScene.Instance.FadeOutMusic();

        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return new WaitForSeconds(2f);
            foreach (var label in Labels)
            {
                label.Show();
                SfxText.Play();
                yield return new WaitForSeconds(6f);
                label.Hide();
                yield return new WaitForSeconds(1f);
            }

            MainMenu();
            yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
            Hide();
        }
    }

    private void MainMenu()
    {
        Data.Game.Save();

        WeatherController.Instance.StopWeather();
        WorldBugController.Instance.Stop();
        Scene.Goto<MainMenuScene>();
    }
}
