using YGOSharp;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using YGOSharp.OCGWrapper.Enums;
using System.Text;
using System.Linq;

public class NewDeckManager : Servant
{
    public DeckManagerMono mono;

    public Deck deck = new Deck();
    public Deck search = new Deck();
    public Deck book = new Deck();
    public Deck history = new Deck();
    public Deck related = new Deck();
    readonly string bookPath = "config/book.ydk";

    public bool deckDirty;
    public string inDeckName;
    public string outDeckName;

    public int mainCount;
    public int extraCount;
    public int sideCount;
    public int pickupCount;

    public DiyScrollView scrollView;

    public int showingCard = 0;
    public int relatedCard = 0;

    public bool firstIn;
    public enum Condition
    {
        editDeck = 1,
        changeSide = 2
    }
    public Condition condition = Condition.editDeck;

    public override void initialize()
    {
        base.initialize();

        mono = Program.I().deckManagerMono;

        condition = Condition.editDeck;

        EventDelegate.Add(mono.exit_.onClick, Exit);
        EventDelegate.Add(mono.finish_.onClick, Exit);
        EventDelegate.Add(mono.card_button.onClick, ShowDetail);
        EventDelegate.Add(mono.id_.GetComponent<UIButton>().onClick, IdChange);
        EventDelegate.Add(mono.plus_.onClick, AddCard);
        EventDelegate.Add(mono.minus_.onClick, DeleteCard);
        EventDelegate.Add(mono.banlist_.onChange, LoadBanlist);
        EventDelegate.Add(mono.restore_.onClick, FromCodedDeckToObjectDeck);
        EventDelegate.Add(mono.sort_.onClick, SortCards);
        EventDelegate.Add(mono.random_.onClick, RandomCards);
        EventDelegate.Add(mono.copy_.onClick, CopyDeck);
        EventDelegate.Add(mono.save_.onClick, OnSave);
        EventDelegate.Add(mono.decoration_.onClick, Decoration);
        EventDelegate.Add(mono.book_.onClick, OnBookMark);
        EventDelegate.Add(mono.search_input.onSubmit, DoSearch);
        EventDelegate.Add(mono.search_clean.onClick, DoClean);
        EventDelegate.Add(mono.search_button.onClick, DoSearch);
        EventDelegate.Add(mono.search_filter.onClick, ShowFilter);
        EventDelegate.Add(mono.search_sort.onClick, ShowSort);
        EventDelegate.Add(mono.search_reset.onClick, ResetFilter);
        EventDelegate.Add(mono.related_.onClick, ShowRelated);
        EventDelegate.Add(mono.related_button.onClick, () => { ShowDescription(relatedCard);});
        EventDelegate.Add(mono.related_return.onClick, OnRelatedReturn);

        scrollView = new DiyScrollView
            (
            mono.tab_panel,
            mono.tab_bar,
            itemOnBook,
            116,
            6,
            -510,
            0,
            80 +9,
            116 + 20,
            0
            );

        var banlistNames = BanlistManager.getAllName();
        currentBanlist = BanlistManager.GetByName(banlistNames[0]);
    }

    public override void show()
    {
        base.show();
        firstIn = true;
        BGMHandler.ChangeBGM("deck");
        mono.gameObject.SetActive(true);
        mono.mainWindow_.alpha = 1.0f;
        showingCard = 0;
        if (File.Exists(bookPath))
        {
            book = new Deck(bookPath);
        }
        else
        {
            book = new Deck();
        }
        search = new Deck();

        mono.filter_.settings = null;
        FilterMaker(false);

        mono.search_label.text = "搜索";
        history = new Deck();

        switch (condition)
        {
            case Condition.editDeck:
                var banlistNames = BanlistManager.getAllName();
                mono.banlist_.items = banlistNames;
                mono.banlist_.value = mono.banlist_.items[0];
                currentBanlist = BanlistManager.GetByName(mono.banlist_.items[0]);
                mono.tab_search.Show();

                break;
            case Condition.changeSide:
                currentBanlist = BanlistManager.GetByHash(Program.I().room.lflist);
                foreach (var item in Program.I().ocgcore.sideReference)
                    history.Main.Add(item.Value);
                mono.tab_history.Show();

                break;
        }
    }

    public override void hide()
    {
        base.hide();

        //Program.I().shiftToServant(Program.I().selectDeck);

        string value = "#这是用于保存【卡片收藏】中的卡的卡组码。\r\n#main\r\n";
        for (var i = 0; i < book.Main.Count; i++) value += book.Main[i] + "\r\n";
        File.WriteAllText(bookPath, value, Encoding.UTF8);

        mono.filter_.gameObject.SetActive(true);
        mono.filter_.ResetFilter();
        mono.filter_.gameObject.SetActive(false);
        mono.gameObject.SetActive(false);
        mono.DestroyCards();
    }

