using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YGOSharp;
using YGOSharp.OCGWrapper.Enums;

public class CardOnSelection : MonoBehaviour
{
    UI2DSprite background;
    UI2DSprite icon_base;
    UI2DSprite icon_location;
    UITexture pic;
    GameObject faceDown;
    UIButton button;
    UI2DSprite icon_level;
    UILabel levelNum;
    GameObject icon_selected;
    GameObject icon_order;
    UILabel num_order;
    GameObject chain_icon;

    public bool theFirst;
    public bool theLast;
    public bool myCard;

    public bool selected;
    public bool selectable;
    public bool dark;
    public bool alreadySelected;

    public GameObject go;
    CardListHandler cardListHandler;
    CardSelectionHandler cardSelectionHandler;

    public gameCard card;
    public int sortID;
    bool sort;

    public GameObject arrow;
    public CardOnSelection(gameCard card, bool theFirst, bool theLast, bool sort)
    {
        this.card = card;
        this.sort = sort;
        go = Instantiate(Program.I().cardSelection.cardOnSelectionPrefab);
        go.transform.parent = Program.I().cardSelection.list;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        cardListHandler = Program.I().new_ui_cardList;
        cardSelectionHandler = Program.I().cardSelection;

        pic = go.transform.GetChild(2).GetComponent<UITexture>();
        pic.mainTexture = card.gameObject_face.GetComponent<Renderer>().material.mainTexture;
        pic.mainTexture = GameTextureManager.GetCardPictureNow(card.get_data().Id);

        icon_level = go.transform.GetChild(5).GetComponent<UI2DSprite>();
        levelNum = go.transform.GetChild(6).GetComponent<UILabel>();
        if ((card.get_data().Type & (uint)CardType.Monster) > 0)
        {
            if ((card.get_data().Type & (uint)CardType.Link) > 0)
            {
                icon_level.sprite2D = cardListHandler.link;
                if (card.get_data().Level == 0)
                {
                    for (var i = 0; i < 32; i++)
                        if ((card.get_data().LinkMarker & (1 << i)) > 0)
                            card.get_data().Level++;
                }
            }
            else if ((card.get_data().Type & (uint)CardType.Xyz) > 0)
                icon_level.sprite2D = cardListHandler.rank;
            levelNum.text = card.get_data().Level.ToString();
        }
        else
        {
            icon_level.sprite2D = null;
            levelNum.text = "";
        }

        faceDown = go.transform.GetChild(3).gameObject;
        if ((card.p.position & (uint)CardPosition.FaceUp) > 0)
            faceDown.SetActive(false);
        if ((card.p.location & (uint)CardLocation.Overlay) > 0)
            faceDown.SetActive(false);

        icon_location = go.transform.GetChild(1).GetComponent<UI2DSprite>();
        if((card.p.location & (uint)CardLocation.Deck) > 0)
            icon_location.sprite2D = Program.I().cardSelection.deck;
        else if ((card.p.location & (uint)CardLocation.Hand) > 0)
            icon_location.sprite2D = Program.I().cardSelection.hand;
        else if ((card.p.location & (uint)CardLocation.Extra) > 0)
            icon_location.sprite2D = Program.I().cardSelection.extra;
        else if ((card.p.location & (uint)CardLocation.Removed) > 0)
            icon_location.sprite2D = Program.I().cardSelection.exclude;
        else if ((card.p.location & (uint)CardLocation.Grave) > 0)
            icon_location.sprite2D = Program.I().cardSelection.grave;
        else if ((card.p.location & (uint)CardLocation.Onfield) > 0)
            icon_location.sprite2D = Program.I().cardSelection.field;

        icon_base = icon_location.transform.GetChild(0).GetComponent<UI2DSprite>();
        background = go.transform.GetChild(0).GetComponent<UI2DSprite>();
        if (theLast)
            background.width = 150;
        if (card.p.controller != 0)
        {
            background.color = new Color(0.6f, 0.2f, 0.2f, 1f);
            icon_base.color = new Color(0.6f, 0.2f, 0.2f, 1f);
        }

        icon_location.gameObject.SetActive(theFirst);

        chain_icon = go.transform.GetChild(7).gameObject;
        chain_icon.SetActive(card.currentKuang == gameCard.kuangType.selected);

        icon_selected = go.transform.GetChild(8).gameObject;
        selectable = true;

        icon_order = go.transform.GetChild(9).gameObject;
        num_order = icon_order.transform.GetChild(0).GetComponent<UILabel>();

        button = go.transform.GetChild(4).GetComponent<UIButton>();
        EventDelegate.Add(button.onClick, OnClick);

        sortID = 0;
    }

