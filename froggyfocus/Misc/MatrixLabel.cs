using Godot;

public partial class MatrixLabel : Label3D
{
    [Export]
    public bool StartOn = true;

    [Export]
    public Vector2I Size;

    [Export]
    public Vector2I SpaceLength;

    [Export]
    public Vector2I StringLength;

    [Export]
    public float Frequency;

    private bool is_on;
    private int string_offset;
    private float time_next;
    private bool[,] letter_map;
    private RandomNumberGenerator rng = new();
    private string symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!#%&/()[]{}=+?^*";

    public override void _Ready()
    {
        base._Ready();
        is_on = StartOn;
        InitializeLetterMap();
        UpdateLabel();
    }

    private void InitializeLetterMap()
    {
        var is_letter = false;
        var string_length = 0;
        letter_map = new bool[Size.X, Size.Y];

        for (int x = 0; x < Size.X; x++)
        {
            string_length = 0;
            is_letter = rng.RandiRange(0, 1) == 0;

            for (int y = 0; y < Size.Y; y++)
            {
                if (y > string_length)
                {
                    is_letter = !is_letter;
                    string_length += is_letter ? StringLength.Range(rng.Randf()) : SpaceLength.Range(rng.Randf());
                }

                letter_map[x, y] = is_letter;
            }
        }
    }

    public void SetOn(bool on)
    {
        is_on = on;
        UpdateLabel();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!is_on) return;
        if (GameTime.Time < time_next) return;
        time_next = GameTime.Time + Frequency;
        string_offset = (string_offset + 1) % Size.Y;
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        var s = "";

        if (is_on)
        {
            for (int y = 0; y < Size.Y; y++)
            {
                for (int x = 0; x < Size.X; x++)
                {
                    s += letter_map[x, (y + string_offset) % Size.Y] ? symbols[rng.RandiRange(0, symbols.Length - 1)] : " ";
                }

                s += "\n";
            }
        }

        Text = s;
    }
}
