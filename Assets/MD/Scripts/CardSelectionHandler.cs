using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YGOSharp;
using YGOSharp.OCGWrapper.Enums;
using static Servant;

public class CardSelectionHandler : MonoBehaviour
{
    public Sprite deck;
    public Sprite hand;
    public Sprite extra;
    public Sprite field;
    public Sprite grave;
    public Sprite exclude;
    public Sprite delayed;

    public Sprite shrink;
    public Sprite expand;

    UI2DSprite base_cardSelect;
    public GameObject cardOnSelectionPrefab;
    public Transform list;
    public UILabel title;
    public UIButton button_cancel;
    public UIButton button_confirm;
    UIButton button_hide;

    UILabel button_cancel_label;
    UILabel button_confirm_label;

    public bool isShowed;

    int ES_min;
    int ES_max;
    public int selected = 0;
    public int alreadySelected = 0;

    Ocgcore core;
    List<CardOnSelection> cardsOnSelection = new List<CardOnSelection>();

    superButtonType superButtonType;
    public bool exitable;
    public bool finishable;
    bool tempHide;
    public bool hideByRMS;
    private void Start()
    {
        base_cardSelect = transform.GetChild(0).GetChild(0).GetComponent<UI2DSprite>();
        list = transform.GetChild(1);
        title = transform.GetChild(4).GetComponent<UILabel>();
        button_confirm = transform.GetChild(2).GetComponent<UIButton>();
        button_cancel = transform.GetChild(3).GetComponent<UIButton>();
        button_hide = transform.GetChild(0).GetChild(4).GetComponent<UIButton>();
        button_confirm_label = button_confirm.transform.GetChild(0).GetComponent<UILabel>();
        button_cancel_label = button_cancel.transform.GetChild(0).GetComponent<UILabel>();
        EventDelegate.Add(button_hide.onClick, TempHide);
        EventDelegate.Add(button_cancel.onClick, hide);
        EventDelegate.Add(button_confirm.onClick, confirm);

        core = Program.I().ocgcore;
    }

    public void show(List<gameCard> cards, bool exitable, int ES_min, int ES_max, string hint, superButtonType superButtonType = superButtonType.no)
    {
        //检查传入gameCard列表数量
        if(cards.Count == 0)
        {
            Debug.Log("没有卡需要选择，请检查。");
            return;
        }
        list.localPosition = Vector3.zero;
        isShowed = true;
        gameObject.transform.DOLocalMoveY(-340, 0.2f);
        selected = 0;
        alreadySelected = 0;
        this.superButtonType = superButtonType;
        this.ES_min = ES_min;
        this.ES_max = ES_max;
        this.exitable = exitable;
        tempHide = false;
        hideByRMS = false;

        if (Program.I().setting.setting.confirmLeft.value)
        {
            button_confirm.gameObject.transform.localPosition = new Vector3(-145, -150, 0);
            button_cancel.gameObject.transform.localPosition = new Vector3(145, -150, 0);
        }
        else
        {
            button_confirm.gameObject.transform.localPosition = new Vector3(145, -150, 0);
            button_cancel.gameObject.transform.localPosition = new Vector3(-145, -150, 0);
        }

        foreach (var widget in list.GetComponentsInChildren<UIWidget>())
            Destroy(widget.gameObject);

        if (!finishable)
        {
            button_confirm.isEnabled = false;
            button_confirm_label.color = new Color(0.2f, 0.25f, 0f, 1f);
        }
        else
        {
            button_confirm.isEnabled = true;
            button_confirm_label.color = new Color(0.8f, 1f, 0f, 1f);
        }

        title.text = hint;
        //判断CurrentMessage，分条件初始化
        if (exitable)
        {
            button_cancel.isEnabled = true;
            button_cancel_label.color = new Color(0.8f, 1f, 0f, 1f);
            if (core.currentMessage == GameMessage.SelectIdleCmd)
            {
                if (superButtonType == superButtonType.spsummon)
                    title.text = "选择怪兽特殊召唤。";
                else if (superButtonType == superButtonType.act)
                    title.text = "选择卡片发动效果。";
            }
        }
        else
        {
            button_cancel.isEnabled = false;
            button_cancel_label.color = new Color(0.2f, 0.25f, 0f, 1f);
        }

        cardsOnSelection.Clear();
        List<gameCard> cleaned = new List<gameCard>();
        foreach (var card in cards)
        {
            if (!cleaned.Contains(card))
                cleaned.Add(card);
        }

        cards = cleaned;

        for (int i = 0; i < cards.Count; i++)
        {
            bool theFirst = false;
            bool theLast = false;
            if(i == 0)
            {
                theFirst = true;
                if (cards.Count == 1)
                    theLast = true;
                else if (cards[i].p.location != cards[i + 1].p.location || cards[i].p.controller != cards[i + 1].p.controller)
                    if(!((cards[i].p.location & (uint)CardLocation.Onfield) >0 && (cards[i + 1].p.location & (uint)CardLocation.Onfield) > 0 && cards[i].p.controller == cards[i + 1].p.controller))
                        theLast = true;
            }
            else
            {
                if (cards[i].p.location != cards[i - 1].p.location || cards[i].p.controller != cards[i - 1].p.controller)
                    theFirst = true;
                if(i + 1 == cards.Count)
                    theLast = true;
                else if (cards[i].p.location != cards[i + 1].p.location || cards[i].p.controller != cards[i + 1].p.controller)
                    if (!((cards[i].p.location & (uint)CardLocation.Onfield) > 0 && (cards[i + 1].p.location & (uint)CardLocation.Onfield) > 0 && cards[i].p.controller == cards[i + 1].p.controller))
                        theLast = true;
            }

            bool sort = false;
            if(core.currentMessage ==GameMessage.SortCard)
                sort = true;
            var cardOnSelection = new CardOnSelection(cards[i], theFirst, theLast, sort);
            cardsOnSelection.Add(cardOnSelection);
            if(cards.Count <= 5)
            {
                float length = cards.Count * 150;
                float startPoint = -length / 2f + 75;
                float point = startPoint + 150 * i;
                cardOnSelection.go.transform.localPosition = new Vector3(point, 0, 0);
            }
            else
                cardOnSelection.go.transform.localPosition = new Vector3(-400 + 150 * i, 0, 0);
        }
        if (cards.Count <= 4)
            base_cardSelect.width = 680;
        else if (cards.Count == 5)
            base_cardSelect.width = 830;
        else
            base_cardSelect.width = 950;

        if (core.currentMessage == GameMessage.SelectUnselect || core.currentMessage == GameMessage.SelectSum)
            foreach (var card in core.cardsSelected)
                foreach (var cardOnSelection_ in cardsOnSelection)
                    if (card == cardOnSelection_.card)
                        cardOnSelection_.PreSelectThis(false);
    }