    public void Exit()
    {
        if (returnAction != null) returnAction();
    }

    public void shiftCondition(Condition condition)
    {
        this.condition = condition;
        switch (condition)
        {
            case Condition.editDeck:
                mono.editDeck_.SetActive(true);
                mono.changeSide_.SetActive(false);
                mono.exit_.gameObject.SetActive(true);
                break;
            case Condition.changeSide:
                mono.editDeck_.SetActive(false);
                mono.changeSide_.SetActive(true);
                mono.exit_.gameObject.SetActive(false);
                break;
        }
    }

    public void loadDeckFromYDK(string deckName)
    {
        inDeckName = deckName;
        outDeckName = deckName;
        var path = "deck/" + deckName + ".ydk";
        deck = new Deck(path);

        FromCodedDeckToObjectDeck();
        deckDirty = false;
    }

    public void FromCodedDeckToObjectDeck()
    {
        mono.DestroyCards();

        mainCount = deck.Main.Count;
        extraCount = deck.Extra.Count;
        sideCount = deck.Side.Count;

        for (int i = 0; i < deck.Main.Count; i++)
            mono.CreateCard(i, deck.Main[i]);
        for (int i = 0; i < deck.Extra.Count; i++)
            mono.CreateCard(i + 100, deck.Extra[i]);
        for (int i = 0; i < deck.Side.Count; i++)
            mono.CreateCard(i + 200, deck.Side[i]);

        InitializeTable();
        RefreshDecorationIcons();
    }

    public void InitializeTable()
    {
        mono.mainCount_.text = mainCount.ToString();
        mono.extraCount_.text = extraCount.ToString();
        mono.sideCount_.text = sideCount.ToString();
        foreach (var card in mono.cardsOnManager)
            card.RefreshPositionInstant();

        mono.deckName_.gameObject.SetActive(true);
        mono.deckName_.value = inDeckName;

        if(condition == Condition.editDeck)
            mono.deckName_.gameObject.SetActive(true);
        else
            mono.deckName_.gameObject.SetActive(false);
        if(showingCard == 0)
            mono.descriptionPage.SetActive(false);
    }

    public void RefreshTable(GameObject except = null)
    {
        foreach (var card in mono.cardsOnManager)
        {
            if(card.gameObject != except)
                card.RefreshPosition();
        }
    }
    public void RefreshLabel()
    {
        mono.mainCount_.text = mainCount.ToString();
        mono.extraCount_.text = extraCount.ToString();
        mono.sideCount_.text = sideCount.ToString();
        mono.deckName_.value = outDeckName;
        foreach (var card in mono.tab_panel.transform.GetComponentsInChildren<CardOnBook>())
            card.ShowDot();
    }


