using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTab : MonoBehaviour
{
    public List<ButtonTab> otherTaps = new List<ButtonTab>();
    public UI2DSprite icon_;
    public UILabel label_;
    public UIPanel panel_;
    public UI2DSprite bar_;
    public UIWidget search_;
    public UIWidget related_;

    public bool isShowed;
    private void Start()
    {
        //GoDefault();
    }

    bool hover;
    bool hoverIn;
    bool hoverOut;
    bool clicked;
    private void Update()
    {
        if (Program.pointedCollider == GetComponent<Collider>())
        {
            hover = true;
            hoverOut = false;
            if(Program.InputGetMouseButtonUp_0)
                clicked = true;
        }
        else
        {
            hover = false;
            hoverIn = false;
        }

        if (hover && hoverIn == false)
        {
            hoverIn = true;
            if(isShowed == false)
            {
                GetComponent<UI2DSprite>().sprite2D = Program.I().newDeckManager.mono.tab_over;
            }
        }
        if (hover == false && hoverOut == false)
        {
            hoverOut = true;
            if (isShowed == false)
            {
                GetComponent<UI2DSprite>().sprite2D = Program.I().newDeckManager.mono.tab_off;
            }
        }
        if (clicked)
        {
            clicked = false;
            SEHandler.PlayInternalAudio("se_sys/SE_MENU_SELECT_01");
            if (isShowed == false)
                Show();
        }
    }

    public void Show()
    {
        isShowed = true;
        foreach (var t in otherTaps)
            t.Hide();

        if (gameObject.name == "button_search")
        {
            if(Program.I().newDeckManager.relatedCard != 0)
            {
                search_.alpha = 0f;
                related_.alpha = 1f;
                panel_.topAnchor.absolute = -150;
                Program.I().newDeckManager.PrintRelatedCards();
            }
            else
            {
                search_.alpha = 1f;
                related_.alpha = 0f;
                panel_.topAnchor.absolute = -150;
                if (Program.I().newDeckManager.firstIn)
                    Program.I().newDeckManager.firstIn = false;
                else
                    Program.I().newDeckManager.PrintSearchedCards();
            }
            GetComponent<UI2DSprite>().rightAnchor.absolute = 279;
        }
        else
        {
            panel_.topAnchor.absolute = -10;
            search_.alpha = 0f;
            related_.alpha = 0f;
        }
        if (gameObject.name == "button_book")
        {
            Program.I().newDeckManager.PrintBookedCards();
            GetComponent<UI2DSprite>().rightAnchor.absolute = 290;
        }
        else if (gameObject.name == "button_history")
        {
            Program.I().newDeckManager.PrintHistoryCards();
        }
        icon_.leftAnchor.absolute = 70;
        icon_.rightAnchor.absolute = 110;
        GetComponent<UI2DSprite>().sprite2D = Program.I().newDeckManager.mono.tab_on;
        icon_.color = Color.black;
        label_.alpha = 1f;

        bar_.GetComponent<UIScrollBar>().foregroundWidget.gameObject.SetActive(false);
        bar_.GetComponent<UIScrollBar>().foregroundWidget.gameObject.SetActive(true);
        panel_.gameObject.SetActive(false);
        bar_.gameObject.SetActive(false);
        panel_.gameObject.SetActive(true);
        bar_.gameObject.SetActive(true);
        bar_.GetComponent<UIScrollBar>().value = 0f;
    }

    public void Hide()
    {
        isShowed = false;
        GetComponent<UI2DSprite>().sprite2D = Program.I().newDeckManager.mono.tab_off;
        icon_.color = Color.white;
        label_.alpha = 0;
        icon_.leftAnchor.absolute = 47;
        icon_.rightAnchor.absolute = 87;
        if (gameObject.name == "button_search")
        {
            GetComponent<UI2DSprite>().rightAnchor.absolute = 129;
        }
        else if (gameObject.name == "button_book")
        {
            GetComponent<UI2DSprite>().rightAnchor.absolute = 140;
        }
        else if (gameObject.name == "button_history")
        {

        }
    }
}
