using Godot;
using Godot.Collections;
using System.Collections;
using System.Collections.Generic;

public partial class BugPreviewScene : Scene
{
    [Export]
    public Camera3D Camera;

    [Export]
    public Array<FocusCharacterInfo> WalkingBugs;

    public override void _Ready()
    {
        base._Ready();
        Camera.Current = true;

        MainMenuView.Instance.Hide();
        GameView.Instance.Hide();

        LoopWalkingBugs();
    }

    private void LoopWalkingBugs()
    {
        var length = WalkingBugs.Count * 1f;
        var start = -length * 0.5f;
        var bugs = new List<FocusCharacter>();

        for (int i = 0; i < WalkingBugs.Count; i++)
        {
            var info = WalkingBugs[i];
            var bug = info.Scene.Instantiate<FocusCharacter>();
            bug.SetParent(this);
            bug.Scale = Vector3.One * 0.6f;
            bug.GlobalPosition = new Vector3(0, 0, start + i);
            bug.SetMoving(true);
            bug.SetAccessory("");

            bugs.Add(bug);
        }

        this.StartCoroutine(Cr, "loop");
        IEnumerator Cr()
        {
            var speed = 0.5f;
            var max = -length * 0.5f;

            while (true)
            {
                foreach (var bug in bugs)
                {
                    bug.GlobalPosition += Vector3.Forward * speed * GameTime.DeltaTime;

                    var z = bug.GlobalPosition.Z;

                    if (bug.GlobalPosition.Z < max)
                    {
                        bug.GlobalPosition = bug.GlobalPosition + Vector3.Back * length;
                    }
                }

                yield return null;
            }
        }
    }
}