    public void ShowDescription(int code, bool addToHistory = true)
    {
        Card card = CardsManager.GetCard(code);
        showingCard = code;
        if (card == null)
        {
            showingCard = 0;
            card = new Card();
        }
        if (condition == Condition.changeSide)
            addToHistory = false;
        if(addToHistory)
        {
            if (showingCard != 0)
            {
                if (history.Main.Contains(code))
                    history.Main.Remove(code);
                history.Main.Insert(0, code);
                if (mono.tab_history.isShowed)
                    PrintHistoryCards();
            }
        }

        mono.descriptionPage.SetActive(true);

        SetFrameColor(card);
        mono.description_.Clear();
        mono.description_.Add(card.Desc);

        mono.cardName_.text = card.Name;
        mono.attribute_.GetComponent<AttributeIcons>().ChangeAttribute(GameStringHelper.attribute(card.Attribute));
        mono.cardFace_.mainTexture = GameTextureManager.GetCardPictureNow(code);

        if((card.Type & (uint)CardType.Monster) > 0)
        {
            mono.lv_icon.alpha = 1f;
            mono.lv_num.alpha = 1f;
            mono.pendulum_icon.alpha = 1f;
            mono.pendulum_num.alpha = 1f;
            mono.atk_icon.alpha = 1f;
            mono.atk_num.alpha = 1f;
            mono.def_icon.alpha = 1f;
            mono.def_num.alpha = 1f;
            mono.race_.alpha = 1f;
            mono.spell_type.alpha = 0f;
            mono.spell_label.alpha = 0f;

            if ((card.Type & (uint)CardType.Pendulum) > 0)
            {
                mono.pendulum_num.text = card.LScale.ToString();
                mono.atk_icon.bottomAnchor.absolute = -96;
                mono.atk_icon.topAnchor.absolute = -61;
            }
            else
            {
                mono.pendulum_icon.alpha = 0;
                mono.pendulum_num.alpha = 0;
                mono.atk_icon.bottomAnchor.absolute = -50;
                mono.atk_icon.topAnchor.absolute = -10;
            }

            if ((card.Type & (uint)CardType.Link) > 0)
            {
                mono.lv_icon.sprite2D = mono.propertyIcons.link;
                mono.def_icon.alpha = 0;
                mono.def_num.alpha = 0;
            }
            else if ((card.Type & (uint)CardType.Xyz) > 0)
            {
                mono.lv_icon.sprite2D = mono.propertyIcons.rank;
            }
            else
            {
                mono.lv_icon.sprite2D = mono.propertyIcons.lv;
            }

            mono.lv_num.text = card.Level.ToString();
            mono.atk_num.text = card.Attack == -2 ? "?" : card.Attack.ToString();
            mono.def_num.text = card.Defense == -2 ? "?" : card.Defense.ToString();

            mono.race_.sprite2D = SetRaceIcon(GameStringHelper.race(card.Race));
        }
        else
        {
            mono.lv_icon.alpha = 0f;
            mono.lv_num.alpha = 0f;
            mono.pendulum_icon.alpha = 0f;
            mono.pendulum_num.alpha = 0f;
            mono.atk_icon.alpha = 0f;
            mono.atk_num.alpha = 0f;
            mono.def_icon.alpha = 0f;
            mono.def_num.alpha = 0f;
            mono.race_.alpha = 0f;
            mono.spell_type.alpha = 1f;
            mono.spell_label.alpha = 1f;

            string type = "";
            mono.spell_type.sprite2D = mono.propertyIcons.nothing;
            if ((card.Type & (uint)CardType.Counter) > 0)
            {
                type += "反击";
                mono.spell_type.sprite2D = mono.propertyIcons.counter;
            }
            else if ((card.Type & (uint)CardType.Field) > 0)
            {
                type += "场地";
                mono.spell_type.sprite2D = mono.propertyIcons.field;
            }
            else if ((card.Type & (uint)CardType.Equip) > 0)
            {
                type += "装备";
                mono.spell_type.sprite2D = mono.propertyIcons.equip;
            }
            else if ((card.Type & (uint)CardType.Continuous) > 0)
            {
                type += "永续";
                mono.spell_type.sprite2D = mono.propertyIcons.continuous;
            }
            else if ((card.Type & (uint)CardType.QuickPlay) > 0)
            {
                type += "速攻";
                mono.spell_type.sprite2D = mono.propertyIcons.quick_play;
            }
            else if ((card.Type & (uint)CardType.Ritual) > 0)
            {
                type += "仪式";
                mono.spell_type.sprite2D = mono.propertyIcons.ritual;
            }
            else if(card.Id != 0)
            {
                type += "通常";
                mono.spell_type.sprite2D = mono.propertyIcons.nothing;
            }
            if ((card.Type & (uint)CardType.Spell) > 0)
                type += "魔法";
            else if((card.Type & (uint)CardType.Trap) > 0)
                type += "陷阱";
            mono.spell_label.text = type;
        }

        mono.type_.text = GameStringHelper.GetType(card);
        mono.id_.text = card.Alias == 0 ? card.Id == 0 ? "" : card.Id.ToString() : card.Alias.ToString();
        showId = false;
        mono.id_.GetComponent<UIButton>().defaultColor = Color.white;
        mono.id_.GetComponent<UIButton>().hover = Color.white;
        int limit = currentBanlist.GetQuantity(code);
        if (limit == 3)
            mono.limit_.sprite2D = mono.limit_.GetComponent<LimitIcons>().null_;
        else if (limit == 2)
            mono.limit_.sprite2D = mono.limit_.GetComponent<LimitIcons>().limit2;
        else if (limit == 1)
            mono.limit_.sprite2D = mono.limit_.GetComponent<LimitIcons>().limit1;
        else if (limit == 0)
            mono.limit_.sprite2D = mono.limit_.GetComponent<LimitIcons>().ban;

        BookMark();
}

    public void CreateCard(int id, int code)
    {
        SEHandler.PlayInternalAudio("se_sys/SE_DECK_PLUS");
        GameObject card = mono.CreateCard(id, code);
        card.GetComponent<CardOnManager>().RefreshPositionInstant();
        card.transform.localScale = Vector3.one * 1.5f;
        deckDirty = true;
    }

