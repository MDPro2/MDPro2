using System;
using System.Collections.Generic;
using DG.Tweening;
using SevenZip.CommandLineParser;
using UnityEngine;
using YGOSharp;
using YGOSharp.OCGWrapper.Enums;

public class CardDescription : Servant
{
    private UIWidget cardShowerWidget;
    public float cHeight = 0;

    private int current;

    private Card currentCard;
    private int currentCardIndex;

    private float currentHeight;
    private int currentLabelIndex;

    private readonly List<data> datas = new List<data>();
    private UIDeckPanel deckPanel;
    public UITextList description;
    private int eachLine;
    private GameObject line;
    private UISprite lineSprite;

    private readonly List<string> Logs = new List<string>();
    private UIPanel monitor;
    private float monitorHeight;
    private string myBanishedStr = "";
    private string myExtraStr = "";

    private string myGraveStr = "";
    private string opBanishedStr = "";
    private string opExtraStr = "";
    private string opGraveStr = "";
    private cardPicLoader picLoader;
    private UITexture picSprite;

    private readonly cardPicLoader[] quickCards = new cardPicLoader[300];
    private UIDragResize resizer;
    private UITexture underSprite;

    public float width = 0;

    private UILabel card_name;
    private UILabel type_name;
    private UI2DSprite name_base;
    private UI2DSprite type_base;
    private UIWidget property;
    private UILabel id_label;
    private UIButton id_button;
    private UIButton pic_button;
    public PropertyIcons pi;
    public AttributeIcons ai;
    public LimitIcons li;

    Card card;

    public bool forcedClosed;
    public override void initialize()
    {
        gameObject = Program.I().new_ui_cardDescription;

        picLoader = gameObject.GetComponent<cardPicLoader>();
        resizer = UIHelper.getByName<UIDragResize>(gameObject, "resizer");
        underSprite = UIHelper.getByName<UITexture>(gameObject, "under_");
        description = UIHelper.getByName<UITextList>(gameObject, "description_");
        cardShowerWidget = UIHelper.getByName<UIWidget>(gameObject, "card_shower");
        monitor = UIHelper.getByName<UIPanel>(gameObject, "monitor");
        deckPanel = gameObject.GetComponentInChildren<UIDeckPanel>();
        line = UIHelper.getByName(gameObject, "line");
        UIHelper.registEvent(gameObject, "pre_", onPre);
        UIHelper.registEvent(gameObject, "next_", onNext);
        UIHelper.registEvent(gameObject, "big_", onb);
        UIHelper.registEvent(gameObject, "small_", ons);
        picSprite = UIHelper.getByName<UITexture>(gameObject, "pic_");
        lineSprite = UIHelper.getByName<UISprite>(gameObject, "line");

        card_name = UIHelper.getByName<UILabel>(gameObject, "card_name");
        type_name = UIHelper.getByName<UILabel>(gameObject, "type_name");
        name_base = UIHelper.getByName<UI2DSprite>(gameObject, "name_base");
        type_base = UIHelper.getByName<UI2DSprite>(gameObject, "type_base");
        property = UIHelper.getByName<UIWidget>(gameObject, "property_");
        id_label = UIHelper.getByName<UILabel>(gameObject, "id_card_description");
        id_button = id_label.GetComponent<UIButton>();
        pic_button = UIHelper.getByName<UIButton>(gameObject, "pic_button");
        EventDelegate.Add(id_button.onClick, IdChange);
        EventDelegate.Add(pic_button.onClick, ShowDetail);

        pi = UIHelper.getByName<PropertyIcons>(gameObject, "property_");
        ai = UIHelper.getByName<AttributeIcons>(gameObject, "attribute_");
        li = UIHelper.getByName<LimitIcons>(gameObject, "limit_");

        try
        {
            description.textLabel.fontSize = int.Parse(Config.Get("fontSize", "14"));
        }
        catch (Exception e)
        {
        }

        read();
        myGraveStr = InterString.Get("我方墓地：");
        myExtraStr = InterString.Get("我方额外：");
        myBanishedStr = InterString.Get("我方除外：");
        opGraveStr = InterString.Get("[8888FF]对方墓地：[-]");
        opExtraStr = InterString.Get("[8888FF]对方额外：[-]");
        opBanishedStr = InterString.Get("[8888FF]对方除外：[-]");
        for (var i = 0; i < quickCards.Length; i++)
        {
            quickCards[i] = deckPanel.createCard();
            quickCards[i].relayer(i);
        }

        monitor.gameObject.SetActive(false);
    }

