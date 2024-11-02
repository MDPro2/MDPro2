using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YGOSharp;

public class DeckManagerMono : MonoBehaviour
{
    public GameObject cardOnManager;
    public GameObject cardOnBook;
    public List<CardOnManager> cardsOnManager = new List<CardOnManager>();
    public Transform cardsContainer;
    public UIPanel mainWindow_;
    public Sprite tab_on;
    public Sprite tab_off;
    public Sprite tab_over;
    public Sprite markOn;
    public Sprite markOff;
    public Sprite markOver;
    public Sprite card_normal;
    public Sprite card_effect;
    public Sprite card_ritual;
    public Sprite card_fusion;
    public Sprite card_spell;
    public Sprite card_trap;
    public Sprite card_synchro;
    public Sprite card_xyz;
    public Sprite card_p_normal;
    public Sprite card_p_effect;
    public Sprite card_p_xyz;
    public Sprite card_p_synchro;
    public Sprite card_p_fusion;
    public Sprite card_link;
    public Sprite card_p_ritual;
    public Sprite toggleOn;
    public Sprite toggleOff;
    public Sprite toggleOffOver;

    public SearchFilter filter_;

    //Top
    public GameObject top_;
    public UIButton exit_;
    public UIPopupList banlist_;
    public UIButton copy_;
    public UIButton restore_;
    public UIButton sort_;
    public UIButton random_;
    public UIButton save_;


    public UIButton decoration_;
    public UI2DSprite decoration_case;
    public UI2DSprite decoration_protector;
    public UI2DSprite decoration_field;
    public UI2DSprite decoration_grave;
    public UI2DSprite decoration_stand;
    public UI2DSprite decoration_mate;

    public UIButton finish_;
    public GameObject editDeck_;
    public GameObject changeSide_;

    //Left
    public GameObject left_;
    public GameObject descriptionPage;

    public UI2DSprite name_base;
    public UI2DSprite type_base;

    public UILabel cardName_;
    public UI2DSprite attribute_;
    public UITexture cardFace_;
    public UIButton card_button;

    public PropertyIcons propertyIcons;
    public UI2DSprite lv_icon;
    public UILabel lv_num;
    public UI2DSprite pendulum_icon;
    public UILabel pendulum_num;
    public UI2DSprite atk_icon;
    public UILabel atk_num;
    public UI2DSprite def_icon;
    public UILabel def_num;
    public UI2DSprite race_;
    public UI2DSprite spell_type;
    public UILabel spell_label;

    public UI2DSprite limit_;
    public UILabel id_;
    public UILabel type_;
    public UITextList description_;
    public UIButton mark_;
    public UIButton plus_;
    public UIButton minus_;
    public UIButton related_;
    public UIButton book_;
    //Middle
    public GameObject middle_;
    public UIInput deckName_;
    public UILabel mainCount_;
    public UILabel extraCount_;
    public UILabel sideCount_;

    public UI2DSprite event_main;
    public UI2DSprite event_extra;
    public UI2DSprite event_side;
    public UI2DSprite small_deckCase;

    //Right
    public GameObject right_;
    public ButtonTab tab_search;
    public ButtonTab tab_book;
    public ButtonTab tab_history;
    public UIPanel tab_panel;
    public UIScrollBar tab_bar;
    public UIInput search_input;
    public UIButton search_clean;
    public UIButton search_button;
    public UILabel search_label;
    public UIButton search_filter;
    public UIButton search_sort;
    public UIButton search_reset;

    public UITexture related_card;
    public UIButton related_button;
    public UILabel related_label;
    public UIButton related_return;

    private void Awake()
    {
        mainWindow_.alpha = 1.0f;
        gameObject.SetActive(false);
    }

    private void Update()
    {
    }


    public GameObject CreateCard(int id, int code)
    {
        GameObject card = Instantiate(cardOnManager);
        card.GetComponent<CardOnManager>().id = id;
        card.GetComponent<CardOnManager>().code = code;

        card.transform.SetParent(cardsContainer, false);
        cardsOnManager.Add(card.GetComponent<CardOnManager>());
        return card;
    }

    public void DestroyCards()
    {
        foreach (var card in cardsOnManager)
            Destroy(card.gameObject);
        cardsOnManager.Clear();
    }
}
