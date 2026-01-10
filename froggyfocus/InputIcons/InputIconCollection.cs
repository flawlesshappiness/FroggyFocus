using Godot;
using Godot.Collections;
using System.Linq;

[GlobalClass]
public partial class InputIconCollection : Resource
{
    private static InputIconCollection _instance;
    public static InputIconCollection Instance => _instance ?? (_instance = Load());

    [Export]
    public Array<Texture2D> Icons;

    private System.Collections.Generic.Dictionary<string, Texture2D> icon_dic;
    private Dictionary<Key, string> key_icons;
    private Dictionary<MouseButton, string> mouse_button_icons;
    private Dictionary<JoyButton, string> xbox_icons;
    private Dictionary<JoyButton, string> playstation_icons;

    public static InputIconCollection Load()
    {
        var res = GD.Load<InputIconCollection>($"InputIcons/Resources/{nameof(InputIconCollection)}.tres");
        res.Initialize();
        return res;
    }

    public void Initialize()
    {
        icon_dic = Icons.ToDictionary(x => x.GetResourceName(), x => x);

        key_icons = new Dictionary<Key, string>
        {
            { Key.Key0, "keyboard_0" },
            { Key.Key1, "keyboard_1" },
            { Key.Key2, "keyboard_2" },
            { Key.Key3, "keyboard_3" },
            { Key.Key4, "keyboard_4" },
            { Key.Key5, "keyboard_5" },
            { Key.Key6, "keyboard_6" },
            { Key.Key7, "keyboard_7" },
            { Key.Key8, "keyboard_8" },
            { Key.Key9, "keyboard_9" },

            { Key.A, "keyboard_a" },
            { Key.B, "keyboard_b" },
            { Key.C, "keyboard_c" },
            { Key.D, "keyboard_d" },
            { Key.E, "keyboard_e" },
            { Key.F, "keyboard_f" },
            { Key.G, "keyboard_g" },
            { Key.H, "keyboard_h" },
            { Key.I, "keyboard_i" },
            { Key.J, "keyboard_j" },
            { Key.K, "keyboard_k" },
            { Key.L, "keyboard_l" },
            { Key.M, "keyboard_m" },
            { Key.N, "keyboard_n" },
            { Key.O, "keyboard_o" },
            { Key.P, "keyboard_p" },
            { Key.Q, "keyboard_q" },
            { Key.R, "keyboard_r" },
            { Key.S, "keyboard_s" },
            { Key.T, "keyboard_t" },
            { Key.U, "keyboard_u" },
            { Key.V, "keyboard_v" },
            { Key.W, "keyboard_w" },
            { Key.X, "keyboard_x" },
            { Key.Y, "keyboard_y" },
            { Key.Z, "keyboard_z" },

            { Key.Left, "keyboard_arrow_left" },
            { Key.Right, "keyboard_arrow_right" },
            { Key.Up, "keyboard_arrow_up" },
            { Key.Down, "keyboard_arrow_down" },

            { Key.Space, "keyboard_space_icon" },
        };

        mouse_button_icons = new Dictionary<MouseButton, string>
        {
            { MouseButton.Left, "mouse_left" },
            { MouseButton.Right, "mouse_right" },
        };

        xbox_icons = new Dictionary<JoyButton, string>
        {
            { JoyButton.A, "xbox_button_color_a" },
            { JoyButton.B, "xbox_button_color_b" },
            { JoyButton.X, "xbox_button_color_x" },
            { JoyButton.Y, "xbox_button_color_y" },
        };

        playstation_icons = new Dictionary<JoyButton, string>
        {
            { JoyButton.A, "playstation_button_color_cross" },
            { JoyButton.B, "playstation_button_color_circle" },
            { JoyButton.X, "playstation_button_color_square" },
            { JoyButton.Y, "playstation_button_color_triangle" },
        };
    }

    public Texture2D GetIcon(Key id)
    {
        if (key_icons.TryGetValue(id, out var name))
        {
            return icon_dic[name];
        }

        return null;
    }

    public Texture2D GetIcon(MouseButton id)
    {
        if (mouse_button_icons.TryGetValue(id, out var name))
        {
            return icon_dic[name];
        }

        return null;
    }

    public Texture2D GetIcon(JoyButton id)
    {
        var gamepad = (GamepadDisplayType)Data.Options.GamepadDisplayIndex;
        var dic = gamepad == GamepadDisplayType.XBox ? xbox_icons : playstation_icons;
        if (dic.TryGetValue(id, out var name))
        {
            return icon_dic[name];
        }

        return null;
    }
}
