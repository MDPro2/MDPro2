using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using YGOSharp;
using YGOSharp.OCGWrapper.Enums;

public class gameHiddenButton : OCGobject
{
    private bool excited;

    public TextMaster hintText;
    public CardLocation location;

    public int player;

    private readonly GPS ps;

    gameButton spButton;
    gameButton actButton;

    GameObject activateNotification;
    GameObject activateEffect;
    Material material;

    GameObject cardsCount;
    public gameHiddenButton(CardLocation l, int p, Vector3 rotation, Vector3 size)
    {
        ps = new GPS();
        ps.controller = (uint) p;
        ps.location = (uint) l;
        ps.position = 0;
        ps.sequence = 0;
        player = p;
        location = l;
        gameObject = create(Program.I().mod_ocgcore_hidden_button);
        Program.I().ocgcore.AddUpdateAction_s(Update);
        gameObject.transform.position = Program.I().ocgcore.get_point_worldposition(ps);
        gameObject.transform.eulerAngles = rotation;
        gameObject.transform.localScale = size;

        spButton = new gameButton(-1, InterString.Get("特殊召唤@ui"), superButtonType.spsummon);
        actButton = new gameButton(-1, InterString.Get("发动效果@ui"), superButtonType.act);
        spButton.gameHiddenButton = this;
        actButton.gameHiddenButton = this;

        if((location & CardLocation.Extra) > 0 && player == 0)
        {
            activateNotification = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_exdeck_001");
            activateNotification.transform.parent = gameObject.transform;
            activateNotification.transform.localPosition = Vector3.zero;
            activateNotification.transform.localEulerAngles = Vector3.zero;
            activateNotification.transform.localScale = Vector3.one;
            activateNotification.transform.GetChild(0).gameObject.SetActive(false);
            activateNotification.SetActive(false);

            activateEffect = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_active/fxp_hl_active_exdeck_001");
            activateEffect.transform.parent = gameObject.transform;
            activateEffect.transform.localPosition = Vector3.zero;
            activateEffect.transform.localEulerAngles = Vector3.zero;
            activateEffect.transform.localScale = new Vector3(1 / activateEffect.transform.parent.localScale.x, 1 / activateEffect.transform.parent.localScale.y, 1 / activateEffect.transform.parent.localScale.z);
            activateEffect.SetActive(false);
        }
        else if ((location & CardLocation.Deck) > 0 && player == 0)
        {
            activateNotification = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_exdeck_001");
            activateNotification.transform.parent = gameObject.transform;
            activateNotification.transform.localPosition = Vector3.zero;
            activateNotification.transform.localEulerAngles = Vector3.zero;
            activateNotification.transform.localScale = Vector3.one;
            activateNotification.transform.GetChild(0).gameObject.SetActive(false);
            activateNotification.SetActive(false);

            activateEffect = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_active/fxp_hl_active_exdeck_001");
            activateEffect.transform.parent = gameObject.transform;
            activateEffect.transform.localPosition = Vector3.zero;
            activateEffect.transform.localEulerAngles = Vector3.zero;
            activateEffect.transform.localScale = new Vector3(1 / activateEffect.transform.parent.localScale.x, 1 / activateEffect.transform.parent.localScale.y, 1 / activateEffect.transform.parent.localScale.z);
            activateEffect.SetActive(false);
        }
        else if ((location & CardLocation.Grave) > 0)
        {
            if(player == 0)
            {
                activateNotification = LoadAssets.grave1_.Find("fxp_HL_som_grave_001_near/fxp_HL_som_grave_001").gameObject;
            }
            else
                activateNotification = LoadAssets.grave2_.Find("fxp_HL_som_grave_001_far/fxp_HL_som_grave_001").gameObject;

            activateEffect = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_active/fxp_hl_active_grave_001");
            activateEffect.transform.parent = gameObject.transform;
            activateEffect.transform.localPosition = Vector3.zero;
            activateEffect.transform.localEulerAngles = Vector3.zero;
            activateEffect.transform.localScale = new Vector3(1 / activateEffect.transform.parent.localScale.x, 1 / activateEffect.transform.parent.localScale.y, 1 / activateEffect.transform.parent.localScale.z);
            activateEffect.SetActive(false);

            GetGraveMaterial();
        }
        else if ((location & CardLocation.Removed) > 0)
        {
            if(player == 0)
            {
                Transform exclude = null;
                exclude = LoadAssets.grave1_.Find("fxp_HL_som_exclude_001_near/fxp_HL_som_exclude_001");
                if (exclude == null)
                    exclude = LoadAssets.grave1_.Find("POS_HLsom_exclude/fxp_HL_som_exclude_001_near/fxp_HL_som_exclude_001");
                if (exclude == null)
                    Debug.LogError("未找到我方场地附件中除外部分的activateNotification");

                activateNotification = exclude.gameObject;
            }
            else
            {
                Transform exclude = null;
                exclude = LoadAssets.grave2_.Find("fxp_HL_som_exclude_001_far/fxp_HL_som_exclude_001");
                if (exclude == null)
                    exclude = LoadAssets.grave2_.Find("POS_HLsom_exclude/fxp_HL_som_exclude_001_far/fxp_HL_som_exclude_001");
                if (exclude == null)
                    Debug.LogError("未找到对方场地附件中除外部分的activateNotification");

                activateNotification = exclude.gameObject;
            }
            activateEffect = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_active/fxp_hl_active_exclude_001");
            activateEffect.transform.parent = gameObject.transform;
            activateEffect.transform.localPosition = Vector3.zero;
            activateEffect.transform.localEulerAngles = Vector3.zero;
            activateEffect.transform.localScale = new Vector3(1 / activateEffect.transform.parent.localScale.x, 1 / activateEffect.transform.parent.localScale.y, 1 / activateEffect.transform.parent.localScale.z);
            activateEffect.SetActive(false);
            GetGraveMaterial();
        }
    }