    public void AddCard()
    {
        if (condition == Condition.changeSide)
            return;

        if (CheckBanlistAvail(showingCard))
        {
            Card data = CardsManager.GetCard(showingCard);
            if (data.IsExtraCard())
            {
                if(extraCount < 15)
                {
                    CreateCard(extraCount + 100, showingCard);
                    extraCount++;
                    RefreshTable();
                    RefreshLabel();
                }
                else if(sideCount < 15)
                {
                    CreateCard(sideCount + 200, showingCard);
                    sideCount++;
                    RefreshTable();
                    RefreshLabel();
                }
            }
            else
            {
                if (mainCount < 60)
                {
                    CreateCard(mainCount, showingCard);
                    mainCount++;
                    RefreshTable();
                    RefreshLabel();
                }
                else if (sideCount < 15)
                {
                    CreateCard(sideCount + 200, showingCard);
                    sideCount++;
                    RefreshTable();
                    RefreshLabel();
                }
            }
        }
    }
    public void AddCardByCode(int code)
    {
        if (CheckBanlistAvail(code))
        {
            ShowDescription(code);
            AddCard();
        }
    }
    public void DeleteCard()
    {
        if (condition == Condition.changeSide)
            return;
        List<CardOnManager> cards = new List<CardOnManager>();
        foreach (var card in mono.cardsOnManager)
        {
            if (card.code == showingCard)
                cards.Add(card);
        }
        if(cards.Count > 0)
        {
            cards.Sort((a, b) =>
            {
                if(a.id >  b.id)
                    return 1;
                if (a.id == b.id)
                    return 0;
                return -1;
            });
            cards[0].DeleteThis();
        }
    }

    public void SetFrameColor(Card data)
    {
        Color color1 = new Color(0.7764f, 0.6784f, 0.6274f, 1f);
        Color color2 = color1;
        if (data == null || data.Id == 0)
        {
            mono.name_base.color = color1;
            mono.type_base.color = color2;
            return;
        }
        int type = data.Type;
        if (data.Id == 10000000)
        {
            color1 = new Color(0.4745f, 0.4549f, 1f, 1f);
            color2 = color1;
        }
        else if (data.Id == 10000020)
        {
            color1 = new Color(1f, 0.2470f, 0.2156f, 1f);
            color2 = color1;
        }
        else if (data.Id == 10000010)
        {
            color1 = new Color(1f, 0.9882f, 0.1882f, 1f);
            color2 = color1;
        }
        else if ((type & (uint)CardType.Pendulum) > 0)
        {
            if ((type & (uint)CardType.Fusion) > 0)
            {
                color1 = new Color(0.8823f, 0.345f, 1f, 1f);
                color2 = new Color(0f, 0.8901f, 0.7411f, 1f);
            }
            else if ((type & (uint)CardType.Synchro) > 0)
            {
                color1 = new Color(1f, 1f, 1f, 1f);
                color2 = new Color(0f, 0.8901f, 0.7411f, 1f);
            }
            else if ((type & (uint)CardType.Xyz) > 0)
            {
                color1 = new Color(0f, 0f, 0f, 1f);
                color2 = new Color(0f, 0.8901f, 0.7411f, 1f);
            }
            else if ((type & (uint)CardType.Ritual) > 0)
            {
                color1 = new Color(0.3176f, 0.5882f, 1f, 1f);
                color2 = new Color(0f, 0.8901f, 0.7411f, 1f);
            }
            else if ((type & (uint)CardType.Effect) > 0)
            {
                color1 = new Color(1f, 0.4745f, 0.1882f, 1f);
                color2 = new Color(0f, 0.8901f, 0.7411f, 1f);
            }
            else if ((type & (uint)CardType.Normal) > 0)
            {
                color1 = new Color(1f, 0.7450f, 0.3294f, 1f);
                color2 = new Color(0f, 0.8901f, 0.7411f, 1f);
            }
        }
        else if ((type & (uint)CardType.Fusion) > 0)
        {
            color1 = new Color(0.8823f, 0.345f, 1f, 1f);
            color2 = color1;
        }
        else if ((type & (uint)CardType.Synchro) > 0)
        {
            color1 = new Color(1f, 1f, 1f, 1f);
            color2 = color1;
        }
        else if ((type & (uint)CardType.Xyz) > 0)
        {
            color1 = new Color(0f, 0f, 0f, 1f);
            color2 = color1;
        }
        else if ((type & (uint)CardType.Link) > 0)
        {
            color1 = new Color(0f, 0.3764f, 0.7764f, 1f);
            color2 = color1;
        }
        else if ((type & (uint)CardType.Ritual) > 0 && (type & (uint)CardType.Monster) > 0)
        {
            color1 = new Color(0.3176f, 0.5882f, 1f, 1f);
            color2 = color1;
        }
        else if ((type & (uint)CardType.Token) > 0)
        {
            color1 = new Color(0.7764f, 0.6784f, 0.6274f, 1f);
            color2 = color1;
        }
        else if ((type & (uint)CardType.Effect) > 0)
        {
            color1 = new Color(1f, 0.4745f, 0.1882f, 1f);
            color2 = color1;
        }
        else if ((type & (uint)CardType.Normal) > 0)
        {
            color1 = new Color(1f, 0.7450f, 0.3294f, 1f);
            color2 = color1;
        }
        else if ((type & (uint)CardType.Spell) > 0)
        {
            color1 = new Color(0f, 0.8901f, 0.7411f, 1f);
            color2 = color1;
        }
        else if ((type & (uint)CardType.Trap) > 0)
        {
            if ((data.Type & (uint)CardType.Effect) > 0)
            {
                color1 = new Color(1f, 0.4745f, 0.1882f, 1f);
                color2 = new Color(1f, 0.0509f, 0.6784f, 1f);
            }
            else if ((data.Type & (uint)CardType.Normal) > 0)
            {
                color1 = new Color(1f, 0.7450f, 0.3294f, 1f);
                color2 = new Color(1f, 0.0509f, 0.6784f, 1f);
            }
            else
            {
                color1 = new Color(1f, 0.0509f, 0.6784f, 1f);
                color2 = color1;
            }
        }
        mono.name_base.color = color1;
        mono.type_base.color = color2;
    }