    private void ShowDetail()
    {
        Program.I().cardDetail.gameObject.SetActive(true);
        Program.I().cardDetail.Show(card);
    }

    bool showId;
    private void IdChange()
    {
        if(card != null && card.Id != 0)
        {
            showId = !showId;
            if (showId)
            {
                id_button.defaultColor = Color.yellow;
                id_label.text = card.Id.ToString();
            }
            else
            {
                id_button.defaultColor = Color.white;
                id_label.text = (card.Alias == 0 ? card.Id : card.Alias).ToString();
            }
        }
    }

    private void onb()
    {
        description.textLabel.fontSize += 1;
        description.Rebuild();
        Config.Set("fontSize", description.textLabel.fontSize.ToString());
    }

    private void ons()
    {
        description.textLabel.fontSize -= 1;
        description.Rebuild();
        Config.Set("fontSize", description.textLabel.fontSize.ToString());
    }

    private void onPre()
    {
        current--;
        loadData();
    }

    private void onNext()
    {
        current++;
        loadData();
    }

    public override void applyHideArrangement()
    {
        if (gameObject != null)
        {
            //underSprite.height = Utils.UIHeight() - 200;
            gameObject.transform.DOMoveX(Utils.UIToWorldPoint(new Vector3(-underSprite.width - 40, 0, 0)).x, 0.001f);
            setTitle("");
        }
    }

    public override void applyShowArrangement()
    {
        if (gameObject != null)
        {
            //underSprite.height = Utils.UIHeight() - 200;
            gameObject.transform.DOMoveX(Utils.UIToWorldPoint(new Vector3(0f, 0, 0)).x, 0.001f);
        }
    }

    public void read()
    {
        try
        {
            var ca = int.Parse(Config.Get("CA", "230"));
            var cb = int.Parse(Config.Get("CB", "270"));
            if (cb > Utils.UIHeight())
            {
                // some dumb ass repack the program and set the pic size so large that small screen users can't realize there is card description under it.
                cb = Utils.UIHeight() / 2;
                ca = (int) (cb * 0.68) + 50;
            }

            //underSprite.width = ca;
            //picSprite.height = cb;
        }
        catch (Exception e)
        {
        }
    }

    public void save()
    {
        Config.Set("CA", underSprite.width.ToString());
        Config.Set("CB", picSprite.height.ToString());
    }

    private void loadData()
    {
        if (current < 0) current = 0;
        if (current > datas.Count - 1) current = datas.Count - 1;
        if (datas.Count == 0) return;
        var d = datas[current];
        apply(d.card, d.def, d.tail);
    }

    public bool ifShowingThisCard(Card card)
    {
        return currentCard == card;
    }

    private void apply(Card card, Texture2D def, string tail, int player =0)
    {
        this.card = card;
        if (!isShowed & !forcedClosed)
            show();

        description.Clear();
        if (GameStringHelper.attribute(card.Attribute).Length > 1)
            tail = GameStringHelper.attribute(card.Attribute) + "\n" + tail;

        if (card.Alias > 0)
            if (card.Alias != card.Id)
            {
                var name = CardsManager.Get(card.Alias).Name;
                if (name != card.Name) tail += "[" + CardsManager.Get(card.Alias).Name + "]\n";
            }

        if (GameStringHelper.getSetName(card.Setcode) != "")
        {
            tail += GameStringHelper.xilie;
            tail += GameStringHelper.getSetName(card.Setcode);
            tail += "\n";
        }

        description.Add("[FFD700]" + tail + "[-]" + card.Desc);
        card_name.text = card.Name;
        type_name.text = GameStringHelper.GetType(card);
        FrameColor(card);
        SetProperty(card);
        SetAttribute(card);
        SetLimit(card);
        if (card.Id != 0)
        {
            if(showId)
                id_label.text = card.Id.ToString();
            else
                id_label.text = (card.Alias == 0 ? card.Id : card.Alias).ToString();
        }
        else 
            id_label.text = "";
        picLoader.player = player;
        picLoader.code = card.Id;
        picLoader.defaults = def;
        currentCard = card;
        shiftCardShower(true);
        //Program.go(50, () => { shiftCardShower(true); });
    }