    public void setTitle(string title)
    {
        //this.title.text = title;
    }

    public void hide()
    {
        core.Sleep(12);
        isShowed = false;
        gameObject.transform.DOLocalMoveY(-830, 0.2f);
        if (core.currentMessage ==GameMessage.SelectChain)
        {
            var binaryMaster = new BinaryMaster();
            binaryMaster.writer.Write(-1);
            core.sendReturn(binaryMaster.get());
        }
        if (core.currentMessage == GameMessage.SelectEffectYn)
        {
            var binaryMaster = new BinaryMaster();
            binaryMaster.writer.Write(0);
            core.sendReturn(binaryMaster.get());
        }
        if (core.currentMessage == GameMessage.AnnounceCard)
        {
            Program.I().ocgcore.RMSshow_input("AnnounceCard", InterString.Get("请输入关键字："), "");
        }
        foreach (var card in cardsOnSelection)
            Destroy(card.arrow);
    }

    public void HideWithoutAction()
    {
        core.Sleep(12);
        isShowed = false;
        gameObject.transform.DOLocalMoveY(-830, 0.2f);
        foreach (var card in cardsOnSelection)
            Destroy(card.arrow);
    }

    public void ShowWithoutAction()
    {
        core.Sleep(12);
        isShowed = true;
        hideByRMS = false;
        gameObject.transform.DOLocalMoveY(-340, 0.2f);
    }

    void TempHide()
    {
        if(!tempHide)
        {
            tempHide = true;
            button_hide.normalSprite2D = expand;
            gameObject.transform.DOLocalMoveY(-750, 0.2f);
            SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_02");
        }
        else
        {
            tempHide = false;
            button_hide.normalSprite2D = shrink;
            gameObject.transform.DOLocalMoveY(-340, 0.2f);
            SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_01");
        }
    }