    public async void PlayParticle(uint controller, uint location, bool isIn, float time)
    {
        if ((uint)this.location == location && player == controller)
        {
            await Task.Delay((int)(time * 1000));
            if (LoadAssets.grave1_ == null || LoadAssets.grave2_ == null)
                return;
            if((location & (uint)CardLocation.Grave) > 0)
            {
                if(controller == 0)
                {
                    if (isIn)
                    {
                        LoadAssets.grave1_.Find("POS_Grave/fxp_UI_grave_in_001").GetComponent<ParticleSystem>().Play();
                        LoadAssets.grave1_.Find("POS_Grave/fxp_UI_grave_inend_001").GetComponent<ParticleSystem>().Play();
                        UIHelper.playSound("SE_DUEL/SE_CEMETARY_ABSORB", 1);
                    }
                    else
                    {
                        LoadAssets.grave1_.Find("POS_Grave/fxp_UI_grave_out_001").GetComponent<ParticleSystem>().Play();
                        LoadAssets.grave1_.Find("POS_Grave/fxp_UI_grave_outend_001").GetComponent<ParticleSystem>().Play();
                        UIHelper.playSound("SE_DUEL/SE_CEMETARY_GOOUT", 1);
                    }
                }
                else
                {
                    if (isIn)
                    {
                        LoadAssets.grave2_.Find("POS_Grave/fxp_UI_grave_in_001").GetComponent<ParticleSystem>().Play();
                        LoadAssets.grave2_.Find("POS_Grave/fxp_UI_grave_inend_001").GetComponent<ParticleSystem>().Play();
                        UIHelper.playSound("SE_DUEL/SE_CEMETARY_ABSORB", 1);
                    }
                    else
                    {
                        LoadAssets.grave2_.Find("POS_Grave/fxp_UI_grave_out_001").GetComponent<ParticleSystem>().Play();
                        LoadAssets.grave2_.Find("POS_Grave/fxp_UI_grave_outend_001").GetComponent<ParticleSystem>().Play();
                        UIHelper.playSound("SE_DUEL/SE_CEMETARY_GOOUT", 1);
                    }
                }
            }
            else if ((location & (uint)CardLocation.Removed) > 0)
            {
                if (controller == 0)
                {
                    if (isIn)
                    {
                        LoadAssets.grave1_.Find("POS_Exclude/fxp_UI_exclude_in_001").GetComponent<ParticleSystem>().Play();
                        LoadAssets.grave1_.Find("POS_Exclude/fxp_UI_exclude_inend_001").GetComponent<ParticleSystem>().Play();
                        UIHelper.playSound("SE_DUEL/SE_EXCLUSION_ABSORB", 1);
                    }
                    else
                    {
                        LoadAssets.grave1_.Find("POS_Exclude/fxp_UI_exclude_out_001").GetComponent<ParticleSystem>().Play();
                        LoadAssets.grave1_.Find("POS_Exclude/fxp_UI_exclude_outend_001").GetComponent<ParticleSystem>().Play();
                        UIHelper.playSound("SE_DUEL/SE_EXCLUSION_GOOUT", 1);
                    }
                }
                else
                {
                    if (isIn)
                    {
                        LoadAssets.grave2_.Find("POS_Exclude/fxp_UI_exclude_in_001").GetComponent<ParticleSystem>().Play();
                        LoadAssets.grave2_.Find("POS_Exclude/fxp_UI_exclude_inend_001").GetComponent<ParticleSystem>().Play();
                        UIHelper.playSound("SE_DUEL/SE_EXCLUSION_ABSORB", 1);
                    }
                    else
                    {
                        LoadAssets.grave2_.Find("POS_Exclude/fxp_UI_exclude_out_001").GetComponent<ParticleSystem>().Play();
                        LoadAssets.grave2_.Find("POS_Exclude/fxp_UI_exclude_outend_001").GetComponent<ParticleSystem>().Play();
                        UIHelper.playSound("SE_DUEL/SE_EXCLUSION_GOOUT", 1);
                    }
                }
            }
        }
    }