    private void SetLimit(Card data)
    {
        if (data == null)
        {
            li.ChangeIcon(3);
        }
        else if (Program.I().deckManager != null)
        {
            if (Program.I().deckManager.currentBanlist == null)
            {
                li.ChangeIcon(3);
                return;
            }
            li.ChangeIcon(Program.I().deckManager.currentBanlist.GetQuantity(data.Id));
        }

    }
    private void SetAttribute(Card data)
    {
        if(data.Id == 0)
        {
            ai.ChangeAttribute("无");
            return;
        }
        Card origin = Card.Get(data.Id);
        if ((origin.Type & (uint)CardType.Monster) > 0 || (data.Type & (uint)CardType.Monster) > 0)
            ai.ChangeAttribute(GameStringHelper.attribute(data.Attribute).Substring(0, 1));
        else if ((data.Type & (uint)CardType.Spell) > 0)
            ai.ChangeAttribute("魔法");
        else if ((data.Type & (uint)CardType.Trap) > 0)
            ai.ChangeAttribute("陷阱");
    }

    private void SetProperty(Card data)
    {
        GameObject property_ = null;
        for(int i = 0; i<5; i++)
            property.transform.GetChild(i).gameObject.SetActive(false);

        Card origin = Card.Get(data.Id);

        if (origin == null)
            return;
        string upColor = "[05C7FB]";
        string downColor = "[FD3E08]";

        if ((data.Type & (uint)CardType.Monster) > 0 || (origin.Type & (uint)CardType.Monster) > 0)
        {
            if ((data.Type & (uint)CardType.Pendulum) > 0)
            {
                property_ = property.transform.Find("monster_pendulum").gameObject;

                if(data.Level > origin.Level)
                    property_.transform.Find("lv_num").GetComponent<UILabel>().text = upColor + data.Level.ToString() + "[-]";
                else if (data.Level < origin.Level)
                    property_.transform.Find("lv_num").GetComponent<UILabel>().text = downColor + data.Level.ToString() + "[-]";
                else
                    property_.transform.Find("lv_num").GetComponent<UILabel>().text = data.Level.ToString();

                if(data.Attack == -2)
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = "?";
                else if (data.Attack > data.rAttack)
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = upColor + data.Attack.ToString() + "[-]";
                else if (data.Attack < data.rAttack)
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = downColor + data.Attack.ToString() + "[-]";
                else
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = data.Attack.ToString();
                
                if (data.Defense == -2)
                    property_.transform.Find("def_num").GetComponent<UILabel>().text = "?";
                else if (data.Defense > data.rDefense)
                    property_.transform.Find("def_num").GetComponent<UILabel>().text = upColor + data.Defense.ToString() + "[-]";
                else if(data.Defense < data.rDefense)
                    property_.transform.Find("def_num").GetComponent<UILabel>().text = downColor + data.Defense.ToString() + "[-]";
                else
                    property_.transform.Find("def_num").GetComponent<UILabel>().text = data.Defense.ToString();

                if(data.LScale > origin.LScale)
                    property_.transform.Find("pendulum_num").GetComponent<UILabel>().text = upColor + data.LScale.ToString() + "[-]";
                else if((data.LScale < origin.LScale))
                    property_.transform.Find("pendulum_num").GetComponent<UILabel>().text = downColor + data.LScale.ToString() + "[-]";
                else
                    property_.transform.Find("pendulum_num").GetComponent<UILabel>().text = data.LScale.ToString();

                SetRaceIcon(property_.transform.Find("race_").GetComponent<UI2DSprite>(), GameStringHelper.race(data.Race));

                if ((data.Type &(uint)CardType.Xyz) >0)
                    property_.transform.Find("lv_").GetComponent<UI2DSprite>().sprite2D = pi.rank;
                else if ((data.Type & (uint)CardType.Link) > 0)
                    property_.transform.Find("lv_").GetComponent<UI2DSprite>().sprite2D = pi.link;
                else
                    property_.transform.Find("lv_").GetComponent<UI2DSprite>().sprite2D = pi.lv;
            }
            else if ((data.Type & (uint)CardType.Link) > 0)
            {
                property_ = property.transform.Find("monster_link").gameObject;


                if(data.Level == 0)
                {
                    for (var i = 0; i < 32; i++)
                        if ((data.LinkMarker & (1 << i)) > 0)
                            data.Level++;
                }

                if (data.Level > origin.Level)
                    property_.transform.Find("lv_num").GetComponent<UILabel>().text = upColor + data.Level.ToString() + "[-]";
                else if (data.Level < origin.Level)
                    property_.transform.Find("lv_num").GetComponent<UILabel>().text = downColor + data.Level.ToString() + "[-]";
                else
                    property_.transform.Find("lv_num").GetComponent<UILabel>().text = data.Level.ToString();

                if (data.Attack == -2)
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = "?";
                else if (data.Attack > data.rAttack)
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = upColor + data.Attack.ToString() + "[-]";
                else if (data.Attack < data.rAttack)
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = downColor + data.Attack.ToString() + "[-]";
                else
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = data.Attack.ToString();

                SetRaceIcon(property_.transform.Find("race_").GetComponent<UI2DSprite>(), GameStringHelper.race(data.Race));
            }
            else if ((data.Type & (uint)CardType.Xyz) > 0)
            {
                property_ = property.transform.Find("monster_rank").gameObject;

                if (data.Level > origin.Level)
                    property_.transform.Find("lv_num").GetComponent<UILabel>().text = upColor + data.Level.ToString() + "[-]";
                else if (data.Level < origin.Level)
                    property_.transform.Find("lv_num").GetComponent<UILabel>().text = downColor + data.Level.ToString() + "[-]";
                else
                    property_.transform.Find("lv_num").GetComponent<UILabel>().text = data.Level.ToString();

                if (data.Attack == -2)
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = "?";
                else if (data.Attack > data.rAttack)
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = upColor + data.Attack.ToString() + "[-]";
                else if (data.Attack < data.rAttack)
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = downColor + data.Attack.ToString() + "[-]";
                else
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = data.Attack.ToString();

                if (data.Defense == -2)
                    property_.transform.Find("def_num").GetComponent<UILabel>().text = "?";
                else if (data.Defense > data.rDefense)
                    property_.transform.Find("def_num").GetComponent<UILabel>().text = upColor + data.Defense.ToString() + "[-]";
                else if (data.Defense < data.rDefense)
                    property_.transform.Find("def_num").GetComponent<UILabel>().text = downColor + data.Defense.ToString() + "[-]";
                else
                    property_.transform.Find("def_num").GetComponent<UILabel>().text = data.Defense.ToString();

                SetRaceIcon(property_.transform.Find("race_").GetComponent<UI2DSprite>(), GameStringHelper.race(data.Race));
            }
            else
            {
                property_ = property.transform.Find("monster_lv").gameObject;

                if (data.Level > origin.Level)
                    property_.transform.Find("lv_num").GetComponent<UILabel>().text = upColor + data.Level.ToString() + "[-]";
                else if (data.Level < origin.Level)
                    property_.transform.Find("lv_num").GetComponent<UILabel>().text = downColor + data.Level.ToString() + "[-]";
                else
                    property_.transform.Find("lv_num").GetComponent<UILabel>().text = data.Level.ToString();

                if (data.Attack == -2)
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = "?";
                else if (data.Attack > data.rAttack)
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = upColor + data.Attack.ToString() + "[-]";
                else if (data.Attack < data.rAttack)
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = downColor + data.Attack.ToString() + "[-]";
                else
                    property_.transform.Find("atk_num").GetComponent<UILabel>().text = data.Attack.ToString();

                if (data.Defense == -2)
                    property_.transform.Find("def_num").GetComponent<UILabel>().text = "?";
                else if (data.Defense > data.rDefense)
                    property_.transform.Find("def_num").GetComponent<UILabel>().text = upColor + data.Defense.ToString() + "[-]";
                else if (data.Defense < data.rDefense)
                    property_.transform.Find("def_num").GetComponent<UILabel>().text = downColor + data.Defense.ToString() + "[-]";
                else
                    property_.transform.Find("def_num").GetComponent<UILabel>().text = data.Defense.ToString();

                SetRaceIcon(property_.transform.Find("race_").GetComponent<UI2DSprite>(), GameStringHelper.race(data.Race));
            }
        }
        else if ((origin.Type & (uint)CardType.Spell) > 0 || (origin.Type & (uint)CardType.Trap) > 0)
        {
            property_ = property.transform.Find("spell_trap").gameObject;
            UI2DSprite uI2DSprite = property_.transform.Find("type_").GetComponent<UI2DSprite>();
            string type = "";
            if ((origin.Type & (uint)CardType.Counter) >0)
            {
                type += "反击";
                uI2DSprite.sprite2D = pi.counter;
            }
            else if ((origin.Type & (uint)CardType.Field) > 0)
            {
                type += "场地";
                uI2DSprite.sprite2D = pi.field;
            }
            else if((origin.Type & (uint)CardType.Equip) > 0)
            {
                type += "装备";
                uI2DSprite.sprite2D = pi.equip;
            }
            else if ((origin.Type & (uint)CardType.Continuous) > 0)
            {
                type += "永续";
                uI2DSprite.sprite2D = pi.continuous;
            }
            else if((origin.Type & (uint)CardType.QuickPlay) > 0)
            {
                type += "速攻";
                uI2DSprite.sprite2D = pi.quick_play;
            }
            else if ((origin.Type & (uint)CardType.Ritual) > 0)
            {
                type += "仪式";
                uI2DSprite.sprite2D = pi.ritual;
            }
            else
            {
                type += "通常";
                uI2DSprite.sprite2D = pi.nothing;
            }
            if ((origin.Type & (uint)CardType.Spell) > 0)
                    type += "魔法";
            else if ((origin.Type & (uint)CardType.Trap) > 0)
                type += "陷阱";

            property_.transform.Find("type_label").GetComponent<UILabel>().text = type;
        }
        if (property_ != null)
            property_.SetActive(true);
    }