    public void PreSelectThis(bool selectable)
    {
        this.selected = true;
        icon_selected.gameObject.SetActive(selected);
        this.alreadySelected = true;
        this.dark = false;
        pic.color = Color.white;
        this.selectable = selectable;
    }

    public void RefreshSelectableState(bool selected, bool selectable, bool dark)
    {
        this.selected = selected;
        icon_selected.gameObject.SetActive(selected);

        this.selectable = selectable;

        this.dark = dark;
        if (!dark)
            pic.color = Color.white;
        else
            pic.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        if (selected == false)
            Destroy(arrow);
    }

    void OnClick()
    {
        if (!sort)
        {
            if (selectable)
            {
                selected = !selected;
                icon_selected.SetActive(selected);
                if (selected)
                {
                    SEHandler.PlayInternalAudio("se_sys/SE_MENU_S_DECIDE_01");
                    cardSelectionHandler.selected++;

                    dark = false;
                    pic.color = Color.white;

                    if((card.p.location & (uint)CardLocation.Onfield) > 0 && arrow == null)
                    {
                        arrow = ABLoader.LoadABFromFile("effects/other/fxp_arrow_aim_001");
                        arrow.transform.position = card.gameObject.transform.position;
                        arrow.transform.GetChild(0).GetComponent<Renderer>().material.renderQueue = 3005;
                    }
                }
                else
                {
                    SEHandler.PlayInternalAudio("se_sys/SE_MENU_S_DECIDE_02");
                    cardSelectionHandler.selected--;

                    dark = false;
                    pic.color = Color.white;

                    if(arrow != null)
                        Destroy(arrow);
                }
                cardSelectionHandler.Refresh(this);
            }
            if (alreadySelected && !selectable)
            {
                SEHandler.PlayInternalAudio("se_sys/SE_MENU_S_DECIDE_02");
                Program.I().ocgcore.cardsSelected.Add(card);
                Program.I().ocgcore.sendSelectedCards();
            }
        }
        else
        {
            if (!selected)
            {
                selected = true;
                cardSelectionHandler.selected++;
                sortID = cardSelectionHandler.selected;
                icon_order.SetActive(true);
                num_order.text = sortID.ToString();
                SEHandler.PlayInternalAudio("se_sys/SE_MENU_S_DECIDE_01");
                cardSelectionHandler.Refresh(this);
            }
            else
            {
                selected = false;
                cardSelectionHandler.selected--;
                SEHandler.PlayInternalAudio("se_sys/SE_MENU_S_DECIDE_02");
                cardSelectionHandler.Refresh(this);
            }
        }
        Program.I().cardDescription.setData(card.get_data(), card.p.controller == 0 ? GameTextureManager.myBack : GameTextureManager.opBack);
    }

    public void resetSortNum(int n)
    {
        Debug.Log("resetSortNum: " + n + ", sortID: " + sortID);
        if(sortID > 0)
        {
            if (sortID > n)
                sortID--;
            else if (sortID == n)
                sortID = 0;
            num_order.text = sortID.ToString();
            if(sortID == 0)
            {
                selected = false;
                icon_order.SetActive(false);
            }
        }
        Debug.Log("after: resetSortNum: " + n + ", sortID: " + sortID);
    }
}