    public Sprite SetRaceIcon(string race)
    {
        switch (race)
        {
            case "龙":
                return mono.propertyIcons.race_01;
            case "不死":
                return mono.propertyIcons.race_02;
            case "恶魔":
                return mono.propertyIcons.race_03;
            case "炎":
                return mono.propertyIcons.race_04;
            case "海龙":
                return mono.propertyIcons.race_05;
            case "岩石":
                return mono.propertyIcons.race_06;
            case "机械":
                return mono.propertyIcons.race_07;
            case "鱼":
                return mono.propertyIcons.race_08;
            case "恐龙":
                return mono.propertyIcons.race_09;
            case "昆虫":
                return mono.propertyIcons.race_10;
            case "兽":
                return mono.propertyIcons.race_11;
            case "兽战士":
                return mono.propertyIcons.race_12;
            case "植物":
                return mono.propertyIcons.race_13;
            case "水":
                return mono.propertyIcons.race_14;
            case "战士":
                return mono.propertyIcons.race_15;
            case "鸟兽":
                return mono.propertyIcons.race_16;
            case "天使":
                return mono.propertyIcons.race_17;
            case "魔法师":
                return mono.propertyIcons.race_18;
            case "雷":
                return mono.propertyIcons.race_19;
            case "爬虫类":
                return mono.propertyIcons.race_20;
            case "念动力":
                return mono.propertyIcons.race_21;
            case "幻龙":
                return mono.propertyIcons.race_22;
            case "电子界":
                return mono.propertyIcons.race_23;
            case "幻神兽":
                return mono.propertyIcons.race_24;
            case "创造神":
                return mono.propertyIcons.race_25;
            case "幻想魔":
                return mono.propertyIcons.race_26;
            default: 
                return mono.propertyIcons.nothing;
        }
    }

    private void ShowDetail()
    {
        Program.I().cardDetail.gameObject.SetActive(true);
        if (showingCard == 0)
            return;
        Program.I().cardDetail.Show(CardsManager.GetCard(showingCard));
    }

    bool showId;
    private void IdChange()
    {
        showId = !showId;
        Card card = CardsManager.GetCard(showingCard);
        if (showId)
        {
            mono.id_.text = card.Id.ToString();
            mono.id_.GetComponent<UIButton>().defaultColor = Color.yellow;
            mono.id_.GetComponent<UIButton>().hover = Color.yellow;
            mono.id_.gameObject.SetActive(false);
            mono.id_.gameObject.SetActive(true);

        }
        else
        {
            mono.id_.text = (card.Alias == 0 ? card.Id : card.Alias).ToString();
            mono.id_.GetComponent<UIButton>().defaultColor = Color.white;
            mono.id_.GetComponent<UIButton>().hover = Color.white;
            mono.id_.gameObject.SetActive(false);
            mono.id_.gameObject.SetActive(true);
        }
    }

    public Banlist currentBanlist;

    private void LoadBanlist()
    {
        currentBanlist = BanlistManager.GetByName(mono.banlist_.value);
        if(showingCard != 0)
            ShowDescription(showingCard);
        foreach(var card in mono.cardsOnManager)
            card.RefreshLimitIcon();
        foreach (var card in mono.tab_panel.transform.GetComponentsInChildren<CardOnBook>())
        {
            card.RefreshLimitIcon();
            card.ShowDot();
        }
    }

    public bool CheckBanlistAvail(int code)
    {
        return CountCard(code) < currentBanlist.GetQuantity(code);
    }

    public int CountCard(int code)
    {
        int alias = 0;
        try
        {
            alias = CardsManager.GetCard(code).Alias;
        }
        catch (Exception e) { }

        int cardCount = 0;
        foreach (var card in mono.cardsOnManager)
        {
            Card data = CardsManager.GetCard(card.code);
            if(data == null)
                break;
            if(alias == 0)
            {
                if (data.Id == code || data.Alias == code)
                    cardCount++;
            }
            else
            {
                if (data.Id == alias || data.Alias == alias)
                    cardCount++;
            }
        }
        return cardCount;
    }