    public void SetRaceIcon(UI2DSprite sprite , string race)
    {
        switch (race)
        {
            case "龙":
                sprite.sprite2D = pi.race_01;
                break;
            case "不死":
                sprite.sprite2D = pi.race_02;
                break;
            case "恶魔":
                sprite.sprite2D = pi.race_03;
                break;
            case "炎":
                sprite.sprite2D = pi.race_04;
                break;
            case "海龙":
                sprite.sprite2D = pi.race_05;
                break;
            case "岩石":
                sprite.sprite2D = pi.race_06;
                break;
            case "机械":
                sprite.sprite2D = pi.race_07;
                break;
            case "鱼":
                sprite.sprite2D = pi.race_08;
                break;
            case "恐龙":
                sprite.sprite2D = pi.race_09;
                break;
            case "昆虫":
                sprite.sprite2D = pi.race_10;
                break;
            case "兽":
                sprite.sprite2D = pi.race_11;
                break;
            case "兽战士":
                sprite.sprite2D = pi.race_12;
                break;
            case "植物":
                sprite.sprite2D = pi.race_13;
                break;
            case "水":
                sprite.sprite2D = pi.race_14;
                break;
            case "战士":
                sprite.sprite2D = pi.race_15;
                break;
            case "鸟兽":
                sprite.sprite2D = pi.race_16;
                break;
            case "天使":
                sprite.sprite2D = pi.race_17;
                break;
            case "魔法师":
                sprite.sprite2D = pi.race_18;
                break;
            case "雷":
                sprite.sprite2D = pi.race_19;
                break;
            case "爬虫类":
                sprite.sprite2D = pi.race_20;
                break;
            case "念动力":
                sprite.sprite2D = pi.race_21;
                break;
            case "幻龙":
                sprite.sprite2D = pi.race_22;
                break;
            case "电子界":
                sprite.sprite2D = pi.race_23;
                break;
            case "幻神兽":
                sprite.sprite2D = pi.race_24;
                break;
            case "创造神":
                sprite.sprite2D = pi.race_25;
                break;
            case "幻想魔":
                sprite.sprite2D = pi.race_26;
                break;
        }
    }

