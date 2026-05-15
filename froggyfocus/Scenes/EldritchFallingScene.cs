using Godot;
using Godot.Collections;
using System.Collections;

public partial class EldritchFallingScene : Scene
{
    [Export]
    public FallingPlayer Player;

    [Export]
    public Node3D CoinGroupParent;

    [Export]
    public AnimationPlayer AnimationPlayer_Pickups;

    [Export]
    public AnimationPlayer AnimationPlayer_Camera;

    [Export]
    public Array<PackedScene> CoinGroupPrefabs;

    private RandomNumberGenerator rng = new();

    public override void _Ready()
    {
        base._Ready();
        WeatherController.Instance.StopWeather();
        AnimateFall();
    }

    private Coroutine AnimateFall()
    {
        return this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return AnimateCoinGroups();

            TransitionView.Instance.StartTransition(new TransitionSettings
            {
                Type = TransitionType.Color,
                Color = Colors.Black,
                Duration = 2f,
                OnTransition = OnTransition
            });

            yield return AnimationPlayer_Camera.PlayAndWaitForAnimation("move_up");
        }

        void OnTransition()
        {
            Data.Game.CurrentScene = nameof(EldritchScene);
            Data.Game.StartingNode = string.Empty;
            Data.Game.Save();

            Scene.Goto<EldritchScene>();
        }
    }

    private Coroutine AnimateCoinGroups()
    {
        return this.StartCoroutine(Cr, "coin_groups");
        IEnumerator Cr()
        {
            var count = 10;
            for (int i = 0; i < count; i++)
            {
                var group = CreateCoinGroup();
                yield return AnimationPlayer_Pickups.PlayAndWaitForAnimation("moving");
                group.QueueFree();
            }
        }
    }

    private Node3D CreateCoinGroup()
    {
        var prefab = CoinGroupPrefabs.PickRandom();
        var group = prefab.Instantiate<Node3D>();
        group.SetParent(CoinGroupParent);
        group.ClearPositionAndRotation();
        group.RotationDegrees = Vector3.Up * 360f * rng.Randf();

        foreach (var coin in group.GetNodesInChildren<CollectableCoin>())
        {
            coin.Initialize(Player);
        }

        return group;
    }
}
