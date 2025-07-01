using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using YGOSharp;
using YGOSharp.OCGWrapper.Enums;
using static CardListHandler;
using static iTween;
using static UnityEngine.Rendering.DebugUI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public enum gameCardCondition
{
    floating_clickable = 1,
    still_unclickable = 2,
    verticle_clickable = 3
}

public class GPS
{
    public uint controller;
    public uint location;
    public uint sequence;
    public int position;
    public uint reason;
}

public class Effect
{
    public string desc;
    public int flag;
    public int ptr;
    public bool forced = false;
}

public class gameCard : OCGobject
{
    public enum flashType
    {
        SpSummon,
        Active,
        Select,
        none
    }

    public enum kuangType
    {
        selected,
        chaining,
        none
    }

    private readonly TextMeshPro cardHint;

    public gameCardCondition condition = gameCardCondition.floating_clickable;

    public uint controllerBased = 0;

    //public bool getIfInMinMode()
    //{
    //    return isMinBlockMode && (ES_excited_unsafe_should_not_be_changed_dont_touch_this==false);
    //}

    public bool cookie_cared = false;

    public bool inAnimation = false;

    public int counterCANcount = 0;

    public int counterSELcount = 0;

    public flashType currentFlash = flashType.none;

    private flashType currentFlashPre = flashType.none;

    public kuangType currentKuang = kuangType.none;

    private kuangType currentKuangPre = kuangType.none;

    private Card data;
    private Card data_beforeMove;

    public bool disabled;
    public bool negated;

    public bool firstOnField;

    public List<Effect> effects = new List<Effect>();

    public bool forceRefreshCondition;

    public bool forSelect = false;

    private GameObject game_object_monster_cloude;

    private ParticleSystem game_object_monster_cloude_ParticleSystem;

    private GameObject game_object_verticle_drawing;

    private GameObject game_object_verticle_Star;

    //private GameObject gameObject_back;
    public GameObject gameObject_back;

    private GameObject gameObject_event_card_bed;

    private readonly GameObject gameObject_event_main;

    public GameObject gameObject_face;

    public bool isMinBlockMode = true;

    public bool isShowed;

    public int levelForSelect_1 = 0;

    public int levelForSelect_2 = 0;

    public int md5 = -233;

    private FlashingController MouseFlash;

    private GameObject nagaSign;

    private int number_showing;

    //public int ability = 2500;

    private GameObject obj_number;
    public int overFatherCount;

    public GPS p;

    public GPS p_beforeOverLayed;

    public GPS p_moveBefore;

    public GPS p_lastMove;

    GPS p_late;
    public bool prefered;

    public List<Action> refreshFunctions = new List<Action>();

    private readonly GameObject selectKuang;
    private readonly GameObject chainKuang;

    public int selectPtr = 0;
    public bool SemiNomiSummoned = false;

    public List<int> sortOptions = new List<int>();

    private readonly FlashingController[] SpSummonFlash;
    private readonly FlashingController[] ActiveFlash;
    private readonly FlashingController[] SelectFlash;

    public List<gameCard> target = new List<gameCard>();

    private TextMeshPro verticle_number;

    private BoxCollider VerticleCollider;

    public Transform cardModel;

    GameObject hl_spsom;
    GameObject hl_spsom_sct;
    GameObject hl_set;
    GameObject hl_set_sct;
    CardHighlightPosition chp;

    CutinLoader cutinLoader;
    LoadSFX loadSFX;
    public CardAnimation cardAnimation;

    public List<gameCard> cardsTargetMe = new List<gameCard>();
    public gameCard()
    {
        gameObject = Program.I().create(Program.I().mod_ocgcore_card);
        cardModel = gameObject.transform.Find("card");

        //添加Highlight
        hl_set = Object.Instantiate(LoadSFX.hl_set, Vector3.zero, Quaternion.Euler(0, 0, 0), gameObject.transform.Find("card/highlight"));
        hl_set_sct = Object.Instantiate(LoadSFX.hl_set_sct, Vector3.zero, Quaternion.Euler(0, 0, 0), gameObject.transform.Find("card/highlight"));
        hl_spsom = Object.Instantiate(LoadSFX.hl_spsom, Vector3.zero, Quaternion.Euler(0, 0, 0), gameObject.transform.Find("card/highlight"));
        hl_spsom_sct = Object.Instantiate(LoadSFX.hl_spsom_sct, Vector3.zero, Quaternion.Euler(0, 0, 0), gameObject.transform.Find("card/highlight"));
        hl_set.transform.localScale = new Vector3(1.03f, 1, 1.02f);
        hl_set_sct.transform.localScale = new Vector3(1.03f, 1, 1.02f);
        chp = gameObject.transform.Find("card/highlight").gameObject.AddComponent<CardHighlightPosition>();
        chp.card = this;

        gameObject_face = gameObject.transform.Find("card").Find("face").gameObject;
        gameObject_back = gameObject.transform.Find("card").Find("back").gameObject;
        gameObject_event_main = gameObject.transform.Find("card").Find("event").gameObject;
        cardHint = gameObject.transform.Find("text").GetComponent<TextMeshPro>();
        SpSummonFlash = insFlash("0099ff");
        ActiveFlash = insFlash("00ff66");
        SelectFlash = insFlash("ff8000");

        for (var i = 0; i < 2; i++)
        {
            SpSummonFlash[i].gameObject.SetActive(false);
            ActiveFlash[i].gameObject.SetActive(false);
            SelectFlash[i].gameObject.SetActive(false);
        }

        selectKuang = insKuang(Program.I().New_selectKuang);
        chainKuang = insKuang(Program.I().New_chainKuang);
        selectKuang.SetActive(false);
        chainKuang.SetActive(false);
        gameObject.SetActive(false);

        cutinLoader = GameObject.Find("Program").GetComponent<CutinLoader>();
        loadSFX = GameObject.Find("Program").GetComponent<LoadSFX>();
        cardAnimation = gameObject.GetComponent<CardAnimation>();
        cardAnimation.card = this;
    }
    public bool alreadyYellow;
    public bool highlightOn;
    bool highlightYellow;
    public void Highlight(bool on, bool yellow, bool clicked)
    {
        if ((p.location & (uint)CardLocation.Onfield) > 0 || (p.location & (uint)CardLocation.Hand) > 0)
        {
        }
        else
        {
            highlightOn = false;
            highlightYellow = false;
            return;
        }

        if (!on)
        {
            hl_spsom.SetActive(false);
            hl_spsom_sct.SetActive(false);
            hl_set.SetActive(false);
            hl_set_sct.SetActive(false);
            highlightOn = false;
            highlightYellow = false;
        }
        else if(yellow && clicked)
        {
            hl_spsom.SetActive(false);
            hl_spsom_sct.SetActive(true);
            hl_set.SetActive(false);
            hl_set_sct.SetActive(false);
            highlightOn = true;
            highlightYellow = true;
        }
        else if (yellow && !clicked)
        {
            hl_spsom.SetActive(true);
            hl_spsom_sct.SetActive(false);
            hl_set.SetActive(false);
            hl_set_sct.SetActive(false);
            highlightOn = true;
            highlightYellow = true;
        }
        else if (!yellow && clicked)
        {
            hl_spsom.SetActive(false);
            hl_spsom_sct.SetActive(false);
            hl_set.SetActive(false);
            hl_set_sct.SetActive(true);
            highlightOn = true;
            highlightYellow = false;
        }
        else if (!yellow && !clicked)
        {
            hl_spsom.SetActive(false);
            hl_spsom_sct.SetActive(false);
            hl_set.SetActive(true);
            hl_set_sct.SetActive(false);
            highlightOn = true;
            highlightYellow = false;
        }
    }

    public void addTarget(gameCard card_)
    {
        var exist = false;
        foreach (var item in target)
            if (item == card_)
                exist = true;
        if (exist == false) target.Add(card_);
    }

    public void removeTarget(gameCard card_)
    {
        target.Remove(card_);
    }

    public void show()
    {
        clearCookie();
        gameObject.SetActive(true);
        Program.I().ocgcore.AddUpdateAction_s(Update);
        refreshFunctions.Clear();
        refreshFunctions.Add(RefreshFunction_ES);
        refreshFunctions.Add(RefreshFunction_decoration);
        refreshFunctions.Add(card_picture_handler);
        forceRefreshCondition = true;
        gameObject.transform.position = accurate_position;
        gameObject.transform.eulerAngles = accurate_rotation;
    }