    public Color color1 = Color.white;
    public Color color2 = Color.white;

    public void FrameColor(Card data, bool change = true)
    {
        Color color1 = new Color(0.7764f, 0.6784f, 0.6274f, 1f);
        Color color2 = color1;
        if(data.Id == 0)
        {
            name_base.color = color1;
            type_base.color = color2;
            return;
        }
        Card origin = Card.Get(data.Id);
        if(origin == null)
        {
            name_base.color = color1;
            type_base.color = color2;
            return;
        }
        int type = origin.Type;
        if(data.Id == 10000000)
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
        if (change)
        {
            name_base.color = color1;
            type_base.color = color2;
        }
        this.color1 = color1;
        this.color2 = color2;
    }

    public void shiftCardShower(bool show)
    {
        if (show)
            cardShowerWidget.alpha = 1f;
        else
            cardShowerWidget.alpha = 0f;
        if (!show)
        {
            if (monitor.gameObject.activeInHierarchy == false)
            {
                monitor.gameObject.SetActive(true);
                realizeMonitor();
            }
        }
        else
        {
            monitor.gameObject.SetActive(false);
        }
    }


    public void realizeMonitor()
    {
        if (monitor.gameObject.activeInHierarchy)
        {
            var myGrave = new List<gameCard>();
            var myExtra = new List<gameCard>();
            var myBanished = new List<gameCard>();
            var opGrave = new List<gameCard>();
            var opExtra = new List<gameCard>();
            var opBanished = new List<gameCard>();
            for (var i = 0; i < Program.I().ocgcore.cards.Count; i++)
            {
                var curCard = Program.I().ocgcore.cards[i];
                var code = curCard.get_data().Id;
                var gps = curCard.p;
                if (code > 0)
                {
                    if (gps.controller == 0)
                    {
                        if ((gps.location & (uint) CardLocation.Grave) > 0) myGrave.Add(curCard);
                        if ((gps.location & (uint) CardLocation.Removed) > 0) myBanished.Add(curCard);
                        if ((gps.location & (uint) CardLocation.Extra) > 0) myExtra.Add(curCard);
                    }
                    else
                    {
                        if ((gps.location & (uint) CardLocation.Grave) > 0) opGrave.Add(curCard);
                        if ((gps.location & (uint) CardLocation.Removed) > 0) opBanished.Add(curCard);
                        if ((gps.location & (uint) CardLocation.Extra) > 0) opExtra.Add(curCard);
                    }
                }
            }

            currentHeight = 0;
            currentLabelIndex = 0;
            currentCardIndex = 0;
            handleMonitorArea(opGrave, opGraveStr);
            handleMonitorArea(opBanished, opBanishedStr);
            handleMonitorArea(opExtra, opExtraStr);
            handleMonitorArea(myGrave, myGraveStr);
            handleMonitorArea(myBanished, myBanishedStr);
            handleMonitorArea(myExtra, myExtraStr);
            while (currentLabelIndex < 6)
            {
                deckPanel.labs[currentLabelIndex].gameObject.SetActive(false);
                currentLabelIndex++;
            }

            while (currentCardIndex < 300)
            {
                quickCards[currentCardIndex].clear();
                currentCardIndex++;
            }
        }
    }