    public void GetGraveMaterial()
    {
        if ((location & CardLocation.Grave) > 0 || (location & CardLocation.Removed) > 0)
        {
            if ((location & CardLocation.Grave) > 0)
            {
                if (player == 0)
                {
                    activateNotification = LoadAssets.grave1_.Find("fxp_HL_som_grave_001_near/fxp_HL_som_grave_001").gameObject;
                }
                else
                    activateNotification = LoadAssets.grave2_.Find("fxp_HL_som_grave_001_far/fxp_HL_som_grave_001").gameObject;
            }
            if ((location & CardLocation.Removed) > 0)
            {
                if (player == 0)
                {
                    Transform exclude = null;
                    exclude = LoadAssets.grave1_.Find("fxp_HL_som_exclude_001_near/fxp_HL_som_exclude_001");
                    if (exclude == null)
                        exclude = LoadAssets.grave1_.Find("POS_HLsom_exclude/fxp_HL_som_exclude_001_near/fxp_HL_som_exclude_001");
                    if (exclude == null)
                        Debug.LogError("未找到我方场地附件中除外部分的activateNotification");

                    activateNotification = exclude.gameObject;
                }
                else
                {
                    Transform exclude = null;
                    exclude = LoadAssets.grave2_.Find("fxp_HL_som_exclude_001_far/fxp_HL_som_exclude_001");
                    if (exclude == null)
                        exclude = LoadAssets.grave2_.Find("POS_HLsom_exclude/fxp_HL_som_exclude_001_far/fxp_HL_som_exclude_001");
                    if (exclude == null)
                        Debug.LogError("未找到对方场地附件中除外部分的activateNotification");

                    activateNotification = exclude.gameObject;
                }
            }
            Transform graveBase = activateNotification.transform.parent.parent.Find("base");
            if (graveBase == null)
                graveBase = activateNotification.transform.parent.parent.Find("base_near");
            if (graveBase == null)
                graveBase = activateNotification.transform.parent.parent.Find("base_far");
            if (graveBase == null)
                graveBase = activateNotification.transform.parent.parent.Find("body_near");
            if (graveBase == null)
                graveBase = activateNotification.transform.parent.parent.Find("body_far");
            if (graveBase == null)
                graveBase = activateNotification.transform.parent.parent.Find("root/base");
            if (graveBase == null)
                graveBase = activateNotification.transform.parent.parent.parent.Find("root/base");
            if (graveBase == null)
                graveBase = activateNotification.transform.parent.parent.Find("basic");
            if (graveBase == null)
                graveBase = activateNotification.transform.parent.parent.Find("Grave_022_far/base_far");
            if (graveBase == null)
                graveBase = activateNotification.transform.parent.parent.Find("Grave_022_near/base_near");
            if (graveBase == null)
                Debug.LogError("未找到场地Material");
            material = graveBase.GetComponent<Renderer>().material;
        }
    }

