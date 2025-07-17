using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class RainController : ResourceController<RainCollection, RainInfo>
{
    public static RainController Instance => Singleton.Get<RainController>();
    public override string Directory => "Rain";

    public Action<float> OnRainIntensityChanged;

    private List<RainPlayer> rain_players = new();

    private class RainPlayer
    {
        public Vector2 Range { get; set; }
        public AudioStreamPlayer Player { get; set; }
        public RainInfo Info { get; set; }

        public void Clear()
        {
            Player.QueueFree();
        }

        public void SetVolume(float t)
        {
            var t_range = Mathf.Clamp((t - Range.X) / (Range.Y - Range.X), 0, 1);
            var volume = AudioMath.PercentageToDecibel(t_range);
            Player.VolumeDb = volume;
        }
    }

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "WEATHER";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Set rain intensity",
            Action = SelectRainIntensity
        });

        void SelectRainIntensity(DebugView v)
        {
            v.SetContent_Search();

            for (float i = 0; i <= 1.1f; i += 0.1f)
            {
                var level = i;
                v.ContentSearch.AddItem(i.ToString("0.0"), () => SetRainIntensity(v, level));
            }

            v.ContentSearch.UpdateButtons();
        }

        void SetRainIntensity(DebugView v, float level)
        {
            SetIntensity(level);
            v.Close();
        }
    }

    private RainInfo GetInfo(RainType type)
    {
        return Collection.Resources.Where(x => x.Type == type).ToList().Random();
    }

    private void Clear()
    {
        rain_players.ForEach(x => x.Clear());
        rain_players.Clear();
    }

    public void StartRain()
    {
        Clear();
        InitializePlayer(RainType.Light, new Vector2(0f, 0.5f));
        InitializePlayer(RainType.Medium, new Vector2(0.33f, 0.66f));
        InitializePlayer(RainType.Heavy, new Vector2(0.5f, 1.0f));
    }

    public void StopRain()
    {
        Clear();
    }

    private RainPlayer InitializePlayer(RainType type, Vector2 range)
    {
        var info = GetInfo(type);
        var asp = SoundController.Instance.Play(info.SoundInfo);
        asp.VolumeLinear = 0;
        var player = new RainPlayer
        {
            Info = info,
            Player = asp,
            Range = range
        };

        rain_players.Add(player);

        return player;
    }

    public void SetIntensity(float t)
    {
        var env = GameScene.Instance.WorldEnvironment.Environment;
        var mat = env.Sky.SkyMaterial as ShaderMaterial;
        mat.SetShaderParameter("blend", Mathf.Clamp(t * 10f, 0f, 1f));

        foreach (var player in rain_players)
        {
            player.SetVolume(t);
        }

        OnRainIntensityChanged?.Invoke(t);
    }
}