    private void SortCards()
    {
        List<CardOnManager> main = new List<CardOnManager>();
        List<CardOnManager> extra = new List<CardOnManager>();
        List<CardOnManager> side = new List<CardOnManager>();
        foreach (var card in mono.cardsOnManager)
        {
            if (card.id < 100)
                main.Add(card);
            else if (card.id < 200)
                extra.Add(card);
            else
                side.Add(card);
        }
        foreach (var card in main)
        {
            if(card.data == null)
            {
                Program.PrintToChat("存在没有数据的卡，无法排序。");
                return;
            }
        }
        foreach (var card in extra)
        {
            if (card.data == null)
            {
                Program.PrintToChat("存在没有数据的卡，无法排序。");
                return;
            }
        }
        foreach (var card in side)
        {
            if (card.data == null)
            {
                Program.PrintToChat("存在没有数据的卡，无法排序。");
                return;
            }
        }
        main.Sort((left, right) =>
        {
            return CardsManager.comparisonOfCard()(left.data, right.data);
        });
        for (int i = 0; i < main.Count; i++)
            main[i].id = i;

        extra.Sort((left, right) =>
        {
            return CardsManager.comparisonOfCard()(left.data, right.data);
        });
        for (int i = 0; i < extra.Count; i++)
            extra[i].id = i + 100;

        side.Sort((left, right) =>
        {
            return CardsManager.comparisonOfCard()(left.data, right.data);
        });
        for (int i = 0; i < side.Count; i++)
            side[i].id = i + 200;

        RefreshTable();
        deckDirty = true;
    }
    private void RandomCards()
    {
        List<CardOnManager> main = new List<CardOnManager>();
        foreach (var card in mono.cardsOnManager)
        {
            if (card.id < 100)
                main.Add(card);
        }
        foreach (var card in main)
        {
            if(card.data == null)
            {
                Program.PrintToChat("存在没有数据的卡，无法打乱。");
                return;
            }
        }
        System.Random rand = new System.Random();
        for (int i = 0;i < main.Count; i++)
        {
            int random_index = rand.Next() % main.Count;
            var buffer = main[i];
            main[i] = main[random_index];
            main[random_index] = buffer;
        }
        for (int i = 0; i < main.Count; i++)
            main[i].id = i;
        RefreshTable();
        deckDirty = true;
    }
    private void CopyDeck()
    {
        if (SaveDeck())
        {
            inDeckName += "_复制";
            outDeckName = inDeckName;
            deckDirty = true;
            RefreshLabel();
        }
    }

    public bool SaveDeck()
    {
        try
        {
            if (mainCount <= 60
                &&
                extraCount <= 15
                &&
                sideCount <= 15
                )
            {
                FromObjectDeckToCodedDeck();
                var value = "#created by mdpro2\r\n#main\r\n";
                for (var i = 0; i < deck.Main.Count; i++) value += deck.Main[i] + "\r\n";
                value += "#extra\r\n";
                for (var i = 0; i < deck.Extra.Count; i++) value += deck.Extra[i] + "\r\n";
                value += "!side\r\n";
                for (var i = 0; i < deck.Side.Count; i++) value += deck.Side[i] + "\r\n";
                value += "#pickup\r\n";
                for (var i = 0; i < deck.Pickup.Count; i++) value += deck.Pickup[i] + "#\r\n";
                value += "#case\r\n";
                for (var i = 0; i < deck.Case.Count; i++) value += deck.Case[i] + "#\r\n";
                value += "#protector\r\n";
                for (var i = 0; i < deck.Protector.Count; i++) value += deck.Protector[i] + "#\r\n";
                value += "#field\r\n";
                for (var i = 0; i < deck.Field.Count; i++) value += deck.Field[i] + "#\r\n";
                value += "#grave\r\n";
                for (var i = 0; i < deck.Grave.Count; i++) value += deck.Grave[i] + "#\r\n";
                value += "#stand\r\n";
                for (var i = 0; i < deck.Stand.Count; i++) value += deck.Stand[i] + "#\r\n";
                value += "#mate\r\n";
                for (var i = 0; i < deck.Mate.Count; i++) value += deck.Mate[i] + "#\r\n";

                outDeckName = mono.deckName_.value;
                File.WriteAllText("deck/" + outDeckName + ".ydk", value, Encoding.UTF8);
                if (inDeckName != outDeckName)
                    File.Delete("deck/" + inDeckName + ".ydk");
                Config.Set("deckInUse", outDeckName);
                inDeckName = outDeckName;
                RMSshow_none(InterString.Get("卡组「[?]」已经被保存。", outDeckName));
                return true;
            }
            RMSshow_none(InterString.Get("卡组内卡片张数超过限制。"));
            return false;
        }
        catch (Exception e)
        {
            RMSshow_none(InterString.Get("保存失败！"));
            return false;
        }
    }
    public void OnSave()
    {
        if (SaveDeck())
            deckDirty = false;
    }