    public void superButonOnClick(gameButton button)
    {
        List<gameCard> cards = new List<gameCard>();
        for (var i = 0; i < Program.I().ocgcore.cards.Count; i++)
        {
            if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
                if ((Program.I().ocgcore.cards[i].p.location & (uint)location) > 0)
                    if (Program.I().ocgcore.cards[i].p.controller == player)
                    {
                        if (Program.I().ocgcore.cards[i].buttons.Count > 0)
                        {
                            foreach (var btn in Program.I().ocgcore.cards[i].buttons)
                            {
                                if (btn.type == button.type)
                                    cards.Add(Program.I().ocgcore.cards[i]);
                            }
                        }
                    }
        }
        Program.I().cardSelection.show(cards, true, 1, 1, "", button.type);
    }

    public void dispose()
    {
        Program.I().ocgcore.RemoveUpdateAction_s(Update);
    }

    public void hideButton(GameObject go)
    {
        if(go == null || go != gameObject)
        {
            spButton.hide();
            actButton.hide();
        }
    }

    public void CheckCount()
    {
        if (activateNotification != null && activateNotification.GetComponent<Animator>() != null)
        {
            int cardsInPosition = 0;
            for (int i = 0; i < Program.I().ocgcore.cards.Count; i++)
                if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
                    if ((Program.I().ocgcore.cards[i].p.location & (uint)location) > 0)
                        if (Program.I().ocgcore.cards[i].p.controller == player)
                            cardsInPosition++;

            Transform grave_idle = activateNotification.transform.parent.parent.Find("POS_Grave_idle");
            Transform exclude_idle = null;
            if (activateNotification.transform.parent.parent.name.StartsWith("Grave_"))
                exclude_idle = activateNotification.transform.parent.parent.Find("POS_Exclude_idle");
            else
                exclude_idle = activateNotification.transform.parent.parent.parent.Find("POS_Exclude_idle");

            if (cardsInPosition > 10)
            {
                if ((location & CardLocation.Grave) > 0)
                    grave_idle.GetChild(2).GetComponent<ParticleSystem>().Play();
                else if ((location & CardLocation.Removed) > 0)
                    exclude_idle.GetChild(2).GetComponent<ParticleSystem>().Play();
            }
            else if (cardsInPosition > 5)
            {
                if ((location & CardLocation.Grave) > 0)
                    grave_idle.GetChild(1).GetComponent<ParticleSystem>().Play();
                else if ((location & CardLocation.Removed) > 0)
                    exclude_idle.GetChild(1).GetComponent<ParticleSystem>().Play();
            }
            if(cardsInPosition > 0)
            {
                if((location & CardLocation.Grave) > 0)
                {
                    material.SetFloat("_GraveCardExist", 1);
                    grave_idle.GetChild(0).GetComponent<ParticleSystem>().Play();
                }
                else if ((location & CardLocation.Removed) > 0)
                {
                    material.SetFloat("_ExcludeCardExist", 1);
                    exclude_idle.GetChild(0).GetComponent<ParticleSystem>().Play();
                }
            }
            else
            {
                if ((location & CardLocation.Grave) > 0)
                {
                    material.SetFloat("_GraveCardExist", 0);
                    grave_idle.GetChild(0).GetComponent<ParticleSystem>().Stop();
                    grave_idle.GetChild(1).GetComponent<ParticleSystem>().Stop();
                    grave_idle.GetChild(2).GetComponent<ParticleSystem>().Stop();
                }
                else if ((location & CardLocation.Removed) > 0)
                {
                    material.SetFloat("_ExcludeCardExist", 0);
                    exclude_idle.GetChild(0).GetComponent<ParticleSystem>().Stop();
                    exclude_idle.GetChild(1).GetComponent<ParticleSystem>().Stop();
                    exclude_idle.GetChild(2).GetComponent<ParticleSystem>().Stop();
                }
            }
        }
    }

