using Godot;
using System.Collections;

public partial class Shop : Area3D, IInteractable
{
    [Export]
    public string IntroDialogue;

    [Export]
    public MoleNpc Mole;

    private const string IntroFlag = "shop_intro_dialogue";

    private static Shop current;

    private bool animating;
    private bool mole_up;
    private float time_mole_down;

    public override void _Ready()
    {
        base._Ready();
        ShopView.Instance.OnClosed += ShopClosed;
        DialogueController.Instance.OnDialogueEnded += DialogueEnded;
        RaceController.Instance.OnCountdownStarted += Race_Start;
        RaceController.Instance.OnRaceEnd += Race_End;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        ShopView.Instance.OnClosed -= ShopClosed;
        DialogueController.Instance.OnDialogueEnded -= DialogueEnded;
        RaceController.Instance.OnCountdownStarted -= Race_Start;
        RaceController.Instance.OnRaceEnd -= Race_End;
    }

    public void Interact()
    {
        if (animating) return;

        current = this;

        if (mole_up)
        {
            ShowShop();
        }
        else
        {
            AnimateShowShop();
        }
    }

    private void ShopClosed()
    {
        if (current != this) return;
        current = null;
        animating = false;
        time_mole_down = GameTime.Time + 5f;

        this.StartCoroutine(Cr, "idle_shop");
        IEnumerator Cr()
        {
            while (current == this || GameTime.Time < time_mole_down)
            {
                yield return null;
            }

            AnimateHideShop();
        }
    }

    private void AnimateShowShop()
    {
        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            animating = true;
            SetLocks(true);
            TurnTowardsPlayer();
            Mole.AnimateShow();
            yield return new WaitForSeconds(1f);
            ShowShop();
            SetLocks(false);

            mole_up = true;
        }
    }

    private void AnimateHideShop()
    {
        this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            animating = true;
            Mole.AnimateHide();
            yield return new WaitForSeconds(1f);
            animating = false;
            mole_up = false;
        }
    }

    private void TurnTowardsPlayer()
    {
        Mole.TurnTowardsPosition(Player.Instance.GlobalPosition);
    }

    private void SetLocks(bool locked)
    {
        var id = nameof(Shop);
        Player.SetAllLocks(id, locked);
    }

    private void ShowShop()
    {
        if (!string.IsNullOrEmpty(IntroDialogue) && GameFlags.IsFlag(IntroFlag, 0))
        {
            GameFlags.SetFlag(IntroFlag, 1);
            StartDialogue(IntroDialogue);
        }
        else
        {
            ShopView.Instance.Show();
        }
    }

    private void StartDialogue(string id)
    {
        DialogueController.Instance.StartDialogue(id);
    }

    private void DialogueEnded(string id)
    {
        if (id == IntroDialogue)
        {
            ShowShop();
        }
    }

    private void Race_Start()
    {
        this.Disable();
    }

    private void Race_End(RaceResult result)
    {
        this.Enable();
    }
}
