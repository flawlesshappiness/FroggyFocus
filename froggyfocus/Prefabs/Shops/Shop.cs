using Godot;
using System.Collections;

public partial class Shop : Area3D, IInteractable
{
    [Export]
    public MoleNpc Mole;

    private static Shop current;

    private bool animating;
    private bool mole_up;
    private float time_mole_down;

    public override void _Ready()
    {
        base._Ready();
        ShopView.Instance.OnClosed += ShopClosed;
    }

    public void Interact()
    {
        if (animating) return;

        current = this;

        if (mole_up)
        {
            ShopView.Instance.Show();
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
            ShopView.Instance.Show();
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
        PauseView.ToggleLock.SetLock(id, locked);
    }
}