    public void CheckNotification()
    {
        if (activateNotification != null)
        {
            bool activated = false;
            for (int i = 0; i < Program.I().ocgcore.cards.Count; i++)
                if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
                    if ((Program.I().ocgcore.cards[i].p.location & (uint)location) > 0)
                        if (Program.I().ocgcore.cards[i].p.controller == player)
                        {
                            if (Program.I().ocgcore.cards[i].buttons.Count > 0)
                            {
                                activated = true;
                                break;
                            }
                        }
            if (activated)
            {
                activateNotification.SetActive(true);
                if (activateNotification.GetComponent<Animator>() != null)
                    activateNotification.GetComponent<Animator>().SetBool("On", true);
                if(activateEffect != null)
                    activateEffect.SetActive(true);
                UIHelper.playSound("SE_DUEL/SE_DUEL_ACTIVE_POSSIBLE", 1);
            }
            else
            {
                activateNotification.SetActive(false);
                if (activateEffect != null)
                    activateEffect.SetActive(false);
            }
        }
    }


    bool hover;
    public void Update()
    {
        //Hover
         if (Program.pointedGameObject == gameObject)
        {
            if(!hover)
            {
                hover = true;

                var vector = gameObject.transform.position;
                vector = Program.I().main_camera.WorldToScreenPoint(vector);
                vector.y += 20;
                vector = Program.I().camera_main_2d.ScreenToWorldPoint(vector);

                if (player == 0 && ((uint)location & ((uint)CardLocation.Deck + (uint)CardLocation.Grave + (uint)CardLocation.Removed)) >0)
                    cardsCount = create(Program.I().cards_count_left, vector, Vector3.zero, false, Program.I().ui_back_ground_2d);
                if (player == 0 && ((uint)location & (uint)CardLocation.Extra) > 0)
                    cardsCount = create(Program.I().cards_count_right, vector, Vector3.zero, false, Program.I().ui_back_ground_2d);
                if (player == 1 && ((uint)location & ((uint)CardLocation.Deck + (uint)CardLocation.Grave + (uint)CardLocation.Removed)) > 0)
                    cardsCount = create(Program.I().cards_count_right, vector, Vector3.zero, false, Program.I().ui_back_ground_2d);
                if (player == 1 && ((uint)location & (uint)CardLocation.Extra) > 0)
                    cardsCount = create(Program.I().cards_count_left, vector, Vector3.zero, false, Program.I().ui_back_ground_2d);
                cardsCount.transform.parent = Program.I().ui_back_ground_2d.transform;
                cardsCount.transform.localScale = Vector3.one;

                //cardsCount.transform.localPosition = vector;
                cardsCount.GetComponent<UIWidget>().alpha = 0f;
                DOTween.To(() => cardsCount.GetComponent<UIWidget>().alpha, x => cardsCount.GetComponent<UIWidget>().alpha = x, 1, 0.2f);

                int cardsInPosition = 0;
                for (int i = 0; i < Program.I().ocgcore.cards.Count; i++)
                    if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
                        if ((Program.I().ocgcore.cards[i].p.location & (uint)location) > 0)
                            if (Program.I().ocgcore.cards[i].p.controller == player)
                                cardsInPosition++;
                cardsCount.GetComponentInChildren<UILabel>().text = cardsInPosition.ToString();
            }
            if (material != null && (location & CardLocation.Grave) > 0)
            {
                material.SetFloat("_GraveMouseOver", 1);
            }
            if (material != null && (location & CardLocation.Removed) > 0)
            {
                material.SetFloat("_ExcludeMouseOver", 1);
            }
            

        }
        else
        {
            if (hover && cardsCount != null)
            {
                hover = false;
                DOTween.To(() => cardsCount.GetComponent<UIWidget>().alpha, x => cardsCount.GetComponent<UIWidget>().alpha = x, 0, 0.2f);
                MonoBehaviour.Destroy(cardsCount, 0.3f);
            }

            if (material != null && (location & CardLocation.Grave) > 0)
            {
                material.SetFloat("_GraveMouseOver", 0);
            }
            if (material != null && (location & CardLocation.Removed) > 0)
            {
                material.SetFloat("_ExcludeMouseOver", 0);
            }
        }
        //Click
        if (Program.pointedGameObject == gameObject && Program.InputGetMouseButton_0)
        {
            if (material != null && (location & CardLocation.Grave) > 0)
            {
                material.SetFloat("_GravePressButton", 1);
            }
            if (material != null && (location & CardLocation.Removed) > 0)
            {
                material.SetFloat("_ExcludePressButton", 1);
            }
        }
        else
        {
            if (material != null && (location & CardLocation.Grave) > 0)
            {
                material.SetFloat("_GravePressButton", 0);
            }
            if (material != null && (location & CardLocation.Removed) > 0)
            {
                material.SetFloat("_ExcludePressButton", 0);
            }
        }
        //ClickUp
        if (Program.pointedGameObject == gameObject && Program.InputGetMouseButtonUp_0)
            {
                showAll();

            if (!Program.I().cardSelection.isShowed)
            {
                bool spsummon = false;
                bool activate = false;
                for (var i = 0; i < Program.I().ocgcore.cards.Count; i++)
                    if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
                        if ((Program.I().ocgcore.cards[i].p.location & (uint)location) > 0)
                            if (Program.I().ocgcore.cards[i].p.controller == player)
                            {
                                if (Program.I().ocgcore.cards[i].buttons.Count > 0)
                                {
                                    foreach (var btn in Program.I().ocgcore.cards[i].buttons)
                                    {
                                        if (btn.type == superButtonType.spsummon)
                                            spsummon = true;
                                        else if (btn.type == superButtonType.act)
                                            activate = true;
                                    }
                                }
                            }
                var vector_of_begin = gameObject.transform.position;
                vector_of_begin = Program.I().main_camera.WorldToScreenPoint(vector_of_begin);
                if (spsummon && activate)
                {
                    spButton.show(vector_of_begin + new Vector3(gameCard.SortLine(0, 2, 180 * Screen.height / 1080f), 150f * Screen.height / 1080f, 0f));
                    actButton.show(vector_of_begin + new Vector3(gameCard.SortLine(1, 2, 180 * Screen.height / 1080f), 150f * Screen.height / 1080f, 0f));
                }
                else if (spsummon)
                    spButton.show(vector_of_begin + new Vector3(gameCard.SortLine(0, 1, 180 * Screen.height / 1080f), 150f * Screen.height / 1080f, 0f));

                else if (activate)
                    actButton.show(vector_of_begin + new Vector3(gameCard.SortLine(0, 1, 180 * Screen.height / 1080f), 150f * Screen.height / 1080f, 0f));
            }
        }
    }

