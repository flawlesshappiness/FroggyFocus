using Godot;
using System.Collections;
using System.Linq;

public partial class GlowingMushroom : Node3DScript
{
    [Export]
    public Color EmissionColor;

    [Export]
    public float EmissionEnergy;

    [Export]
    public MeshInstance3D Mesh;

    private BaseMaterial3D[] mats;

    public override void _Ready()
    {
        base._Ready();
        InitializeMaterials();
        WeatherController.Instance.OnWeatherStart += WeatherStart;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        WeatherController.Instance.OnWeatherStart -= WeatherStart;
    }

    protected override void Initialize()
    {
        base.Initialize();

        var weather = WeatherController.Instance.GetCurrentWeather();
        var glow = ShouldGlow(weather);
        SetGlowing(glow);
    }

    private void InitializeMaterials()
    {
        var mat_count = Mesh.GetSurfaceOverrideMaterialCount();
        mats = new BaseMaterial3D[mat_count];

        for (int i = 0; i < mat_count; i++)
        {
            mats[i] = Mesh.GetSurfaceOverrideMaterial(i).Duplicate() as BaseMaterial3D;
            Mesh.SetSurfaceOverrideMaterial(i, mats[i]);
        }
    }

    private void WeatherStart(WeatherInfo info)
    {
        var glow = ShouldGlow(info);

        this.StartCoroutine(Cr, "glow");
        IEnumerator Cr()
        {
            var arr_c_start = mats.Select(x => glow ? x.AlbedoColor : x.Emission).ToArray();
            var arr_c_end = mats.Select(x => glow ? EmissionColor : x.AlbedoColor).ToArray();
            var arr_e_start = mats.Select(x => x.EmissionEnergyMultiplier).ToArray();
            var arr_e_end = mats.Select(x => glow ? EmissionEnergy : 0f).ToArray();
            var duration = WeatherController.Instance.GetTransitionDuration();
            yield return LerpEnumerator.Lerp01(duration, f =>
            {
                for (int i = 0; i < mats.Length; i++)
                {
                    var mat = mats[i];
                    var color = arr_c_start[i].Lerp(arr_c_end[i], f);
                    var energy = Mathf.Lerp(arr_e_start[i], arr_e_end[i], f);
                    mat.Emission = color;
                    mat.EmissionEnergyMultiplier = energy;
                }
            });
        }
    }

    private void SetGlowing(bool glow)
    {
        var arr_c = mats.Select(x => glow ? EmissionColor : x.AlbedoColor).ToArray();
        var arr_e = mats.Select(x => glow ? EmissionEnergy : 0f).ToArray();

        for (int i = 0; i < mats.Length; i++)
        {
            var mat = mats[i];
            var color = arr_c[i];
            var energy = arr_e[i];
            mat.Emission = color;
            mat.EmissionEnergyMultiplier = energy;
        }
    }

    public static bool ShouldGlow(WeatherInfo info)
    {
        var has_rain = info.Rain > 0;
        var has_fog = info.FogEnabled;
        var glow = has_rain || has_fog;
        return glow;
    }
}