    public void hide()
    {
        try
        {
            set_overlay_light(0);
            clearCookie();
            UIHelper.clearITWeen(gameObject);
            del_all_decoration();
            for (var i = 0; i < allObjects.Count; i++) Object.Destroy(allObjects[i]);
            allObjects.Clear();
            set_text("", false);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        gameObject.SetActive(false);
    }

    private void clearCookie()
    {
        Program.I().ocgcore.RemoveUpdateAction_s(Update);
        isShowed = false;
        prefered = false;
        erase_data();
        target.Clear();
        loaded_cardPictureCode = -1;
        loaded_cardCode = -1;
        loaded_back = -1;
        loaded_specialHint = -1;
        loaded_verticalDrawingReal = Program.getVerticalTransparency() > 0.5f;
        loaded_verticalDrawingNumber = -1;
        loaded_verticalOverAttribute = -1;
        loaded_verticalatk = -1;
        loaded_verticaldef = -1;
        loaded_verticalpos = -1;
        loaded_verticalcon = -1;
        loaded_controller = -1;
        loaded_location = -1;
        p = new GPS
        {
            controller = 0,
            location = 0,
            position = 0,
            sequence = 0
        };
        CS_clear();
    }
    public void Update()
    {
        for (var i = 0; i < refreshFunctions.Count; i++)
            try
            {
                refreshFunctions[i]();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
    }

    public bool IsExtraCard()
    {
        return data.IsExtraCard();
    }

    private void RefreshFunction_decoration()
    {
        for (var i = 0; i < cardDecorations.Count; i++)
            if (cardDecorations[i].game_object != null)
            {
                var screenposition = Vector3.zero;
                if (cardDecorations[i].up_of_card)
                    screenposition = Program.I().main_camera.WorldToScreenPoint(gameObject_face.transform.position +
                        new Vector3(0, 1.2f, 1.2f * 1.732f));
                else
                    screenposition = Program.I().main_camera.WorldToScreenPoint(gameObject_face.transform.position);
                var worldposition = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y,
                    screenposition.z - cardDecorations[i].relative_position));
                cardDecorations[i].game_object.transform.eulerAngles = cardDecorations[i].rotation;
                cardDecorations[i].game_object.transform.position = worldposition;
                if (cardDecorations[i].scale_change_ignored == false)
                    cardDecorations[i].game_object.transform.localScale +=
                        (new Vector3(1, 1, 1) - cardDecorations[i].game_object.transform.localScale) * 0.3f;
            }

        for (var i = 0; i < overlay_lights.Count; i++)
            overlay_lights[i].transform.position = gameObject_face.transform.position + new Vector3(0, 1.8f, 0);
        if (obj_number != null)
        {
            var screenposition =
                Program.I().main_camera.WorldToScreenPoint(gameObject_face.transform.position +
                                                           new Vector3(0, 1f * 2.4f, 1.732f * 2.4f));
            var worldposition =
                Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z - 5));
            obj_number.transform.position = worldposition;
        }

        if (disabled && ((p.location & (uint) CardLocation.MonsterZone) > 0 ||
                         (p.location & (uint) CardLocation.SpellZone) > 0))
        {
            if (nagaSign == null)
            {
                nagaSign = create(Program.I().mod_simple_quad);
                nagaSign.transform.localScale = Vector3.zero;
                nagaSign.GetComponent<Renderer>().material.mainTexture = GameTextureManager.negated;
                nagaSign.GetComponent<Renderer>().material.color = Color.clear;
            }

            if (game_object_verticle_drawing != null && Program.getVerticalTransparency() > 0.5f)
            {
                if (nagaSign.transform.parent != game_object_verticle_drawing.transform)
                {
                    nagaSign.transform.SetParent(game_object_verticle_drawing.transform);
                    nagaSign.transform.localRotation = Quaternion.identity;
                    nagaSign.transform.localScale = Vector3.zero;
                    nagaSign.transform.localPosition = new Vector3(0, 0, 0);//new Vector3(0, 0, -0.25f);
                    //mark 无效
                    nagaSign.SetActive(false);
                    nagaSign.transform.parent.GetChild(0).GetComponent<MeshRenderer>().material.SetVector("RGBA",new Vector4(0,0,0,0));
                    nagaSign.transform.parent.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("Mono", 1);
                }

                try
                {
                    var devide = game_object_verticle_drawing.transform.localScale;
                    if (Vector3.Distance(Vector3.zero, devide) > 0.01f)
                        nagaSign.transform.localScale = new Vector3(2.4f / devide.x, 2.4f / devide.y, 1);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                if (nagaSign.transform.parent != gameObject_face.transform)
                {
                    nagaSign.transform.SetParent(gameObject_face.transform);
                    nagaSign.transform.localEulerAngles = new Vector3(-90, 0, 0);
                    nagaSign.transform.localScale = Vector3.zero;
                    nagaSign.transform.localPosition = new Vector3(0, -0.2f, 0);
                }

                try
                {
                    var devide = gameObject_face.transform.localScale;
                    if (Vector3.Distance(Vector3.zero, devide) > 0.01f)
                        nagaSign.transform.localScale = new Vector3(4f / devide.x, 4f / devide.y, 1);
                }
                catch (Exception)
                {
                }
            }
        }
        else
        {
            if (nagaSign != null)
            {
                if (nagaSign.transform.parent != null)
                {
                    nagaSign.transform.parent.GetChild(0).GetComponent<MeshRenderer>().material.SetVector("RGBA", new Vector4(1, 1, 1, 1));
                    nagaSign.transform.parent.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("Mono", 0);
                }
                destroy(nagaSign, 0.6f, true, true);
            }
        }

        if (currentKuangPre != currentKuang)
        {
            currentKuangPre = currentKuang;
            switch (currentKuang)
            {
                //case kuangType.selected:
                //    selectKuang.SetActive(true);
                //    chainKuang.SetActive(false);
                //    break;
                //case kuangType.chaining:
                //    selectKuang.SetActive(false);
                //    chainKuang.SetActive(true);
                //    break;
                //case kuangType.none:
                //    selectKuang.SetActive(false);
                //    chainKuang.SetActive(false);
                //    break;
            }
        }

        if (currentFlashPre != currentFlash)
        {
            currentFlashPre = currentFlash;
            switch (currentFlash)
            {
                case flashType.SpSummon:
                    for (var i = 0; i < 2; i++)
                    {
                        ActiveFlash[i].gameObject.SetActive(false);
                        SelectFlash[i].gameObject.SetActive(false);
                    }

                    for (var i = 0; i < 2; i++) SpSummonFlash[i].gameObject.SetActive(true);
                    break;
                case flashType.Active:
                    for (var i = 0; i < 2; i++)
                    {
                        SpSummonFlash[i].gameObject.SetActive(false);
                        SelectFlash[i].gameObject.SetActive(false);
                    }

                    for (var i = 0; i < 2; i++) ActiveFlash[i].gameObject.SetActive(true);
                    break;
                case flashType.Select:
                    for (var i = 0; i < 2; i++)
                    {
                        SpSummonFlash[i].gameObject.SetActive(false);
                        ActiveFlash[i].gameObject.SetActive(false);
                    }

                    for (var i = 0; i < 2; i++) SelectFlash[i].gameObject.SetActive(true);
                    break;
                case flashType.none:
                    for (var i = 0; i < 2; i++)
                    {
                        SpSummonFlash[i].gameObject.SetActive(false);
                        ActiveFlash[i].gameObject.SetActive(false);
                        SelectFlash[i].gameObject.SetActive(false);
                    }

                    break;
            }
        }

        handlerChain();
    }

    #region publicTools

    public void show_number(int number, bool add = false)
    {
        if (add)
        {
            show_number(number_showing * 10 + number);
            return;
        }

        if (number == 0)
        {
            if (obj_number != null)
            {
                iTween.ScaleTo(obj_number, Vector3.zero, 0.3f);
                destroy(obj_number, 0.6f);
            }
        }
        else
        {
            if (obj_number == null)
            {
                obj_number = create(Program.I().mod_ocgcore_card_number_shower);
                obj_number.transform.GetComponent<TextMeshPro>().text = number.ToString();
                obj_number.transform.localScale = Vector3.zero;
                iTween.ScaleTo(obj_number, new Vector3(1, 1, 1), 0.3f);
                iTween.RotateTo(obj_number, new Vector3(70, 0, 0), 0.3f);
            }
            else if (number_showing != number)
            {
                iTween.ScaleTo(obj_number, Vector3.zero, 0.6f);
                destroy(obj_number, 0.6f);
                obj_number = create(Program.I().mod_ocgcore_card_number_shower);
                obj_number.transform.GetComponent<TextMeshPro>().text = number.ToString();
                obj_number.transform.localScale = Vector3.zero;
                iTween.ScaleTo(obj_number, new Vector3(1, 1, 1), 0.3f);
                iTween.RotateTo(obj_number, new Vector3(70, 0, 0), 0.3f);
            }
        }

        number_showing = number;
    }

    #endregion

    #region ES_system

    private bool ES_mouse_check()
    {
        var re = false;
        if (gameObject_event_main != null)
            if (Program.pointedGameObject == gameObject_event_main)
                re = true;
        if (gameObject_event_card_bed != null)
            if (Program.pointedGameObject == gameObject_event_card_bed)
                re = true;
        if (game_object_verticle_drawing != null)
            if (Program.pointedGameObject == game_object_verticle_drawing)
                re = true;
        for (var i = 0; i < buttons.Count; i++)
            if (buttons[i].gameObjectEvent != null)
                if (Program.pointedGameObject == buttons[i].gameObjectEvent)
                    re = true;
        if (condition == gameCardCondition.still_unclickable) re = false;
        return re;
    }

    public void ES_lock(float time, bool exitExcited = true)
    {
        if(exitExcited)
            ES_exit_excited(false);
        Object.Destroy(gameObject.AddComponent<card_locker>(), time);
    }

    private bool ES_check_locked()
    {
        var return_value = false;
        if (gameObject.transform.GetComponent<card_locker>() != null) return_value = true;
        return return_value;
    }

    private bool ES_excited_unsafe_should_not_be_changed_dont_touch_this;

    private void RefreshFunction_ES()
    {
        if (Program.InputGetMouseButtonUp_0 && ES_mouse_check())
        {
            Program.I().ocgcore.ES_cardClicked(this);
        }
            
        if (ES_excited_unsafe_should_not_be_changed_dont_touch_this)
        {
            //当前在excited态
            if (ES_mouse_check())
                //刷新excited的数据
                ES_excited_handler();
            else if (!clicked)
                    ES_exit_excited(true);
                //退出excited态
        }
        if (clicked)
        {
            if (!ES_mouse_check() && Program.InputGetMouseButtonUp_0)
                ES_exit_clicked();
            ES_clicked_handler();
        }
        else
        {
            //当前不在excited态
            if (ES_mouse_check())
            {
                if (ES_check_locked() == false)
                    //进入excited态
                    ES_enter_excited();
            }
        }
    }

    private void ES_excited_handler() //mark 鼠标命中卡片时弹起确认功能
    {
        if (ES_excited_unsafe_should_not_be_changed_dont_touch_this)
        {
            ES_excited_handler_close_up_handler();
            if (Program.InputGetMouseButtonUp_0 && !clicked && (p.location & (uint)CardLocation.Hand) > 0)
                ES_enter_clicked();
            else if(Program.InputGetMouseButtonUp_0 && (p.location & (uint)CardLocation.Hand) == 0)
                ES_enter_clicked();
            //ES_excited_handler_event_cookie_card_bed();
        }
    }

    bool clicked;

    private void ES_clicked_handler()
    {
        if (p.location != (uint)CardLocation.Hand || forSelect)
            return;
        var screenposition = Program.I().main_camera.WorldToScreenPoint(accurate_position);
        if (p.controller == 0)
        {
            var worldposition = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y + 80f * Screen.height / 1080f, screenposition.z - 10f * Screen.height / 1080f));
            gameObject.transform.position += (worldposition - gameObject.transform.position) * 40f * Program.deltaTime;
            gameObject.transform.localEulerAngles = new Vector3(-20, 0, 0);
        }
        else
        {
            var worldposition = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y - 80f * Screen.height / 1080f, screenposition.z - 10f * Screen.height / 1080f));
            gameObject.transform.position += (worldposition - gameObject.transform.position) * 40f * Program.deltaTime;
        }
    }
    private void ES_enter_clicked()
    {
        clicked = true;
        ES_excited_handler_button_shower();
        if (highlightOn)
        {
            if (highlightYellow)
                Highlight(true, true, true);
            else
                Highlight(true, false, true);
        }
        if((data.Type & (uint)CardType.Xyz) > 0 && (p.location & (uint)CardLocation.MonsterZone) > 0)
        {
            var overlayed_cards = Program.I().ocgcore.GCS_cardGetOverlayElements(this);
            var cards = new gameCard[overlayed_cards.Count];
            for (int i = 0; i <  overlayed_cards.Count; i++)
            {
                cards[i] = overlayed_cards[i];
            }

            Program.I().new_ui_cardList.show(p.controller, p.location, cards);
        }
        else
            Program.I().new_ui_cardList.hide();

    }
    public void ES_exit_clicked()
    {
        clicked = false;
        if (highlightOn)
        {
            if (highlightYellow)
                Highlight(true, true, false);
            else
                Highlight(true, false, false);
        }
    }

    //float deltaTimeCloseUp=0;
    //private void ES_excited_handler_close_up_handler()
    //{
    //    float faT = 0.25f;
    //    deltaTimeCloseUp += Time.deltaTime;
    //    if (deltaTimeCloseUp > faT)
    //    {
    //        deltaTimeCloseUp = faT;
    //    }
    //    Vector3 screenposition = Program.I().main_camera.WorldToScreenPoint(accurate_position);
    //    Vector3 worldposition = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z - 10));
    //    gameObject.transform.position = new Vector3
    //        (
    //        iTween.easeOutQuad(accurate_position.x, worldposition.x, deltaTimeCloseUp / faT),
    //        iTween.easeOutQuad(accurate_position.y, worldposition.y, deltaTimeCloseUp / faT),
    //        iTween.easeOutQuad(accurate_position.z, worldposition.z, deltaTimeCloseUp / faT)
    //        );
    //    if (game_object_verticle_drawing != null)
    //    {
    //        card_verticle_drawing_handler();
    //    }
    //}

    private void ES_excited_handler_close_up_handler()//mark 鼠标命中时抬起高度
    {
        if (p.location != (uint)CardLocation.Hand || clicked)
            return;
        var screenposition = Program.I().main_camera.WorldToScreenPoint(accurate_position);
        var worldposition = Camera.main.ScreenToWorldPoint(new Vector3(screenposition.x, screenposition.y, screenposition.z - 5f));
        gameObject.transform.position += (worldposition - gameObject.transform.position) * 25f * Program.deltaTime;
    }

    public static float SortLine(int i, int all, float spacing)
    {
        float re = 0;
        float length = spacing * (all - 1);
        re = - (length / 2) + spacing * i;
        return re;
    }

    private void ES_excited_handler_button_shower()
    {
        var vector_of_begin = gameObject_face.transform.position;
        vector_of_begin = Program.I().main_camera.WorldToScreenPoint(vector_of_begin);
        for (var i = 0; i < buttons.Count; i++)
        {
            if((p.location & (uint)CardLocation.Hand) > 0)
                buttons[i].show(vector_of_begin + new Vector3(SortLine(i, buttons.Count, 180 * Screen.height / 1080f), 270f * Screen.height / 1080f, 0f));
            else
                buttons[i].show(vector_of_begin + new Vector3(SortLine(i, buttons.Count, 180 * Screen.height / 1080f), 150f * Screen.height / 1080f, 0f));
        }
    }

    private void ES_excited_handler_event_cookie_card_bed()
    {
        if (condition != gameCardCondition.verticle_clickable)
        {
            if (gameObject_event_card_bed == null)
                gameObject_event_card_bed
                    = create(Program.I().mod_ocgcore_hidden_button, gameObject.transform.position);
        }
        else
        {
            if (gameObject_event_card_bed != null) destroy(gameObject_event_card_bed);
        }
    }

    private void ES_enter_excited()
    {
        var iTweens = gameObject.GetComponents<iTween>();
        for (var i = 0; i < iTweens.Length; i++) Object.DestroyImmediate(iTweens[i]);
        if (condition == gameCardCondition.floating_clickable)
        {
            flash_line_on();
            //if ((p.location & (uint)CardLocation.Onfield) > 0)
                //iTween.RotateTo(gameObject, new Vector3(0f, 0, 0), 0.3f);
        }

        ES_excited_unsafe_should_not_be_changed_dont_touch_this = true;
        //showMeLeft(true);


        var screen = Program.I().main_camera.WorldToScreenPoint(gameObject.transform.position);
        screen.z = 0;

        //var overlayed_cards = Program.I().ocgcore.GCS_cardGetOverlayElements(this);
        //for (var x = 0; x < overlayed_cards.Count; x++)

        //    if (overlayed_cards[x].isShowed == false)
        //    {
        //        var pianyi = 130f;
        //        if (Program.getVerticalTransparency() < 0.5f) pianyi = 90f;
        //        var screen_vector_to_move = screen +
        //                                    new Vector3(
        //                                        pianyi + 75f * (overlayed_cards.Count - overlayed_cards[x].p.position - 1),
        //                                                        0,
        //                                                        12f * 7 + 2f * (overlayed_cards.Count - overlayed_cards[x].p.position - 1));
        //        overlayed_cards[x].flash_line_on();
        //        overlayed_cards[x].TweenTo(Camera.main.ScreenToWorldPoint(screen_vector_to_move),
        //            new Vector3(-20, 0, 0), 0, 0, true);//mark 超量素材
        //    }
    }

    public void showMeLeft(bool force = false)
    {
        Program.I().cardDescription.setData(data, GameTextureManager.myBack, tails.managedString, force, (int)p.controller);
    }

    public void ES_exit_excited(bool move_to_original_place)
    {
        var iTweens = gameObject.GetComponents<iTween>();
        for (var i = 0; i < iTweens.Length; i++) Object.DestroyImmediate(iTweens[i]);
        flash_line_off();
        ES_excited_unsafe_should_not_be_changed_dont_touch_this = false;
        for (var i = 0; i < buttons.Count; i++) buttons[i].hide();
        destroy(gameObject_event_card_bed);
        if (move_to_original_place) ES_safe_card_move_to_original_place();
        var overlayed_cards = Program.I().ocgcore.GCS_cardGetOverlayElements(this);
        for (var x = 0; x < overlayed_cards.Count; x++)
        {
            overlayed_cards[x].ES_safe_card_move_to_original_place();
            overlayed_cards[x].flash_line_off();
        }

        Object.Destroy(gameObject.AddComponent<card_locker>(), 0.3f);
    }

    public void ES_safe_card_move_to_original_place()
    {
        TweenTo(accurate_position, accurate_rotation);
    }

    private void ES_safe_card_move(Hashtable move_hash, Hashtable rotate_hash)
    {
        UIHelper.clearITWeen(gameObject);
        Object.DestroyImmediate(gameObject.GetComponent<screenFader>());
        if (Math.Abs((int)gameObject.transform.eulerAngles.z) == 180)
        {
            var p = gameObject.transform.eulerAngles;
            p.z = 179f;
            gameObject.transform.eulerAngles = p;
        }

        iTween.MoveTo(gameObject, move_hash);
        iTween.RotateTo(gameObject, rotate_hash);
    } 

    #endregion

    #region UA_system

    //UA_system
    private Vector3 gived_position = Vector3.zero;
    private Vector3 gived_rotation = Vector3.zero;
    public Vector3 accurate_position = Vector3.zero;
    public Vector3 accurate_rotation = Vector3.zero;

    public void UA_give_position(Vector3 p)
    {
        gived_position = p;
    }

    public Vector3 UA_get_accurate_position()
    {
        return accurate_position;
    }

    public void UA_give_rotation(Vector3 r)
    {
        gived_rotation = r;
    }

    public void UA_flush_all_gived_witn_lock(bool rush, float voiceTime = 0)
    {
        if (gameObject.GetComponent<TraceCard>().trace)
        {
            Debug.Log(get_data().Name + "--动了");
        }
        if (Vector3.Distance(gived_position, accurate_position) > 0.001f ||
            Vector3.Distance(gived_rotation, accurate_rotation) > 0.001f)
        {
            UA_reloadCardHintPosition();

            foreach (var btn in Program.I().ocgcore.gameField.gameHiddenButtons)
                btn.CheckCount();


            bool toGrave = false;
            bool toExclude = false;
            bool fromGrave = false;
            bool fromExclude = false;
            bool toField = false;
            bool fromField = false;

            if (p_lastMove == null)
                p_lastMove = p;
            if (Program.I().ocgcore.currentMessage == GameMessage.Move)
            {
                if ((p.location & (uint)CardLocation.Grave) > 0 && (p_lastMove.location & (uint)CardLocation.Grave) == 0)
                    toGrave = true;
                if (p.location == 16 && p_lastMove.location == 144)
                    toGrave = true;

                if ((p.location & (uint)CardLocation.Grave) == 0 && (p_lastMove.location & (uint)CardLocation.Grave) > 0)
                    fromGrave = true;

                if ((p.location & (uint)CardLocation.Removed) > 0 && (p_lastMove.location & (uint)CardLocation.Removed) == 0)
                    toExclude = true;
                if (p.location == 32 && p_lastMove.location == 160)
                    toExclude = true;

                if ((p.location & (uint)CardLocation.Removed) == 0 && (p_lastMove.location & (uint)CardLocation.Removed) > 0)
                    fromExclude = true;

                if ((p.location & (uint)CardLocation.Onfield) > 0 && (p_lastMove.location & (uint)CardLocation.Onfield) == 0)
                    toField = true;

                if ((p.location & (uint)CardLocation.Onfield) == 0 && (p_lastMove.location & (uint)CardLocation.Onfield) > 0)
                    fromField = true;
            }
            p_lastMove = p;

            if (rush)
            {
                if(condition == gameCardCondition.verticle_clickable)
                    refreshFunctions.Add(card_verticle_drawing_handler);
                UIHelper.clearITWeen(gameObject);
                gameObject.transform.position = gived_position;
                gameObject.transform.eulerAngles = gived_rotation;
                accurate_position = gived_position;
                accurate_rotation = gived_rotation;
                if ((p.location & (uint)CardLocation.Grave) > 0)
                {
                    cardModel.localPosition = new Vector3(0f, -1f, 0f);
                    cardModel.localEulerAngles = new Vector3(0f, 0f, 180f);
                    cardModel.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
                else if ((p.location & (uint)CardLocation.Removed) > 0)
                {
                    cardModel.localPosition = new Vector3(0f, -1f, 0f);
                    cardModel.localEulerAngles = new Vector3(0f, 0f, 180f);
                    cardModel.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                }
                else
                {
                    cardModel.localPosition = new Vector3(0f, 0f, 0f);
                    cardModel.localEulerAngles = new Vector3(90f, 0f, 0f);
                    cardModel.localScale = new Vector3(1f, 1f, 1f);
                }
            }
            else
            {
                float time_voice = 0f;
                float time_materialShow = 0;
                float time_summonAnimation = 0;
                float time_cutin = 0;
                float time_fromGraveOrExclude = 0;
                float time_move = 0.1f;
                time_move += Vector3.Distance(gived_position, gameObject.transform.position) * 0.2f / 30f;
                if (time_move > 0.3f) time_move = 0.3f;
                float time_toGraveOrExclude = 0;
                float time_landing = 0;
                float time_lead = Ocgcore.landDelay;

                if (fromGrave || fromExclude)
                    time_fromGraveOrExclude = 23 / 60f;
                if (toGrave || toExclude)
                    time_toGraveOrExclude = 30 / 60f;

                float time_delay_waitForVoice = 0f;
                float time_delay_materialsShow = 0;
                float time_delay_summonAnimation = 0f;
                float time_delay_cutin = 0f;
                float time_delay_rise = 0;
                float time_delay_fromGraveOrExclude = 0f;
                float time_delay_move = 0f;
                float time_delay_toGraveOrExclude = 0f;
                float time_delay_landing = 0f;

                bool rise = false;
                bool landing = false;
                bool summonAnimation = false;
                bool cutin = false;

                if (toField)
                {
                    //怪兽召唤+特殊召唤
                    if ((p.location & (uint)CardLocation.MonsterZone) > 0
                        &&
                        (p_moveBefore.location & (uint)CardLocation.MonsterZone) == 0
                        &&
                        (p.location & (uint)CardLocation.Overlay) == 0
                        &&
                        (p.position & (uint)CardPosition.FaceUp) > 0
                        &&
                        ((p.reason & (uint)CardReason.SPSUMMON) > 0
                        ||
                        (p_moveBefore.location & (uint)CardLocation.Hand) > 0)
                        )
                    {
                        //默认融合
                        int showUnlitCardFrames = 80;
                        int summonFrames1 = 108;
                        int summonFrames2 = 182;
                        if (LoadSFX.spType == "RITUAL")
                        {
                            showUnlitCardFrames = 0;
                            summonFrames1 = 282;
                            summonFrames2 = 353;
                        }
                        else if (LoadSFX.spType == "LINK")
                        {
                            showUnlitCardFrames = 0;
                            summonFrames1 = 217;
                            summonFrames2 = 288;

                            if (get_data().Level > 1)
                            {
                                summonFrames1 += 30;
                                summonFrames2 += 30;
                            }
                            if (get_data().Level > 2)
                            {
                                summonFrames1 += 30;
                                summonFrames2 += 30;
                            }
                        }
                        else if (LoadSFX.spType == "SYNCHRO")
                        {
                            showUnlitCardFrames = 0;
                            summonFrames1 = 245;
                            summonFrames2 = 300;
                        }
                        else if (LoadSFX.spType == "XYZ")
                        {
                            showUnlitCardFrames = 0;
                            summonFrames1 = 220;
                            summonFrames2 = 300;
                        }
                        int cutinFrames = 96;

                        //是否有Cutin
                        if (CutinLoader.HasCutin(data.Id) != 0 || cutinLoader.test)
                        {
                            BGMHandler.ChangeBGM("duel_keycard");

                            CutinLoader.id = data.Id;
                            CutinLoader.attribute = data.Attribute;
                            CutinLoader.level = data.Level;
                            CutinLoader.type = data.Type;
                            CutinLoader.controller = p.controller;
                            CutinLoader.cardName = data.Name;
                            CutinLoader.atk = data.Attack;
                            CutinLoader.def = data.Defense;

                            cutin = true;
                            time_cutin = cutinFrames / 60f;
                            Program.I().cardDescription.hide();
                        }

                        //有召唤动画
                        if (
                            Program.I().setting.setting.Vsum.value
                            &&
                            LoadSFX.materials.Count > 0
                            &&
                            loadSFX.TypeMatch(data.Type)
                            &&
                            p.controller == p_moveBefore.controller
                            &&
                            (p.reason & (uint)CardReason.SPSUMMON) > 0
                            &&
                            ((p_moveBefore.location & (uint)CardLocation.Extra) > 0 || LoadSFX.spType == "RITUAL")
                            )
                        {
                            if (!VoiceHandler.PlayCardLine(p.controller, "summon", this, false, true, 12, 1, true))
                            {
                                if (LoadSFX.spType == "RITUAL")
                                    VoiceHandler.PlayFixedLine(p.controller, "RitualSummon", false, get_data().Name);
                                else if (LoadSFX.spType == "FUSION")
                                    VoiceHandler.PlayFixedLine(p.controller, "FusionSummon", false, get_data().Name);
                                else if (LoadSFX.spType == "SYNCHRO")
                                    VoiceHandler.PlayFixedLine(p.controller, "SynchroSummon", false, get_data().Name);
                                else if (LoadSFX.spType == "XYZ")
                                    VoiceHandler.PlayFixedLine(p.controller, "XyzSummon", false, get_data().Name);
                                else if (LoadSFX.spType == "LINK")
                                    VoiceHandler.PlayFixedLine(p.controller, "LinkSummon", false, get_data().Name);
                            }
                            Program.I().cardDescription.hide();
                            summonAnimation = true;
                            time_voice = VoiceHandler.voiceTime;
                            time_materialShow = showUnlitCardFrames / 60f;
                            if(cutin)
                                time_summonAnimation = summonFrames1 / 60f + 0.3f;
                            else
                                time_summonAnimation = summonFrames2 / 60f + 0.3f;
                        }
                        else//没有召唤动画
                        {
                            bool summonline = true;
                            if (IsExtraCard())
                                summonline = false;
                            if (!VoiceHandler.PlayCardLine(p.controller, "summon", this, false, true, 12, 1, summonline))
                            {
                                if ((p.reason & (uint)CardReason.SPSUMMON) > 0)
                                    VoiceHandler.PlayFixedLine(p.controller, "SpecialSummon", false, get_data().Name);
                                else if((p.position & (uint)CardPosition.FaceUpAttack) > 0)
                                    VoiceHandler.PlayFixedLine(p.controller, "Summon", false, get_data().Name);
                                else if ((p.position & (uint)CardPosition.FaceUpDefence) > 0)
                                    VoiceHandler.PlayFixedLine(p.controller, "SummonInDefence", false, get_data().Name);
                            }
                            time_voice = VoiceHandler.voiceTime;
                        }
                    }
                    else if ((p.position & (uint)CardPosition.FaceUp) > 0
                        &&
                        fromExclude
                        )//一时除外回场
                    {
                        refreshFunctions.Add(card_verticle_drawing_handler);
                        LoadCard();
                    }
                    //怪兽里侧登场
                    else if ((p.location & (uint)CardLocation.MonsterZone) > 0
                        &&
                        (p_moveBefore.location & (uint)CardLocation.MonsterZone) == 0
                        &&
                        (p.position & (uint)CardPosition.FaceDown) > 0)
                    {
                        if((p_moveBefore.location & (uint)CardLocation.Hand) > 0)
                            VoiceHandler.PlayFixedLine(p.controller, "SetMonster", false);
                        time_voice = VoiceHandler.voiceTime;
                    }
                    //非怪兽登场
                    else
                    {

                    }

                    if(fromGrave || fromExclude)
                    {
                        landing = true;
                        time_landing = 10 / 60f;
                    }
                    else if ((p.position & (uint)CardPosition.FaceUp) > 0)
                    {
                        rise = true;
                        landing = true;
                        time_landing = 10 / 60f;
                    }

                }//To Field
                else//非登场
                {
                    if (fromGrave)
                        if (!toExclude)
                        {
                            landing = true;
                            time_landing = 10 / 60f;
                        }
                    if (fromExclude)
                        if (!toGrave)
                        {
                            landing = true;
                            time_landing = 10 / 60f;
                        }
                    if (toGrave)
                        if (!fromExclude)
                            rise = true;
                    if(toExclude)
                        if(!fromGrave)
                            rise= true;
                }

                if(voiceTime != 0)
                    time_voice = voiceTime;

                float time_fullTimeWithoutVoice = time_summonAnimation + time_cutin + time_fromGraveOrExclude + time_move + time_toGraveOrExclude + time_landing;
                if (time_voice >= time_fullTimeWithoutVoice)
                    time_delay_waitForVoice = time_voice - time_fullTimeWithoutVoice;
                else
                    time_delay_waitForVoice = 0f;
                time_delay_materialsShow = time_delay_waitForVoice;
                time_delay_summonAnimation = time_delay_materialsShow + time_materialShow;
                time_delay_cutin = time_delay_summonAnimation + time_summonAnimation;
                time_delay_fromGraveOrExclude = time_delay_cutin + time_cutin;
                time_delay_rise = time_delay_fromGraveOrExclude;
                time_delay_move = time_delay_fromGraveOrExclude + time_fromGraveOrExclude;
                time_delay_toGraveOrExclude = time_delay_move + time_move;
                time_delay_landing = time_delay_toGraveOrExclude + time_toGraveOrExclude;

                if (toField)
                    Program.I().ocgcore.Sleep((int)((time_delay_landing + time_landing - time_lead) * 60));
                else
                    Program.I().ocgcore.Sleep(10);
                ES_lock(time_delay_landing + time_landing, false);

                if (summonAnimation)
                {
                    gameObject.transform.position = new Vector3(0, 65f, -26.142f);
                    gameObject.transform.eulerAngles = new Vector3(-20f, 0f, 0f);
                    TweenTo(gived_position, gived_rotation, time_delay_move, time_move, false, iTween.EaseType.easeInSine);
                    gameObject.transform.position = accurate_position;
                    gameObject.transform.eulerAngles = accurate_rotation;
                }
                else
                    TweenTo(gived_position, gived_rotation, time_delay_move, time_move);

                if (fromGrave)
                {
                    if (summonAnimation)
                    {
                        gameObject.transform.position = new Vector3(0, -50, 0);
                        cardModel.localEulerAngles = new Vector3(90, 0, 0);
                        cardModel.localScale = new Vector3(1, 1, 1);
                        cardModel.localPosition = Vector3.zero;
                        rise = true;
                    }
                    else
                    {
                        cardAnimation.PlayDelayedAnimation("duelffromgrave", time_delay_fromGraveOrExclude);
                        foreach (var btn in Program.I().ocgcore.gameField.gameHiddenButtons)
                            btn.PlayParticle(p.controller, (uint)CardLocation.Grave, false, time_delay_fromGraveOrExclude);
                    }
                }
                else if (fromExclude)
                {
                    if (summonAnimation)
                    {
                        gameObject.transform.position = new Vector3(0, -50, 0);
                        cardModel.localEulerAngles = new Vector3(90, 0, 0);
                        cardModel.localScale = new Vector3(1, 1, 1);
                        cardModel.localPosition = Vector3.zero;
                        rise = true;
                    }
                    else
                    {
                        if (p_moveBefore != null && (p_moveBefore.position & (uint)CardPosition.FaceDown) > 0)
                            cardAnimation.PlayDelayedAnimation("duelffromexclude_back", time_delay_fromGraveOrExclude);
                        else
                            cardAnimation.PlayDelayedAnimation("duelffromexclude", time_delay_fromGraveOrExclude);
                        foreach (var btn in Program.I().ocgcore.gameField.gameHiddenButtons)
                            btn.PlayParticle(p.controller, (uint)CardLocation.Removed, false, time_delay_fromGraveOrExclude);
                    }
                }
                if (toGrave)
                {
                    cardAnimation.PlayDelayedAnimation("dueltograve", time_delay_toGraveOrExclude);
                    foreach (var btn in Program.I().ocgcore.gameField.gameHiddenButtons)
                        btn.PlayParticle(p.controller, (uint)CardLocation.Grave, true, time_delay_toGraveOrExclude + 0.25f);
                }
                else if (toExclude)
                {
                    if((p.position & (uint)CardPosition.FaceUp) >0)
                        cardAnimation.PlayDelayedAnimation("dueltoexclude", time_delay_toGraveOrExclude);
                    else
                        cardAnimation.PlayDelayedAnimation("dueltoexclude_back", time_delay_toGraveOrExclude);
                    foreach (var btn in Program.I().ocgcore.gameField.gameHiddenButtons)
                        btn.PlayParticle(p.controller, (uint)CardLocation.Removed, true, time_delay_toGraveOrExclude + 0.25f);
                }
                if (rise)
                {
                    if((fromGrave || fromExclude) && summonAnimation)
                        cardAnimation.PlayDelayedAnimation("card_rise", time_delay_move);
                    else if(toExclude && (p.position & (uint)CardPosition.FaceDown) > 0)
                        cardAnimation.PlayDelayedAnimation("card_rise_back", time_delay_rise);
                    else
                        cardAnimation.PlayDelayedAnimation("card_rise", time_delay_rise);
                }
                if (landing)
                {
                    if((p.position & (uint)CardPosition.FaceDown) > 0)
                        cardAnimation.PlayDelayedAnimation("card_land_back", time_delay_move);
                    else
                        cardAnimation.PlayDelayedAnimation("card_land", time_delay_landing);
                }
                if (summonAnimation)
                {
                    loadSFX.MaterialShow(time_delay_materialsShow, cutin, time_move, this);
                    loadSFX.SummonAnimation(time_delay_summonAnimation, this);
                }
                if (cutin)
                    cutinLoader.Start_Delay_LoadCutin(time_delay_cutin);
                accurate_position = gived_position;
                accurate_rotation = gived_rotation;
            }
        }
    }


    public void TweenTo(Vector3 pos, Vector3 rot, float delay = 0f, float newtime = 0f, bool exciting = false, iTween.EaseType easeType = iTween.EaseType.easeOutQuad) //mark 卡片移动功能
    {
        if (inAnimation)
            return;

        var time = 0.1f;
        time += Vector3.Distance(pos, gameObject.transform.position) * 0.2f / 30f;
        if (time < 0.1f) time = 0.1f;
        if (time > 0.3f) time = 0.3f;
        if (newtime != 0f) time = newtime;


        Transform childCard = gameObject_face.transform.parent;
        if ((p.location & (uint)CardLocation.Grave) > 0
            ||
            (p.location & (uint)CardLocation.Removed) > 0
            )
        {
            Sequence quence = DOTween.Sequence();
            quence.Append(childCard.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
            quence.AppendInterval(time + delay - 0.2f + 0.5f).OnComplete(() => {
                childCard.localPosition = new Vector3(0, -1, 0);
                childCard.localEulerAngles = new Vector3(0, 0, 180);
                childCard.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            });
        }
        else if((p.location & (uint)CardLocation.SpellZone) > 0)
        {
            Sequence quence = DOTween.Sequence();
            quence.AppendInterval(time + delay - 0.2f).OnComplete(() => {
                //childCard.localPosition = new Vector3(0, 0, 0);
                childCard.localEulerAngles = new Vector3(90, 0, 0);
            });
            quence.Append(childCard.DOScale(new Vector3(0.8f, 0.8f, 1f), 0.2f));
        }
        else
        {
            Sequence quence = DOTween.Sequence();
            quence.AppendInterval(time + delay - 0.2f).OnComplete(() => {
                //childCard.localPosition = new Vector3(0, 0, 0);
                childCard.localEulerAngles = new Vector3(90, 0, 0);
            });
            if (p_lastMove != null && (p_lastMove.location & ((uint)CardLocation.Grave) + (uint)CardLocation.Removed) == 0)
                quence.Append(childCard.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
        }




        var e = easeType;
        if (
            Math.Abs(gived_rotation.x) < 10 && Vector3.Distance(pos, gameObject.transform.position) > 1f
            ||
            accurate_position.x == pos.x && accurate_position.y < pos.y && accurate_position.z == pos.z
        )
        {
            var from = gameObject.transform.position;
            var to = pos;
            var path = new Vector3[30];
            for (var i = 0; i < 30; i++)
                path[i] = from + (to - from) * i / 29f +
                          new Vector3(0, 1.5f, 0) * (float) Math.Sin(3.1415926 * i / 29d);
            if (exciting)
            {
                ES_safe_card_move(
                    iTween.Hash(
                        "x", pos.x,
                        "y", pos.y,
                        "z", pos.z,
                        "path", path,
                        "time", time
                    ),
                    iTween.Hash
                    (
                        "x", rot.x,
                        "y", rot.y,
                        "z", rot.z,
                        "time", time
                    )
                );
            }                
            else
            {
                ES_safe_card_move(
                    iTween.Hash(
                        "x", pos.x,
                        "y", pos.y,
                        "z", pos.z,
                        "path", path,
                        "time", time,
                        "delay", delay,
                        "easetype", e
                    ),
                    iTween.Hash
                    (
                        "x", rot.x,
                        "y", rot.y,
                        "z", rot.z,
                        "time", time-0.1f,
                        "delay", delay,
                        "easetype", e
                    ));                
            }                
        }
        else
        {
            if (exciting)
            {
                ES_safe_card_move(
                    iTween.Hash(
                        "x", pos.x,
                        "y", pos.y,
                        "z", pos.z,
                        "time", time
                    ),
                    iTween.Hash
                    (
                        "x", rot.x,
                        "y", rot.y,
                        "z", rot.z,
                        "time", time
                    )
                );
            }
            else
            {
                ES_safe_card_move(//洗手牌 抽卡
                    iTween.Hash(
                        "x", pos.x,
                        "y", pos.y,
                        "z", pos.z,
                        "time", time,
                        "easetype", e
                    ),
                    iTween.Hash
                    (
                        "x", rot.x,
                        "y", rot.y,
                        "z", rot.z,
                        "time", time,
                        "easetype", e
                    )
                );
            }                
        }
    }

    public void UA_reloadCardHintPosition()
    {
        if ((p.location & (uint) CardLocation.MonsterZone) > 0 && (p.location & (uint) CardLocation.Overlay) == 0)
        {
            if (p.controller == 0)
            {
                if ((p.position & (uint) CardPosition.Attack) > 0)
                {
                    cardHint.gameObject.transform.localPosition = new Vector3(0, 0.01f, -4.5f);
                    cardHint.gameObject.transform.localEulerAngles = new Vector3(90, 0, 0);
                }
                else
                {
                    cardHint.gameObject.transform.localPosition = new Vector3(-4.5f, 0.01f, 0f);
                    cardHint.gameObject.transform.localEulerAngles = new Vector3(90, 90, 0);
                }
            }
            else
            {
                if (Program.I().setting.setting.closeUp.value == false)
                {
                    if ((p.position & (uint)CardPosition.Attack) > 0)
                    {
                        cardHint.gameObject.transform.localPosition = new Vector3(0, 0.01f, -4.5f);
                        cardHint.gameObject.transform.localEulerAngles = new Vector3(90, 180, 0);
                    }
                    else
                    {
                        cardHint.gameObject.transform.localPosition = new Vector3(-4.5f, 0.01f, 0);
                        cardHint.gameObject.transform.localEulerAngles = new Vector3(90, -90, 0);
                    }
                }
                else
                {
                    if ((p.position & (uint)CardPosition.Attack) > 0)
                    {
                        cardHint.gameObject.transform.localPosition = new Vector3(0, 0.01f, 4.3f);
                        cardHint.gameObject.transform.localEulerAngles = new Vector3(90, 180, 0);
                    }
                    else
                    {
                        cardHint.gameObject.transform.localPosition = new Vector3(4.3f, 0.01f, 0);
                        cardHint.gameObject.transform.localEulerAngles = new Vector3(90, -90, 0);
                    }
                }
            }
        }
        else
        {
            cardHint.gameObject.transform.localPosition = new Vector3(0, 0, -1.5f);
            cardHint.gameObject.transform.localEulerAngles = new Vector3(90, 0, 0);
        }
    }

    public void UA_give_condition(gameCardCondition c)
    {
        if (condition != c || forceRefreshCondition)
        {
            condition = c;
            forceRefreshCondition = false;
            if (condition == gameCardCondition.floating_clickable)
            {
                try
                {
                    gameObject_event_main.GetComponent<MeshCollider>().enabled = true;
                    gameObject.transform.Find("card").GetComponent<animation_floating_slow>().enabled = true;
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                destroy(game_object_monster_cloude);
                destroy(game_object_verticle_drawing);
                if (verticle_number != null) destroy(verticle_number.gameObject);
                destroy(game_object_verticle_Star);
                refreshFunctions.Remove(card_verticle_drawing_handler);
                refreshFunctions.Remove(monster_cloude_handler);
                loaded_controller = -1;
                loaded_location = -1;
                refreshFunctions.Add(card_floating_text_handler);
                //caculateAbility();
            }

            if (condition == gameCardCondition.still_unclickable)
            {
                try
                {
                    gameObject_event_main.GetComponent<MeshCollider>().enabled = false;
                    gameObject.transform.Find("card").GetComponent<animation_floating_slow>().enabled = false;
                    destroy(gameObject_event_card_bed);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                destroy(game_object_monster_cloude);
                destroy(game_object_verticle_drawing);
                if (verticle_number != null) destroy(verticle_number.gameObject);
                destroy(game_object_verticle_Star);
                refreshFunctions.Remove(card_verticle_drawing_handler);
                refreshFunctions.Remove(monster_cloude_handler);
                refreshFunctions.Remove(card_floating_text_handler);
                gameObject.transform.Find("card").transform.localPosition = Vector3.zero;
                set_text("", false);
                //caculateAbility();
            }

            if (condition == gameCardCondition.verticle_clickable)
            {
                try
                {
                    gameObject_event_main.GetComponent<MeshCollider>().enabled = true;
                    gameObject.transform.Find("card").GetComponent<animation_floating_slow>().enabled = true;
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                loaded_verticalDrawingNumber = -1;
                loaded_verticalOverAttribute = -1;
                loaded_verticalatk = -1;
                loaded_verticaldef = -1;
                loaded_verticalpos = -1;
                loaded_verticalcon = -1;
                //refreshFunctions.Add(card_verticle_drawing_handler);
                GameTextureManager.CacheCardCloseUp(data.Id);
                refreshFunctions.Add(monster_cloude_handler);
                refreshFunctions.Remove(card_floating_text_handler);
                //caculateAbility();
            }
        }
    }

    private int loaded_controller = -1;
    private int loaded_location = -1;

    private void card_floating_text_handler()//mark 各区域卡片数量设定
    {
        if (loaded_controller != p.controller || loaded_location != p.location)
        {
            loaded_controller = (int) p.controller;
            loaded_location = (int) p.location;
            var loc = "";
            if ((p.location & (uint) CardLocation.Deck) > 0) loc = GameStringHelper.kazu;
            if ((p.location & (uint) CardLocation.Extra) > 0) loc = GameStringHelper.ewai;
            if ((p.location & (uint) CardLocation.Grave) > 0) loc = GameStringHelper.mudi;
            if ((p.location & (uint) CardLocation.Removed) > 0) loc = GameStringHelper.chuwai;
            if (!SemiNomiSummoned && (data.Type & 0x68020C0) > 0 &&
                (p.location & ((uint) CardLocation.Grave + (uint) CardLocation.Removed)) > 0)
            {
                loc = GameStringHelper.SemiNomi;
            }

            if (p.controller == 1 && loc != "") loc = "<#ff8888>" + loc + "</color>";
            set_text(loc, false);
        }
    }

    private void monster_cloude_handler()
    {
        if (Program.MonsterCloud)
        {
            if (game_object_monster_cloude == null)
            {
                game_object_monster_cloude = create(Program.I().mod_ocgcore_card_cloude, gameObject.transform.position);
                game_object_monster_cloude_ParticleSystem = game_object_monster_cloude.GetComponent<ParticleSystem>();
            }
        }
        else
        {
            if (game_object_monster_cloude != null)
            {
                destroy(game_object_monster_cloude);
                game_object_monster_cloude = null;
                game_object_monster_cloude_ParticleSystem = null;
            }
        }

        if (game_object_monster_cloude != null)
            if (game_object_monster_cloude_ParticleSystem != null)
            {
                var screenposition = Program.I().main_camera.WorldToScreenPoint(gameObject.transform.position);
                game_object_monster_cloude.transform.position =
                    Camera.main.ScreenToWorldPoint(
                        new Vector3(screenposition.x, screenposition.y, screenposition.z + 3));
                game_object_monster_cloude_ParticleSystem.startSize = Random.Range(3f,
                    3f + (20f - 3f) * Mathf.Clamp(data.Attack, 0, 3000) / 3000f);
                if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Earth))
                    game_object_monster_cloude_ParticleSystem.startColor =
                        new Color(
                            200f / 255f + Random.Range(-0.2f, 0.2f),
                            80f / 255f + Random.Range(-0.2f, 0.2f),
                            0f / 255f + Random.Range(-0.2f, 0.2f));
                if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Water))
                    game_object_monster_cloude_ParticleSystem.startColor =
                        new Color(
                            0f / 255f + Random.Range(-0.2f, 0.2f),
                            0f / 255f + Random.Range(-0.2f, 0.2f),
                            255f / 255f + Random.Range(-0.2f, 0.2f));
                if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Fire))
                    game_object_monster_cloude_ParticleSystem.startColor =
                        new Color(
                            255f / 255f + Random.Range(-0.2f, 0.2f),
                            0f / 255f + Random.Range(-0.2f, 0.2f),
                            0f / 255f + Random.Range(-0.2f, 0.2f));
                if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Wind))
                    game_object_monster_cloude_ParticleSystem.startColor =
                        new Color(
                            0f / 255f + Random.Range(-0.2f, 0.2f),
                            140f / 255f + Random.Range(-0.2f, 0.2f),
                            0f / 255f + Random.Range(-0.2f, 0.2f));
                if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Dark))
                    game_object_monster_cloude_ParticleSystem.startColor =
                        new Color(
                            158f / 255f + Random.Range(-0.2f, 0.2f),
                            0f / 255f + Random.Range(-0.2f, 0.2f),
                            158f / 255f + Random.Range(-0.2f, 0.2f));
                if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Light))
                    game_object_monster_cloude_ParticleSystem.startColor =
                        new Color(
                            255f / 255f + Random.Range(-0.2f, 0.2f),
                            140f / 255f + Random.Range(-0.2f, 0.2f),
                            0f / 255f + Random.Range(-0.2f, 0.2f));
                if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Divine))
                    game_object_monster_cloude_ParticleSystem.startColor =
                        new Color(
                            255f / 255f + Random.Range(-0.2f, 0.2f),
                            140f / 255f + Random.Range(-0.2f, 0.2f),
                            0f / 255f + Random.Range(-0.2f, 0.2f));
            }
    }

    private bool loaded_verticalDrawingReal;
    private float loaded_verticalDrawingK = 1;
    private int loaded_verticalDrawingNumber = -1;
    private int loaded_verticalatk = -1;
    private int loaded_verticaldef = -1;
    private int loaded_verticalpos = -1;
    private int loaded_verticalcon = -1;
    private int loaded_verticalColor = -1;
    private int loaded_verticalOverAttribute = -1;
    private float k_verticle = 1;
    private float VerticleTransparency = 1f;
    public bool opMonsterWithBackGroundCard = false;
    public bool justSummon;
    public void card_verticle_drawing_handler()
    {
        if (game_object_verticle_drawing == null ||
            loaded_verticalDrawingReal != Program.getVerticalTransparency() > 0.5f)
        {
            if (Program.getVerticalTransparency() > 0.5f)
            {
                loaded_verticalDrawingK = 1; //GameTextureManager.getK(data.Id, GameTextureType.card_verticle_drawing);
                if (game_object_verticle_drawing == null)
                {
                    game_object_verticle_drawing = create(Program.I().mod_simple_quad,
                        gameObject.transform.position + Vector3.forward * 2, new Vector3(70, 0, 0));//mark 立绘
                    game_object_verticle_drawing.transform.parent = gameObject.transform;
                    game_object_verticle_drawing.transform.localEulerAngles = new Vector3(70, 0, 0);
                    game_object_verticle_drawing.transform.localPosition = new Vector3(0f, 1.5f, -0.54f);
                    //cardAnimation.PlayDelayActivateQuad(0.2f);
                }

                if (loaded_verticalDrawingReal != Program.getVerticalTransparency() > 0.5f)
                {
                    loaded_verticalDrawingReal = Program.getVerticalTransparency() > 0.5f;
                    game_object_verticle_drawing.transform.localScale = Vector3.zero;
                }
            }
            else
            {
                var texture = GameTextureManager.N;
                loaded_verticalDrawingK = 1;
                if (game_object_verticle_drawing == null)
                {
                    game_object_verticle_drawing = create(Program.I().mod_simple_quad, gameObject.transform.position,
                        new Vector3(70, 0, 0));
                    VerticleTransparency = 1f;
                }

                if (loaded_verticalDrawingReal != Program.getVerticalTransparency() > 0.5f)
                {
                    loaded_verticalDrawingReal = Program.getVerticalTransparency() > 0.5f;
                    game_object_verticle_drawing.transform.localScale = Vector3.zero;
                }
            }
        }
        else
        {
            var trans = 1f;
            trans *= Program.getVerticalTransparency();
            if (trans < 0) trans = 0;
            if (trans > 1) trans = 1;
            if (trans != VerticleTransparency)
            {
                VerticleTransparency = trans;
                game_object_verticle_drawing.GetComponent<Renderer>().material.color = new Color(1, 1, 1, trans);
            }

            if (Program.getVerticalTransparency() <= 0.5f || opMonsterWithBackGroundCard)
            {
                if (VerticleCollider != null)
                {
                    Object.DestroyImmediate(VerticleCollider);
                    VerticleCollider = null;
                }
            }
            else
            {
                if (VerticleCollider == null)
                    VerticleCollider = game_object_verticle_drawing.AddComponent<BoxCollider>();
            }

            var want_scale = Vector3.zero;
            var showscale = (isMinBlockMode ? 4.2f : Program.verticleScale) / loaded_verticalDrawingK;
            want_scale = new Vector3(10 * k_verticle, 10,10);//new Vector3(showscale * k_verticle, showscale, 1);//mark 立绘大小
            Vector3 want_position = gameObject_face.transform.position + new Vector3(0, 1.5f, -0.4f);
            game_object_verticle_drawing.transform.position = want_position;
            //mark 立绘位置
            //game_object_verticle_drawing.transform.position =
            //    get_verticle_drawing_vector(gameObject_face.transform.position);

            game_object_verticle_drawing.transform.localScale +=
                (want_scale - game_object_verticle_drawing.transform.localScale) * Program.deltaTime * 10f;
            //game_object_verticle_drawing.transform.localScale = want_scale;

            if (VerticleCollider != null)
            {
                var h = loaded_verticalDrawingK * 0.618f;
                VerticleCollider.size = new Vector3(4.3f / want_scale.x, h, 0.5f);
                VerticleCollider.center = new Vector3(0, -0.5f + 0.5f * h, 0);
            }


            var color = 0;

            if ((data.Type & (int)CardType.Tuner) > 0)
                color = 1;
            if ((data.Type & (int) CardType.Xyz) > 0) color = 2;
            if ((data.Type & (int) CardType.Link) > 0)
            {
                color = 3;
                data.Level = 0;
                for (var i = 0; i < 32; i++)
                    if ((data.LinkMarker & (1 << i)) > 0)
                        data.Level++;
            }

            if (verticle_number == null || loaded_verticalDrawingNumber != data.Level || loaded_verticalColor != color)
            {
                loaded_verticalDrawingNumber = data.Level;
                loaded_verticalColor = color;
                if (verticle_number == null)
                    verticle_number =
                        create(Program.I().new_ui_textMesh,Vector3.zero, new Vector3(90, 0, 0), true, null, true,
                                new Vector3(0.3f, 0.3f, 1f))//mark 等级文字创建-大小
                            .GetComponent<TextMeshPro>();
                if (game_object_verticle_Star == null)
                    game_object_verticle_Star = create(Program.I().mod_simple_quad_star, Vector3.zero, new Vector3(90, 0, 0),
                        true, null, true, new Vector3(3 * 1.8f * 0.17f, 3 * 1.8f * 0.17f, 3 * 1.8f * 0.17f));
                if (color == 0)
                {
                    verticle_number.text = "<#FFFFFF><size=50>" + data.Level.ToString() + "</size></color>";
                    game_object_verticle_Star.GetComponent<Renderer>().material.mainTexture = GameTextureManager.L;
                }

                if (color == 1)
                {
                    verticle_number.text = "<#FFFF00><size=50>" + data.Level + "</size></color>";
                    game_object_verticle_Star.GetComponent<Renderer>().material.mainTexture = GameTextureManager.L;
                }

                if (color == 2)
                {
                    verticle_number.text = "<#999999><size=50>" + data.Level + "</size></color>";
                    game_object_verticle_Star.GetComponent<Renderer>().material.mainTexture = GameTextureManager.R;
                }

                if (color == 3)
                {
                    verticle_number.text = "<#E1FFFF><size=50>" + data.Level + "</size></color>";
                    game_object_verticle_Star.GetComponent<Renderer>().material.mainTexture = GameTextureManager.LINK;
                }
                game_object_verticle_Star.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
            }

            //if (Program.getVerticalTransparency() < 0.5f)
            //{
            //    Vector3 screen_number_pos;
            //    screen_number_pos = 2 * gameObject_face.transform.position - cardHint.gameObject.transform.position;
            //    screen_number_pos =
            //        Program.I().main_camera.WorldToScreenPoint(screen_number_pos + new Vector3(-0.25f, 0, -0.7f));
            //    screen_number_pos.z -= 2f;
            //    verticle_number.transform.position = Program.I().main_camera.ScreenToWorldPoint(screen_number_pos);
            //    if (game_object_verticle_Star != null)
            //    {
            //        screen_number_pos = 2 * gameObject_face.transform.position - cardHint.gameObject.transform.position;
            //        screen_number_pos =
            //            Program.I().main_camera.WorldToScreenPoint(screen_number_pos + new Vector3(-1.5f, 0, -0.7f));
            //        screen_number_pos.z -= 2f;
            //        game_object_verticle_Star.transform.position =
            //            Program.I().main_camera.ScreenToWorldPoint(screen_number_pos);
            //    }
            //}
            //else
            //{
            //    Vector3 screen_number_pos;
            //    screen_number_pos = Program.I().main_camera.WorldToScreenPoint(cardHint.gameObject.transform.position +
            //                                                                   new Vector3(-0.3f, 0f, 1f));
            //    screen_number_pos.z -= 2f;
            //    verticle_number.transform.position = Program.I().main_camera.ScreenToWorldPoint(screen_number_pos);
            //    if (game_object_verticle_Star != null)
            //    {
            //        screen_number_pos = Program.I().main_camera.WorldToScreenPoint(
            //            cardHint.gameObject.transform.position + new Vector3(-1.3f, 0.3f, 1f));
            //        screen_number_pos.z -= 0;//2f;
            //        game_object_verticle_Star.transform.position =
            //            Program.I().main_camera.ScreenToWorldPoint(screen_number_pos);
            //    }
            //}

            //mark 卡片星数
            if (p.controller == 0)
            {
                verticle_number.transform.position = cardHint.gameObject.transform.position + new Vector3(2.7f, 0, 1.6f);
                verticle_number.transform.eulerAngles = new Vector3(90, 0, 0);
                game_object_verticle_Star.transform.position = cardHint.gameObject.transform.position + new Vector3(1.4f, 0, 1.6f);
                game_object_verticle_Star.transform.eulerAngles = new Vector3(90, 0, 0);
            }
            else
            {
                if (Program.I().setting.setting.closeUp.value == false)
                {
                    if ((p.position & (uint)CardPosition.Attack) > 0)
                    {
                        cardHint.gameObject.transform.localPosition = new Vector3(0, 0.01f, -5f);
                        cardHint.gameObject.transform.localEulerAngles = new Vector3(90, 180, 0);
                        verticle_number.transform.position = cardHint.gameObject.transform.position + new Vector3(-1.2f, 0, -2.1f);
                        verticle_number.transform.eulerAngles = new Vector3(90, 0, 0);
                        game_object_verticle_Star.transform.position = cardHint.gameObject.transform.position + new Vector3(-2.5f, 0, -2.1f);
                        game_object_verticle_Star.transform.eulerAngles = new Vector3(90, 0, 0);
                    }
                    else
                    {
                        cardHint.gameObject.transform.localPosition = new Vector3(-5f, 0.01f, 0);
                        cardHint.gameObject.transform.localEulerAngles = new Vector3(90, -90, 0);
                        verticle_number.transform.position = cardHint.gameObject.transform.position + new Vector3(-1.2f, 0, -2.1f);
                        verticle_number.transform.eulerAngles = new Vector3(90, 0, 0);
                        game_object_verticle_Star.transform.position = cardHint.gameObject.transform.position + new Vector3(-2.5f, 0, -2.1f);
                        game_object_verticle_Star.transform.eulerAngles = new Vector3(90, 0, 0);
                    }
                }
                else
                {
                    if ((p.position & (uint)CardPosition.Attack) > 0)
                    {
                        cardHint.gameObject.transform.localPosition = new Vector3(0, 0.01f, 4.3f);
                        cardHint.gameObject.transform.localEulerAngles = new Vector3(90, 180, 0);
                        verticle_number.transform.position = cardHint.gameObject.transform.position + new Vector3(-1.2f, 0, 1.7f);
                        verticle_number.transform.eulerAngles = new Vector3(90, 0, 0);
                        game_object_verticle_Star.transform.position = cardHint.gameObject.transform.position + new Vector3(-2.5f, 0, 1.7f);
                        game_object_verticle_Star.transform.eulerAngles = new Vector3(90, 0, 0);
                    }
                    else
                    {
                        cardHint.gameObject.transform.localPosition = new Vector3(4.3f, 0.01f, 0);
                        cardHint.gameObject.transform.localEulerAngles = new Vector3(90, -90, 0);
                        verticle_number.transform.position = cardHint.gameObject.transform.position + new Vector3(-1.2f, 0, 1.7f);
                        verticle_number.transform.eulerAngles = new Vector3(90, 0, 0);
                        game_object_verticle_Star.transform.position = cardHint.gameObject.transform.position + new Vector3(-2.5f, 0, 1.7f);
                        game_object_verticle_Star.transform.eulerAngles = new Vector3(90, 0, 0);
                    }
                }
            }
            game_object_verticle_Star.transform.localScale = new Vector3(1.4f,1.4f,1);//mark 星 等级大小
            //mark cardHint 攻守数字设置
            if (loaded_verticalatk != data.Attack || loaded_verticaldef != data.Defense ||
                loaded_verticalpos != p.position || loaded_verticalcon != p.controller)
            {
                loaded_verticalatk = data.Attack;
                loaded_verticaldef = data.Defense;
                loaded_verticalpos = p.position;
                loaded_verticalcon = (int) p.controller;
                set_text("0", true);
            }
        }
    }

    //private float caculateBoxWidth()
    //{
    //    float colliderWidth = 1f;
    //    float showscale = 2f + (float)(ability - 1000) / 1000f;
    //    if (showscale > 4) showscale = 4;
    //    if (showscale < 2) showscale = 2;
    //    showscale *= 1.8f / loaded_verticalDrawingK;
    //    showscale *= k_verticle;
    //    colliderWidth = 4.3f / showscale;
    //    return colliderWidth;
    //}

    //public void caculateAbility()
    //{
    //    if (condition== gameCardCondition.verticle_clickable)
    //    {
    //        if ((p.position & (UInt32)CardPosition.Attack) > 0)
    //        {
    //            ability = data.Attack;
    //        }
    //        else
    //        {
    //            ability = data.Defense;
    //        }
    //    }
    //    else
    //    {
    //        ability = data.Attack;
    //    }
    //    if (ability > 3000)
    //    {
    //        ability = 3000;
    //    }
    //    if (ability < 0)
    //    {
    //        ability = 0;
    //    }
    //}

    #endregion

    #region data

    public void set_data(Card d)
    {
        data = d;

        if (Program.I().cardDescription.ifShowingThisCard(data)) showMeLeft();
        LoadCard();
    }

    public void set_data_beforeMove(Card d)
    {
        data_beforeMove = d;
    }

    public void set_code(int code)
    {
        if (code > 0)
            if (data.Id != code)
            {
                set_data(CardsManager.Get(code));
                data.Id = code;
                if (p.controller == 1)
                    if (Program.I().ocgcore.condition == Ocgcore.Condition.duel)
                        if (!Program.I().ocgcore.sideReference.ContainsKey(code))
                            Program.I().ocgcore.sideReference.Add(code, code);
            }
    }

    public void refreshData()
    {
        CardsManager.Get(data.Id).cloneTo(data);
        set_data(data);
        clear_all_tail();
    }

    public void erase_data()
    {
        set_data(CardsManager.Get(0));
        disabled = false;
        clear_all_tail();
    }

    public Card get_data()
    {
        return data;
    }
    public Card get_data_beforeMove()
    {
        return data_beforeMove;
    }

    private int loaded_cardPictureCode = -1;
    private int loaded_cardCode = -1;
    private int loaded_back = -1;
    private int loaded_specialHint = -1;
    private bool cardCodeChangedButNowLoadedPic;

    public async void LoadCard()
    {
        gameObject_face.GetComponent<Renderer>().material.mainTexture = await GameTextureManager.GetCardPicture(data.Id,
            p.controller == 0 ? GameTextureManager.myBack : GameTextureManager.opBack);
        if (game_object_verticle_drawing != null)
        {
            if (Program.getVerticalTransparency() > 0.5f)
            {
                game_object_verticle_drawing.transform.GetChild(0).GetComponent<Renderer>().material.mainTexture = await GameTextureManager.GetCardCloseUp(data.Id);
                //mark 立绘上颜色
                float intensity =1.5f;
                if (game_object_verticle_drawing != null)
                {
                    if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Earth)) game_object_verticle_drawing.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(1, 0.2f, 0, 1) * intensity;
                    if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Water)) game_object_verticle_drawing.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(0, 1, 1, 1) * intensity;
                    if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Fire)) game_object_verticle_drawing.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(1, 0, 0, 1) * intensity;
                    if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Wind)) game_object_verticle_drawing.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(0, 1, 0, 1) * intensity;
                    if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Light)) game_object_verticle_drawing.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(1, 1, 0, 1) * intensity;
                    if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Dark)) game_object_verticle_drawing.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(1, 0, 1, 1) * intensity;
                    if (GameStringHelper.differ(data.Attribute, (long)CardAttribute.Divine)) game_object_verticle_drawing.transform.GetChild(0).GetComponent<Renderer>().material.color = new Color(1, 1, 0, 1) * intensity;
                }                
            }
            else
                game_object_verticle_drawing.GetComponent<Renderer>().material.mainTexture =
                    GameTextureManager.N;
        }
    }

    private void card_picture_handler()
    {
        if (p.controller != loaded_back)
        {
            loaded_back = (int) p.controller;
            UIHelper.getByName(gameObject, "back").GetComponent<Renderer>().material.mainTexture =
                loaded_back == 0 ? GameTextureManager.myBack : GameTextureManager.opBack;
            if (data.Id == 0)
                UIHelper.getByName(gameObject, "face").GetComponent<Renderer>().material.mainTexture =
                    loaded_back == 0 ? GameTextureManager.myBack : GameTextureManager.opBack;
            del_one_tail(GameStringHelper.opHint);
            if (loaded_back != controllerBased) add_string_tail(GameStringHelper.opHint);
        }

        var special_hint = 0;
        if ((p.position & (int) CardPosition.FaceDown) > 0)
            if ((p.location & (int) CardLocation.Removed) > 0)
                special_hint = 1;
        if ((p.position & (int) CardPosition.FaceUp) > 0)
            if ((p.location & (int) CardLocation.Extra) > 0)
                special_hint = 2;
        if (loaded_specialHint != special_hint)
        {
            loaded_specialHint = special_hint;
            if (loaded_specialHint == 0)
            {
                del_one_tail(GameStringHelper.licechuwai);
                del_one_tail(GameStringHelper.biaoceewai);
            }

            if (loaded_specialHint == 1) add_string_tail(GameStringHelper.licechuwai);
            if (loaded_specialHint == 2) add_string_tail(GameStringHelper.biaoceewai);
        }
    }

    public MultiStringMaster tails = new MultiStringMaster();

    public void add_string_tail(string str)
    {
        tails.Add(str);
        if (Program.I().cardDescription.ifShowingThisCard(data)) showMeLeft();
    }

    public void clear_all_tail()
    {
        tails.clear();
        if (Program.I().cardDescription.ifShowingThisCard(data)) showMeLeft();
    }

    public void del_one_tail(string str)
    {
        tails.remove(str);
        if (Program.I().cardDescription.ifShowingThisCard(data)) showMeLeft();
    }

    #endregion

    #region tools

    public bool isHided()
    {
        if ((p.location & (int) CardLocation.Deck) > 0) return true;
        if ((p.location & (int) CardLocation.Extra) > 0) return true;
        if ((p.location & (int) CardLocation.Removed) > 0) return true;
        if ((p.location & (int) CardLocation.Grave) > 0) return true;
        return false;
    }

    public void set_text(string s, bool isNumber = true)
    {
        if (isNumber)
        {
            cardHint.gameObject.SetActive(true);
            cardHint.GetComponent<CardHintHandler>().card = this;
            cardHint.GetComponent<CardHintHandler>().setHint();
        }
        else
        {
            cardHint.gameObject.SetActive(false);
        }
    }

    private int get_color_num_int()
    {
        var re = 0;
        //
        if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Earth)) re = 0;
        if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Water)) re = 3;
        if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Fire)) re = 5;
        if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Wind)) re = 2;
        if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Dark)) re = 4;
        if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Light)) re = 1;
        if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Divine)) re = 1;
        //
        return re;
    }

    private Vector3 get_verticle_drawing_vector(Vector3 facevector)
    {
        var want_position = Vector3.zero;
        if (isMinBlockMode)
        {
            want_position = facevector;
            want_position.y += 4.2f / loaded_verticalDrawingK / 2f * 0.5f;
            want_position.z += 4.2f / loaded_verticalDrawingK / 2f * 1.732f * 0.5f - 1.85f;
        }
        else
        {
            var showscale = Program.verticleScale;
            want_position = facevector;
            want_position.y += showscale / loaded_verticalDrawingK / 2f * 0.5f;
            want_position.z += showscale / loaded_verticalDrawingK / 2f * 1.732f * 0.5f -
                               (showscale * 1.3f / 3.6f - 0.8f);
        }

        return want_position;
    }

    #endregion

    #region button

    public List<gameButton> buttons = new List<gameButton>();

    public void add_one_button(gameButton b)
    {
        b.cookieCard = this;
        buttons.Add(b);
    }

    public bool query_hint_button(string hint)
    {
        for (var i = 0; i < buttons.Count; i++)
            if (buttons[i].hint == hint)
                return true;
        return false;
    }

    public void remove_all_cookie_button()
    {
        var buttons_to_remove = new List<gameButton>();
        for (var i = 0; i < buttons.Count; i++)
            if (buttons[i].notCookie == false)
            {
                buttons[i].hide();
                buttons_to_remove.Add(buttons[i]);
            }

        for (var i = 0; i < buttons_to_remove.Count; i++) buttons.Remove(buttons_to_remove[i]);
        buttons_to_remove.Clear();
    }

    public void remove_all_unCookie_button()
    {
        var buttons_to_remove = new List<gameButton>();
        for (var i = 0; i < buttons.Count; i++)
            if (buttons[i].notCookie)
            {
                buttons[i].hide();
                buttons_to_remove.Add(buttons[i]);
            }

        for (var i = 0; i < buttons_to_remove.Count; i++) buttons.Remove(buttons_to_remove[i]);
        buttons_to_remove.Clear();
    }

    #endregion

    #region decoration

    public class cardDecoration
    {
        public bool cookie = true;
        public string desctiption;
        public GameObject game_object;
        public float relative_position;
        public Vector3 rotation;
        public bool scale_change_ignored = false;
        public bool up_of_card;
    }

    private readonly List<cardDecoration> cardDecorations = new List<cardDecoration>();

    public cardDecoration add_one_decoration(GameObject mod, float relative_position, Vector3 rotation,
        string desctiption, bool cookie = true, bool up = false)
    {
        var c = new cardDecoration();
        c.desctiption = desctiption;
        c.up_of_card = up;
        c.cookie = cookie;
        c.relative_position = relative_position;
        c.rotation = rotation;
        c.game_object = create(mod, gameObject_face.transform.position);
        c.game_object.transform.eulerAngles = rotation;
        c.game_object.transform.localScale = Vector3.zero;
        cardDecorations.Add(c);
        return c;
    }

    public void fast_decoration(GameObject mod)
    {
        destroy(add_one_decoration(mod, -0.5f, Vector3.zero, "", false).game_object, 5);
    }

    public void animationEffect(GameObject mod)
    {
        Object.Destroy(Object.Instantiate(mod, UA_get_accurate_position(), Quaternion.identity), 10f);
    }

    public void positionEffect(GameObject mod)
    {
        Object.Destroy(Object.Instantiate(mod, Program.I().ocgcore.get_point_worldposition(p), Quaternion.identity),
            5f);
    }

    public void positionShot(GameObject mod)
    {
        Object.Destroy(Object.Instantiate(mod, Program.I().ocgcore.get_point_worldposition(p), Quaternion.identity),
            2f);
    }

    public void del_all_decoration_by_string(string desctiption)
    {
        var to_remove = new List<cardDecoration>();
        for (var i = 0; i < cardDecorations.Count; i++)
            if (cardDecorations[i].desctiption == desctiption)
            {
                to_remove.Add(cardDecorations[i]);
                destroy(cardDecorations[i].game_object);
            }

        for (var i = 0; i < to_remove.Count; i++) cardDecorations.Remove(to_remove[i]);
    }

    public void del_all_decoration()
    {
        var to_remove = new List<cardDecoration>();
        for (var i = 0; i < cardDecorations.Count; i++)
            if (cardDecorations[i].game_object != null && cardDecorations[i].cookie)
            {
                to_remove.Add(cardDecorations[i]);
                destroy(cardDecorations[i].game_object);
            }

        for (var i = 0; i < to_remove.Count; i++) cardDecorations.Remove(to_remove[i]);
    }

    #endregion

    #region overlay

    private readonly List<GameObject> overlay_lights = new List<GameObject>();

    public void add_one_overlay_light()
    {
        var mod = Program.I().mod_ocgcore_ol_light;
        if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Earth)) mod = Program.I().mod_ocgcore_ol_earth;
        if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Water)) mod = Program.I().mod_ocgcore_ol_water;
        if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Fire)) mod = Program.I().mod_ocgcore_ol_fire;
        if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Wind)) mod = Program.I().mod_ocgcore_ol_wind;
        if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Dark)) mod = Program.I().mod_ocgcore_ol_dark;
        if (GameStringHelper.differ(data.Attribute, (long) CardAttribute.Light)) mod = Program.I().mod_ocgcore_ol_light;
        var obj = create(mod, gameObject_face.transform.position);
        overlay_lights.Add(obj);
    }

    public void del_one_overlay_light()
    {
        if (overlay_lights.Count > 0)
        {
            destroy(overlay_lights[0]);
            overlay_lights.RemoveAt(0);
        }
    }


    public void set_overlay_light(int number)
    {
        if (number != 0)
            if (loaded_verticalOverAttribute != data.Attribute)
            {
                loaded_verticalOverAttribute = data.Attribute;
                while (overlay_lights.Count > 0) del_one_overlay_light();
            }

        while (overlay_lights.Count != number)
        {
            if (number > overlay_lights.Count) add_one_overlay_light();
            if (number < overlay_lights.Count) del_one_overlay_light();
        }
    }

    public void set_overlay_see_button(bool on)
    {
        //gameButton re = null;
        //for (var i = 0; i < buttons.Count; i++)
        //    if (buttons[i].cookieString == "see_overlay")
        //        re = buttons[i];
        //if (on)
        //{
        //    if (re == null)
        //    {
        //        var button = new gameButton(0, InterString.Get("查看素材"), superButtonType.see);
        //        button.cookieString = "see_overlay";
        //        button.notCookie = true;
        //        button.cookieCard = this;
        //        add_one_button(button);
        //    }
        //}
        //else
        //{
        //    if (re != null) remove_all_unCookie_button();
        //}
    }

    #endregion

    #region lines

    private FlashingController[] insFlash(string color)
    {
        var ret = new FlashingController[2];
        ret[0] = insFlashONE(color);
        ret[1] = insFlashONE(color);
        ret[1].transform.localEulerAngles = new Vector3(180, 0, 0);
        return ret;
    }

    private GameObject insKuang(GameObject mod)
    {
        GameObject ret = null;
        ret = Program.I().create(mod);
        ret.transform.SetParent(gameObject_face.transform, false);
        ret.transform.localScale = new Vector3(0.195f, 0.1f, 0.215f);
        ret.transform.localEulerAngles = new Vector3(0, 0, 0);
        return ret;
    }

    private FlashingController insFlashONE(string color)
    {
        FlashingController flash = null;
        Program.I().main_camera.GetComponent<HighlightingEffect>().enabled = true;
        flash = Program.I().create(Program.I().mod_ocgcore_card_figure_line).GetComponent<FlashingController>();
        flash.transform.SetParent(gameObject_face.transform, false);
        flash.transform.localPosition = Vector3.zero;
        var tcl = Color.yellow;
        ColorUtility.TryParseHtmlString(color, out tcl);
        flash.flashingStartColor = tcl;
        ColorUtility.TryParseHtmlString("000000", out tcl);
        flash.flashingEndColor = tcl;
        return flash;
    }

    public void flash_line_on()
    {
        Program.I().main_camera.GetComponent<HighlightingEffect>().enabled = true;
        if (MouseFlash == null)
        {
            MouseFlash = create(Program.I().mod_ocgcore_card_figure_line).GetComponent<FlashingController>();
            MouseFlash.transform.SetParent(gameObject_face.transform, false);
            MouseFlash.transform.localPosition = Vector3.zero;
            var tcl = Color.yellow;
            ColorUtility.TryParseHtmlString("ff8000", out tcl);
            MouseFlash.flashingStartColor = tcl;
            ColorUtility.TryParseHtmlString("ffffff", out tcl);
            MouseFlash.flashingEndColor = tcl;
        }

        MouseFlash.gameObject.SetActive(true);
    }

    public void flash_line_off()
    {
        if (MouseFlash != null) MouseFlash.gameObject.SetActive(false);
    }

    private GameObject p_line;

    public void p_line_on()
    {
        Program.I().main_camera.GetComponent<HighlightingEffect>().enabled = true;
        if (p_line != null) destroy(p_line);
        p_line = create(Program.I().mod_ocgcore_card_figure_line);
        p_line.transform.SetParent(gameObject_face.transform, false);
        p_line.transform.localPosition = Vector3.zero;
        p_line.GetComponent<FlashingController>().flashingStartColor = Color.blue;
        p_line.GetComponent<FlashingController>().flashingEndColor = Color.gray;
        p_line.GetComponent<FlashingController>().flashingFrequency = 0.5f;
    }

    public void p_line_off()
    {
        if (p_line != null)
        {
            destroy(p_line);
            p_line = null;
        }
    }

    #endregion

    #region animation

    public void animation_confirm(Vector3 position, Vector3 rotation, float time_move, float time_still)
    {
        ES_lock(time_move + time_move + time_still);
        confirm_step_time_still = time_still;
        confirm_step_time_move = time_move;
        confirm_step_r = rotation;
        var iTweens = gameObject.GetComponents<iTween>();
        for (var i = 0; i < iTweens.Length; i++) Object.Destroy(iTweens[i]);
        iTween.MoveTo(gameObject, iTween.Hash(
            "x", position.x,
            "y", position.y,
            "z", position.z,
            "onupdate", (Action) RefreshFunction_decoration,
            "oncomplete", (Action) confirm_step_2,
            "time", confirm_step_time_move
        ));
        iTween.RotateTo(gameObject, iTween.Hash(
            "x", confirm_step_r.x,
            "y", confirm_step_r.y,
            "z", confirm_step_r.z,
            "time", confirm_step_time_move
        ));
    }

    public void animation_confirm_screenCenter(Vector3 rotation, float time_move, float time_still)//mark 卡片确认
    {
        ES_lock(time_move + time_move + time_still);
        confirm_step_time_still = time_still;
        confirm_step_time_move = time_move;
        confirm_step_r = rotation;
        var iTweens = gameObject.GetComponents<iTween>();
        for (var i = 0; i < iTweens.Length; i++) Object.DestroyImmediate(iTweens[i]);
        iTween.RotateTo(gameObject, iTween.Hash(
            "x", confirm_step_r.x,
            "y", confirm_step_r.y,
            "z", confirm_step_r.z,
            "easetype", iTween.EaseType.spring,
            "onupdate", (Action)RefreshFunction_decoration,
            "oncomplete", (Action)confirm_step_2,
            "time", confirm_step_time_move
        ));
        var ttt = gameObject.AddComponent<screenFader>();
        ttt.from = gameObject.transform.position;
        ttt.time = time_move;
        ttt.deltaTimeCloseUp = 0;
        Object.Destroy(ttt, time_move + time_still);
    }

    private Vector3 confirm_step_r = Vector3.zero;

    private float confirm_step_time_still;

    private float confirm_step_time_move;

    private void confirm_step_2()
    {
        iTween.RotateTo(gameObject, iTween.Hash(
            "x", confirm_step_r.x,
            "y", confirm_step_r.y,
            "z", confirm_step_r.z,
            "onupdate", (Action) RefreshFunction_decoration,
            "oncomplete", (Action) confirm_step_3,
            "time", confirm_step_time_still
        ));
    }

    private void confirm_step_3()
    {
        iTween.RotateTo(gameObject, iTween.Hash(
            "x", accurate_rotation.x,
            "y", accurate_rotation.y,
            "z", accurate_rotation.z,
            "time", confirm_step_time_move
        ));
        iTween.MoveTo(gameObject, iTween.Hash(
            "x", accurate_position.x,
            "y", accurate_position.y,
            "z", accurate_position.z,
            "onupdate", (Action) RefreshFunction_decoration,
            "time", confirm_step_time_move,
            "easetype", iTween.EaseType.easeInQuad
        ));
    }

    public void animation_shake_to(float time)
    {
        ES_lock(time);
        gameObject.transform.position = accurate_position;
        gameObject.transform.eulerAngles = accurate_rotation;
        var iTweens = gameObject.GetComponents<iTween>();
        for (var i = 0; i < iTweens.Length; i++) Object.Destroy(iTweens[i]);
        iTween.ShakePosition(gameObject, iTween.Hash(
            "x", 1,
            "y", 1,
            "z", 1,
            "time", time,
            "oncomplete", (Action) ES_safe_card_move_to_original_place
        ));
    }


    public void animation_rush_to(Vector3 position, Vector3 rotation)
    {
        ES_lock(0.4f);
        var iTweens = gameObject.GetComponents<iTween>();
        for (var i = 0; i < iTweens.Length; i++)
            Object.Destroy(iTweens[i]);
        Object.DestroyImmediate(gameObject.GetComponent<screenFader>());
        iTween.MoveTo(gameObject, iTween.Hash(
            "x", position.x,
            "y", position.y,
            "z", position.z,
            "time", 0.2f
        ));
        iTween.RotateTo(gameObject, iTween.Hash(
            "x", rotation.x,
            "y", rotation.y,
            "z", rotation.z,
            "onupdate", (Action) RefreshFunction_decoration,
            "oncomplete", (Action) ES_safe_card_move_to_original_place,
            "time", 0.21f
        ));
    }

    public void animation_show_off(bool summon, bool disabled = false)
    {
        if (Ocgcore.inSkiping) return;

        show_off_disabled = disabled;
        show_off_begin_time = Program.TimePassed();
        show_off_shokewave = summon;

        if (show_off_disabled)
        {
            //SOH_dis();
            Program.I().ocgcore.Sleep(42);
        }
        else if (show_off_shokewave)
        {
            //if (Program.I().setting.setting.showoff.value && File.Exists("picture/closeup/" + data.Id + ".png") &&
            //    (data.Attack >= Program.I().setting.atk || data.Level >= Program.I().setting.star))
            //{
            //    SOH_sum();
            //    Program.I().ocgcore.Sleep(72);
            //}
            //else
            //{
            //    SOH_nSum();
            //    Program.I().ocgcore.Sleep(30);
            //}
            Program.I().ocgcore.Sleep(30);
        }
        else
        {
            //if (Program.I().setting.setting.showoffWhenActived.value &&
            //    File.Exists("picture/closeup/" + data.Id + ".png"))
            //{
            //    SOH_act();
            //}
            //else
            //{
            //    SOH_nAct();                
            //}
            //if (p.location == (uint)CardLocation.Hand)
            //    cardAnimation.PlayDelayedAnimation("activateHand", 0.2f);
            //else if (LoadSFX.me_pendulum && p.controller == 0 && (data.Type & (int)CardType.Pendulum) > 0 && (p.location & (uint)CardLocation.SpellZone) > 0 && (p.sequence == 0 || p.sequence == 4))
            //    cardAnimation.PlayDelayedAnimation("activateHand", 0f);
            //else if (LoadSFX.op_pendulum && p.controller == 1 && (data.Type & (int)CardType.Pendulum) > 0 && (p.location & (uint)CardLocation.SpellZone) > 0 && (p.sequence == 0 || p.sequence == 4))
            //    cardAnimation.PlayDelayedAnimation("activateHand", 0f);
            //else
            //    cardAnimation.PlayDelayedAnimation("activate", 0);
            float delayTime = 0;
            if (p_moveBefore != null && (p_moveBefore.location & (uint)CardLocation.Hand) > 0 && firstOnField)
                delayTime = 0.15f;
            cardAnimation.PlayDelayedAnimation("activate", delayTime);
            Program.I().ocgcore.Sleep(70);
        }
    }

    private bool show_off_shokewave;

    private bool show_off_disabled;

    private int show_off_begin_time;

    private async void SOH_act()
    {
        var k = 1; //GameTextureManager.getK(data.Id, GameTextureType.card_feature);
        var shower =
            create(Program.I().Pro1_superCardShowerA, Program.I().ocgcore.centre(true), Vector3.zero, false,
                Program.I().ui_main_2d).GetComponent<YGO1superShower>();
        shower.card.mainTexture = await GameTextureManager.GetCardPicture(data.Id);
        shower.closeup.mainTexture = await GameTextureManager.GetCardCloseUp(data.Id);
        shower.closeup.height = (int) (350f / k);
        shower.closeup.width = (int) (350f / k * shower.closeup.mainTexture.width / shower.closeup.mainTexture.height);
        Ocgcore.LRCgo = shower.gameObject;
        destroy(shower.gameObject, 0.7f, false, true);
    }

    private async void SOH_nAct()
    {
        var shower =
            create(Program.I().Pro1_CardShower, Program.I().ocgcore.centre(), Vector3.zero, false,
                Program.I().ui_main_2d).GetComponent<pro1CardShower>();
        shower.card.mainTexture = await GameTextureManager.GetCardPicture(data.Id);
        shower.mask.mainTexture = GameTextureManager.Mask;
        shower.disable.mainTexture = GameTextureManager.negated;
        shower.transform.localScale = Vector3.zero;
        shower.gameObject.transform.localScale = Utils.UIHeight() / 650f * Vector3.one;
        shower.run();
        Ocgcore.LRCgo = shower.gameObject;
        destroy(shower.gameObject, 0.7f, false, true);
    }

    private async void SOH_sum()
    {
        var k = 1; //GameTextureManager.getK(data.Id, GameTextureType.card_feature);
        var shower =
            create(Program.I().Pro1_superCardShower, Program.I().ocgcore.centre(true), Vector3.zero, false,
                Program.I().ui_main_2d).GetComponent<YGO1superShower>();
        shower.card.mainTexture = await GameTextureManager.GetCardPicture(data.Id);
        shower.closeup.mainTexture = await GameTextureManager.GetCardCloseUp(data.Id);
        shower.closeup.height = (int)(350f / k);
        shower.closeup.width = (int)(350f / k * shower.closeup.mainTexture.width / shower.closeup.mainTexture.height);
        Ocgcore.LRCgo = shower.gameObject;
        destroy(shower.gameObject, 2f, false, true);
    }

    private async void SOH_nSum()
    {
        var shower =
            create(Program.I().Pro1_CardShower, Program.I().ocgcore.centre(), Vector3.zero, false,
                Program.I().ui_main_2d).GetComponent<pro1CardShower>();
        shower.card.mainTexture = await GameTextureManager.GetCardPicture(data.Id);
        shower.mask.mainTexture = GameTextureManager.Mask;
        shower.disable.mainTexture = GameTextureManager.negated;
        shower.transform.localScale = Vector3.zero;
        shower.transform.DOScale(Utils.UIHeight() / 650f * Vector3.one, 0.5f);
        Ocgcore.LRCgo = shower.gameObject;
        destroy(shower.gameObject, 0.5f, false, true);
    }

    private async void SOH_dis()
    {
        var shower =
            create(Program.I().Pro1_CardShower, Program.I().ocgcore.centre(), Vector3.zero, false,
                Program.I().ui_main_2d).GetComponent<pro1CardShower>();
        shower.card.mainTexture = await GameTextureManager.GetCardPicture(data.Id);
        shower.mask.mainTexture = GameTextureManager.Mask;
        shower.disable.mainTexture = GameTextureManager.negated;
        shower.transform.localScale = Vector3.zero;
        shower.gameObject.transform.localScale = Utils.UIHeight() / 650f * Vector3.one;
        shower.Dis();
        Ocgcore.LRCgo = shower.gameObject;
        destroy(shower.gameObject, 0.7f, false, true);
    }

    public void sortButtons()
    {
        buttons.Sort((left, right) => { return getButtonGravity(right) - getButtonGravity(left); });
    }

    private int getButtonGravity(gameButton left)
    {
        var button = left;
        var gravity = 0;
        switch (button.type)
        {
            case superButtonType.pendulumSummon:
                gravity = 25;
                break;
            case superButtonType.set_pendulum:
                gravity = 24;
                break;
            case superButtonType.act:
                gravity = 20;
                break;
            case superButtonType.attack:
                gravity = 19;
                break;
            case superButtonType.spsummon:
                gravity = 18;
                break;
            case superButtonType.summon:
                gravity = 16;
                break;
            case superButtonType.set_monster:
                gravity = 14;
                break;
            case superButtonType.set_spell_trap:
                gravity = 12;
                break;
            case superButtonType.change_atk:
                gravity = 10;
                break;
            case superButtonType.change_def:
                gravity = 9;
                break;
        }

        return gravity;
    }

    #endregion

    #region cs

    public void ChainUNlock()
    {
        for (var i = 0; i < chains.Count; i++)
            if (chains[i].G != null)
            {
                Program.I().ocgcore.allChainPanelFixedContainer.Remove(chains[i].G.gameObject);
                chains[i].G.transform.SetParent(Program.I().transform, true);
            }
    }

    private void handlerChain()
    {
        for (var i = 0; i < chains.Count; i++)
        {
            if (chains[i].G == null)
            {
                chains[i].G = create(Program.I().new_ocgcore_chainCircle).GetComponent<chainMono>();
                Program.I().ocgcore.allChainPanelFixedContainer.Add(chains[i].G.gameObject);
                chains[i].G.text.text = chains[i].i.ToString();
                chains[i].G.text.color = GameTextureManager.chainColor;
                chains[i].G.text.enableVertexGradient = false;
                chains[i].G.circle.material.mainTexture = GameTextureManager.Chain;
                chains[i].G.gameObject.transform.localScale = Vector3.zero;
                chains[i].G.flashing = false;
            }

            var decorationChain = chains[i].G;
            if (game_object_verticle_drawing != null && Program.getVerticalTransparency() > 0.5f)
            {
                if (decorationChain.transform.parent != Program.I().transform)
                {
                    if (decorationChain.transform.parent != game_object_verticle_drawing.transform)
                    {
                        decorationChain.transform.SetParent(game_object_verticle_drawing.transform);
                        decorationChain.transform.localRotation = Quaternion.identity;
                        decorationChain.transform.localScale = Vector3.zero;
                        decorationChain.transform.localPosition = Vector3.zero;

                    }

                    try
                    {
                        var devide = game_object_verticle_drawing.transform.localScale;
                        if (Vector3.Distance(Vector3.zero, devide) > 0.01f)
                            decorationChain.transform.localScale =
                                new Vector3(4f / devide.x, 4f / devide.y, 1);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    decorationChain.transform.localScale = new Vector3(5, 5, 1);
                }
            }
            else
            {
                if (decorationChain.transform.parent != Program.I().transform)
                {
                    if (decorationChain.transform.parent != gameObject_face.transform)
                    {
                        decorationChain.transform.SetParent(gameObject_face.transform);
                        decorationChain.transform.localEulerAngles = new Vector3(-90,180,0);
                        decorationChain.transform.localScale = Vector3.zero;
                        decorationChain.transform.localPosition = Vector3.zero;

                    }

                    try
                    {
                        decorationChain.text.transform.localEulerAngles = new Vector3(decorationChain.text.transform.localEulerAngles.x,
                                                                                        decorationChain.text.transform.localEulerAngles.y,
                                                                                        decorationChain.text.transform.parent.parent.parent.parent.localEulerAngles.y);
                        var devide = gameObject_face.transform.localScale;
                        if (Vector3.Distance(Vector3.zero, devide) > 0.01f)
                            decorationChain.transform.localScale =
                                new Vector3(4.4f / devide.x, 4.4f / devide.y, 1);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    decorationChain.transform.localScale = new Vector3(5, 5, 1);
                }
            }
        }

        if (CS_ballIsShowed)
        {
            if (Program.I().setting.setting.Vchain.value)
                if (ballChain == null)
                {
                    //ballChain = add_one_decoration(Program.I().mod_ocgcore_cs_chaining, 3, Vector3.zero, "chaining",
                    //    false).game_object;
                    //ballChain.GetComponent<slowFade>().yse = condition != gameCardCondition.verticle_clickable ||
                    //                                         Program.getVerticalTransparency() < 0.5f;
                }
        }
        else
        {
            if (ballChain != null)
            {
                //del_all_decoration_by_string("chaining");
                //var pos = UIHelper.get_close(gameObject.transform.position, Program.I().main_camera, 5);
                //if (Program.I().setting.setting.Vchain.value)
                //    Object.Destroy(Object.Instantiate(Program.I().mod_ocgcore_cs_end, pos, Quaternion.identity), 5f);
                //if (ballChain != null) destroy(ballChain);
                //ballChain = null;
            }
        }
    }

    private GameObject ballChain;

    public bool CS_ballIsShowed;

    public void CS_showBall()
    {
        CS_ballIsShowed = true;
        currentKuang = kuangType.chaining;
    }

    public void CS_hideBall()
    {
        CS_ballIsShowed = false;
    }

    public void CS_ballToNumber()
    {
        if (CS_ballIsShowed)
        {
            currentKuang = kuangType.chaining;
            CS_hideBall();
            CS_addChainNumber(1);
        }
    }

    public List<chainMonoW> chains = new List<chainMonoW>();

    public class chainMonoW
    {
        public chainMono G;

        public int i;
        //public Vector3 bornPosition = default(Vector3);
        //public Vector3 bornAngle = default(Vector3);
    }

    public void CS_addChainNumber(int i)
    {
        currentKuang = kuangType.chaining;
        var w = new chainMonoW();
        w.i = i;
        w.G = null;
        chains.Add(w);
    }

    public void CS_removeOneChainNumber()
    {
        if (chains.Count > 0)
        {
            var decorationChain = chains[chains.Count - 1].G;
            if (decorationChain != null)
            {
                if (Program.I().ocgcore.inTheWorld())
                {
                    destroy(decorationChain.gameObject);
                }
                else
                {
                    decorationChain.flashing = true;
                    destroy(decorationChain.gameObject, 0.7f);
                }
            }

            chains.RemoveAt(chains.Count - 1);
            currentKuang = kuangType.none;
        }
    }

    public void CS_chainCircleYellow()
    {
        if (chains.Count > 0)
        {
            var decorationChain = chains[chains.Count - 1].G;
            if (decorationChain != null)
            {
                decorationChain.ChangeYellow();
            }
        }
    }

    public void CS_removeAllChainNumber()
    {
        while (chains.Count > 0) CS_removeOneChainNumber();
    }

    public void CS_clear()
    {
        CS_hideBall();
        CS_removeAllChainNumber();
        currentKuang = kuangType.none;
    }

    #endregion
}