    private void showAll()
    {
        if (location == CardLocation.Grave && Program.I().ocgcore.cantCheckGrave)
        {
            Program.I().cardDescription.RMSshow_none(InterString.Get("不能确认墓地里的卡"));
            return;
        }

        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SELECT_01");

        //if (location == CardLocation.Deck)
        //    return;

        List <gameCard> cardlist = new List <gameCard>();
        for (var i = 0; i < Program.I().ocgcore.cards.Count; i++)
            if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
                if ((Program.I().ocgcore.cards[i].p.location & (uint)location) > 0)
                    if (Program.I().ocgcore.cards[i].p.controller == player)
                        cardlist.Add(Program.I().ocgcore.cards[i]);
        gameCard[] gameCards = new gameCard[cardlist.Count];
        for(int i =0; i < cardlist.Count; i++)
            gameCards[i] = cardlist[i];
        Program.I().new_ui_cardList.show((uint)player, (uint)location, gameCards);
        if (gameCards.Length > 0)
            Program.I().cardDescription.setData(cardlist[0].get_data(), player == 0 ? GameTextureManager.myBack: GameTextureManager.opBack);
    }

    private void calm()
    {
        if (Program.I().ocgcore.condition == Ocgcore.Condition.duel && Program.I().ocgcore.InAI == false &&
            Program.I().room.mode != 2)
            if (player == 0)
                if (location == CardLocation.Deck)
                {
                    if (Program.I().book.lab != null)
                    {
                        destroy(Program.I().book.lab.gameObject);
                        Program.I().book.lab = null;
                    }

                    return;
                }

        if (player == 1)
            if (location == CardLocation.Deck)
            {
                if (Program.I().book.labop != null)
                {
                    destroy(Program.I().book.labop.gameObject);
                    Program.I().book.labop = null;
                }

                return;
            }

        for (var i = 0; i < Program.I().ocgcore.cards.Count; i++)
            if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
                if ((Program.I().ocgcore.cards[i].p.location & (uint) location) > 0)
                    if (Program.I().ocgcore.cards[i].p.controller == player &&
                        Program.I().ocgcore.cards[i].isShowed == false)
                        Program.I().ocgcore.cards[i].ES_safe_card_move_to_original_place();
        if (hintText != null)
        {
            hintText.dispose();
            hintText = null;
        }
    }

