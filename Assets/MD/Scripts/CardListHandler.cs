using DG.Tweening;
using Percy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YGOSharp;

public class CardListHandler : MonoBehaviour
{
    public Sprite myOverlay;
    public Sprite myExclude;
    public Sprite myGrave;
    public Sprite myExtra;
    public Sprite myDeck;

    public Sprite opOverlay;
    public Sprite opExclude;
    public Sprite opGrave;
    public Sprite opExtra;
    public Sprite opDeck;

    public Sprite level;
    public Sprite rank;
    public Sprite link;

    public GameObject cardOnListPrefab;

    UI2DSprite icon;

    UIselectableList list;
    public Transform grid;

    float timeCount;

    public uint controller;
    public uint location;
    public uint sequence;

    public static bool isShowed;
    private void Start()
    {
        icon = transform.Find("icon_").GetComponent<UI2DSprite>();
    }

    public void show(uint controller, uint location, gameCard[] cards, bool fast = false)
    {

        if (isShowed)
        {
            hide(true);
            isShowed = false;
            timeCount = 0;
            DOTween.To(() => timeCount, a => timeCount = a, 1, 0.2f).OnComplete(() =>show(controller, location, cards, true));
        }
        else
        {
            isShowed = true;
            if(fast)
                gameObject.transform.DOMoveX(Utils.UIToWorldPoint(new Vector3(Utils.UIWidth() - 190f / 2f + 10f, 0, 0)).x, 0.1f);
            else
                gameObject.transform.DOMoveX(Utils.UIToWorldPoint(new Vector3(Utils.UIWidth() - 190f / 2f + 10f, 0, 0)).x, 0.2f);
            RefreshList(controller, location, sequence, cards);
        }
    }
    public void hide(bool fast = false)
    {
        isShowed = false;
        if(fast)
            //gameObject.transform.DOMoveX(Utils.UIToWorldPoint(new Vector3(1920f + 190f / 2f, 0f, 0f)).x, 0.1f);
            gameObject.transform.DOMoveX(Utils.UIToWorldPoint(new Vector3(Utils.UIWidth() + 190f / 2f, 0f, 0f)).x, 0.1f);
        else
            gameObject.transform.DOMoveX(Utils.UIToWorldPoint(new Vector3(Utils.UIWidth() + 190f / 2f, 0f, 0f)).x, 0.2f);
    }

    void RefreshList(uint controller, uint location, uint sequence, gameCard[] cards)
    {
        this.controller = controller;
        this.location = location;
        this.sequence = sequence;
        foreach (var widget in grid.GetComponentsInChildren<UIWidget>())
            Destroy(widget.gameObject);
        if ((location & (uint)CardLocation.Extra) > 0)
        {
            if (controller == 0)
                icon.sprite2D = myExtra;
            else
                icon.sprite2D = opExtra;
        }
        else if ((location & (uint)CardLocation.Grave) > 0)
        {
            if (controller == 0)
                icon.sprite2D = myGrave;
            else
                icon.sprite2D = opGrave;
        }
        else if ((location & (uint)CardLocation.Removed) > 0)
        {
            if (controller == 0)
                icon.sprite2D = myExclude;
            else
                icon.sprite2D = opExclude;
        }
        else if ((location & (uint)CardLocation.Deck) > 0)
        {
            if (controller == 0)
                icon.sprite2D = myDeck;
            else
                icon.sprite2D = opDeck;
        }
        else if ((location & (uint)CardLocation.MonsterZone) > 0)
        {
            if (controller == 0)
                icon.sprite2D = myOverlay;
            else
                icon.sprite2D = opOverlay;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            var card = new CardOnList(cards[i]);
            card.go.transform.localPosition = new Vector3(0f, -150f * (cards.Length - 1 - i), 0f);
        }
        grid.parent.localPosition = new Vector3(-55f, 0f, 0f);
        grid.parent.GetComponent<UIScrollView>().verticalScrollBar.value = 0f;
    }
}