    void confirm()
    {
        switch (core.currentMessage)
        {
            case GameMessage.SortCard:
                var binaryMaster = new BinaryMaster();
                foreach (var btn in cardsOnSelection)
                    binaryMaster.writer.Write((byte)(btn.sortID - 1));
                core.sendReturn(binaryMaster.get());
                break;
            case GameMessage.SelectChain:
                gameCard cardOnSortChain = null;
                foreach (var btn in cardsOnSelection)
                    if (btn.selected)
                    {
                        cardOnSortChain = btn.card; 
                        break;
                    }
                if (cardOnSortChain.effects.Count > 0)
                {
                    if (cardOnSortChain.effects.Count == 1)
                    {
                        binaryMaster = new BinaryMaster();
                        binaryMaster.writer.Write(cardOnSortChain.effects[0].ptr);
                        core.sendReturn(binaryMaster.get());
                    }
                    else
                    {
                        var values = new List<messageSystemValue>();
                        for (var i = 0; i < cardOnSortChain.effects.Count; i++)
                        {
                            if (cardOnSortChain.effects[i].flag == 0)
                            {
                                if (cardOnSortChain.effects[i].desc.Length > 2)
                                    values.Add(new messageSystemValue
                                    { hint = cardOnSortChain.effects[i].desc, value = cardOnSortChain.effects[i].ptr.ToString() });
                                else
                                    values.Add(new messageSystemValue
                                    {
                                        hint = InterString.Get("发动效果@ui"),
                                        value = cardOnSortChain.effects[i].ptr.ToString()
                                    });
                            }

                            if (cardOnSortChain.effects[i].flag == 1)
                                values.Add(new messageSystemValue
                                {
                                    hint = InterString.Get("适用「[?]」的效果", cardOnSortChain.get_data().Name),
                                    value = cardOnSortChain.effects[i].ptr.ToString()
                                });
                            if (cardOnSortChain.effects[i].flag == 2)
                                values.Add(new messageSystemValue
                                {
                                    hint = InterString.Get("重置「[?]」的控制权", cardOnSortChain.get_data().Name),
                                    value = cardOnSortChain.effects[i].ptr.ToString()
                                });
                        }

                        values.Add(new messageSystemValue { hint = InterString.Get("取消"), value = "hide" });
                        core.RMSshow_singleChoice("return", values);
                        hideByRMS = true;
                    }
                }
                break;
            case GameMessage.SelectBattleCmd:
            case GameMessage.SelectIdleCmd:
                foreach (var card in cardsOnSelection)
                    if (card.selected)
                        foreach (var btn in card.card.buttons)
                            if (btn.type == superButtonType)
                            {
                                btn.clicked();
                                break;
                            }
                break;
            case GameMessage.SelectEffectYn:
                binaryMaster = new BinaryMaster();
                binaryMaster.writer.Write(1);
                core.sendReturn(binaryMaster.get());
                break;
            case GameMessage.SelectSum:
                foreach (var card in cardsOnSelection)
                    if (card.selected && !card.alreadySelected)
                        core.cardsSelected.Add(card.card);
                core.realizeCardsForSelect();
                break;
            case GameMessage.AnnounceCard:
                foreach(var card in cardsOnSelection)
                {
                    if (card.selected)
                    {
                        binaryMaster = new BinaryMaster();
                        binaryMaster.writer.Write((uint)card.card.get_data().Id);
                        core.sendReturn(binaryMaster.get());
                        break;
                    }
                }
                break;
            default:
                foreach (var card in cardsOnSelection)
                    if (card.selected && !card.alreadySelected)
                        core.cardsSelected.Add(card.card);
                core.sendSelectedCards();
                break;
        }
        if(core.currentMessage != GameMessage.SelectSum)
            HideWithoutAction();
        else
        {
            foreach (var card in cardsOnSelection)
                Destroy(card.arrow);
        }
    }

    public void Refresh(CardOnSelection card)
    {
        bool sendable = false;
        switch (core.currentMessage)
        {
            case GameMessage.SelectCard:
                if (selected >= ES_min)
                    sendable = true;
                break;
            case GameMessage.SelectTribute:
                var all = 0;
                foreach (var cardOnSelection in cardsOnSelection)
                    if (cardOnSelection.selected)
                        all += cardOnSelection.card.levelForSelect_1;
                if (all >= ES_min)
                    sendable = true;
                break;
            case GameMessage.SelectSum:
            case GameMessage.SelectUnselect:
                if (selected >= alreadySelected + 1)
                    sendable = true;
                break;

            case GameMessage.SortCard:
                if (selected >= ES_max)
                    sendable = true;
                else
                    sendable = false;
                break;
            case GameMessage.SelectChain:
                if (selected >= 1)
                    sendable = true;
                else
                    sendable = false;
                break;
        }

        if (exitable)
        {
            if (selected >= 1)
                sendable = true;
        }

        if (sendable || finishable)
        {
            button_confirm.isEnabled = true;
            button_confirm_label.color = new Color(0.8f, 1f, 0f, 1f);
        }
        else
        {
            button_confirm.isEnabled = false;
            button_confirm_label.color = new Color(0.2f, 0.25f, 0f, 1f);
        }

        if (core.currentMessage != GameMessage.SortCard)
        {
            if (selected >= ES_max)
            {
                if (ES_max == 1)
                {
                    foreach (var cardOnSelection in cardsOnSelection)
                        if (card.selected == true)
                        {
                            if (cardOnSelection.go != card.go)
                            {
                                cardOnSelection.RefreshSelectableState(false, true, true);
                            }
                        }
                }
                else
                {
                    foreach (var cardOnSelection in cardsOnSelection)
                        if (!cardOnSelection.selected)
                            cardOnSelection.RefreshSelectableState(false, false, true);
                }
            }
            else
            {
                foreach (var cardOnSelection in cardsOnSelection)
                    if (!cardOnSelection.selected)
                        cardOnSelection.RefreshSelectableState(false, true, false);
            }

            if (core.currentMessage == GameMessage.SelectUnselect || core.currentMessage == GameMessage.SelectSum)
                if (selected >= alreadySelected + 1)
                    foreach (var cardOnSelection in cardsOnSelection)
                        if (!cardOnSelection.selected)
                            cardOnSelection.RefreshSelectableState(false, false, true);
        }
        else
        {
            if(!card.selected)
            {
                int id = card.sortID;
                foreach (var cardOnSelection in cardsOnSelection)
                    cardOnSelection.resetSortNum(id);
            }
        }
    }
}