    private void excite()
    {
        excited = true;
        if (location == CardLocation.Grave && Program.I().ocgcore.cantCheckGrave) return;
        Card data = null;
        var tailString = "";
        uint con = 0;
        for (var i = 0; i < Program.I().ocgcore.cards.Count; i++)
            if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
                if ((Program.I().ocgcore.cards[i].p.location & (uint) location) > 0)
                    if (Program.I().ocgcore.cards[i].p.controller == player)
                        if (Program.I().ocgcore.cards[i].isShowed == false)
                        {
                            data = Program.I().ocgcore.cards[i].get_data();
                            tailString = Program.I().ocgcore.cards[i].tails.managedString;
                            con = Program.I().ocgcore.cards[i].p.controller;
                        }

        Program.I().cardDescription.setData(data, con == 0 ? GameTextureManager.myBack : GameTextureManager.opBack, tailString, data != null);
        if (Program.I().ocgcore.condition == Ocgcore.Condition.duel && Program.I().ocgcore.InAI == false &&
            Program.I().room.mode != 2)
            if (player == 0)
                if (location == CardLocation.Deck)
                {
                    if (Program.I().book.lab != null)
                    {
                        destroy(Program.I().book.lab.gameObject);
                        Program.I().book.lab = null;
                    }


                    Program.I().book.lab =
                        create(Program.I().New_decker, Vector3.zero, Vector3.zero, false, Program.I().ui_main_2d)
                            .GetComponent<UILabel>();
                    Program.I().book.realize();


                    var screenPosition = Input.mousePosition;
                    screenPosition.x -= 90;
                    screenPosition.y += Program.I().book.lab.height / 4;
                    screenPosition.z = 0;
                    var worldPositin = Program.I().camera_main_2d.ScreenToWorldPoint(screenPosition);
                    Program.I().book.lab.transform.position = worldPositin;

                    return;
                }


        if (player == 1)
            if (location == CardLocation.Deck)
            {
                if (Program.I().book.labop != null)
                {
                    destroy(Program.I().book.labop.gameObject);
                    Program.I().book.labop = null;
                }


                Program.I().book.labop =
                    create(Program.I().New_decker, Vector3.zero, Vector3.zero, false, Program.I().ui_main_2d)
                        .GetComponent<UILabel>();
                Program.I().book.realize();


                var screenPosition = Input.mousePosition;
                screenPosition.x -= 90;
                screenPosition.y -= Program.I().book.labop.height / 4;
                screenPosition.z = 0;
                var worldPositin = Program.I().camera_main_2d.ScreenToWorldPoint(screenPosition);
                Program.I().book.labop.transform.position = worldPositin;

                return;
            }

        var count = 0;
        for (var i = 0; i < Program.I().ocgcore.cards.Count; i++)
            if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
                if ((Program.I().ocgcore.cards[i].p.location & (uint) location) > 0)
                    if (Program.I().ocgcore.cards[i].p.controller == player)
                        count++;
        var count_show = 0;
        for (var i = 0; i < Program.I().ocgcore.cards.Count; i++)
            if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
                if ((Program.I().ocgcore.cards[i].p.location & (uint) location) > 0)
                    if (Program.I().ocgcore.cards[i].p.controller == player &&
                        Program.I().ocgcore.cards[i].isShowed == false)
                        count_show++;
        if (hintText != null)
        {
            hintText.dispose();
            hintText = null;
        }

        if (count > 0) hintText = new TextMaster(count.ToString(), Input.mousePosition, false);
        var qidian = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 80);
        var zhongdian = new Vector3(2f * Program.I().ocgcore.getScreenCenter() - Input.mousePosition.x,
            Input.mousePosition.y, 50f);
        var i_real = 0;
        for (var i = 0; i < Program.I().ocgcore.cards.Count; i++)
            if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
                if ((Program.I().ocgcore.cards[i].p.location & (uint) location) > 0)
                    if (Program.I().ocgcore.cards[i].p.controller == player)
                        if (Program.I().ocgcore.cards[i].isShowed == false)
                        {
                            var screen_vector_to_move = Vector3.zero;
                            var gezi = 8;
                            if (count_show > 8) gezi = count_show;
                            var index = count_show - 1 - i_real;
                            i_real++;
                            screen_vector_to_move =
                                new Vector3(0, 50f * (float)Math.Sin(index / (float)count * 3.1415926f), 0)
                                +
                                qidian
                                +
                                index / (float)(gezi - 1) * (zhongdian - qidian);
                            //iTween.MoveTo(Program.I().ocgcore.cards[i].gameObject, Camera.main.ScreenToWorldPoint(screen_vector_to_move), 0.5f);
                            //iTween.RotateTo(Program.I().ocgcore.cards[i].gameObject, new Vector3(-30, 0, 0), 0.1f);
                            Program.I().ocgcore.cards[i].TweenTo(Camera.main.ScreenToWorldPoint(screen_vector_to_move),
                                new Vector3(-20, 0, 0), 0, 0, true);// mark 卡组额外等鼠标命中时浮起
                        }

        if (count_show > 0)
        {
            Program.I().audio.clip = Program.I().zhankai;
            Program.I().audio.Play();
        }
    }
}