    public void FromObjectDeckToCodedDeck()
    {
        mono.cardsOnManager.Sort((left, right) =>
        {
            if(left.id > right.id) return 1;
            if(left.id < right.id) return -1;
            return 0;
        });
        deck.Main.Clear();
        deck.Extra.Clear();
        deck.Side.Clear();
        foreach(var card in mono.cardsOnManager)
        {
            if (card.id < 100)
                deck.Main.Add(card.code);
            else if (card.id <200)
                deck.Extra.Add(card.code);
            else
                deck.Side.Add(card.code);
        }
    }

    public Deck FromObjectDeckToCodedDeckForSideChange()
    {
        mono.cardsOnManager.Sort((left, right) =>
        {
            if (left.id > right.id) return 1;
            if (left.id < right.id) return -1;
            return 0;
        });
        Deck deck = new Deck();
        foreach (var card in mono.cardsOnManager)
        {
            if (card.id < 100)
                deck.Main.Add(card.code);
            else if (card.id < 200)
                deck.Extra.Add(card.code);
            else
                deck.Side.Add(card.code);
        }
        return deck;
    }

    public Action returnAction = null;

    public void RefreshDecorationIcons()
    {
        try
        {
            mono.decoration_case.sprite2D = Resources.Load<Sprite>("Texture/DeckCase/DeckCase" + AddZero(deck.Case[0]) + "_L");
            mono.small_deckCase.sprite2D = mono.decoration_case.sprite2D;
            mono.decoration_protector.sprite2D = Resources.Load<Sprite>("Texture/Protector/ProtectorIcon107" + AddZero(deck.Protector[0]));
            mono.decoration_field.sprite2D = Resources.Load<Sprite>("Texture/Field/FieldIcon109" + AddZero(deck.Field[0]));
            mono.decoration_grave.sprite2D = Resources.Load<Sprite>("Texture/Grave/FieldObjIcon110" + AddZero(deck.Grave[0]));
            mono.decoration_stand.sprite2D = Resources.Load<Sprite>("Texture/Avatarbase/FieldAvatarBaseIcon111" + AddZero(deck.Grave[0]));
            mono.decoration_mate.sprite2D = Resources.Load<Sprite>("Texture/Mate/100" + AddZero(deck.Mate[0]));
        }
        catch (Exception e) { }
    }

    public string AddZero(int code)
    {
        string returnValue = "";
        if(code > 9999)
            returnValue = code.ToString().Substring(1, 4) + "_" + code.ToString().Substring(0, 1);
        else if (code > 999)
            returnValue = code.ToString();
        else if(code >99)
            returnValue = "0" + code.ToString();
        else if(code > 9)
            returnValue = "00" + code.ToString();
        else
            returnValue = "000" + code.ToString();
        return returnValue;
    }

    private void Decoration()
    {
        Program.I().deckDecoration.gameObject.SetActive(true);
        Program.I().deckDecoration.Show(deck.Case[0], deck.Protector[0], deck.Field[0], deck.Grave[0], deck.Stand[0], deck.Mate[0]);
    }


    private GameObject itemOnBook(string[] args)
    {
        var returnValue = create(Program.I().newDeckManager.mono.cardOnBook, Vector3.zero, Vector3.zero, false, Program.I().ui_back_ground_2d);
        //UIHelper.getRealEventGameObject(returnValue).name = args[0];
        returnValue.GetComponent<CardOnBook>().id = int.Parse(args[0]);
        returnValue.GetComponent<CardOnBook>().code = int.Parse(args[1]);
        return returnValue;
    }

    private void PrintCard(List<int> codes)
    {
        var args = new List<string[]>();

        for(int i = 0; i < codes.Count; i++)
        {
            string[] arg = new string[2];
            arg[0] = i.ToString();
            arg[1] = codes[i].ToString();
            args.Add(arg);
        }
        safeGogo(50, () => { scrollView.Print(args); });
    }