    private void handleMonitorArea(List<gameCard> list, string hint)
    {
        if (list.Count > 0)
        {
            deckPanel.labs[currentLabelIndex].gameObject.SetActive(true);
            deckPanel.labs[currentLabelIndex].text = hint;
            deckPanel.labs[currentLabelIndex].width = (int) (monitor.width - 12);
            deckPanel.labs[currentLabelIndex].transform.localPosition = new Vector3(monitor.width / 2,
                (monitor.height - 8) / 2 - 12 - currentHeight, 0);
            currentLabelIndex++;
            currentHeight += 24;
            float beginX = 6 + 22;
            var beginY = monitor.height / 2 - currentHeight - 36;
            eachLine = (int) ((monitor.width - 12f) / 44f);
            for (var i = 0; i < list.Count; i++)
            {
                var gp = UIHelper.get_hang_lie(i, eachLine);
                quickCards[currentCardIndex].code = list[i].get_data().Id;
                quickCards[currentCardIndex].transform.localPosition =
                    new Vector3(beginX + 44 * gp.y, beginY - 60 * gp.x, 0);
                currentCardIndex++;
            }

            var hangshu = list.Count / eachLine;
            var yushu = list.Count % eachLine;
            if (yushu > 0) hangshu++;
            currentHeight += 60 * hangshu;
        }
    }

