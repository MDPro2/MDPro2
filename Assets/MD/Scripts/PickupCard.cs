using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupCard : MonoBehaviour
{
    public UI2DSprite icon;

    public Sprite notShow;
    public Sprite isShow;
    public Sprite notshowingFrame;
    public Sprite showingFrame;
    public Sprite hoverFrame;

    public static bool showing;

    private void Start()
    {
        EventDelegate.Add(GetComponent<UIButton>().onClick, Shift);
        Program.I().selectDeck.pickup = this;
    }

    public void Show()
    {
        showing = false;
        icon.sprite2D = notShow;
        icon.leftAnchor.absolute = -18;
        icon.rightAnchor.absolute = 18;
        icon.bottomAnchor.absolute = -24;
        icon.topAnchor.absolute = 24;
        icon.color = Color.white;
        GetComponent<UIButton>().normalSprite2D = notshowingFrame;
        GetComponent<UIButton>().hoverSprite2D = hoverFrame;


        Program.I().selectDeck.showingPickup = false;
        foreach (var animator in transform.parent.GetComponentsInChildren<Animator>(true))
        {
            animator.SetBool("Hover", false);
        }

        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    private void Shift()
    {
        if (showing)
        {
            Show();
        }
        else 
        { 
            showing = true;
            icon.sprite2D = isShow;
            icon.leftAnchor.absolute = -33;
            icon.rightAnchor.absolute = 33;
            icon.bottomAnchor.absolute = -24;
            icon.topAnchor.absolute = 24;
            icon.color = Color.black;
            GetComponent<UIButton>().normalSprite2D = showingFrame;
            GetComponent<UIButton>().hoverSprite2D = null;

            Program.I().selectDeck.showingPickup = true;
            foreach (var animator in transform.parent.GetComponentsInChildren<Animator>())
                animator.SetBool("Hover", true);

            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }
}