    void BookMark()
    {
        if (book.Main.Contains(showingCard))
        {
            mono.book_.normalSprite2D = Program.I().buttonToggleL_On;
            mono.book_.hoverSprite2D = null;
            mono.book_.transform.GetChild(0).GetComponent<UI2DSprite>().color = Color.black;
            
        }
        else
        {
            mono.book_.normalSprite2D = Program.I().buttonToggleL;
            mono.book_.hoverSprite2D = Program.I().buttonToggleL_Over;
            mono.book_.transform.GetChild(0).GetComponent<UI2DSprite>().color = Color.white;
        }
        mono.book_.gameObject.SetActive(false);
        mono.book_.gameObject.SetActive(true);
    }
    void OnBookMark()
    {
        if (showingCard == 0)
            return;

        if (book.Main.Contains(showingCard))
            book.Main.Remove(showingCard);
        else
            book.Main.Add(showingCard);

        List<Card> cards = new List<Card>();
        foreach (var code in book.Main)
            cards.Add(CardsManager.GetCard(code));
        cards.Sort(CardsManager.comparisonOfCard());
        book.Main.Clear();
        foreach (var card in cards)
            book.Main.Add(card.Id);

        if(mono.tab_book.isShowed)
            PrintBookedCards();
        BookMark();
    }
    public void PrintSearchedCards()
    {
        PrintCard(search.Main.ToList());
        mono.search_label.text = search.Main.Count.ToString();
    }
    public void PrintRelatedCards()
    {
        PrintCard(related.Main.ToList());
    }
    public void PrintBookedCards()
    {
        PrintCard(book.Main.ToList());
    }
    public void PrintHistoryCards()
    {
        PrintCard(history.Main.ToList());
    }

    public void DoSearch()
    {
        var result = CardsManager.NewSearch(mono.search_input.value, mono.filter_.settings, currentBanlist);
        Sort(result);
        search.Main.Clear();
        foreach(Card card in result)
            search.Main.Add(card.Id);
        PrintSearchedCards();
    }

    void Sort(List<Card> cards)
    {
        CardsManager.nameInSearch = mono.search_input.value;
        switch (Program.I().searchFliter.GetSortOrder())
        {
            case SearchFilter.SortOrder.type_up:
                cards.Sort(CardsManager.comparisonOfCard());
                break;
            case SearchFilter.SortOrder.type_down:
                cards.Sort(CardsManager.comparisonOfCardReverse());
                break;
            case SearchFilter.SortOrder.lv_up:
                cards.Sort(CardsManager.comparisonOfCard_LV_Up());
                break;
            case SearchFilter.SortOrder.lv_down:
                cards.Sort(CardsManager.comparisonOfCard_LV_Down());
                break;
            case SearchFilter.SortOrder.atk_up:
                cards.Sort(CardsManager.comparisonOfCard_ATK_Up());
                break;
            case SearchFilter.SortOrder.atk_down:
                cards.Sort(CardsManager.comparisonOfCard_ATK_Down());
                break;
            case SearchFilter.SortOrder.def_up:
                cards.Sort(CardsManager.comparisonOfCard_DEF_Up());
                break;
            case SearchFilter.SortOrder.def_down:
                cards.Sort(CardsManager.comparisonOfCard_DEF_Down());
                break;
        }
    }


    void DoClean()
    {
        mono.search_input.value = "";
        DoSearch();
    }

    void ShowFilter()
    {
        mono.filter_.gameObject.SetActive(true);
        mono.filter_.Show();
    }
    void ShowSort()
    {
        mono.filter_.gameObject.SetActive(true);
        mono.filter_.ShowSort();
    }
    void ResetFilter()
    {
        mono.filter_.settings = null;
        mono.search_input.value = "";
        FilterMaker(false);
        DoSearch();
    }

    public void FilterMaker(bool on)
    {
        if (on)
        {
            mono.search_filter.normalSprite2D = Program.I().buttonToggleM_On;
            mono.search_filter.hoverSprite2D = null;
            mono.search_filter.transform.GetChild(0).GetComponent<UI2DSprite>().color = Color.black;
        }
        else
        {
            mono.search_filter.normalSprite2D = Program.I().buttonToggleM;
            mono.search_filter.hoverSprite2D = Program.I().buttonToggleM_Over;
            mono.search_filter.transform.GetChild(0).GetComponent<UI2DSprite>().color = Color.white;
        }
    }

    public void ShowRelated()
    {
        relatedCard = showingCard;
        if (relatedCard == 0)
            return;
        mono.related_card.mainTexture = mono.cardFace_.mainTexture;
        if(mono.cardName_.text.Length > 6 && mono.cardName_.text.Length < 12)
            mono.related_label.text = "「" + mono.cardName_.text + "」\n  的相关卡片";
        else
            mono.related_label.text = "「" + mono.cardName_.text + "」的相关卡片";
        related.Main.Clear();
        foreach (var card in CardsManager.RelatedSearch(relatedCard))
            related.Main.Add(card.Id);
        mono.tab_search.Show();
    }
    public void OnRelatedReturn()
    {
        related = new Deck();
        relatedCard = 0;
        mono.tab_search.Show();
    }

    public void OnDisConnectExit()
    {
        mono.exit_.gameObject.SetActive(true);
        returnAction = ReturnTo;
    }

    void ReturnTo()
    {
        Program.I().shiftToServant(Program.I().selectServer);
    }
}