    public void onResized()
    {
        if (monitor.gameObject.activeInHierarchy)
        {
            var newEach = (int) ((monitor.width - 12f) / 44f);
            if (newEach != eachLine || monitorHeight != monitor.height)
            {
                monitorHeight = monitor.height;
                eachLine = newEach;
                realizeMonitor();
            }
        }
    }

    public void setData(Card card, Texture2D def, string tail = "", bool force = false, int player =0)
    {
        if (cardShowerWidget.alpha == 0 && force == false) return;
        if (card == null) return;
        if (card.Id == 0)
        {
            apply(card, def, tail, player);
            return;
        }

        if (datas.Count > 0)
            if (datas[datas.Count - 1].card.Id == card.Id)
            {
                datas[datas.Count - 1] = new data
                {
                    card = card,
                    def = def,
                    tail = tail
                };
                if (datas.Count > 300) datas.RemoveAt(0);
                current = datas.Count - 1;
                loadData();
                return;
            }

        datas.Add(new data
        {
            card = card,
            def = def,
            tail = tail
        });
        if (datas.Count > 300) datas.RemoveAt(0);
        current = datas.Count - 1;
        loadData();
    }

    public void setTitle(string title)
    {
        UIHelper.trySetLableText(gameObject, "title_", title);
    }

    public void mLog(string result)
    {
        Logs.Add(result);
        var all = "";
        for (var i = 0; i < Logs.Count; i++)
            if (i == Logs.Count - 1)
                all += Logs[i].Replace("\0", "");
            else
                all += Logs[i].Replace("\0", "") + "\n";
        UIHelper.trySetLableTextList(UIHelper.getByName(gameObject, "chat_"), all);
        Program.go(8000, clearOneLog);
    }

    private void clearOneLog()
    {
        if (Logs.Count > 0)
        {
            Logs.RemoveAt(0);
            var all = "";
            foreach (var item in Logs) all += item + "\n";
            try
            {
                all = all.Substring(0, all.Length - 1);
            }
            catch (Exception e)
            {
            }

            UIHelper.trySetLableTextList(UIHelper.getByName(gameObject, "chat_"), all);
        }
        else
        {
            UIHelper.trySetLableTextList(UIHelper.getByName(gameObject, "chat_"), "");
        }
    }

    public void clearAllLog()
    {
        Program.notGo(clearOneLog);
        Logs.Clear();
        UIHelper.trySetLableTextList(UIHelper.getByName(gameObject, "chat_"), "");
    }

    private class data
    {
        public Card card;
        public Texture2D def;
        public string tail = "";
    }
}