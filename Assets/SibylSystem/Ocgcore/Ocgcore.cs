﻿using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using YgomSystem.Effect;
using YGOSharp;
using YGOSharp.OCGWrapper.Enums;
using Object = UnityEngine.Object;

public class Ocgcore : ServantWithCardDescription
{
    public delegate void responseHandler(byte[] buffer);

    public enum Condition
    {
        N = 0,
        duel = 1,
        watch = 2,
        record = 3
    }

    public static IEnumerator enumeratorMeWin;
    public static IEnumerator enumeratorOpWin;

    public static bool inSkiping;

    public static GameObject LRCgo = null;

    public List<gameCard> allCardsInSelectMessage = new List<gameCard>();

    private readonly List<gameCard> cardsForConfirm = new List<gameCard>();

    private readonly List<gameCard> cardsInChain = new List<gameCard>();

    private readonly List<gameCard> cardsInSelectAnimation = new List<gameCard>();

    private readonly List<gameCard> cardsInSort = new List<gameCard>();

    public List<gameCard> cardsMustBeSelected = new List<gameCard>();

    public List<gameCard> cardsSelectable = new List<gameCard>();

    public List<gameCard> cardsSelected = new List<gameCard>();

    private readonly List<int> ES_searchCode = new List<int>();

    private readonly List<sortResult> ES_sortCurrent = new List<sortResult>();

    private readonly List<sortResult> ES_sortResult = new List<sortResult>();

    private readonly List<int> keys = new List<int>();

    private readonly List<linkMask> linkMaskList = new List<linkMask>();
    private readonly List<Package> Packages_ALL = new List<Package>();

    private readonly List<placeSelector> placeSelectors = new List<placeSelector>();

    public List<GameObject> allChainPanelFixedContainer = new List<GameObject>();

    private arrow Arrow;
    private autoForceChainHandlerType autoForceChainHandler = autoForceChainHandlerType.manDoAll;

    private List<gameCard> chainCards = new List<gameCard>();
    private float camera_max = -15f;

    private float camera_min = -27.5f;
    public bool cantCheckGrave;

    public List<gameCard> cards = new List<gameCard>();
    private bool clearAllShowedB;
    private bool clearTimeFlag;
    private int code_for_show;

    public Condition condition = Condition.duel;
    public List<string> confirmedCards = new List<string>();

    private GameObject cookie_AttackEffect;

    private int cookie_matchKill;

    public GameMessage currentMessage = GameMessage.Waiting;

    private int currentMessageIndex = -1;
    private bool deckReserved;

    private string ES_hint = "";

    public int ES_level;

    private int ES_max;

    private int ES_min;

    public bool ES_overFlow;

    private string ES_phaseString = "";

    private string ES_selectHint = "";
    private int Es_selectMSGHintData;
    private int Es_selectMSGHintPlayer;
    private int Es_selectMSGHintType;

    private string ES_selectUnselectHint = "";

    private int ES_sortSum;

    private string ES_turnString = "";

    public string MD_phaseString = "";
    public string MD_battleString = "";

    private bool flagForCancleChain;

    private bool flagForTimeConfirm;

    public GameField gameField;

    public gameInfo gameInfo;
    public responseHandler handler = null;

    public bool InAI;

    public bool isFirst;

    public bool isObserver;
    private int keysTempCount;
    private float lastAlpha;
    private int lastExcitedController = -1;

    private int lastExcitedLocation = -1;

    private int lastReszieTime;

    private float lastSize;


    private bool leftExcited;

    public int life_0;

    public int life_1;

    public int lpLimit = 8000;

    public int MasterRule;

    private int md5Maker;

    public static int MessageBeginTime;

    public string name_0 = "";

    public string name_0_c = "";

    public string name_0_tag = "";

    public string name_1 = "";

    public string name_1_c = "";

    public string name_1_tag = "";

    private List<Package> Packages = new List<Package>();

    public bool paused;

    private bool replayShowAll;
    private bool reportShowAll;

    private duelResult result = duelResult.disLink;


    public Servant returnServant;

    public static bool right;

    private bool rightExcited;

    public Dictionary<int, int> sideReference = new Dictionary<int, int>();

    private bool someCardIsShowed;

    public bool surrended;

    private int theWorldIndex;

    public int timeLimit = 180;
    public int turns;

    public GameObject waitObject;

    private lazyWin winCaculator;

    private string winReason = "";

    public bool myTurn;

    public static float landDelay = 0.1f;

    private bool hasPlayedDamageSE;

    public float getScreenCenter()
    {
        return Screen.width / 2f;//(Screen.width + Program.I().cardDescription.width - gameInfo.width) / 2f;//mark 屏幕中心
    }

    private linkMask makeLinkMask(GPS p)
    {
        var ma = new linkMask();
        ma.p = p;
        ma.eff = !Program.I().setting.setting.Vlink.value;
        shift_effect(ma, Program.I().setting.setting.Vlink.value);
        return ma;
    }

    private void shift_effect(linkMask target, bool value)
    {
        if (target.eff != value)
        {
            if (target.gameObject != null) destroy(target.gameObject);
            if (value)
            {
                target.gameObject = create_s(Program.I().mod_ocgcore_ss_link_mark,
                    get_point_worldposition(target.p) + new Vector3(0, -0.1f, 0), Vector3.zero, true);
            }
            else
            {
                target.gameObject = create_s(Program.I().mod_simple_quad,
                    get_point_worldposition(target.p) + new Vector3(0, -0.1f, 0), new Vector3(90, 0, 0));
                target.gameObject.transform.localScale = new Vector3(4, 4, 4);
                target.gameObject.GetComponent<Renderer>().material.mainTexture = GameTextureManager.LINKm;
                target.gameObject.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0f);
            }

            target.eff = value;
        }
    }

    private gameCardCondition get_point_worldcondition(GPS p)
    {
        var return_value = gameCardCondition.floating_clickable;
        if ((p.location & (uint) CardLocation.Deck) > 0) return_value = gameCardCondition.still_unclickable;
        if ((p.location & (uint) CardLocation.Extra) > 0) return_value = gameCardCondition.still_unclickable;
        if ((p.location & (uint) CardLocation.MonsterZone) > 0)
        {
            return_value = gameCardCondition.floating_clickable;
            if ((p.position & (uint) CardPosition.FaceUp) > 0) return_value = gameCardCondition.verticle_clickable;
        }

        if ((p.location & (uint) CardLocation.SpellZone) > 0) return_value = gameCardCondition.floating_clickable;
        if ((p.location & (uint) CardLocation.Grave) > 0) return_value = gameCardCondition.still_unclickable;
        if ((p.location & (uint) CardLocation.Hand) > 0) return_value = gameCardCondition.floating_clickable;
        if ((p.location & (uint) CardLocation.Removed) > 0) return_value = gameCardCondition.still_unclickable;
        if ((p.location & (uint) CardLocation.Overlay) > 0) return_value = gameCardCondition.still_unclickable;
        return return_value;
    }

    public Vector3 get_point_worldposition(GPS p, gameCard c = null) //mark 场地中卡片位置
    {
        var return_value = Vector3.zero;
        var real = (Program.fieldSize - 1) * 0.9f + 1f;

        if ((p.location & (uint)CardLocation.Search) > 0)
        {
            return_value = new Vector3(0, -50, 0);
        }

        if ((p.location & (uint) CardLocation.Deck) > 0)
        {
            if (p.controller == 0)
                return_value = new Vector3(26.67f, 1.5f, -23.5f);
            else
                return_value = new Vector3(-26.67f, 1.5f, 23.5f);
            return_value.y += p.sequence * 0.03f;
        }

        if ((p.location & (uint) CardLocation.Extra) > 0)
        {
            if (p.controller == 0)
                return_value = new Vector3(-26.67f, 1.5f, -23.5f);
            else
                return_value = new Vector3(26.67f, 1.5f, 23.5f);
            return_value.y += p.sequence * 0.03f;
        }

        if ((p.location & (uint) CardLocation.Grave) > 0)
        {
            //if (p.controller == 0)
            //    return_value = new Vector3(26f, 0.2f, -12.8f);
            //else
            //    return_value = new Vector3(-26f, 0.2f, 12.8f);
            //return_value.y += p.sequence * 0.03f;
            if (p.controller == 0)
                return_value = new Vector3(25.74f, 0f, -14.26f);
            else
                return_value = new Vector3(-25.74f, 0f, 14.26f);
        }

        if ((p.location & (uint) CardLocation.Removed) > 0)
        {
            //if (p.controller == 0)
            //    return_value = new Vector3(26f, 0.2f, -4.58f);
            //else
            //    return_value = new Vector3(-26f, 0.2f, 4.58f);

            //return_value.y += p.sequence * 0.03f;
            if (p.controller == 0)
                return_value = new Vector3(25.74f + 1.842971f, 0f, -14.26f + 6.236011f);
            else
                return_value = new Vector3(-25.74f - 1.842971f, 0f, 14.26f - 6.236011f);
        }

        if ((p.location & (uint) CardLocation.MonsterZone) > 0)
        {
            var realIndex = p.sequence;
            if (p.controller == 0)
            {
                realIndex = p.sequence;
                return_value.y = 0.2f;
                return_value.z = -9.48f;
            }
            else
            {
                if (realIndex <= 4)
                    realIndex = 4 - p.sequence;
                else if (realIndex == 5)
                    realIndex = 6;
                else if (realIndex == 6) realIndex = 5;
                return_value.y = 0.2f;
                return_value.z = 9.51f;
            }

            switch (realIndex)
            {
                case 0:
                    return_value.x = -17.2f;
                    break;
                case 1:
                    return_value.x = -8.6f;
                    break;
                case 2:
                    return_value.x = 0f;
                    break;
                case 3:
                    return_value.x = 8.6f;
                    break;
                case 4:
                    return_value.x = 17.2f;
                    break;
                case 5:
                    return_value.x = -8.6f;
                    return_value.z = 0;
                    break;
                case 6:
                    return_value.x = 8.6f;
                    return_value.z = 0;
                    break;
            }

            return_value.x *= real;
        }

        if ((p.location & (uint) CardLocation.SpellZone) > 0)
        {
            if (p.sequence < 5 || (p.sequence == 6 || p.sequence == 7) && MasterRule >= 4)
            {
                var realIndex = p.sequence;
                if (p.controller == 0)
                {
                    realIndex = p.sequence;
                    return_value.y = 0.2f;
                    return_value.z = -18f;
                }
                else
                {
                    if (realIndex <= 4)
                        realIndex = 4 - p.sequence;
                    else if (realIndex == 7)
                        realIndex = 6;
                    else if (realIndex == 6) realIndex = 7;
                    return_value.y = 0.2f;
                    return_value.z = 18f;
                }

                switch (realIndex)
                {
                    case 0:
                        return_value.x = -17.2f;
                        break;
                    case 1:
                        return_value.x = -8.6f;
                        break;
                    case 2:
                        return_value.x = 0f;
                        break;
                    case 3:
                        return_value.x = 8.6f;
                        break;
                    case 4:
                        return_value.x = 17.2f;
                        break;
                    case 6:
                        return_value.x = -8.6f;
                        break;
                    case 7:
                        return_value.x = 8.6f;
                        break;
                }

                return_value.x *= real;
                if (gameField.isLong)
                    if (p.controller == 1)
                        if (5.85f * real < 10f)
                            return_value.z = return_value.z - 5.85f * real + 10f;
            }

            if (p.sequence == 5)
            {
                if (p.controller == 0)
                    return_value = new Vector3(-25f * real, 0.1f, -10f);
                else
                    return_value = new Vector3(25f * real, 0.1f, 10f);
            }

            if (MasterRule <= 3)
            {
                if (p.sequence == 6)
                {
                    if (p.controller == 0)
                        return_value = new Vector3(-30f * real, 10, -15f);
                    else
                        return_value = new Vector3(30f * real, 10, 10f);
                }

                if (p.sequence == 7)
                {
                    if (p.controller == 0)
                        return_value = new Vector3(30f * real, 10, -15f);
                    else
                        return_value = new Vector3(-30f * real, 10, 10f);
                }
            }
        }

        if ((p.location & (uint) CardLocation.Overlay) > 0)
        {
            if (c != null)
            {
                var pposition = c.overFatherCount - 1 - p.position;
                return_value.y -= (pposition + 2) * 0.02f;
                return_value.x += (pposition + 1) * 0.2f;
            }
            else
            {
                return_value.y -= (p.position + 2) * 0.02f;
                return_value.x += (p.position + 1) * 0.2f;
            }
        }

        return return_value;
    }

    public override void initialize()
    {
        Arrow = Object.Instantiate(Program.I().New_arrow).GetComponent<arrow>();
        Arrow.gameObject.SetActive(false);
        replayShowAll = Config.Get("replayShowAll", "0") != "0";
        reportShowAll = Config.Get("reportShowAll", "0") != "0";


        gameInfo = Program.I().new_ui_gameInfo;


        gameInfo.ini();
        UIHelper.InterGameObject(gameInfo.gameObject);
        shiftCondition(Condition.duel);

        Program.go(1, () =>
        {
            MHS_creatBundle(60, localPlayer(0), CardLocation.Deck);
            MHS_creatBundle(15, localPlayer(0), CardLocation.Extra);
            MHS_creatBundle(60, localPlayer(1), CardLocation.Deck);
            MHS_creatBundle(15, localPlayer(1), CardLocation.Extra);
            for (var i = 0; i < cards.Count; i++) cards[i].hide();
        });
    }

    public override void applyHideArrangement()
    {
        base.applyHideArrangement();
        gameInfo.gameObject.SetActive(false);
        hideCaculator();
    }

    public override void applyShowArrangement()
    {
        base.applyShowArrangement();
        if (gameInfo.gameObject.activeInHierarchy == false)
        {
            gameInfo.gameObject.transform.localPosition = new Vector3(300, 0, 0);
            gameInfo.gameObject.SetActive(true);
            iTween.MoveToLocal(gameInfo.gameObject, Vector3.zero, 0.6f);
            gameInfo.ini();
        }
        if (Program.I().setting.setting.timming.value)
            gameInfo.set_condition(gameInfo.chainCondition.smart);
        else
            gameInfo.set_condition(gameInfo.chainCondition.standard);
    }

    public void shiftCondition(Condition condition)
    {
        this.condition = condition;
        switch (condition)
        {
            case Condition.duel:
                CreateBar(Program.I().new_bar_duel, 0, 0);
                UIHelper.registEvent(toolBar, "input_", onChat);
                UIHelper.registEvent(toolBar, "gg_", onDuelResultConfirmed);
                UIHelper.registEvent(toolBar, "left_", on_left);
                UIHelper.registEvent(toolBar, "right_", on_right);
                UIHelper.registEvent(toolBar, "rush_", on_rush);
                UIHelper.addButtonEvent_toolShift(toolBar, "go_", on_go);
                UIHelper.addButtonEvent_toolShift(toolBar, "stop_", on_stop);
                break;
            case Condition.watch:
                CreateBar(Program.I().new_bar_watchDuel, 0, 0);
                UIHelper.registEvent(toolBar, "input_", onChat);
                UIHelper.registEvent(toolBar, "exit_", onExit);
                UIHelper.registEvent(toolBar, "left_", on_left);
                UIHelper.registEvent(toolBar, "right_", on_right);
                UIHelper.addButtonEvent_toolShift(toolBar, "go_", on_go);
                UIHelper.addButtonEvent_toolShift(toolBar, "stop_", on_stop);
                break;
            case Condition.record:
                CreateBar(Program.I().new_bar_watchRecord, 0, 0);
                UIHelper.registEvent(toolBar, "home_", onHome);
                UIHelper.registEvent(toolBar, "left_", on_left);
                UIHelper.registEvent(toolBar, "right_", on_right);
                UIHelper.addButtonEvent_toolShift(toolBar, "go_", on_go);
                UIHelper.addButtonEvent_toolShift(toolBar, "stop_", on_stop);
                break;
        }
    }


    public void dangerTicking()
    {
        if (paused)
        {
            RMSshow_none(InterString.Get("您的时间不足无法使用ReadingSteiner，时间线强制收束！"));
            on_rush();
        }
    }

    private void on_left()
    {
        if (winCaculator != null) destroy(winCaculator.gameObject);
        var preStepPackagesIndex = 0;
        for (var i = 0; i < keys.Count; i++)
            if (keys[i] < currentMessageIndex)
            {
                preStepPackagesIndex = keys[i];
                break;
            }

        if (keys.Count > 0)
            if (keys[0] != currentMessageIndex)
                for (var i = 0; i < keys.Count; i++)
                    if (keys[i] < preStepPackagesIndex)
                    {
                        preStepPackagesIndex = keys[i];
                        break;
                    }

        if (Packages_ALL.Count <= preStepPackagesIndex) return;
        if (condition == Condition.duel)
        {
            if (cantCheckGrave)
            {
                RMSshow_none(InterString.Get("不能确认墓地里的卡，无法跨越时间线！"));
                return;
            }

            if (gameInfo.amIdanger())
            {
                RMSshow_none(InterString.Get("您的时间不足无法使用ReadingSteiner！"));
                return;
            }
        }

        var needSwap = gameInfo.swaped;
        right = false;
        if (paused == false) EventDelegate.Execute(UIHelper.getByName<UIButton>(toolBar, "stop_").onClick);
        keys.Clear();
        currentMessageIndex = -1;
        Program.I().book.clear();
        inSkiping = true;
        for (var i = 0; i <= preStepPackagesIndex; i++)
            if (i == preStepPackagesIndex)
            {
                currentMessage = (GameMessage) Packages_ALL[i].Fuction;
                //太阳
                try
                {
                    logicalizeMessage(Packages_ALL[i]);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                if (needSwap) GCS_swapALL(false);
                try
                {
                    practicalizeMessage(Packages_ALL[i]);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                clearResponse();
            }
            else
            {
                currentMessage = (GameMessage) Packages_ALL[i].Fuction;
                try
                {
                    logicalizeMessage(Packages_ALL[i]);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }

        Packages.Clear();
        for (var i = 0; i < Packages_ALL.Count - preStepPackagesIndex - 1; i++)
            Packages.Add(Packages_ALL[i + preStepPackagesIndex + 1]);
        specialLR();
        inSkiping = false;
    }

    private void specialLR()
    {
        try
        {
            if (LRCgo != null) destroy(LRCgo);
            if (gameField != null) gameField.shiftBlackHole(false, new Vector3(0, 0, 0));
            Nconfirm();
            cardsForConfirm.Clear();
            if (flagForTimeConfirm)
            {
                flagForTimeConfirm = false;
                MessageBeginTime = Program.TimePassed();
                clearAllShowed();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private void on_right()
    {
        specialLR();
        if (right)
        {
            inSkiping = true;
            while (keys.Count == keysTempCount && Packages.Count > 0)
            {
                currentMessage = (GameMessage) Packages[0].Fuction;
                try
                {
                    logicalizeMessage(Packages[0]);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                try
                {
                    practicalizeMessage(Packages[0]);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                Packages.RemoveAt(0);
            }

            inSkiping = false;
        }

        right = true;
        keysTempCount = keys.Count;
    }

    private void on_rush()
    {
        specialLR();
        while (Packages.Count > 0)
        {
            currentMessage = (GameMessage) Packages[0].Fuction;
            try
            {
                logicalizeMessage(Packages[0]);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            if (Packages.Count == 1)
                try
                {
                    practicalizeMessage(Packages[0]);
                    realize();
                    toNearest();
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

            Packages.RemoveAt(0);
        }

        keysTempCount = keys.Count;
        if (paused) EventDelegate.Execute(UIHelper.getByName<UIButton>(toolBar, "go_").onClick);
    }

    private void on_go()
    {
        paused = false;
        if (condition == Condition.duel)
        {
            if (isShowed)
            {
                UIHelper.playSound("phase", 1f);
                gameField.animation_show_big_string(Program.I().ts, true);
            }

            //Program.I().cardDescription.clearAllLog();
            RMSshow_none(InterString.Get("[7CFC00]ReadingSteiner结束，回归到主时间轴。[-]"));
            Program.I().cardDescription.setTitle("");
        }
    }

    private void on_stop()
    {
        if (cantCheckGrave)
        {
            RMSshow_none(InterString.Get("不能确认墓地里的卡，无法跨越时间线！"));
            return;
        }

        if (paused == false)
        {
            //destroy(waitObject, 0, false, true);
            PlayableGuide.state = 1;
            paused = true;
            if (currentMessageIndex > theWorldIndex) theWorldIndex = currentMessageIndex;
        }

        if (condition == Condition.record) return;
        if (condition == Condition.duel)
        {
            if (isShowed)
            {
                UIHelper.playSound("nextturn", 1f);
                gameField.animation_show_big_string(Program.I().rs, true);
            }

            Program.I().cardDescription.clearAllLog();
            RMSshow_none(InterString.Get("[FF3030]ReadingSteiner被启动成功！您现在可以随意操作时间。@n长按按钮跳跃时间，闪电按钮回到现在。[-]"));
            Program.I().cardDescription.setTitle(InterString.Get("[FF3030]ReadingSteiner 正在跨越时间线[-]"));
        }
    }

    public void onHome()
    {
        returnTo();
    }

    public void returnTo()//mark 退出按钮
    {
        TcpHelper.SaveRecord();
        if (Program.exitOnReturn && returnServant != Program.I().newDeckManager)
            Program.I().menu.onClickExit();
        else if (returnServant != null)
        {
            Program.I().shiftToServant(returnServant);
            if(returnServant != Program.I().newDeckManager)
            {
                BGMHandler.ChangeBGM("menu");
                BackgroundControl.Show();
            }
            else
            {
                BGMHandler.ChangeBGM("deck");
                BackgroundControl.Show();
            }
        }
        else
        {
            Program.I().shiftToServant(Program.I().selectServer);
        }
    }

    public void onExit()
    {
        if (TcpHelper.tcpClient != null)
        {
            if (TcpHelper.tcpClient.Connected)
            {
                //Program.I().ocgcore.returnServant = Program.I().selectServer;
                TcpHelper.tcpClient.Client.Shutdown(0);
                TcpHelper.tcpClient.Close();
            }
            //Program.I().aiRoom.killServerProcess();
        }
        YgoServer.StopServer();
        TcpHelper.tcpClient = null;
        returnTo();
    }

    public void onChat()
    {
        Program.I().room.onSubmit(UIHelper.getByName<UIInput>(toolBar, "input_").value);
        UIHelper.getByName<UIInput>(toolBar, "input_").value = "";
    }

    public void addPackage(Package p)
    {
        TcpHelper.AddRecordLine(p);
        Packages.Add(p);
        Packages_ALL.Add(p);
    }

    public void flushPackages(List<Package> ps)
    {
        Packages.Clear();
        Packages = null;
        Packages = ps;
        Packages_ALL.Clear();
        foreach (var item in Packages) Packages_ALL.Add(item);
    }

    private void pre200Frame()
    {
        lastReszieTime = Program.TimePassed();
        if (lastSize != Program.fieldSize || lastAlpha != Program.getVerticalTransparency())
        {
            lastSize = Program.fieldSize;
            lastAlpha = Program.getVerticalTransparency();
            reSize();
        }

        if (allChainPanelFixedContainer.Count > 0)
        {
            allChainPanelFixedContainer.RemoveAll(a => { return a == null; });
            for (var i = 0; i < allChainPanelFixedContainer.Count; i++)
                allChainPanelFixedContainer[i].transform.localPosition = Vector3.zero;
            var groups = new List<List<GameObject>>();
            for (var i = 0; i < allChainPanelFixedContainer.Count; i++)
            {
                var currentGameobject = allChainPanelFixedContainer[i];
                List<GameObject> toList = null;
                for (var a = 0; a < groups.Count; a++)
                    if (UIHelper.getScreenDistance(groups[a][0], currentGameobject) < 5f * Screen.height / 700f)
                        toList = groups[a];
                if (toList == null)
                {
                    toList = new List<GameObject>();
                    groups.Add(toList);
                }

                toList.Add(currentGameobject);
            }

            for (var a = 0; a < groups.Count; a++)
            for (var b = 0; b < groups[a].Count; b++)
                groups[a][b].transform.localPosition =
                    new Vector3(0.35f * (groups[a].Count - b - 1), 0, -0.05f * b - 0.2f);
        }
    }


    public override void preFrameFunction()
    {
        base.preFrameFunction();
        Program.reMoveCam(getScreenCenter());
        Program.cameraPosition.z += Program.wheelValue;
        if (Program.cameraPosition.z < camera_min)
        {
            Program.cameraPosition.z = camera_min;
        }
        if (Program.cameraPosition.z > camera_max)
        {
            Program.cameraPosition.z = camera_max;
        }
        //快捷键
        //if (Input.GetKeyDown(KeyCode.C)) gameInfo.set_condition(gameInfo.chainCondition.smart);
        if (Input.GetKeyDown(KeyCode.A)) gameInfo.set_condition(gameInfo.chainCondition.smart);
        if (Input.GetKeyDown(KeyCode.S)) gameInfo.set_condition(gameInfo.chainCondition.no);

        //if (Input.GetKeyUp(KeyCode.C)) gameInfo.set_condition(gameInfo.chainCondition.standard);
        if (Input.GetKeyUp(KeyCode.A)) gameInfo.set_condition(gameInfo.chainCondition.standard);
        if (Input.GetKeyUp(KeyCode.S)) gameInfo.set_condition(gameInfo.chainCondition.standard);

        if (Input.GetMouseButtonDown(2))
        {
            if (Program.I().book.isShowed)
                Program.I().book.hide();
            else
                Program.I().book.show();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
            if (Program.I().book.isShowed == false)
                Program.I().book.show();
        if (Input.GetKeyUp(KeyCode.Tab))
            if (Program.I().book.isShowed)
                Program.I().book.hide();
        if (paused == false) sibyl();
        if (right)
        {
            if (keys.Count == keysTempCount && Packages.Count > 0)
                sibyl();
            else
                right = false;
        }

        if (Program.TimePassed() > lastReszieTime + 200) pre200Frame();
    }

    private void sibyl()
    {
        try
        {
            var messageIsHandled = false;
            while (true)
            {
                if (Packages.Count == 0) break;
                var currentPackage = Packages[0];
                currentMessage = (GameMessage) currentPackage.Fuction;
                if (ifMessageImportant(currentPackage))
                    if (Program.TimePassed() < MessageBeginTime)
                        break;
                messageIsHandled = true;
                try
                {
                    logicalizeMessage(Packages[0]);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                try
                {
                    practicalizeMessage(Packages[0]);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }

                Packages.RemoveAt(0);
            }

            //if (messageIsHandled)
            //{
            //    realize(false);
            //}
            if (messageIsHandled)
                if (condition == Condition.record)
                    if (Packages.Count == 0)
                        RMSshow_none(InterString.Get("录像播放结束。"));
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    private bool ifMessageImportant(Package package)
    {
        var r = package.Data.reader;
        r.BaseStream.Seek(0, 0);
        var msg = (GameMessage) Packages[0].Fuction;
        switch (msg)
        {
            case GameMessage.Start:
            case GameMessage.Win:
            case GameMessage.ConfirmDecktop:
            case GameMessage.ConfirmCards:
            case GameMessage.ShuffleDeck:
            case GameMessage.ShuffleHand:
            case GameMessage.SwapGraveDeck:
            case GameMessage.ShuffleSetCard:
            case GameMessage.ReverseDeck:
            case GameMessage.DeckTop:
            case GameMessage.NewTurn:
            case GameMessage.NewPhase:
            case GameMessage.Move:
            case GameMessage.PosChange:
            case GameMessage.Swap:
            case GameMessage.ChainSolved:
            case GameMessage.ChainNegated:
            case GameMessage.ChainDisabled:
            case GameMessage.RandomSelected:
            case GameMessage.BecomeTarget:
            case GameMessage.Draw:
            case GameMessage.Damage:
            case GameMessage.Recover:
            case GameMessage.PayLpCost:
            case GameMessage.TossCoin:
            case GameMessage.TossDice:
            case GameMessage.TagSwap:
            case GameMessage.ReloadField:
                return true;
            case GameMessage.FlipSummoning:
            case GameMessage.Summoning: 
            case GameMessage.SpSummoning:
            case GameMessage.Chaining:
                return true;
            case GameMessage.Hint:
                int type = r.ReadChar();
                if (type == 8) return true;
                if (type == 10) return true;
                return false;
            case GameMessage.CardHint:
                r.ReadGPS();
                int ctype = r.ReadByte();
                if (ctype == 1) return true;
                return false;
            case GameMessage.SelectBattleCmd:
            case GameMessage.SelectIdleCmd:
            case GameMessage.SelectEffectYn:
            case GameMessage.SelectYesNo:
            case GameMessage.SelectOption:
            case GameMessage.SelectCard:
            case GameMessage.SelectPosition:
            case GameMessage.SelectTribute:
            case GameMessage.SortChain:
            case GameMessage.SelectCounter:
            case GameMessage.SelectSum:
            case GameMessage.SortCard:
            case GameMessage.AnnounceRace:
            case GameMessage.AnnounceAttrib:
            case GameMessage.AnnounceCard:
            case GameMessage.AnnounceNumber:
            case GameMessage.SelectDisfield:
            case GameMessage.SelectPlace:
                if (inIgnoranceReplay() || currentMessageIndex + 1 < theWorldIndex) return false;
                return true;
            case GameMessage.SelectChain:
                if (inIgnoranceReplay() || currentMessageIndex + 1 < theWorldIndex) return false;
                r.ReadChar();
                int count = r.ReadByte();
                int spcount = r.ReadByte();
                var hint0 = r.ReadInt32();
                var hint1 = r.ReadInt32();
                var ignore = false;
                bool forced = false;
                for (int i = 0; i < count; i++)
                {
                    r.ReadByte(); // flag
                    int f = r.ReadByte(); // forced
                    if (f == 1) forced = true;
                    r.ReadInt32(); // card id
                    r.ReadGPS();
                    r.ReadInt32(); // desc
                }
                if (!forced)
                {
                    var condition = gameInfo.get_condition();
                    if (condition == gameInfo.chainCondition.no)
                    {
                        ignore = true;
                    }
                    else
                    {
                        if (condition == gameInfo.chainCondition.all)
                        {
                            ignore = false;
                        }
                        else
                        {
                            if (condition == gameInfo.chainCondition.smart)
                            {
                                if (count == 0)
                                    ignore = true;
                                else
                                    ignore = false;
                            }
                            else
                            {
                                if (spcount == 0)
                                    ignore = true;
                                else
                                    ignore = false;
                            }
                        }
                    }
                }
                return true;
            case GameMessage.Attack:
                return true;
        }

        return false;
    }

    public void forceMSquit()
    {
        var p = new Package();
        p.Fuction = (int) GameMessage.sibyl_quit;
        Packages.Add(p);
    }

    private void logicalizeMessage(Package p)
    {
        currentMessageIndex++;
        var r = p.Data.reader;
        r.BaseStream.Seek(0, 0);
        var code = 0;
        var count = 0;
        var controller = 0;
        var location = 0;
        var sequence = 0;
        var player = 0;
        var data = 0;
        var type = 0;
        GPS gps;
        gameCard game_card;
        GPS from;
        GPS to;
        gameCard card;
        int val;
        string name;
        surrended = false;
        AnimationControl ac = GameObject.Find("new_gameField(Clone)/Assets_Loader").GetComponent<AnimationControl>();
        switch ((GameMessage) p.Fuction)
        {
            case GameMessage.sibyl_chat:
                printDuelLog(r.ReadALLUnicode());
                break;
            case GameMessage.sibyl_name:
                name_0 = r.ReadUnicode(50);
                name_0_tag = r.ReadUnicode(50);
                name_0_c = r.ReadUnicode(50);
                name_1 = r.ReadUnicode(50);
                name_1_tag = r.ReadUnicode(50);
                name_1_c = r.ReadUnicode(50);
                var isTag = !(name_0_tag == "---" && name_1_tag == "---" && name_0 == name_0_c && name_1 == name_1_c);
                if (isTag)
                {
                    if (isFirst)
                    {
                        name_0_c = name_0;
                        name_1_c = name_1_tag;
                    }
                    else
                    {
                        name_0_c = name_0_tag;
                        name_1_c = name_1;
                    }
                }

                if (r.BaseStream.Position < r.BaseStream.Length)
                    MasterRule = r.ReadInt32();
                else
                    MasterRule = 3;
                break;
            case GameMessage.AiName:
                int length = r.ReadUInt16();
                var buffer = r.ReadBytes(length + 1);
                var n = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                name_1 = n;
                name_1_tag = n;
                name_1_c = n;
                break;
            case GameMessage.Win:
                deckReserved = false;
                cantCheckGrave = false;
                player = localPlayer(r.ReadByte());
                int winType = r.ReadByte();
                keys.Insert(0, currentMessageIndex);
                if (player == 2)
                {
                    result = duelResult.draw;
                    printDuelLog(InterString.Get("游戏平局！"));
                }
                else if (player == 0 || winType == 4)
                {
                    result = duelResult.win;
                    if (cookie_matchKill > 0)
                    {
                        winReason = CardsManager.Get(cookie_matchKill).Name;
                        printDuelLog(InterString.Get("比赛胜利，卡片：[?]", winReason));
                        AnimationControl.p2hasEnteredEnd = true;
                    }
                    else
                    {
                        winReason = GameStringManager.get("victory", winType);
                        printDuelLog(InterString.Get("游戏胜利，原因：[?]", winReason));
                        AnimationControl.p2hasEnteredEnd = true;
                    }
                }
                else
                {
                    result = duelResult.lose;
                    if (cookie_matchKill > 0)
                    {
                        winReason = CardsManager.Get(cookie_matchKill).Name;
                        printDuelLog(InterString.Get("比赛败北，卡片：[?]", winReason));
                        AnimationControl.p1hasEnteredEnd = true;
                    }
                    else
                    {
                        winReason = GameStringManager.get("victory", winType);
                        printDuelLog(InterString.Get("游戏败北，原因：[?]", winReason));
                        AnimationControl.p1hasEnteredEnd = true;
                    }
                }

                break;
            case GameMessage.Start:
                confirmedCards.Clear();
                gameField.currentPhase = GameField.ph.dp;
                result = duelResult.disLink;
                logicalClearChain();
                surrended = false;
                Program.I().room.duelEnded = false;
                Program.I().room.joinWithReconnect = false;
                turns = 0;
                deckReserved = false;
                cantCheckGrave = false;
                keys.Insert(0, currentMessageIndex);
                RMSshow_clear();
                md5Maker = 0;
                for (var i = 0; i < cards.Count; i++)
                {
                    cards[i].p.location = (uint)CardLocation.Unknown;                    
                }
                int playertype = r.ReadByte();
                isFirst = (playertype & 0xf) > 0 ? false : true;
                gameInfo.swaped = false;
                isObserver = (playertype & 0xf0) > 0 ? true : false;
                if (r.BaseStream.Length > 17) // dumb fix for yrp3d replay older than v1.034.9
                    MasterRule = r.ReadByte(); // duel_rule
                life_0 = r.ReadInt32();
                life_1 = r.ReadInt32();
                lpLimit = life_0;
                name_0_c = name_0;
                name_1_c = name_1;
                if (Program.I().room.mode == 2)
                {
                    if (isFirst)
                        name_1_c = name_1_tag;
                    else
                        name_0_c = name_0_tag;
                }

                cookie_matchKill = 0;
                MHS_creatBundle(r.ReadInt16(), localPlayer(0), CardLocation.Deck);
                MHS_creatBundle(r.ReadInt16(), localPlayer(0), CardLocation.Extra);
                MHS_creatBundle(r.ReadInt16(), localPlayer(1), CardLocation.Deck);
                MHS_creatBundle(r.ReadInt16(), localPlayer(1), CardLocation.Extra);
                gameField.clearDisabled();
                if (Program.I().room.mode == 0) printDuelLog(InterString.Get("单局模式 决斗开始！"));
                if (Program.I().room.mode == 1) printDuelLog(InterString.Get("比赛模式 决斗开始！"));
                if (Program.I().room.mode == 2) printDuelLog(InterString.Get("双打模式 决斗开始！"));
                printDuelLog(InterString.Get("双方生命值：[?]", lpLimit.ToString()));
                printDuelLog(InterString.Get("Tip：鼠标中键/[FF0000]TAB键[-]可以打开/关闭哦。"));
                printDuelLog(InterString.Get("Tip：强烈建议使用[FF0000]TAB键[-]。"));
                arrangeCards();
                Sleep(21);
                break;
            case GameMessage.ReloadField:
                MasterRule = r.ReadByte() + 1;
                if (MasterRule > 255) MasterRule -= 255;
                confirmedCards.Clear();
                gameField.currentPhase = GameField.ph.dp;
                result = duelResult.disLink;
                deckReserved = false;
                // isFirst = true;
                gameInfo.swaped = false;
                logicalClearChain();
                surrended = false;
                Program.I().room.duelEnded = false;
                turns = 0;
                keys.Insert(0, currentMessageIndex);
                RMSshow_clear();
                md5Maker = 0;
                for (var i = 0; i < cards.Count; i++)
                    if (cards[i].gameObject.activeInHierarchy)
                    {
                        cards[i].p.location = (uint)CardLocation.Unknown;
                    }
                cookie_matchKill = 0;

                if (Program.I().room.mode == 0) printDuelLog(InterString.Get("单局模式 决斗开始！"));
                if (Program.I().room.mode == 1) printDuelLog(InterString.Get("比赛模式 决斗开始！"));
                if (Program.I().room.mode == 2) printDuelLog(InterString.Get("双打模式 决斗开始！"));
                printDuelLog(InterString.Get("双方生命值：[?]", lpLimit.ToString()));
                printDuelLog(InterString.Get("Tip：鼠标中键/[FF0000]TAB键[-]可以打开/关闭哦。"));
                printDuelLog(InterString.Get("Tip：强烈建议使用[FF0000]TAB键[-]。"));
                for (var p_ = 0; p_ < 2; p_++)
                {
                    player = localPlayer(p_);
                    if (player == 0)
                        life_0 = r.ReadInt32();
                    else
                        life_1 = r.ReadInt32();
                    for (var i = 0; i < 7; i++)
                    {
                        val = r.ReadByte();
                        if (val > 0)
                        {
                            gps = new GPS
                            {
                                controller = (uint) player,
                                location = (uint) CardLocation.MonsterZone,
                                position = r.ReadByte(),
                                sequence = (uint) i
                            };
                            GCS_cardCreate(gps);
                            val = r.ReadByte();
                            for (var xyz = 0; xyz < val; ++xyz)
                            {
                                gps.location |= (uint) CardLocation.Overlay;
                                gps.position = xyz;
                                GCS_cardCreate(gps);
                            }
                        }
                    }

                    for (var i = 0; i < 8; i++)
                    {
                        val = r.ReadByte();
                        if (val > 0)
                        {
                            gps = new GPS
                            {
                                controller = (uint) player,
                                location = (uint) CardLocation.SpellZone,
                                position = r.ReadByte(),
                                sequence = (uint) i
                            };
                            GCS_cardCreate(gps);
                        }
                    }

                    val = r.ReadByte();
                    for (var i = 0; i < val; i++)
                    {
                        gps = new GPS
                        {
                            controller = (uint) player,
                            location = (uint) CardLocation.Deck,
                            position = (int) CardPosition.FaceDownAttack,
                            sequence = (uint) i
                        };
                        GCS_cardCreate(gps);
                    }

                    val = r.ReadByte();
                    for (var i = 0; i < val; i++)
                    {
                        gps = new GPS
                        {
                            controller = (uint) player,
                            location = (uint) CardLocation.Hand,
                            position = (int) CardPosition.FaceDownAttack,
                            sequence = (uint) i
                        };
                        GCS_cardCreate(gps);
                    }

                    val = r.ReadByte();
                    for (var i = 0; i < val; i++)
                    {
                        gps = new GPS
                        {
                            controller = (uint) player,
                            location = (uint) CardLocation.Grave,
                            position = (int) CardPosition.FaceUpAttack,
                            sequence = (uint) i
                        };
                        GCS_cardCreate(gps);
                    }

                    val = r.ReadByte();
                    for (var i = 0; i < val; i++)
                    {
                        gps = new GPS
                        {
                            controller = (uint) player,
                            location = (uint) CardLocation.Removed,
                            position = (int) CardPosition.FaceUpAttack,
                            sequence = (uint) i
                        };
                        GCS_cardCreate(gps);
                    }

                    val = r.ReadByte();
                    int val_up = r.ReadByte();
                    for (var i = 0; i < val - val_up; i++)
                    {
                        gps = new GPS
                        {
                            controller = (uint) player,
                            location = (uint) CardLocation.Extra,
                            position = (int) CardPosition.FaceDownAttack,
                            sequence = (uint) i
                        };
                        GCS_cardCreate(gps);
                    }

                    for (var i = 0; i < val_up; i++)
                    {
                        gps = new GPS
                        {
                            controller = (uint) player,
                            location = (uint) CardLocation.Extra,
                            position = (int) CardPosition.FaceUpAttack,
                            sequence = (uint) (val + i)
                        };
                        GCS_cardCreate(gps);
                    }
                }

                gameField.clearDisabled();
                arrangeCards();
                break;
            case GameMessage.UpdateData:
                controller = localPlayer(r.ReadChar());
                location = r.ReadChar();
                try
                {
                    while (true)
                    {
                        var len = r.ReadInt32();
                        if (len == 4) continue;
                        var pos = r.BaseStream.Position;
                        r.readCardData();
                        r.BaseStream.Position = pos + len - 4;
                    }
                }
                catch (Exception e)
                {
                    // UnityEngine.Debug.Log(e);
                }

                break;
            case GameMessage.UpdateCard:
                gps = r.ReadShortGPS();
                var cardToRefresh = GCS_cardGet(gps, false);
                r.ReadUInt32();
                r.readCardData(cardToRefresh);

                //Transform childCard = cardToRefresh.gameObject_face.transform.parent;
                //if ((cardToRefresh.p.location & (uint)CardLocation.Grave) > 0
                //    ||
                //    (cardToRefresh.p.location & (uint)CardLocation.Removed) > 0
                //    )
                //{
                //    childCard.localPosition = new Vector3 (0, -1, 0);
                //    childCard.localEulerAngles = new Vector3(0, 0, 180);
                //    childCard.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                //}
                //else
                //{
                //    childCard.localPosition = new Vector3(0, 0, 0);
                //    childCard.localEulerAngles = new Vector3(90, 0, 0);
                //    childCard.localScale = new Vector3(1, 1, 1);
                //}
                break;
            case GameMessage.ReverseDeck:
                deckReserved = !deckReserved;
                break;
            case GameMessage.Move:
                keys.Insert(0, currentMessageIndex);
                code = r.ReadInt32();
                from = r.ReadGPS();
                to = r.ReadGPS();
                uint reason = r.ReadUInt32();
                card = GCS_cardGet(from, false);
                if (card != null)
                {
                    card.set_code(code);
                    if (Program.I().setting.setting.Voice.value)
                    {
                        if (
                            ((reason & (uint)CardReason.SPSUMMON) > 0
                            ||
                            ((to.location & (uint)CardLocation.MonsterZone) > 0 && (from.location & (uint)CardLocation.Hand) > 0))
                            &&
                            (from.location & (uint)CardLocation.MonsterZone) == 0
                            &&
                            (to.position & (uint)CardPosition.FaceUp) > 0
                            &&
                            !inSkiping
                            )
                        {
                            int cardId = card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias;
                            var clip = VoiceHandler.GetClip("chants/summon/" + cardId);
                            if (clip != null)
                            {
                                VoiceHandler.voiceTime = clip.length;
                                Sleep((int)(clip.length * 60));
                                if((reason & (uint)CardReason.SPSUMMON) > 0)
                                    VoiceHandler.PlayVoice(clip, card.p.controller == 0, "特殊召唤 " + card.get_data().Name);
                                else
                                    VoiceHandler.PlayVoice(clip, card.p.controller == 0, "召唤 " + card.get_data().Name);
                                VoiceHandler.occupy = true;
                            }
                            else
                            {
                                VoiceHandler.voiceTime = 0f;
                                VoiceHandler.occupy = false;
                            }
                        }
                    }
                    else
                    {
                        VoiceHandler.occupy = false;
                        VoiceHandler.voiceTime = 0f;
                    }
                }
                GCS_cardMove(from, to);
                break;
            case GameMessage.PosChange:
                keys.Insert(0, currentMessageIndex);
                ES_hint = GameStringManager.get_unsafe(1600);
                code = r.ReadInt32();
                from = r.ReadGPS();
                to = from;
                to.position = r.ReadByte();
                card = GCS_cardGet(from, false);
                if (card != null)
                {
                    card.set_code(code);
                    if ((to.position & (uint)CardPosition.FaceUp) > 0 && (card.get_data().Type & (uint)CardType.Monster) > 0)
                    {
                        card.refreshFunctions.Add(card.card_verticle_drawing_handler);
                        card.card_verticle_drawing_handler();
                        card.LoadCard();
                    }
                }
                GCS_cardMove(from, to);
                break;
            case GameMessage.Set:
                ES_hint = GameStringManager.get_unsafe(1601);
                break;
            case GameMessage.Swap:
                keys.Insert(0, currentMessageIndex);
                ES_hint = GameStringManager.get_unsafe(1602);
                code = r.ReadInt32();
                from = r.ReadGPS();
                code = r.ReadInt32();
                to = r.ReadGPS();
                GCS_cardMove(from, to, true, true);
                break;
            case GameMessage.FlipSummoned:
                ES_hint = GameStringManager.get_unsafe(1608);
                break;
            case GameMessage.Summoned:
                ES_hint = GameStringManager.get_unsafe(1604);
                break;
            case GameMessage.SpSummoned:
                ES_hint = GameStringManager.get_unsafe(1606);
                break;
            case GameMessage.Chaining:
                code = r.ReadInt32();
                gps = r.ReadGPS();
                card = GCS_cardGet(gps, false);
                if (card != null)
                {
                    card.set_code(code);
                    cardsInChain.Add(card);
                    if (cardsInChain.Count == 1)
                    {
                        cardsInChain[0].CS_showBall();
                    }
                    else
                    {
                        cardsInChain[0].CS_ballToNumber();
                        cardsInChain[cardsInChain.Count - 1].CS_addChainNumber(cardsInChain.Count);
                    }

                    ES_hint = InterString.Get("「[?]」被发动时", card.get_data().Name);
                    if (card.p.controller == 0)
                    {
                        ///printDuelLog("●" + InterString.Get("[?]被发动", UIHelper.getGPSstringName(card)));
                    }
                }
                break;
            case GameMessage.ChainSolved:
                var id = r.ReadByte() - 1;
                if (id < 0) id = 0;
                if (id < cardsInChain.Count)
                {
                    card = cardsInChain[id];
                    card.CS_hideBall();
                    card.CS_removeOneChainNumber();
                }
                break;
            case GameMessage.ChainEnd:
                logicalClearChain();
                foreach (var ccard in cards)
                    ccard.negated = false;
                break;
            case GameMessage.ChainNegated:
            case GameMessage.ChainDisabled:
                var id_ = r.ReadByte() - 1;
                if (id_ < 0) id_ = 0;
                if (id_ < cardsInChain.Count)
                {
                    card = cardsInChain[id_];
                    card.CS_hideBall();
                    card.CS_removeOneChainNumber();
                }

                break;
            case GameMessage.Damage:
                ES_hint = InterString.Get("玩家受到伤害时");
                player = localPlayer(r.ReadByte());
                player = unSwapPlayer(player);
                val = r.ReadInt32();                
                if (player == 0)
                {
                    //printDuelLog(InterString.Get("受到伤害[?]", val.ToString()));
                    if (!inSkiping)
                    {
                        AnimationControl.PlayAnimation(LoadAssets.mate1_, "Damage", true);
                        AnimationControl.PlayAnimation(LoadAssets.field1_, "PhaseToDamagePhaseAll");
                    }
                }
                else 
                    if (!inSkiping)
                    {
                        AnimationControl.PlayAnimation(LoadAssets.mate2_, "Damage", true);
                        AnimationControl.PlayAnimation(LoadAssets.field2_, "PhaseToDamagePhaseAll");
                    }

                if (player == 0)
                    life_0 -= val;
                else
                    life_1 -= val;
                break;
            case GameMessage.PayLpCost:
                player = localPlayer(r.ReadByte());
                player = unSwapPlayer(player);
                val = r.ReadInt32();
                if (player == 0)
                {
                    //printDuelLog(InterString.Get("支付生命值[?]", val.ToString()));
                    if (!Ocgcore.inSkiping)
                    {
                        AnimationControl.PlayAnimation(LoadAssets.mate1_, "Cost");
                        AnimationControl.PlayAnimation(LoadAssets.field1_, "PhaseToDamagePhaseAll");
                    }
                }
                else 
                    if (!Ocgcore.inSkiping)
                {
                    AnimationControl.PlayAnimation(LoadAssets.mate2_, "Cost");
                    AnimationControl.PlayAnimation(LoadAssets.field2_, "PhaseToDamagePhaseAll");
                }

                if (player == 0)
                    life_0 -= val;
                else
                    life_1 -= val;
                break;
            case GameMessage.Recover:
                ES_hint = InterString.Get("玩家生命值回复时");
                player = localPlayer(r.ReadByte());
                player = unSwapPlayer(player);
                val = r.ReadInt32();
                if (player == 0)
                {
                    //printDuelLog(InterString.Get("回复生命值[?]", val.ToString()));
                }

                if (player == 0)
                    life_0 += val;
                else
                    life_1 += val;
                break;
            case GameMessage.LpUpdate:
                player = localPlayer(r.ReadByte());
                player = unSwapPlayer(player);
                val = r.ReadInt32();
                if (player == 0)
                {
                    //printDuelLog(InterString.Get("刷新生命值[?]", val.ToString()));
                }

                if (player == 0)
                    life_0 = val;
                else
                    life_1 = val;
                break;
            case GameMessage.RandomSelected:
                player = localPlayer(r.ReadByte());
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    gps = r.ReadGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null) printDuelLog(InterString.Get("对象选择：[?]", UIHelper.getGPSstringName(card)));
                }

                break;
            case GameMessage.BecomeTarget:
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    gps = r.ReadGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        printDuelLog(InterString.Get("对象选择：[?]", UIHelper.getGPSstringName(card)));

                        if (cardsInChain.Count > 0)
                            card.cardsTargetMe.Add(cardsInChain[cardsInChain.Count - 1]);
                    }
                }
                break;
            case GameMessage.TossCoin:
                player = r.ReadByte();
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    data = r.ReadByte();
                    if (data == 0)
                        printDuelLog(InterString.Get("硬币反面"));
                    else
                        printDuelLog(InterString.Get("硬币正面"));
                }

                break;
            case GameMessage.TossDice:
                player = r.ReadByte();
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    data = r.ReadByte();
                    printDuelLog(InterString.Get("骰子结果：[?]", data.ToString()));
                }

                break;
            case GameMessage.HandResult:
                data = r.ReadByte();
                var res1 = (data & 0x3) - 1;
                var res2 = ((data >> 2) & 0x3) - 1;
                if (isFirst)
                {
                    Program.I().new_ui_handShower.GetComponent<handShower>().me = res1;
                    Program.I().new_ui_handShower.GetComponent<handShower>().op = res2;
                }
                else
                {
                    Program.I().new_ui_handShower.GetComponent<handShower>().me = res2;
                    Program.I().new_ui_handShower.GetComponent<handShower>().op = res1;
                }

                var handres = create(Program.I().new_ui_handShower, Vector3.zero, Vector3.zero, false,
                    Program.I().ui_main_2d);
                destroy(handres, 10f);
                Sleep(60);
                break;
            case GameMessage.Attack:
                game_card = GCS_cardGet(r.ReadGPS(), false);
                var derectattack = "";
                if (game_card != null)
                {
                    name = game_card.get_data().Name;
                    ES_hint = InterString.Get("「[?]」攻击时", game_card.get_data().Name);
                    //printDuelLog("●" + InterString.Get("[?]发动攻击！", UIHelper.getGPSstringLocation(game_card.p) + UIHelper.getGPSstringName(game_card)));
                    if (game_card.p.controller == 0)
                        derectattack = "●" + InterString.Get("对方被直接攻击！");
                    else
                        derectattack = "●" + InterString.Get("被直接攻击！");
                }

                game_card = GCS_cardGet(r.ReadGPS(), false);
                if (game_card != null)
                    name = game_card.get_data().Name;
                //printDuelLog("●" + InterString.Get("[?]被攻击！", UIHelper.getGPSstringLocation(game_card.p) + UIHelper.getGPSstringName(game_card)));

                break;
            case GameMessage.AttackDisabled:
                ES_hint = InterString.Get("攻击被无效时");
                //printDuelLog(InterString.Get("攻击被无效"));
                break;
            case GameMessage.Battle:
                break;
            case GameMessage.FlipSummoning:
                code = r.ReadInt32();
                name = CardsManager.Get(code).Name;
                card = GCS_cardGet(r.ReadShortGPS(), false);
                if (card != null)
                {
                    card.set_code(code);
                    card.p.position = (int) CardPosition.FaceUpAttack;
                    card.refreshData();
                    ES_hint = InterString.Get("「[?]」反转召唤宣言时", card.get_data().Name);
                    if (card.p.controller == 0)
                    {
                        //printDuelLog("●" + InterString.Get("[?]被反转召唤", UIHelper.getGPSstringName(card)));
                    }
                }
                break;
            case GameMessage.Summoning:
                code = r.ReadInt32();
                name = CardsManager.Get(code).Name;
                card = GCS_cardGet(r.ReadShortGPS(), false);
                if (card != null)
                {
                    card.set_code(code);
                    ES_hint = InterString.Get("「[?]」通常召唤宣言时", card.get_data().Name);
                    if (card.p.controller == 0)
                    {
                        //printDuelLog("●" + InterString.Get("[?]被通常召唤", UIHelper.getGPSstringName(card)));
                    }
                }

                break;
            case GameMessage.SpSummoning:                
                code = r.ReadInt32();
                name = CardsManager.Get(code).Name;
                card = GCS_cardGet(r.ReadShortGPS(), false);
                if (card != null)
                {
                    card.set_code(code);
                    card.add_string_tail(GameStringHelper.teshuzhaohuan);
                    ES_hint = InterString.Get("「[?]」特殊召唤宣言时", card.get_data().Name);
                    if (card.p.controller == 0)
                    {
                        //printDuelLog("●" + InterString.Get("[?]被特殊召唤", UIHelper.getGPSstringName(card)));
                    }
                }

                break;
            case GameMessage.Draw:
                keys.Insert(0, currentMessageIndex);
                ES_hint = InterString.Get("玩家抽卡时");
                controller = localPlayer(r.ReadByte());
                count = r.ReadByte();
                var deckCC = MHS_getBundle(controller, (int) CardLocation.Deck).Count;
                for (var isa = 0; isa < count; isa++)
                {
                    card = GCS_cardMove(
                        new GPS
                        {
                            controller = (uint)controller,
                            location = (uint)CardLocation.Deck,
                            sequence = (uint)(deckCC - 1 - isa),
                            position = (int)CardPosition.FaceDownAttack
                        },
                        new GPS
                        {
                            controller = (uint)controller,
                            location = (uint)CardLocation.Hand,
                            sequence = 1000,
                            position = (int)CardPosition.FaceDownAttack
                        }, false);
                    card.set_code(r.ReadInt32() & 0x7fffffff);
                    if (controller == 0)
                    {
                        //printDuelLog(InterString.Get("抽卡[?]", UIHelper.getGPSstringName(card)));
                    }
                }

                break;
            case GameMessage.TagSwap:
                keys.Insert(0, currentMessageIndex);
                controller = localPlayer(r.ReadByte());
                if (controller == 0)
                {
                    if (name_0_c == name_0)
                        name_0_c = name_0_tag;
                    else
                        name_0_c = name_0;
                }
                else
                {
                    if (name_1_c == name_1)
                        name_1_c = name_1_tag;
                    else
                        name_1_c = name_1;
                }

                int mcount = r.ReadByte();
                var cardsInDeck = MHS_resizeBundle(mcount, controller, CardLocation.Deck);
                int ecount = r.ReadByte();
                var cardsInExtra = MHS_resizeBundle(ecount, controller, CardLocation.Extra);
                int pcount = r.ReadByte();
                int hcount = r.ReadByte();
                var cardsInHand = MHS_resizeBundle(hcount, controller, CardLocation.Hand);
                if (cardsInDeck.Count > 0) cardsInDeck[cardsInDeck.Count - 1].set_code(r.ReadInt32());
                for (var i = 0; i < cardsInHand.Count; i++) cardsInHand[i].set_code(r.ReadInt32());
                for (var i = 0; i < cardsInExtra.Count; i++) cardsInExtra[i].set_code(r.ReadInt32() & 0x7fffffff);
                for (var i = 0; i < pcount; i++)
                    if (cardsInExtra.Count - 1 - i > 0)
                        cardsInExtra[cardsInExtra.Count - 1 - i].p.position = (int) CardPosition.FaceUpAttack;
                if (controller == 0)
                {
                    //printDuelLog(InterString.Get("切换玩家，手牌张数变为[?]", hcount.ToString()));
                }

                //Program.DEBUGLOG("TAG SWAP->controller:" + controller + "mcount:" + mcount + "ecount:" + ecount + "pcount:" + pcount + "hcount:" + hcount);
                break;
            case GameMessage.MatchKill:
                cookie_matchKill = r.ReadInt32();
                break;
            case GameMessage.PlayerHint:
                controller = localPlayer(r.ReadByte());
                int ptype = r.ReadByte();
                var pvalue = r.ReadInt32();
                var valstring = GameStringManager.get(pvalue);
                if (pvalue == 38723936) valstring = InterString.Get("不能确认墓地里的卡");
                if (ptype == 6)
                {
                    if (controller == 0)
                        printDuelLog(InterString.Get("我方状态：[?]", valstring));
                    else
                        printDuelLog(InterString.Get("对方状态：[?]", valstring));
                }
                else if (ptype == 7)
                {
                    if (controller == 0)
                        printDuelLog(InterString.Get("我方取消状态：[?]", valstring));
                    else
                        printDuelLog(InterString.Get("对方取消状态：[?]", valstring));
                }
                break;
            case GameMessage.CardHint:
                game_card = GCS_cardGet(r.ReadGPS(), false);
                int ctype = r.ReadByte();
                var value = r.ReadInt32();
                if (game_card != null)
                {
                    if (ctype == 1)
                    {
                        game_card.del_one_tail(InterString.Get("数字记录："));
                        game_card.add_string_tail(InterString.Get("数字记录：") + value);
                    }

                    if (ctype == 2)
                    {
                        game_card.del_one_tail(InterString.Get("卡片记录："));
                        game_card.add_string_tail(InterString.Get("卡片记录：") +
                                                  UIHelper.getSuperName(CardsManager.Get(value).Name, value));
                    }

                    if (ctype == 3)
                    {
                        game_card.del_one_tail(InterString.Get("种族记录："));
                        game_card.add_string_tail(InterString.Get("种族记录：") + GameStringHelper.race(value));
                    }

                    if (ctype == 4)
                    {
                        game_card.del_one_tail(InterString.Get("属性记录："));
                        game_card.add_string_tail(InterString.Get("属性记录：") + GameStringHelper.attribute(value));
                    }

                    if (ctype == 5)
                    {
                        game_card.del_one_tail(InterString.Get("数字记录："));
                        game_card.add_string_tail(InterString.Get("数字记录：") + value);
                    }

                    if (ctype == 6) game_card.add_string_tail(GameStringManager.get(value));
                    if (ctype == 7) game_card.del_one_tail(GameStringManager.get(value));
                }
                break;
            case GameMessage.Hint:
                Es_selectMSGHintType = r.ReadChar();
                Es_selectMSGHintPlayer = localPlayer(r.ReadChar());
                Es_selectMSGHintData = r.ReadInt32();
                type = Es_selectMSGHintType;
                player = Es_selectMSGHintPlayer;
                data = Es_selectMSGHintData;
                if (type == 1) ES_hint = GameStringManager.get(data);
                if (type == 2) printDuelLog(GameStringManager.get(data));
                if (type == 3) ES_selectHint = GameStringManager.get(data);
                if (type == 4) printDuelLog(InterString.Get("效果选择：[?]", GameStringManager.get(data)));
                if (type == 5) printDuelLog(GameStringManager.get(data));
                if (type == 6) printDuelLog(InterString.Get("种族选择：[?]", GameStringHelper.race(data)));
                if (type == 7) printDuelLog(InterString.Get("属性选择：[?]", GameStringHelper.attribute(data)));
                if (type == 8)
                    printDuelLog(InterString.Get("卡片展示：[?]", UIHelper.getSuperName(CardsManager.Get(data).Name, data)));
                if (type == 9) printDuelLog(InterString.Get("数字选择：[?]", data.ToString()));
                if (type == 10)
                    printDuelLog(InterString.Get("卡片展示：[?]", UIHelper.getSuperName(CardsManager.Get(data).Name, data)));
                if (type == 11)
                {
                    if (player == 1)
                        data = (data >> 16) | (data << 16);
                    printDuelLog(InterString.Get("区域选择：[?]", GameStringHelper.zone(data)));
                }
                break;
            case GameMessage.MissedEffect:
                r.ReadInt32();
                code = r.ReadInt32();
                printDuelLog(InterString.Get("「[?]」失去了时点。", UIHelper.getSuperName(CardsManager.Get(code).Name, code)));
                break;
            case GameMessage.NewTurn:
                toDefaultHintLogical();
                gameField.currentPhase = GameField.ph.dp;
                //  keys.Insert(0, currentMessageIndex);
                player = localPlayer(r.ReadByte());
                if (player == 0)
                    ES_turnString = InterString.Get("我方的");
                else
                    ES_turnString = InterString.Get("对方的");
                MD_phaseString = "Draw";
                turns++;
                ES_phaseString = InterString.Get("回合");
                //printDuelLog(InterString.Get("进入[?]", ES_turnString + ES_phaseString)+"  "+ InterString.Get("回合计数[?]", turns.ToString()));
                ES_hint = ES_turnString + ES_phaseString;
                break;
            case GameMessage.NewPhase:
                toDefaultHintLogical();
                autoForceChainHandler = autoForceChainHandlerType.manDoAll;
                // keys.Insert(0, currentMessageIndex);
                var ph = r.ReadUInt16();
                if (ph == 0x01)
                {
                    ES_phaseString = InterString.Get("抽卡阶段");
                    MD_phaseString = "Draw";
                    MD_battleString = "";
                    gameField.currentPhase = GameField.ph.dp;
                }

                if (ph == 0x02)
                {
                    ES_phaseString = InterString.Get("准备阶段");
                    MD_phaseString = "Standby";
                    MD_battleString = "";
                    gameField.currentPhase = GameField.ph.sp;
                }

                if (ph == 0x04)
                {
                    ES_phaseString = InterString.Get("主要阶段1");
                    MD_phaseString = "Main1";
                    MD_battleString = "";
                    gameField.currentPhase = GameField.ph.mp1;
                }

                if (ph == 0x08)
                {
                    ES_phaseString = InterString.Get("战斗阶段");
                    MD_phaseString = "Battle";
                    MD_battleString = "";
                    gameField.currentPhase = GameField.ph.bp;
                }

                if (ph == 0x10)
                {
                    ES_phaseString = InterString.Get("战斗步骤");
                    MD_battleString = "01";
                    gameField.currentPhase = GameField.ph.bp;
                }

                if (ph == 0x20)
                {
                    ES_phaseString = InterString.Get("伤害步骤");
                    MD_battleString = "02";
                    gameField.currentPhase = GameField.ph.bp;
                }

                if (ph == 0x40)
                {
                    ES_phaseString = InterString.Get("伤害判定时");
                    MD_battleString = "03";
                    gameField.currentPhase = GameField.ph.bp;
                }

                if (ph == 0x80)
                {
                    ES_phaseString = InterString.Get("战斗阶段");
                    MD_battleString = "";
                    gameField.currentPhase = GameField.ph.bp;
                }

                if (ph == 0x100)
                {
                    ES_phaseString = InterString.Get("主要阶段2");
                    MD_phaseString = "Main2";
                    MD_battleString = "";
                    gameField.currentPhase = GameField.ph.mp2;
                }

                if (ph == 0x200)
                {
                    ES_phaseString = InterString.Get("结束阶段");
                    MD_phaseString = "End";
                    MD_battleString = "";
                    gameField.currentPhase = GameField.ph.ep;
                }

                //printDuelLog(InterString.Get("进入[?]", ES_turnString + ES_phaseString));
                ES_hint = ES_turnString + ES_phaseString;
                break;
            case GameMessage.ConfirmDecktop:
                player = localPlayer(r.ReadByte());
                count = r.ReadByte();
                var countOfDeck = countLocation(player, CardLocation.Deck);
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(new GPS
                    {
                        controller = (uint) player,
                        location = (uint) CardLocation.Deck,
                        sequence = (uint) (countOfDeck - 1 - i)
                    }, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        printDuelLog(InterString.Get("[ff0000]确认卡片：[?][-]", UIHelper.getGPSstringName(card, true)));
                        confirmedCards.Add("「" + UIHelper.getSuperName(card.get_data().Name, card.get_data().Id) + "」");
                        if (confirmedCards.Count >= 6) confirmedCards.RemoveAt(0);
                    }
                }
                break;
            case GameMessage.ConfirmCards:
                player = localPlayer(r.ReadByte());
                bool skip_panel = r.ReadByte() == 1;
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        printDuelLog(InterString.Get("[ff0000]确认卡片：[?][-]", UIHelper.getGPSstringName(card, true)));
                        confirmedCards.Add("「" + UIHelper.getSuperName(card.get_data().Name, card.get_data().Id) + "」");
                        if (confirmedCards.Count >= 6) confirmedCards.RemoveAt(0);
                    }
                }
                break;
            case GameMessage.DeckTop:
                player = localPlayer(r.ReadByte());
                var countOfDeck_ = countLocation(player, CardLocation.Deck);
                gps = new GPS
                {
                    controller = (uint) player,
                    location = (uint) CardLocation.Deck,
                    sequence = (uint) (countOfDeck_ - 1 - r.ReadByte())
                };
                code = r.ReadInt32();
                card = GCS_cardGet(gps, false);
                if (card != null)
                {
                    card.set_code(code);
                    printDuelLog(InterString.Get("确认卡片：[?]", UIHelper.getGPSstringName(card)));
                }

                break;
            case GameMessage.RefreshDeck:
            case GameMessage.ShuffleDeck:
                player = localPlayer(r.ReadByte());
                if (player == 0)
                {
                    //printDuelLog(InterString.Get("洗牌"));
                }

                for (var i = 0; i < cards.Count; i++)
                    if (cards[i].gameObject.activeInHierarchy)
                        if ((cards[i].p.location & (uint) CardLocation.Deck) > 0)
                            if (cards[i].p.controller == player)
                                cards[i].erase_data();
                break;
            case GameMessage.ShuffleHand:
                player = localPlayer(r.ReadByte());
                for (var i = 0; i < cards.Count; i++)
                    if (cards[i].gameObject.activeInHierarchy)
                        if ((cards[i].p.location & (uint) CardLocation.Hand) > 0)
                            if (cards[i].p.controller == player)
                                cards[i].erase_data();
                break;
            case GameMessage.SwapGraveDeck:
                player = localPlayer(r.ReadByte());
                for (var i = 0; i < cards.Count; i++)
                    if (cards[i].gameObject.activeInHierarchy)
                        if (cards[i].p.controller == player)
                        {
                            if ((cards[i].p.location & (uint) CardLocation.Deck) > 0)
                            {
                                if (cards[i].p.controller == player)
                                {
                                    cards[i].p.location = (uint)CardLocation.Grave;
                                }
                            }
                            else if ((cards[i].p.location & (uint) CardLocation.Grave) > 0)
                            {
                                if (cards[i].p.controller == player)
                                {
                                    if (cards[i].IsExtraCard())
                                    {
                                        cards[i].p.location = (uint)CardLocation.Extra;
                                    }

                                    else
                                    {
                                        cards[i].p.location = (uint)CardLocation.Deck;
                                    }                                        
                                }
                            }
                        }
                break;
            case GameMessage.ShuffleSetCard:
                location = r.ReadByte();
                count = r.ReadByte();
                var gpss = new List<GPS>();
                for (var i = 0; i < count; i++)
                {
                    gps = r.ReadGPS();
                    gpss.Add(gps);
                    card = GCS_cardGet(gps, false);
                    if (card != null) card.erase_data();
                }

                for (var i = 0; i < count; i++)
                {
                    gps = r.ReadGPS();
                    if (gps.location > 0) GCS_cardMove(gpss[i], gps);
                }

                break;
            case GameMessage.FieldDisabled:
                var selectable_field = r.ReadUInt32();
                var filter = 0x1;
                for (var i = 0; i < 5; ++i, filter <<= 1)
                {
                    gps = new GPS
                    {
                        controller = (uint) localPlayer(0),
                        location = (uint) CardLocation.MonsterZone,
                        sequence = (uint) i
                    };
                    if ((selectable_field & filter) > 0)
                        gameField.set_point_disabled(gps, true);
                    else
                        gameField.set_point_disabled(gps, false);
                }

                filter = 0x100;
                for (var i = 0; i < 8; ++i, filter <<= 1)
                {
                    gps = new GPS
                    {
                        controller = (uint) localPlayer(0),
                        location = (uint) CardLocation.SpellZone,
                        sequence = (uint) i
                    };
                    if ((selectable_field & filter) > 0)
                        gameField.set_point_disabled(gps, true);
                    else
                        gameField.set_point_disabled(gps, false);
                }

                filter = 0x10000;
                for (var i = 0; i < 5; ++i, filter <<= 1)
                {
                    gps = new GPS
                    {
                        controller = (uint) localPlayer(1),
                        location = (uint) CardLocation.MonsterZone,
                        sequence = (uint) i
                    };
                    if ((selectable_field & filter) > 0)
                        gameField.set_point_disabled(gps, true);
                    else
                        gameField.set_point_disabled(gps, false);
                }

                filter = 0x1000000;
                for (var i = 0; i < 8; ++i, filter <<= 1)
                {
                    gps = new GPS
                    {
                        controller = (uint) localPlayer(1),
                        location = (uint) CardLocation.SpellZone,
                        sequence = (uint) i
                    };
                    if ((selectable_field & filter) > 0)
                        gameField.set_point_disabled(gps, true);
                    else
                        gameField.set_point_disabled(gps, false);
                }
                break;
            case GameMessage.CardTarget:
            case GameMessage.Equip:
                from = r.ReadGPS();
                to = r.ReadGPS();
                var card_from = GCS_cardGet(from, false);
                var card_to = GCS_cardGet(to, false);
                if (card_from != null)
                {
                    if ((int) GameMessage.Equip == p.Fuction) card_from.target.Clear();
                    card_from.addTarget(card_to);
                }

                break;
            case GameMessage.CancelTarget:
            case GameMessage.Unequip:
                from = r.ReadGPS();
                card = GCS_cardGet(from, false);
                card.target.Clear();
                break;
            case GameMessage.AddCounter:
                type = r.ReadUInt16();
                gps = r.ReadShortGPS();
                card = GCS_cardGet(gps, false);
                count = r.ReadUInt16();
                if (card != null)
                {
                    name = GameStringManager.get("counter", type);
                    for (var i = 0; i < count; i++) card.add_string_tail(name);
                }

                break;
            case GameMessage.RemoveCounter:
                type = r.ReadUInt16();
                gps = r.ReadShortGPS();
                card = GCS_cardGet(gps, false);
                count = r.ReadUInt16();
                if (card != null)
                {
                    name = GameStringManager.get("counter", type);
                    for (var i = 0; i < count; i++) card.del_one_tail(name);
                }

                break;
        }

        r.BaseStream.Seek(0, 0);
    }

    private int unSwapPlayer(int player)
    {
        if (gameInfo.swaped)
            return 1 - player;
        return player;
    }

    public Package getNamePacket()
    {
        var p__ = new Package();
        p__.Fuction = (int) GameMessage.sibyl_name;
        p__.Data = new BinaryMaster();
        p__.Data.writer.WriteUnicode(name_0, 50);
        p__.Data.writer.WriteUnicode(name_0_tag, 50);
        p__.Data.writer.WriteUnicode(name_0_c != "" ? name_0_c : name_0, 50);
        p__.Data.writer.WriteUnicode(name_1, 50);
        p__.Data.writer.WriteUnicode(name_1_tag, 50);
        p__.Data.writer.WriteUnicode(name_1_c != "" ? name_1_c : name_1, 50);
        p__.Data.writer.Write(Program.I().ocgcore.MasterRule);
        return p__;
    }

    private static void printDuelLog(string toPrint)
    {
        Program.I().book.add(toPrint);
    }

    private int countLocation(int player, CardLocation location_)
    {
        var re = 0;

        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if ((cards[i].p.location & (uint) location_) > 0)
                    if (cards[i].p.controller == player)
                        re++;

        return re;
    }

    private int countLocationSequence(int player, CardLocation location_)
    {
        var re = 0;

        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if ((cards[i].p.location & (uint) location_) > 0)
                    if (cards[i].p.controller == player)
                        if (cards[i].p.sequence > re)
                            re = (int) cards[i].p.sequence;

        return re;
    }

    public bool inIgnoranceReplay()
    {
        return InAI == false && condition != Condition.duel;
    }

    public void reSize()
    {
        realize(true);
    }

    private static void shiftArrowHandlerF()
    {
        if (Program.I().ocgcore.Arrow != null) Program.I().ocgcore.Arrow.gameObject.SetActive(false);
    }

    private static void shiftArrowHandlerT()
    {
        if (Program.I().ocgcore.Arrow != null) Program.I().ocgcore.Arrow.gameObject.SetActive(true);
    }

    private void shiftArrow(Vector3 from, Vector3 to, bool on, int delay)
    {
        Program.notGo(shiftArrowHandlerT);
        Program.notGo(shiftArrowHandlerF);
        if (on)
            Program.go(delay, shiftArrowHandlerT);
        else
            Program.go(delay, shiftArrowHandlerF);
        if (on)
        {
            Arrow.from.position = from;
            Arrow.to.position = to;
        }
        else
        {
            Arrow.from.position = new Vector3(25, 0, 0);
            Arrow.to.position = new Vector3(25, 0, 5);
        }

        var collection = Arrow.GetComponentsInChildren<Transform>(true);
        foreach (var item in collection) item.gameObject.layer = on ? 0 : 4;
    }

    private void showCaculator()
    {
        if (winCaculator == null)
        {
            if (condition == Condition.watch)
                if (paused == false)
                    EventDelegate.Execute(UIHelper.getByName<UIButton>(toolBar, "stop_").onClick);
            RMSshow_clear();
            var real = (Program.fieldSize - 1) * 0.9f + 1f;
            var point = Program.I().main_camera.WorldToScreenPoint(new Vector3(0, 0, -5.65f * real));
            point.z = 2;
            if (Program.I().setting.setting.Vwin.value)//mark 胜利特效
            {
                UIHelper.playSound("explode", 0.5f);
                var explode =
                    create(result == duelResult.win ? Program.I().mod_winExplode : Program.I().mod_loseExplode);
                var co = explode.AddComponent<animation_screen_lock>();
                co.screen_point = point;
                explode.transform.position = Camera.main.ScreenToWorldPoint(point);
            }

            if (condition == Condition.record)
            {
                winCaculator = create
                (
                    Program.I().New_winCaculatorRecord,
                    Program.I().camera_main_2d.ScreenToWorldPoint(point),
                    new Vector3(0, 0, 0),
                    true,
                    Program.I().ui_main_2d
                ).GetComponent<lazyWin>();
            }
            else
            {
                winCaculator = create
                (
                    Program.I().New_winCaculator,
                    Program.I().camera_main_2d.ScreenToWorldPoint(point),
                    new Vector3(0, 0, 0),
                    true,
                    Program.I().ui_main_2d
                ).GetComponent<lazyWin>();
                UIHelper.InterGameObject(winCaculator.gameObject);
                winCaculator.input.value = UIHelper.getTimeString();
                UIHelper.registEvent(winCaculator.gameObject, "yes_", onSaveReplay);
                UIHelper.registEvent(winCaculator.gameObject, "no_", onGiveUpReplay);
            }

            switch (result)
            {
                case duelResult.disLink:
                    winCaculator.win.text = "Disconnected";
                    break;
                case duelResult.win:
                    winCaculator.win.text = "You Win";
                    break;
                case duelResult.lose:
                    winCaculator.win.text = "You Lose";
                    break;
                case duelResult.draw:
                    winCaculator.win.text = "Draw Game";
                    break;
                default:
                    winCaculator.win.text = "Disconnected";
                    break;
            }
        }
        else
        {
            switch (result)
            {
                case duelResult.win:
                    winCaculator.win.text = "You Win";
                    break;
                case duelResult.lose:
                    winCaculator.win.text = "You Lose";
                    break;
                case duelResult.draw:
                    winCaculator.win.text = "Draw Game";
                    break;
            }
        }
        winCaculator.reason.text = winReason;
    }

    private void onSaveReplay()
    {
        if (winCaculator != null)
            try
            {
                if (File.Exists("replay/" + TcpHelper.lastRecordName + ".yrp3d"))
                {
                    if (TcpHelper.lastRecordName != winCaculator.input.value)
                        if (File.Exists("replay/" + winCaculator.input.value + ".yrp3d"))
                            File.Delete("replay/" + winCaculator.input.value + ".yrp3d");
                    File.Move("replay/" + TcpHelper.lastRecordName + ".yrp3d",
                        "replay/" + winCaculator.input.value + ".yrp3d");
                }

                TcpHelper.lastRecordName = "";
            }
            catch (Exception e)
            {
                RMSshow_none(e.ToString());
            }

        onDuelResultConfirmed();
    }

    private void onGiveUpReplay()
    {
        if (winCaculator != null)
            try
            {
                if (File.Exists("replay/" + TcpHelper.lastRecordName + ".yrp3d"))
                {
                    if (File.Exists("replay/" + "-lastReplay" + ".yrp3d"))
                        File.Delete("replay/" + "-lastReplay" + ".yrp3d");
                    File.Move("replay/" + TcpHelper.lastRecordName + ".yrp3d", "replay/-lastReplay.yrp3d");
                }
            }
            catch (Exception e)
            {
                RMSshow_none(e.ToString());
            }

        onDuelResultConfirmed();
    }

    private void hideCaculator()
    {
        if (winCaculator != null)
        {
            if (condition == Condition.watch)
                if (paused)
                    EventDelegate.Execute(UIHelper.getByName<UIButton>(toolBar, "go_").onClick);
            destroy(winCaculator.gameObject);
        }
    }

    private void practicalizeMessage(Package p)
    {
        var player = 0;
        var count = 0;
        var code = 0;
        var min = 0;
        var max = 0;
        var cancalable = false;
        GPS gps;
        gameCard card;
        var r = p.Data.reader;
        r.BaseStream.Seek(0, 0);
        gameButton btn;
        var desc = "";
        uint available;
        BinaryMaster binaryMaster;
        Vector3 VectorAttackCard;
        Vector3 VectorAttackTarget;
        char type;
        int data;
        int val;
        int cctype;
        GameObject tempobj;
        var psum = false;
        var pIN = false;
        BinaryMaster bin;
        var length_of_message = r.BaseStream.Length;
        List<messageSystemValue> values;
        int turn = turns % 2;
        if (isFirst)
        {
            if (turn == 0) myTurn = false;
            else myTurn = true;
        }
        else
        {
            if (turn == 0) myTurn = true;
            else myTurn = false;
        }
        if (turns == 0) myTurn = true;
        switch ((GameMessage) p.Fuction)
        {
            //case GameMessage.sibyl_clear:
            //    clearResponse();
            //    break;
            case GameMessage.sibyl_quit:
                ////Debug.Log("GameMessage.sibyl_quit");
                Program.I().room.duelEnded = true;
                result = duelResult.disLink;
                showCaculator();
                break;
            case GameMessage.Retry:
                ////Debug.Log("GameMessage.Retry");
                Program.PrintToChat(InterString.Get("[ff5555]游戏崩了！！！退出决斗吧。[-]"));
                Debug.Log("Retry");
                break;
            //case GameMessage.sibyl_delay:
            //    if (inIgnoranceReplay())
            //    {
            //        break;
            //    }
            //    player = localPlayer(r.ReadChar());
            //    gameInfo.setTime(player, Program.I().room.time_limit);
            //    break;
            case GameMessage.sibyl_chat:
                ////Debug.Log("GameMessage.sibyl_chat");
                var sss = r.ReadALLUnicode();
                RMSshow_none(sss);
                break;
            case GameMessage.ShowHint:
                ////Debug.Log("GameMessage.ShowHint");
                int length = r.ReadUInt16();
                var buffer = r.ReadToEnd();
                var n = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                RMSshow_none(n);
                break;
            case GameMessage.sibyl_name:
                ////Debug.Log("GameMessage.sibyl_name");
                gameInfo.realize();
                if (MasterRule >= 4)
                    gameField.loadNewField();
                else
                    gameField.loadOldField();
                break;
            case GameMessage.Hint:
                ////Debug.Log("GameMessage.Hint");
                type = r.ReadChar();
                player = r.ReadChar();
                data = r.ReadInt32();
                if (type == 1) ES_hint = GameStringManager.get(data);
                if (type == 2) RMSshow_none(GameStringManager.get(data));
                if (type == 3) ES_selectHint = GameStringManager.get(data);
                if (type == 4) RMSshow_none(InterString.Get("效果选择：[?]", GameStringManager.get(data)));
                if (type == 5) RMSshow_none(GameStringManager.get(data));
                if (type == 6) RMSshow_none(InterString.Get("种族选择：[?]", GameStringHelper.race(data)));
                if (type == 7) RMSshow_none(InterString.Get("属性选择：[?]", GameStringHelper.attribute(data)));
                if (type == 8) animation_show_card_code(data);
                if (type == 9) RMSshow_none(InterString.Get("数字选择：[?]", data.ToString()));
                if (type == 10) animation_show_card_code(data);
                if (type == 11)
                {
                    if (localPlayer(player) == 1)
                        data = (data >> 16) | (data << 16);
                    RMSshow_none(InterString.Get("区域选择：[?]", GameStringHelper.zone(data)));
                }
                break;
            case GameMessage.MissedEffect:
                ////Debug.Log("GameMessage.MissedEffect");
                break;
            case GameMessage.Waiting:
                ////Debug.Log("GameMessage.Waiting");
                if (inIgnoranceReplay() || inTheWorld()) break;
                showWait();
                break;
            case GameMessage.Start:
                ////Debug.Log("GameMessage.Start");
                if (MasterRule >= 4)
                    gameField.loadNewField();
                else
                    gameField.loadOldField();
                realize(true);
                if (condition != Condition.record)
                {
                    if (isObserver)
                    {
                        if (condition != Condition.watch) shiftCondition(Condition.watch);
                        Program.I().setting.RefreshAppearance();
                    }
                    else
                    {
                        if (condition != Condition.duel) shiftCondition(Condition.duel);
                        LoadAssets.CreateTimer();
                        LoadAssets.CreateGuide();
                    }
                }
                else
                {
                    if (condition != Condition.record) shiftCondition(Condition.record);
                    Program.I().setting.RefreshAppearance();
                }
                card = GCS_cardGet(new GPS
                {
                    controller = 0,
                    location = (uint) CardLocation.Deck,
                    position = (int) CardPosition.FaceDownAttack,
                    sequence = 0
                }, false);
                if (card != null)
                    Program.I().cardDescription.setData(card.get_data(),
                        card.p.controller == 0 ? GameTextureManager.myBack : GameTextureManager.opBack,
                        card.tails.managedString);
                clearChainEnd();
                hideCaculator();

                GameTextureManager.RefreshBack();
                gameInfo.SetFace();
                break;
            case GameMessage.ReloadField:
                ////Debug.Log("GameMessage.ReloadField");
                if (MasterRule >= 4)
                    gameField.loadNewField();
                else
                    gameField.loadOldField();
                realize(true);
                if (condition != Condition.record)
                {
                    if (isObserver)
                    {
                        if (condition != Condition.watch) shiftCondition(Condition.watch);
                    }
                    else
                    {
                        if (condition != Condition.duel) shiftCondition(Condition.duel);
                    }
                }
                else
                {
                    if (condition != Condition.record) shiftCondition(Condition.record);
                }

                card = GCS_cardGet(new GPS
                {
                    controller = 0,
                    location = (uint) CardLocation.Hand,
                    position = (int) CardPosition.FaceDownAttack,
                    sequence = 0
                }, false);
                if (card != null)
                    Program.I().cardDescription.setData(card.get_data(),
                        card.p.controller == 0 ? GameTextureManager.myBack : GameTextureManager.opBack,
                        card.tails.managedString);
                clearChainEnd();
                hideCaculator();
                break;
            case GameMessage.Win:
                ////Debug.Log("GameMessage.Win");
                player = localPlayer(r.ReadByte());
                int winType = r.ReadByte();
                showCaculator();
                Sleep(120);
                if (player == 2)
                {
                    RMSshow_none(InterString.Get("游戏平局！"));
                }
                else if (player == 0 || winType == 4)
                {
                    if (cookie_matchKill > 0)
                        RMSshow_none(InterString.Get("比赛胜利，卡片：[?]", winReason));
                    else
                        RMSshow_none(InterString.Get("游戏胜利，原因：[?]", winReason));
                }
                else
                {
                    if (cookie_matchKill > 0)
                        RMSshow_none(InterString.Get("比赛败北，卡片：[?]", winReason));
                    else
                        RMSshow_none(InterString.Get("游戏败北，原因：[?]", winReason));
                }

                break;
            case GameMessage.RequestDeck:
                ////Debug.Log("GameMessage.RequestDeck");
                break;
            case GameMessage.SelectBattleCmd:
                ////Debug.Log("GameMessage.SelectBattleCmd");
                for (int i = 0; i < cards.Count; i++)
                    cards[i].alreadyYellow = false;

                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(20);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                toDefaultHint();
                player = localPlayer(r.ReadChar());
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    desc = GameStringManager.get(r.ReadInt32());
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        card.ES_exit_clicked();
                        var eff = new Effect();
                        eff.ptr = (i << 16) + 0;
                        eff.desc = desc;
                        card.effects.Add(eff);
                        if (card.query_hint_button(InterString.Get("发动效果@ui")) == false)
                        {
                            btn = new gameButton((i << 16) + 0, InterString.Get("发动效果@ui"), superButtonType.act);
                            btn.cookieCard = card;
                            card.add_one_button(btn);
                            card.Highlight(true, true, false);
                            card.alreadyYellow = true;
                            if (card.condition != gameCardCondition.verticle_clickable)
                            {
                                //card.add_one_decoration(Program.I().mod_ocgcore_decoration_card_active, 2, Vector3.zero, "active", true, true);
                                if (card.isHided()) card.currentFlash = gameCard.flashType.Active;
                            }
                        }
                    }
                }

                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    r.ReadByte();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        btn = new gameButton((i << 16) + 1, InterString.Get("攻击宣言@ui"), superButtonType.attack);
                        card.add_one_button(btn);
                        //card.add_one_decoration(Program.I().mod_ocgcore_bs_atk_decoration, 3f, Vector3.zero, "atk");
                        if (card.alreadyYellow)
                            card.Highlight(true, true, false);
                        else
                            card.Highlight(true, false, false);
                    }
                }

                var mp = r.ReadByte();
                var ep = r.ReadByte();
                if (mp == 1)
                {
                    gameInfo.addHashedButton("", 2, superButtonType.mp, InterString.Get("主要阶段@ui"));
                    gameField.retOfMp = 2;
                    gameField.Phase.colliderMp2.enabled = true;
                    PhaseUIBehaviour.clickable = true;
                    PhaseUIBehaviour.retOfMp = 2;
                }

                if (ep == 1)
                {
                    gameInfo.addHashedButton("", 3, superButtonType.ep, InterString.Get("结束回合@ui"));
                    gameField.retOfEp = 3;
                    gameField.Phase.colliderEp.enabled = true;

                    PhaseUIBehaviour.retOfEp = 3;
                }

                realize();

                foreach (var hiddenButton in gameField.gameHiddenButtons)
                    hiddenButton.CheckNotification();

                break;
            case GameMessage.SelectIdleCmd:
                //Debug.Log("GameMessage.SelectIdleCmd");
                for (int i = 0; i < cards.Count; i++)
                    cards[i].alreadyYellow = false;

                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(20);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                toDefaultHint();
                player = localPlayer(r.ReadChar());
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.ES_exit_clicked();
                        btn = new gameButton((i << 16) + 0, InterString.Get("通常召唤@ui"), superButtonType.summon);
                        card.add_one_button(btn);
                        card.Highlight(true, false, false);
                    }
                }

                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        if (card.query_hint_button(InterString.Get("特殊召唤@ui")) == false)
                        {
                            btn = new gameButton((i << 16) + 1, InterString.Get("特殊召唤@ui"), superButtonType.spsummon);
                            if ((card.p.location & (uint)CardLocation.SpellZone) >0 && (card.get_data().Type & (uint)CardType.Pendulum) >0)
                                btn = new gameButton((i << 16) + 1, InterString.Get("灵摆召唤@ui"), superButtonType.pendulumSummon);
                            card.add_one_button(btn);

                            card.Highlight(true, true, false);
                            card.alreadyYellow = true;
                            if (card.condition != gameCardCondition.verticle_clickable)
                            {
                                //card.add_one_decoration(Program.I().mod_ocgcore_decoration_spsummon, 2, Vector3.zero, "chain_selecting", true, true);
                                if (card.isHided())
                                    card.currentFlash = gameCard.flashType.SpSummon;
                            }
                        }
                    }
                }

                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        btn = new gameButton((i << 16) + 2, InterString.Get("变为\n攻击表示@ui"), superButtonType.change_atk);
                        if((card.p.position & (uint)CardPosition.Attack) > 0)
                            btn = new gameButton((i << 16) + 2, InterString.Get("变为\n守备表示@ui"), superButtonType.change_def);
                        card.add_one_button(btn);
                        if(card.alreadyYellow)
                            card.Highlight(true, true, false);
                        else
                            card.Highlight(true, false, false);
                    }
                }

                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        btn = new gameButton((i << 16) + 3, InterString.Get("前场放置@ui"), superButtonType.set_monster);
                        card.add_one_button(btn);
                        if (card.alreadyYellow)
                            card.Highlight(true, true, false);
                        else
                            card.Highlight(true, false, false);
                    }
                }

                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        btn = new gameButton((i << 16) + 4, InterString.Get("后场放置@ui"), superButtonType.set_spell_trap);
                        card.add_one_button(btn);
                        if (card.alreadyYellow)
                            card.Highlight(true, true, false);
                        else
                            card.Highlight(true, false, false);
                    }
                }

                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    var descP = r.ReadInt32();
                    desc = GameStringManager.get(descP);
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        if (descP == 1160)
                        {
                            btn = new gameButton((i << 16) + 5, InterString.Get("灵摆发动@ui"), superButtonType.set_pendulum);
                            card.add_one_button(btn);
                            if (card.condition != gameCardCondition.verticle_clickable)
                            {
                                //card.add_one_decoration(Program.I().mod_ocgcore_decoration_card_active, 2, Vector3.zero, "active", true, true);
                                if (card.isHided()) card.currentFlash = gameCard.flashType.Active;
                                card.Highlight(true, true, false);
                                card.alreadyYellow = true;
                            }
                        }
                        else
                        {
                            var eff = new Effect();
                            eff.ptr = (i << 16) + 5;
                            eff.desc = desc;
                            card.effects.Add(eff);
                            if (card.query_hint_button(InterString.Get("发动效果@ui")) == false)
                            {
                                btn = new gameButton((i << 16) + 5, InterString.Get("发动效果@ui"), superButtonType.act);
                                btn.cookieCard = card;
                                card.add_one_button(btn);
                                if (true)
                                {
                                    //card.add_one_decoration(Program.I().mod_ocgcore_decoration_card_active, 2, Vector3.zero, "active", true, true);
                                    if (card.isHided()) card.currentFlash = gameCard.flashType.Active;
                                    card.Highlight(true, true, false);
                                    card.alreadyYellow= true;
                                }
                            }
                        }
                    }
                }

                var bp = r.ReadByte();
                var ep2 = r.ReadByte();
                var shuffle = r.ReadByte();
                if (bp == 1)
                {
                    gameInfo.addHashedButton("", 6, superButtonType.bp, InterString.Get("战斗阶段@ui"));
                    gameField.retOfbp = 6;
                    gameField.Phase.colliderBp.enabled = true;
                    PhaseUIBehaviour.clickable = true;
                    PhaseUIBehaviour.retOfBp = 6;
                }

                if (ep2 == 1)
                {
                    gameInfo.addHashedButton("", 7, superButtonType.ep, InterString.Get("结束回合@ui"));
                    gameField.retOfEp = 7;
                    gameField.Phase.colliderEp.enabled = true;
                    PhaseUIBehaviour.clickable = true;
                    PhaseUIBehaviour.retOfEp = 7;
                }

                //if (shuffle == 1) gameInfo.addHashedButton("", 8, superButtonType.change, InterString.Get("洗切手牌@ui"));
                realize();

                foreach (var hiddenButton in gameField.gameHiddenButtons)
                    hiddenButton.CheckNotification();

                break;
            case GameMessage.SelectEffectYn:
                //Debug.Log("GameMessage.SelectEffectYn");
                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(20);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                player = localPlayer(r.ReadByte());
                code = r.ReadInt32();
                gps = r.ReadShortGPS();
                r.ReadByte();
                var cr = r.ReadInt32();
                card = GCS_cardGet(gps, false);
                if (card != null)
                {
                    var displayname = "「" + card.get_data().Name + "」";
                    if (cr == 0)
                    {
                        desc = GameStringManager.get(200);
                        var forReplaceFirst = new Regex("\\[%ls\\]");
                        desc = forReplaceFirst.Replace(desc, GameStringManager.formatLocation(gps), 1);
                        desc = forReplaceFirst.Replace(desc, displayname, 1);
                    }
                    else if (cr == 221)
                    {
                        desc = GameStringManager.get(221);
                        var forReplaceFirst = new Regex("\\[%ls\\]");
                        desc = forReplaceFirst.Replace(desc, GameStringManager.formatLocation(gps), 1);
                        desc = forReplaceFirst.Replace(desc, displayname, 1);
                        desc = desc + "\n" + GameStringManager.get(223);
                    }
                    else
                    {
                        desc = GameStringManager.get(cr);
                        var forReplaceFirst = new Regex("\\[%ls\\]");
                        desc = forReplaceFirst.Replace(desc, displayname, 1);
                    }

                    var hin = ES_hint + "，\n" + desc;
                    //RMSshow_yesOrNo("return", hin, new messageSystemValue {value = "1", hint = "yes"},
                    //    new messageSystemValue {value = "0", hint = "no"});
                    List<gameCard> oneCardToSend = new List<gameCard>() { card };
                    Program.I().cardSelection.show(oneCardToSend, true, 1, 1, hin);
                    //if((card.p.location & (uint)CardLocation.Onfield) >0)
                    //    card.add_one_decoration(Program.I().mod_ocgcore_decoration_chain_selecting, 4, Vector3.zero, "chain_selecting");
                    card.currentFlash = gameCard.flashType.Active;
                }
                break;
            case GameMessage.SelectYesNo:
                //Debug.Log("GameMessage.SelectYesNo");
                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(20);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                player = localPlayer(r.ReadByte());
                desc = GameStringManager.get(r.ReadInt32());
                RMSshow_yesOrNo("return", desc, new messageSystemValue {value = "1", hint = "yes"},
                    new messageSystemValue {value = "0", hint = "no"});
                break;
            case GameMessage.SelectOption:
                //Debug.Log("GameMessage.SelectOption");
                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(60);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                player = localPlayer(r.ReadByte());
                count = r.ReadByte();
                if (count > 1)
                {
                    values = new List<messageSystemValue>();
                    for (var i = 0; i < count; i++)
                    {
                        desc = GameStringManager.get(r.ReadInt32());
                        values.Add(new messageSystemValue {hint = desc, value = i.ToString()});
                    }

                    RMSshow_singleChoice("return", values);
                }
                else
                {
                    binaryMaster = new BinaryMaster();
                    binaryMaster.writer.Write(0);
                    sendReturn(binaryMaster.get());
                }

                break;
            case GameMessage.SelectTribute:
                //Debug.Log("GameMessage.SelectTribute");
                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(60);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                player = localPlayer(r.ReadByte());
                cancalable = r.ReadByte() != 0;
                ES_min = r.ReadByte();
                ES_max = r.ReadByte();
                ES_level = 0;
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        card.forSelect = true;
                        card.selectPtr = i;
                        int para = r.ReadByte();
                        card.levelForSelect_1 = para;
                        card.levelForSelect_2 = para;
                        allCardsInSelectMessage.Add(card);
                    }
                }

                if (cancalable)
                {
                    gameInfo.addHashedButton("cancleSelected", -1, superButtonType.no, InterString.Get("取消选择@ui"));
                    Program.I().ocgcore.gameField.ShowCancelButton("取消选择");
                }
                if (ES_selectHint != "")
                    gameField.setHint(ES_selectHint + " " + ES_min + "-" + ES_max);
                else
                    gameField.setHint(InterString.Get("请选择卡片。") + " " + ES_min + "-" + ES_max);
                realizeCardsForSelect(InterString.Get("请选择卡片。") + " " + ES_min + "-" + ES_max);
                break;
            case GameMessage.SelectCard:
                //Debug.Log("GameMessage.SelectCard");
                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(60);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                player = localPlayer(r.ReadByte());
                cancalable = r.ReadByte() != 0;
                ES_min = r.ReadByte();
                ES_max = r.ReadByte();
                ES_level = 0;
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        card.forSelect = true;
                        card.selectPtr = i;
                        allCardsInSelectMessage.Add(card);
                    }
                }

                if (cancalable)
                {
                    gameInfo.addHashedButton("cancleSelected", -1, superButtonType.no, InterString.Get("取消选择@ui"));
                    Program.I().ocgcore.gameField.ShowCancelButton("取消选择");
                }
                string hint = "";
                if (ES_selectHint != "")
                {
                    bool need = true;
                    if (ES_selectHint.StartsWith("请选择攻击的对象"))
                        need = false;
                    if (ES_selectHint.StartsWith("请选择要加入手卡的卡"))
                        need = false;
                    if (ES_selectHint.StartsWith("请选择要特殊召唤的卡"))
                        need = false;
                    if (ES_selectHint.StartsWith("请选择要取除的超量素材"))
                        need = false;
                    if (need)
                        gameField.setHint(ES_selectHint + " " + ES_min + "-" + ES_max);
                    hint = ES_selectHint + " " + ES_min + "-" + ES_max;
                }
                else
                {
                    gameField.setHint(InterString.Get("请选择卡片。") + " " + ES_min + "-" + ES_max);
                    hint = InterString.Get("请选择卡片。") + " " + ES_min + "-" + ES_max;
                }
                realizeCardsForSelect(hint);
                break;
            case GameMessage.SelectUnselect:
                //Debug.Log("GameMessage.SelectUnselect");
                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(60);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                player = localPlayer(r.ReadByte());
                var finishable = r.ReadByte() != 0;
                cancalable = r.ReadByte() != 0 || finishable;
                ES_min = r.ReadByte();
                ES_max = r.ReadByte();
                ES_level = 0;
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        card.forSelect = true;
                        card.selectPtr = i;
                        allCardsInSelectMessage.Add(card);
                    }
                }

                cardsSelected.Clear();
                int count2 = r.ReadByte();
                for (var i = count; i < count + count2; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        card.forSelect = true;
                        card.selectPtr = i;
                        allCardsInSelectMessage.Add(card);
                        cardsSelected.Add(card);
                    }
                }

                if (cancalable && !finishable)
                {
                    gameInfo.addHashedButton("cancleSelected", -1, superButtonType.no, InterString.Get("取消选择@ui"));
                    Program.I().ocgcore.gameField.ShowCancelButton("取消选择");
                }
                    
                if (finishable)
                {
                    gameInfo.addHashedButton("sendSelected", 0, superButtonType.yes, InterString.Get("完成选择@ui"));
                    Program.I().ocgcore.gameField.ShowConfirmButton();
                    Program.I().cardSelection.finishable = true;
                }
                if (ES_selectHint != "")
                    ES_selectUnselectHint = ES_selectHint;
                if (ES_selectUnselectHint != "")
                {
                    bool need = true;
                    if (ES_selectHint.StartsWith("请选择要特殊召唤的卡"))
                        need = false;
                    if (need)
                        gameField.setHint(ES_selectUnselectHint + " " + ES_min + "-" + ES_max);
                    hint = ES_selectUnselectHint + " " + ES_min + "-" + ES_max;
                }
                else
                {
                    gameField.setHint(InterString.Get("请选择卡片。") + " " + ES_min + "-" + ES_max);
                    hint = InterString.Get("请选择卡片。") + " " + ES_min + "-" + ES_max;
                }
                realizeCardsForSelect(hint);
                cardsSelected.Clear();
                break;
            case GameMessage.SelectChain:
                //Debug.Log("GameMessage.SelectChain");
                if (inIgnoranceReplay() || inTheWorld()) break;
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                player = localPlayer(r.ReadChar());
                count = r.ReadByte();
                int spcount = r.ReadByte();
                var hint0 = r.ReadInt32();
                var hint1 = r.ReadInt32();
                chainCards = new List<gameCard>();
                int forceCount = 0;
                for (var i = 0; i < count; i++)
                {
                    int flag = r.ReadChar();
                    int forced = r.ReadByte();
                    forceCount += forced;
                    code = r.ReadInt32() % 1000000000;
                    gps = r.ReadGPS();
                    desc = GameStringManager.get(r.ReadInt32());
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        chainCards.Add(card);
                        card.set_code(code);
                        card.prefered = true;
                        var eff = new Effect();
                        eff.flag = flag;
                        eff.ptr = i;
                        eff.desc = desc;
                        eff.forced = forced > 0;
                        card.effects.Add(eff);
                    }
                }

                var chain_condition = gameInfo.get_condition();
                var handle_flag = 0;

                //无强制发动的卡
                if (forceCount == 0)
                {
                    if (spcount == 0)
                    {
                        //无关键卡
                        if (chain_condition == gameInfo.chainCondition.no)
                        {
                            //无关键卡 连锁被无视 直接回答---
                            handle_flag = 0;
                        }
                        else if (chain_condition == gameInfo.chainCondition.all)
                        {
                            //无关键卡但是连锁被监控
                            if (chainCards.Count == 0)
                            {
                                //欺骗--
                                handle_flag = -1;
                            }
                            else
                            {
                                if (chainCards.Count == 1 && chainCards[0].effects.Count == 1)
                                    //只有一张要处理的卡 常规处理 一张---
                                    handle_flag = 1;
                                else
                                    //常规处理 多张---
                                    handle_flag = 2;
                            }
                        }
                        else if (chain_condition == gameInfo.chainCondition.smart)
                        {
                            //无关键卡但是连锁被智能过滤
                            if (chainCards.Count == 0)
                            {
                                //根本没卡 直接回答---
                                handle_flag = 0;
                            }
                            else
                            {
                                if (chainCards.Count == 1 && chainCards[0].effects.Count == 1)
                                    //只有一张要处理的卡 常规处理 一张---
                                    handle_flag = 1;
                                else
                                    //常规处理 多张---
                                    handle_flag = 2;
                            }
                        }
                        else
                        {
                            //无关键卡而且连锁没有被监控    直接回答---
                            handle_flag = 0;
                        }
                    }
                    else
                    {
                        //有关键卡
                        if (chainCards.Count == 0)
                        {
                            //根本没卡 直接回答---
                            handle_flag = 0;
                            if (chain_condition == gameInfo.chainCondition.all)
                                //欺骗--
                                handle_flag = -1;
                        }
                        else if (chain_condition == gameInfo.chainCondition.no)
                        {
                            //有关键卡 连锁被无视 直接回答---
                            handle_flag = 0;
                        }
                        else
                        {
                            if (chainCards.Count == 1 && chainCards[0].effects.Count == 1)
                                //只有一张要处理的卡 常规处理 一张---
                                handle_flag = 1;
                            else
                                //常规处理 多张---
                                handle_flag = 2;
                        }
                    }
                }
                else
                {
                    if (chainCards.Count == 1 && chainCards[0].effects.Count == 1)
                    {
                        //有一张强制发动的卡 回应--
                        handle_flag = 4;
                    }
                    else
                    {
                        //有强制发动的卡 处理强制发动的卡--
                        handle_flag = 3;
                        if (autoForceChainHandler == autoForceChainHandlerType.autoHandleAll) handle_flag = 4;
                        if (autoForceChainHandler == autoForceChainHandlerType.afterClickManDo) handle_flag = 5;
                    }

                    if (UIHelper.fromStringToBool(Config.Get("autoChain_", "0")))
                        //自动回应--
                        handle_flag = 4;
                }

                if (handle_flag == -1)
                {
                    //欺骗
                    RMSshow_onlyYes("return", InterString.Get("[?]，@n没有卡片可以连锁。", ES_hint),
                        new messageSystemValue {hint = "yes", value = "-1"});
                    flagForCancleChain = true;
                    if (condition == Condition.record) Sleep(60);
                }

                if (handle_flag == 0)
                {
                    //直接回答
                    binaryMaster = new BinaryMaster();
                    binaryMaster.writer.Write(-1);
                    sendReturn(binaryMaster.get());
                }

                if (handle_flag == 1)
                    //处理一张   废除
                    handle_flag = 2;
                if (handle_flag == 2)
                {
                    //处理多张
                    for (var i = 0; i < chainCards.Count; i++)
                    {
                        //if ((chainCards[i].p.location & (uint)CardLocation.Hand) > 0 || (chainCards[i].p.location & (uint)CardLocation.Onfield) > 0)
                        //    chainCards[i].add_one_decoration(Program.I().mod_ocgcore_decoration_chain_selecting, 4, Vector3.zero, "chain_selecting");
                        chainCards[i].forSelect = true;
                        chainCards[i].currentFlash = gameCard.flashType.Active;
                    }

                    flagForCancleChain = true;

                    Program.I().cardSelection.show(chainCards, true, 1, 1, InterString.Get("[?]，@n是否连锁？", ES_hint));
                    //RMSshow_yesOrNo("return", InterString.Get("[?]，@n是否连锁？", ES_hint),
                    //    new messageSystemValue { value = "hide", hint = "yes" },
                    //    new messageSystemValue { value = "-1", hint = "no" });

                    gameInfo.addHashedButton("cancleChain", -1, superButtonType.no, InterString.Get("取消连锁@ui"));
                    //Program.I().ocgcore.gameField.ShowCancelButton("取消连锁");
                    if (condition == Condition.record) Sleep(60);
                }

                if (handle_flag == 3)
                {
                    //处理强制发动的卡
                    cardsWaitToSend.Clear();
                    for (var i = 0; i < chainCards.Count; i++)
                    {
                        //if ((chainCards[i].p.location & (uint)CardLocation.Onfield) > 0)
                        //    chainCards[i].add_one_decoration(Program.I().mod_ocgcore_decoration_chain_selecting, 4, Vector3.zero, "chain_selecting");
                        chainCards[i].forSelect = true;
                        chainCards[i].currentFlash = gameCard.flashType.Active;
                        cardsWaitToSend.Add(chainCards[i]);
                    }
                    
                    RMSshow_yesOrNo("autoForceChainHandler", InterString.Get("[?]，@n自动处理强制发动的卡？", ES_hint),
                        new messageSystemValue {value = "yes", hint = "yes"},
                        new messageSystemValue {value = "no", hint = "no"});
                    if (condition == Condition.record) Sleep(60);
                }

                if (handle_flag == 5)
                {
                    for (var i = 0; i < chainCards.Count; i++)
                    {
                        //if ((chainCards[i].p.location & (uint)CardLocation.Onfield) > 0)
                        //    chainCards[i].add_one_decoration(Program.I().mod_ocgcore_decoration_chain_selecting, 4, Vector3.zero, "chain_selecting");
                        chainCards[i].forSelect = true;
                        chainCards[i].currentFlash = gameCard.flashType.Active;
                    }
                }
                    //处理强制发动的卡 AfterClick


                if (handle_flag == 4)
                {
                    //有一张强制发动的卡 回应--
                    int answer = -1;
                    foreach (var ccard in chainCards)
                    {
                        foreach (var effect in ccard.effects)
                        {
                            if (effect.forced)
                            {
                                answer = effect.ptr;
                                break;
                            }
                        }
                        if (answer >= 0) break;
                    }
                    binaryMaster = new BinaryMaster();
                    binaryMaster.writer.Write(answer >= 0 ? answer : 0);
                    sendReturn(binaryMaster.get());
                }

                break;
            case GameMessage.SelectPosition:
                //Debug.Log("GameMessage.SelectPosition");
                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(60);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                player = localPlayer(r.ReadByte());
                code = r.ReadInt32();
                int positions = r.ReadByte();
                var op1 = 0x1;
                var op2 = 0x4;
                if (positions == 0x1 || positions == 0x2 || positions == 0x4 || positions == 0x8)
                {
                    binaryMaster = new BinaryMaster();
                    binaryMaster.writer.Write(positions);
                    sendReturn(binaryMaster.get());
                }
                if (positions == (0x1 | 0x4 | 0x8))
                {
                    RMSshow_position3("return", code);
                }
                else
                {
                    if ((positions & 0x1) > 0) op1 = 0x1;
                    if ((positions & 0x2) > 0) op1 = 0x2;
                    if ((positions & 0x4) > 0) op2 = 0x4;
                    if ((positions & 0x8) > 0)
                    {
                        if ((positions & 0x4) > 0) op1 = 0x4;
                        op2 = 0x8;
                    }

                    RMSshow_position("return", code, new messageSystemValue {value = op1.ToString(), hint = "atk"},
                        new messageSystemValue {value = op2.ToString(), hint = "def"});
                }
                break;
            case GameMessage.SortCard:
            case GameMessage.SortChain:
                //Debug.Log("GameMessage.SortChain");
                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(60);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                player = localPlayer(r.ReadByte());
                ES_sortSum = 0;
                count = r.ReadByte();
                cardsInSort.Clear();
                ES_sortResult.Clear();
                List <gameCard> cardsToSend = new List<gameCard>();
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        card.forSelect = true;
                        //card.isShowed = true;
                        cardsToSend.Add(card);
                        card.sortOptions.Add(i);
                        cardsInSort.Remove(card);
                        cardsInSort.Add(card);
                        ES_sortSum++;
                        //card.add_one_decoration(Program.I().mod_ocgcore_decoration_card_selecting, 4, Vector3.zero, "card_selecting");
                        card.currentFlash = gameCard.flashType.Select;
                    }
                }
                if(currentMessage == GameMessage.SortCard)
                    Program.I().cardSelection.show(cardsToSend, false, cardsToSend.Count, cardsToSend.Count, InterString.Get("请为卡片排序。"));
                else
                    Program.I().cardSelection.show(cardsToSend, false, cardsToSend.Count, cardsToSend.Count, InterString.Get("请为连锁手动排序。"));

                if (UIHelper.fromStringToBool(Config.Get("autoChain_", "0")))
                    if (currentMessage == GameMessage.SortChain)
                    {
                        bin = new BinaryMaster();
                        for (var i = 0; i < count; i++) bin.writer.Write((byte) i);
                        sendReturn(bin.get());
                    }

                realize();
                toNearest();
                //if (currentMessage == GameMessage.SortCard)
                //{
                //    gameField.setHint(InterString.Get("请为卡片排序。"));
                //    Program.I().new_ui_cardSelection.setTitle(InterString.Get("请为卡片排序。"));
                //}
                //else
                //{
                //    gameField.setHint(InterString.Get("请为连锁手动排序。"));
                //    Program.I().new_ui_cardSelection.setTitle(InterString.Get("请为连锁手动排序。"));
                //}
                break;
            case GameMessage.SelectCounter:
                //Debug.Log("GameMessage.SelectCounter");
                if (inIgnoranceReplay() || inTheWorld()) break;
                var Version1033b = (length_of_message - 5) % 8 == 0;
                if (condition == Condition.record) Sleep(60);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                player = localPlayer(r.ReadByte());
                r.ReadInt16();
                if (Version1033b)
                    ES_min = r.ReadByte();
                else
                    ES_min = r.ReadUInt16();
                count = r.ReadByte();

                GameObject decoration = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_select/fxp_hl_select_card_001");
                var main = decoration.GetComponent<ParticleSystem>().main;
                main.playOnAwake = true;
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    var pew = 0;
                    if (Version1033b)
                        pew = r.ReadByte();
                    else
                        pew = r.ReadUInt16();
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        card.counterCANcount = pew;
                        card.counterSELcount = 0;
                        allCardsInSelectMessage.Add(card);
                        card.selectPtr = i;
                        card.forSelect = true;
                        //card.add_one_decoration(Program.I().mod_ocgcore_decoration_card_selecting, 4, Vector3.zero, "card_selecting");
                        if ((card.p.position & (uint)CardPosition.Attack) > 0)
                            decoration.transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 0, 0);
                        else
                            decoration.transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 90, 0);
                        card.add_one_decoration(decoration, 4, Vector3.zero, "card_selecting");
                        card.isShowed = true;
                        card.currentFlash = gameCard.flashType.Select;
                    }
                }
                Object.Destroy(decoration);

                if (gameInfo.queryHashedButton("clearCounter") == false)
                {
                    gameInfo.addHashedButton("clearCounter", 0, superButtonType.no, InterString.Get("重新选择@ui"));
                    Program.I().ocgcore.gameField.ShowCancelButton("重新选择");
                }
                realize();
                toNearest();
                gameField.setHint(InterString.Get("请移除[?]个指示物。", ES_min.ToString()));
                break;
            case GameMessage.SelectSum:
                //Debug.Log("GameMessage.SelectSum");
                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(60);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                ES_overFlow = r.ReadByte() != 0;
                player = localPlayer(r.ReadByte());
                ES_level = r.ReadInt32();
                ES_min = r.ReadByte();
                ES_max = r.ReadByte();
                if (ES_min < 1) ES_min = 1;
                if (ES_max < 1) ES_max = 99;
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    var para = r.ReadInt32();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.selectPtr = i;
                        card.levelForSelect_1 = para & 0xffff;
                        card.levelForSelect_2 = para >> 16;
                        if ((para & 0x80000000) > 0)
                        {
                            card.levelForSelect_1 = para & 0x7fffffff;
                            card.levelForSelect_2 = card.levelForSelect_1;
                        }
                        if (card.levelForSelect_2 == 0) card.levelForSelect_2 = card.levelForSelect_1;
                        allCardsInSelectMessage.Add(card);
                        cardsMustBeSelected.Add(card);
                        card.forSelect = true;
                    }
                }

                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    var para = r.ReadInt32();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        card.set_code(code);
                        card.prefered = true;
                        card.selectPtr = i;
                        card.levelForSelect_1 = para & 0xffff;
                        card.levelForSelect_2 = para >> 16;
                        if ((para & 0x80000000) > 0)
                        {
                            card.levelForSelect_1 = para & 0x7fffffff;
                            card.levelForSelect_2 = card.levelForSelect_1;
                        }
                        if (card.levelForSelect_2 == 0) card.levelForSelect_2 = card.levelForSelect_1;
                        allCardsInSelectMessage.Add(card);
                        card.forSelect = true;
                    }
                }

                realizeCardsForSelect(ES_selectHint);
                gameField.setHint(ES_selectHint);
                break;
            case GameMessage.SelectPlace:
            case GameMessage.SelectDisfield:
                //Debug.Log("GameMessage.SelectDisfield");
                if (inIgnoranceReplay() || inTheWorld()) break;
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                binaryMaster = new BinaryMaster();
                player = r.ReadByte();
                min = r.ReadByte();
                bool cancelable = false;
                if (min == 0)
                {
                    cancelable = true;
                    min = 1;
                }
                var _field = ~r.ReadUInt32();
                if (Program.I().setting.setting.hand.value || Program.I().setting.setting.handm.value ||
                    currentMessage == GameMessage.SelectDisfield)
                {
                    ES_min = min;
                    for (var i = 0; i < min; i++)
                    {
                        var resp = new byte[3];
                        uint filter;

                        for (var j = 0; j < 2; j++)
                        {
                            resp = new byte[3];
                            filter = 0;
                            uint field;

                            if (j == 0)
                            {
                                resp[0] = (byte) player;
                                field = _field & 0xffff;
                            }
                            else
                            {
                                resp[0] = (byte) (1 - player);
                                field = _field >> 16;
                            }

                            if ((field & 0x7f) != 0)
                            {
                                resp[1] = (byte) CardLocation.MonsterZone;
                                filter = field & 0x7f;
                                for (var k = 0; k < 7; k++)
                                    if ((filter & (1u << k)) != 0)
                                    {
                                        resp[2] = (byte) k;
                                        createPlaceSelector(resp);
                                    }
                            }

                            if ((field & 0x1f00) != 0)
                            {
                                resp[1] = (byte) CardLocation.SpellZone;
                                filter = (field >> 8) & 0x1f;
                                for (var k = 0; k < 5; k++)
                                    if ((filter & (1u << k)) != 0)
                                    {
                                        resp[2] = (byte) k;
                                        createPlaceSelector(resp);
                                    }
                            }
                            if ((field & 0x2000) != 0)
                            {
                                resp[1] = (byte)CardLocation.SpellZone;
                                filter = (field >> 8) & 0x20;
                                resp[2] = 5;
                                createPlaceSelector(resp);
                            }
                            if ((field & 0xc000) != 0)
                            {
                                resp[1] = (byte) CardLocation.SpellZone;
                                filter = (field >> 14) & 0x3;
                                if ((filter & 0x2) != 0)
                                {
                                    resp[2] = 7;
                                    createPlaceSelector(resp);
                                }

                                if ((filter & 0x1) != 0)
                                {
                                    resp[2] = 6;
                                    createPlaceSelector(resp);
                                }
                            }
                        }
                    }

                    if (currentMessage == GameMessage.SelectPlace)
                    {
                        if (Es_selectMSGHintType == 3)
                        {
                            if (Es_selectMSGHintPlayer == 0)
                                gameField.setHint(InterString.Get("请为我方的「[?]」选择位置。",
                                    CardsManager.Get(Es_selectMSGHintData).Name));
                            else
                                gameField.setHint(InterString.Get("请为对方的「[?]」选择位置。",
                                    CardsManager.Get(Es_selectMSGHintData).Name));
                        }
                    }
                    else
                    {
                        if (ES_selectHint != "")
                        {
                            gameField.setHint(ES_selectHint);
                            Program.I().cardSelection.setTitle(ES_selectHint);
                        }
                        else
                        {
                            gameField.setHint(GameStringManager.get_unsafe(570));
                            Program.I().cardSelection.setTitle(GameStringManager.get_unsafe(570));
                        }
                    }
                    if (cancelable)
                    {
                        gameInfo.addHashedButton("cancelPlace", -1, superButtonType.no, InterString.Get("取消操作@ui"));
                        Program.I().ocgcore.gameField.ShowCancelButton("取消操作");
                    }
                }
                else
                {
                    var field = _field;
                    for (var i = 0; i < min; i++)
                    {
                        var resp = new byte[3];
                        var pendulumZone = false;
                        uint filter;

                        if ((field & 0x7f0000) != 0)
                        {
                            resp[0] = (byte) (1 - player);
                            resp[1] = (byte) CardLocation.MonsterZone;
                            filter = (field >> 16) & 0x7f;
                        }
                        else if ((field & 0x1f000000) != 0)
                        {
                            resp[0] = (byte) (1 - player);
                            resp[1] = (byte) CardLocation.SpellZone;
                            filter = (field >> 24) & 0x1f;
                        }
                        else if ((field & 0xc0000000) != 0)
                        {
                            resp[0] = (byte) (1 - player);
                            resp[1] = (byte) CardLocation.SpellZone;
                            filter = (field >> 30) & 0x3;
                            pendulumZone = true;
                        }
                        else if ((field & 0x7f) != 0)
                        {
                            resp[0] = (byte) player;
                            resp[1] = (byte) CardLocation.MonsterZone;
                            filter = field & 0x7f;
                        }
                        else if ((field & 0x1f00) != 0)
                        {
                            resp[0] = (byte) player;
                            resp[1] = (byte) CardLocation.SpellZone;
                            filter = (field >> 8) & 0x1f;
                        }
                        else if ((field & 0x2000) != 0)
                        {
                            resp[0] = (byte)player;
                            resp[1] = (byte)CardLocation.SpellZone;
                            filter = (field >> 8) & 0x20;
                        }
                        else
                        {
                            resp[0] = (byte) player;
                            resp[1] = (byte) CardLocation.SpellZone;
                            filter = (field >> 14) & 0x3;
                            pendulumZone = true;
                        }

                        if (!pendulumZone)
                        {
                            if ((filter & 0x4) != 0)
                            {
                                resp[2] = 2;
                            }
                            else if ((filter & 0x2) != 0)
                            {
                                resp[2] = 1;
                            }
                            else if ((filter & 0x8) != 0)
                            {
                                resp[2] = 3;
                            }
                            else if ((filter & 0x1) != 0)
                            {
                                resp[2] = 0;
                            }
                            else if ((filter & 0x10) != 0)
                            {
                                resp[2] = 4;
                            }
                            else
                            {
                                if (resp[1] == (byte) CardLocation.MonsterZone)
                                {
                                    if ((filter & 0x20) != 0) resp[2] = 5;
                                    else if ((filter & 0x40) != 0) resp[2] = 6;
                                }
                                else
                                {
                                    if ((filter & 0x20) != 0) resp[2] = 5;
                                }
                            }
                        }
                        else
                        {
                            if ((filter & 0x2) != 0) resp[2] = 7;
                            if ((filter & 0x1) != 0) resp[2] = 6;
                        }

                        binaryMaster.writer.Write(resp);
                    }

                    sendReturn(binaryMaster.get());
                }

                break;
            case GameMessage.RockPaperScissors:
                //Debug.Log("GameMessage.RockPaperScissors");
                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(60);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                player = localPlayer(r.ReadByte());
                RMSshow_tp("RockPaperScissors"
                    , new messageSystemValue {hint = "jiandao", value = "1"}
                    , new messageSystemValue {hint = "shitou", value = "2"}
                    , new messageSystemValue {hint = "bu", value = "3"});
                break;
            case GameMessage.ConfirmDecktop:
                //Debug.Log("GameMessage.ConfirmDecktop");
                player = localPlayer(r.ReadByte());
                count = r.ReadByte();
                var countOfDeck = countLocation(player, CardLocation.Deck);
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    gps = new GPS
                    {
                        controller = (uint) player,
                        location = (uint) CardLocation.Deck,
                        sequence = (uint) (countOfDeck - 1 - i)
                    };
                    card = GCS_cardGet(gps, false);
                    if (card != null) confirm(card);
                }

                Sleep(count * 40);
                break;
            case GameMessage.ConfirmCards:
                //Debug.Log("GameMessage.ConfirmCards");
                player = localPlayer(r.ReadByte());
                bool skip_panel = r.ReadByte() == 1;
                count = r.ReadByte();
                var t2 = 0;
                var t3 = 0;
                var pan_mode = false;
                for (var i = 0; i < count; i++)
                {
                    code = r.ReadInt32();
                    gps = r.ReadShortGPS();
                    card = GCS_cardGet(gps, false);
                    var showC = false;
                    if (gps.controller != 0)
                    {
                        showC = true;
                    }
                    else
                    {
                        if (gps.location != (int) CardLocation.Hand) showC = true;
                        if (Program.I().room.mode == 2) showC = true;
                        if (condition != Condition.duel)
                            if (InAI == false)
                                showC = true;
                    }

                    if (showC)
                        if (card != null)
                        {
                            if (
                                (card.p.location & (uint) CardLocation.Deck) > 0
                                ||
                                (card.p.location & (uint) CardLocation.Grave) > 0
                                ||
                                (card.p.location & (uint) CardLocation.Extra) > 0
                                ||
                                (card.p.location & (uint) CardLocation.Removed) > 0
                            )
                            {
                                card.currentKuang = gameCard.kuangType.selected;
                                cardsInSelectAnimation.Add(card);
                                card.isShowed = true;
                                pan_mode = true;
                                if (condition != Condition.record)
                                {
                                    t2 += 100000;
                                    clearTimeFlag = true;
                                }

                                t3++;
                            }
                            else if (card.condition != gameCardCondition.verticle_clickable)
                            {
                                if ((card.p.location & (uint) CardLocation.Hand) > 0)
                                {
                                    if (i == 0)
                                    {
                                        confirm(card);
                                        t2 += 50;
                                    }
                                    else
                                    {
                                        Nconfirm();
                                        t2 = 50;
                                    }
                                }
                                else
                                {
                                    confirm(card);
                                    t2 += 50;
                                }
                            }
                            else
                            {
                                card.currentKuang = gameCard.kuangType.selected;
                                cardsInSelectAnimation.Add(card);
                            }
                        }
                }

                realize();
                toNearest();
                if (pan_mode)
                {
                    clearAllShowedB = true;
                    flagForTimeConfirm = true;
                    gameField.setHint(InterString.Get("请确认[?]张卡片。", t3.ToString()));
                    Program.I().cardSelection.setTitle(InterString.Get("请确认[?]张卡片。", t3.ToString()));
                    if (inIgnoranceReplay() || inTheWorld())
                    {
                        t2 = 0;
                        clearResponse();
                    }
                    else if (skip_panel)
                    {
                        Sleep(t2);
                        t2 = 0;
                        clearResponse();
                    }
                }

                Sleep(t2);
                break;
            case GameMessage.RefreshDeck:
            case GameMessage.ShuffleDeck:
                //Debug.Log("GameMessage.ShuffleDeck");
                UIHelper.playSound("shuffle", 1f);
                player = localPlayer(r.ReadByte());
                for (var i = 0; i < cards.Count; i++)
                    if (cards[i].gameObject.activeInHierarchy)
                        if ((cards[i].p.location & (uint) CardLocation.Deck) > 0)
                            if (cards[i].p.controller == player)
                                if (i % 2 == 0)
                                    cards[i].animation_shake_to(1.2f);
                //Sleep(30);
                break;
            case GameMessage.ShuffleHand:
                //Debug.Log("GameMessage.ShuffleHand");
                realize();
                UIHelper.playSound("shuffle", 1f);
                player = localPlayer(r.ReadByte());
                animation_suffleHand(player);
                Sleep(21);
                break;
            case GameMessage.SwapGraveDeck:
                //Debug.Log("GameMessage.SwapGraveDeck");
                realize();
                Sleep(120);
                break;
            case GameMessage.ShuffleSetCard:
                //Debug.Log("GameMessage.ShuffleSetCard");
                UIHelper.playSound("shuffle", 1f);
                count = r.ReadByte();
                var gpss = new List<GPS>();
                for (var i = 0; i < count; i++)
                {
                    gps = r.ReadGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        var position = Vector3.zero;
                        if (card.p.controller == 1)
                            card.animation_confirm(new Vector3(0, 5, 5), new Vector3(0, 90, 180), 0.2f, 0.01f);
                        else
                            card.animation_confirm(new Vector3(0, 5, -5), new Vector3(0, -90, 180), 0.2f, 0.01f);
                    }
                }

                Sleep(30);
                break;
            case GameMessage.ReverseDeck:
                //Debug.Log("GameMessage.ReverseDeck");
                break;
            case GameMessage.DeckTop:
                //Debug.Log("GameMessage.DeckTop");
                break;
            case GameMessage.NewTurn://mark 回合横幅
                //Debug.Log("GameMessage.NewTurn");
                removeSelectedAnimations();
                player = localPlayer(r.ReadByte());

                if (condition != Condition.duel) gameInfo.setTimeStill(player);
                //else
                //{
                //    gameInfo.setTime(player, timeLimit);
                //}
                toDefaultHint();

                //if (player == 1 && InAI == true)
                //{
                //    showWait();
                //}
                if (turns == 1) break;
                if (myTurn)
                {
                    VoiceHandler.PlayFixedLine(0, "MyTurn", true);
                    UIHelper.playSound("SE_DUEL/SE_NEXT_TURN_PLAYER", 1f);
                    gameField.animation_show_big_string_tc_me();
                }
                else
                {
                    VoiceHandler.PlayFixedLine(1, "MyTurn", true);
                    UIHelper.playSound("SE_DUEL/SE_NEXT_TURN_RIVAL", 1f);
                    gameField.animation_show_big_string_tc_op();
                }
                gameInfo.setExcited(turns % 2 == (isFirst ? 0 : 1) ? 1 : 0);
                break;
            case GameMessage.NewPhase:
                //Debug.Log("GameMessage.NewPhase");
                removeSelectedAnimations();
                toDefaultHint();
                int phase = r.ReadUInt16();
                //mark 阶段phase 横幅和音效

                if (GameStringHelper.differ(phase, (long)DuelPhase.BattleStart))
                {
                    if (myTurn)
                    {
                        VoiceHandler.PlayFixedLine(0, "BattlePhase");
                        UIHelper.playSound("SE_DUEL/SE_PHASE_BATTLE", 1f);
                        gameField.animation_show_big_string(Program.I().bp_b);
                    }
                    else
                    {
                        VoiceHandler.PlayFixedLine(1, "BattlePhase");
                        UIHelper.playSound("SE_DUEL/SE_PHASE_OPP_BATTLE", 1f);
                        gameField.animation_show_big_string(Program.I().bp_r);
                    }
                }
                if (GameStringHelper.differ(phase, (long)DuelPhase.Draw))
                {
                    if (myTurn)
                    {
                        UIHelper.playSound("SE_DUEL/SE_PHASE_DRAW", 1f);
                        gameField.animation_show_big_string(Program.I().dp_b);
                    }
                    else
                    {
                        UIHelper.playSound("SE_DUEL/SE_PHASE_OPP_DRAW", 1f);
                        gameField.animation_show_big_string(Program.I().dp_r);
                    }
                }
                if (GameStringHelper.differ(phase, (long)DuelPhase.End))
                {
                    if (myTurn)
                    {
                        VoiceHandler.PlayFixedLine(0, "TurnEnd");
                        UIHelper.playSound("SE_DUEL/SE_PHASE_END", 1f);
                        gameField.animation_show_big_string(Program.I().ep_b);
                    }
                    else
                    {
                        VoiceHandler.PlayFixedLine(1, "TurnEnd");
                        UIHelper.playSound("SE_DUEL/SE_PHASE_OPP_END", 1f);
                        gameField.animation_show_big_string(Program.I().ep_r);
                    }
                }
                if (GameStringHelper.differ(phase, (long)DuelPhase.Main1))
                {
                    if (myTurn)
                    {
                        UIHelper.playSound("SE_DUEL/SE_PHASE_MAIN1", 1f);
                        gameField.animation_show_big_string(Program.I().mp1_b);
                    }
                    else
                    {
                        UIHelper.playSound("SE_DUEL/SE_PHASE_OPP_MAIN1", 1f);
                        gameField.animation_show_big_string(Program.I().mp1_r);
                    }
                }
                if (GameStringHelper.differ(phase, (long)DuelPhase.Main2))
                {
                    if (myTurn)
                    {
                        UIHelper.playSound("SE_DUEL/SE_PHASE_MAIN2", 1f);
                        gameField.animation_show_big_string(Program.I().mp2_b);
                    }
                    else
                    {
                        UIHelper.playSound("SE_DUEL/SE_PHASE_OPP_MAIN2", 1f);
                        gameField.animation_show_big_string(Program.I().mp2_r);
                    }
                }
                if (GameStringHelper.differ(phase, (long)DuelPhase.Standby))
                {
                    if (myTurn)
                    {
                        UIHelper.playSound("SE_DUEL/SE_PHASE_STANDBY", 1f);
                        gameField.animation_show_big_string(Program.I().sp_b);
                    }
                    else
                    {
                        UIHelper.playSound("SE_DUEL/SE_PHASE_OPP_STANDBY", 1f);
                        gameField.animation_show_big_string(Program.I().sp_r);
                    }
                }
                gameField.realize();
                break;
            case GameMessage.Move:
                //Debug.Log("GameMessage.Move");
                code = r.ReadInt32();
                var from = r.ReadGPS();
                var to = r.ReadGPS();
                uint reason = r.ReadUInt32();
                card = GCS_cardGet(to, false);
                //Move实例前
                if(card != null)
                {
                    card.set_code(code);
                    card.p.reason = reason;
                    card.p_moveBefore = from;
                    if (!card.SemiNomiSummoned && (card.get_data().Type & 0x68020C0) > 0 &&
                        (to.location & ((uint)CardLocation.Grave + (uint)CardLocation.Removed)) > 0)
                    {
                        card.add_string_tail("未正规登场");
                    }
                    else
                        card.del_one_tail("未正规登场");


                    if ((from.location & (uint)CardLocation.Onfield) == 0 && (from.location & (uint)CardLocation.Hand) > 0 && (to.location & (uint)CardLocation.Onfield) > 0)
                        card.firstOnField = true;
                    if ((to.location & (uint)CardLocation.Onfield) == 0)
                        card.firstOnField = false;

                    //Debug.LogFormat("{0}: {1:X}", card.get_data().Name, reason);

                    if ((card.p.reason & (uint)CardReason.FUSION) > 0 && !right && (card.p.reason & (uint)CardReason.MATERIAL) > 0 && from.location != to.location)
                    {
                        LoadSFX.spType = "FUSION";
                        Card _card = card.get_data_beforeMove().clone();
                        LoadSFX.materials.Add(_card);
                    }
                    else if ((card.p.reason & (uint)CardReason.SYNCHRO) > 0 && !right && (card.p.reason & (uint)CardReason.MATERIAL) > 0 && from.location != to.location)
                    {
                        LoadSFX.spType = "SYNCHRO";
                        Card _card = card.get_data_beforeMove().clone();
                        LoadSFX.materials.Add(_card);
                    }
                    else if ((card.p.reason & (uint)CardReason.XYZ) > 0 && !right && (card.p.reason & (uint)CardReason.MATERIAL) > 0 && (card.p_moveBefore.location & (uint)CardLocation.Overlay) == 0 && from.location != to.location)
                    {
                        LoadSFX.spType = "XYZ";
                        Card _card = card.get_data_beforeMove().clone();
                        LoadSFX.materials.Add(_card);
                    }
                    else if ((card.p.reason & (uint)CardReason.LINK) > 0 && !right && (card.p.reason & (uint)CardReason.MATERIAL) > 0 && from.location != to.location)
                    {
                        LoadSFX.spType = "LINK";
                        Card _card = card.get_data_beforeMove().clone();
                        LoadSFX.materials.Add(_card);
                    }
                    else if ((card.p.reason & (uint)CardReason.RITUAL) > 0 && !right && (card.p.reason & (uint)CardReason.MATERIAL) > 0 && from.location != to.location)
                    {
                        LoadSFX.spType = "RITUAL";
                        Card _card = card.get_data_beforeMove().clone();
                        LoadSFX.materials.Add(_card);
                    }
                }
                else//衍生物
                {
                    card = GCS_cardGet(to, false, code);
                    Card _card = card.get_data_beforeMove().clone();
                    _card.reason = (int)reason;
                    if ((_card.reason & (uint)CardReason.FUSION) > 0 && !right && (_card.reason & (uint)CardReason.MATERIAL) > 0)
                    {
                        LoadSFX.spType = "FUSION";
                        LoadSFX.materials.Add(_card);
                    }
                    else if ((_card.reason & (uint)CardReason.SYNCHRO) > 0 && !right && (_card.reason & (uint)CardReason.MATERIAL) > 0)
                    {
                        LoadSFX.spType = "SYNCHRO";
                        LoadSFX.materials.Add(_card);
                    }
                    else if ((_card.reason & (uint)CardReason.LINK) > 0 && !right && (_card.reason & (uint)CardReason.MATERIAL) > 0)
                    {
                        LoadSFX.spType = "LINK";
                        LoadSFX.materials.Add(_card);
                    }
                    else if ((_card.reason & (uint)CardReason.RITUAL) > 0 && !right && (_card.reason & (uint)CardReason.MATERIAL) > 0)
                    {
                        LoadSFX.spType = "RITUAL";
                        LoadSFX.materials.Add(_card);
                    }
                }
                realize();
                //Move实例后
                if (card != null)
                {
                    if ((to.position & (int) CardPosition.FaceDown) > 0)//盖卡
                        if (to.location == (uint) CardLocation.MonsterZone ||
                            to.location == (uint)CardLocation.SpellZone)
                        {
                            if(from.location == (uint)CardLocation.Hand)
                                VoiceHandler.PlayFixedLine(to.controller, "SetCard");
                            LoadSFX.DelayDecoration(card.UA_get_accurate_position(), card.p.position, "effects/summon/fxp_som_mgctrpfld_001", "SE_DUEL/SE_LAND_MT_SET", 0.2f, true);
                        }
                            
                    if ((card.p.reason & (uint)CardReason.RELEASE) > 0)//解放
                    {
                        if((card.p.location & (uint)CardLocation.MonsterZone) > 0)
                            VoiceHandler.PlayFixedLine(to.controller, "Release", true, card.get_data().Name);
                        LoadSFX.DelayDecoration(card.gameObject.transform.position, (int)CardPosition.FaceUpAttack, "effects/sacrifice/fxp_sacrifice_rls_001", "无", 0f, true);
                        UIHelper.playSound("SE_DUEL/SE_SUMMON_ADVANCE", 1f);
                    }
                    if ((card.p.reason & (uint)CardReason.DESTROY) > 0)//破坏
                    {
                        LoadSFX.DelayDecoration(card.gameObject.transform.position, (int)CardPosition.FaceUpAttack, "effects/break/fxp_cardbrk_bff_001", "SE_DUEL/SE_CARDBREAK_01", 0f, true);
                    }
                    if ((from.location & (uint)CardLocation.Overlay) > 0 //Overlay out
                        &&
                        ((to.location & (uint)CardLocation.Overlay) == 0 || (card.p.reason & (uint)CardReason.EFFECT) > 0)
                        &&
                        (card.p.reason & (uint)CardReason.RULE) == 0)
                        LoadSFX.DelayDecoration(get_point_worldposition(from) + new Vector3(0f, 1.5f, 0f), (int)CardPosition.FaceUpAttack, "effects/buff/fxp_bff_overlay/fxp_bff_overlay_out_001", "SE_DUEL/SE_CARD_XYZ_OUT", 0f, true);
                    if ((to.location & (uint)CardLocation.Overlay) > 0 //Overlay in
                        &&
                        ((from.location & (uint)CardLocation.Overlay) == 0 && (from.location &(uint)CardLocation.MonsterZone) == 0 || card.p.controller != card.p_moveBefore.controller)
                        &&
                        (card.p.reason & (uint)CardReason.XYZ) > 0
                        &&
                        (card.p.reason & (uint)CardReason.RULE) == 0)
                    {
                        if((from.location & ((uint)CardLocation.Grave + (uint)CardLocation.Removed))> 0)
                            LoadSFX.DelayDecoration(get_point_worldposition(to) + new Vector3(0f, 1.5f, 0f), (int)CardPosition.FaceUpAttack, "effects/buff/fxp_bff_overlay/fxp_bff_overlay_in_001", "SE_DUEL/SE_CARD_XYZ_IN", 0.5f, true);
                        else
                            LoadSFX.DelayDecoration(get_point_worldposition(to) + new Vector3(0f, 1.5f, 0f), (int)CardPosition.FaceUpAttack, "effects/buff/fxp_bff_overlay/fxp_bff_overlay_in_001", "SE_DUEL/SE_CARD_XYZ_IN", 0.2f, true);
                    }
                    //if (to.location == (uint)CardLocation.Removed)
                    //{
                    //    UIHelper.playSound("SE_DUEL/SE_CARDBREAK_01", 1f);
                    //    if (Program.I().setting.setting.Vmove.value)
                    //        card.fast_decoration(Program.I().mod_ocgcore_decoration_removed);
                    //}
                    if ((card.p.reason & (uint)CardReason.MATERIAL) == 0
                        && (card.p_moveBefore.location & (uint)CardLocation.Overlay) > 0 
                        && (card.p.reason & (uint)CardReason.RULE) == 0 
                        && (card.get_data_beforeMove().Type & (uint)CardType.Monster) > 0)
                    {
                        LoadSFX.materials.Clear();
                    }
                }
                break;
            case GameMessage.PosChange:
                //Debug.Log("GameMessage.PosChange");
                realize();
                break;
            case GameMessage.Set:
                //Debug.Log("GameMessage.Set");
                break;
            case GameMessage.Swap:
                //Debug.Log("GameMessage.Swap");
                realize();
                break;
            case GameMessage.FieldDisabled:
                //Debug.Log("GameMessage.FieldDisabled");
                realize();
                break;
            case GameMessage.Summoning:
                //Debug.Log("GameMessage.Summoning");
                code = r.ReadInt32();
                gps = r.ReadGPS();
                card = GCS_cardGet(gps, false);
                removeSelectedAnimations();
                if (card != null)
                {
                    card.set_code(code);
                    if ((gps.position & (uint)CardPosition.FaceUp) > 0)
                    {
                        card.refreshFunctions.Add(card.card_verticle_drawing_handler);
                        card.card_verticle_drawing_handler();
                        card.LoadCard();

                        if (Program.I().setting.setting.spyer.value && (gps.position & (uint)CardPosition.FaceUp) > 0)
                            Program.I().cardDescription.setData(card.get_data(), gps.controller == 0 ? GameTextureManager.myBack : GameTextureManager.opBack, "", true);
                    }
                    BGMHandler.ChangeCardBGM(card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias);
                    if (card.get_data().Level > 6)
                    {

                        LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Advance_s2", "SE_DUEL/SE_LAND_ADVANCE_HIGH", landDelay);
                        CameraControl.ShakeCamera(0.2f);
                    }
                    else if(card.get_data().Level > 4)
                    {
                        LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Advance_s1", "SE_DUEL/SE_LAND_ADVANCE_MIDDLE", landDelay);
                        CameraControl.ShakeCamera(0.2f, false);
                    }
                    else
                    {
                        LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_som_mgctrpfld_001", "SE_DUEL/SE_LAND_NORMAL", landDelay, true);
                        CameraControl.ShakeCamera(0.2f, false);
                    }
                    var light = ABLoader.LoadABFromFile("effects/summon/fxp_somldg/hand/fxp_somldg_hand_001");
                    light.transform.position = card.gameObject.transform.position;
                    if ((gps.position & (uint)CardPosition.Attack) > 0)
                        Object.Destroy(light.transform.GetChild(1).gameObject);
                    else
                        Object.Destroy(light.transform.GetChild(0).gameObject);
                    Object.Destroy(light, 2f);

                    card.animation_show_off(true);
                    LoadSFX.spType = "SPECIAL";
                    LoadSFX.materials.Clear();
                }
                break;
            case GameMessage.Summoned:
                //Debug.Log("GameMessage.Summoned");
                break;
            case GameMessage.SpSummoning:
                //Debug.Log("GameMessage.SpSummoning");
                //Mark 特召装饰
                code = r.ReadInt32();
                gps = r.ReadGPS();
                removeSelectedAnimations();
                gameField.shiftBlackHole(false, get_point_worldposition(gps));
                card = GCS_cardGet(gps, false);

                if (card != null)
                {
                    card.set_code(code);
                    if ((gps.position & (uint)CardPosition.FaceUp) > 0)
                    {
                        card.refreshFunctions.Add(card.card_verticle_drawing_handler);
                        card.card_verticle_drawing_handler();
                        card.LoadCard();

                        if (Program.I().setting.setting.spyer.value && (gps.position & (uint)CardPosition.FaceUp) > 0)
                            Program.I().cardDescription.setData(card.get_data(), gps.controller == 0 ? GameTextureManager.myBack : GameTextureManager.opBack, "", true);
                    }
                    BGMHandler.ChangeCardBGM(card.get_data().Alias == 0 ? card.get_data().Id : card.get_data().Alias);
                    if (LoadSFX.spType == "FUSION")
                    {
                        if (card.get_data().Level > 5)
                        {
                            LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Fusion_s2", "SE_DUEL/SE_LAND_FUSION_HIGH", landDelay);
                            CameraControl.ShakeCamera(0.2f);
                        }
                        else
                        {
                            LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Fusion_s1", "SE_DUEL/SE_LAND_FUSION_MIDDLE", landDelay);
                            CameraControl.ShakeCamera(0.2f, false);
                        }
                    }
                    else if (LoadSFX.spType == "SYNCHRO")
                    {
                        if (card.get_data().Level > 4)
                        {
                            LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Synchro_s2", "SE_DUEL/SE_LAND_SYNCHRO_HIGH", landDelay);
                            CameraControl.ShakeCamera(0.2f);
                        }
                        else
                        {
                            LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Synchro_s1", "SE_DUEL/SE_LAND_SYNCHRO_MIDDLE", landDelay);
                            CameraControl.ShakeCamera(0.2f, false);
                        }
                    }
                    else if (LoadSFX.spType == "XYZ" && (card.get_data().Type & (int)CardType.Xyz) > 0)
                    {
                        if (card.get_data().Level > 3)
                        {
                            LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Xyz_s2", "SE_DUEL/SE_LAND_XYZ_HIGH", landDelay);
                            CameraControl.ShakeCamera(0.2f);
                        }
                        else
                        {
                            LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Xyz_s1", "SE_DUEL/SE_LAND_XYZ_MIDDLE", landDelay);
                            CameraControl.ShakeCamera(0.2f, false);
                        }
                    }
                    else if (LoadSFX.spType == "LINK")
                    {
                        if (card.get_data().Level > 1)
                        {
                            LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Link_s2", "SE_DUEL/SE_LAND_LINK_HIGH", landDelay);
                            CameraControl.ShakeCamera(0.2f);
                        }
                        else
                        {
                            LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Link_s1", "SE_DUEL/SE_LAND_LINK_MIDDLE", landDelay);
                            CameraControl.ShakeCamera(0.2f, false);
                        }
                    }
                    else if (LoadSFX.spType == "RITUAL")
                    {
                        if (card.get_data().Level > 5)
                        {
                            LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Ritual_s2", "SE_DUEL/SE_LAND_RITUAL_HIGH", landDelay);
                            CameraControl.ShakeCamera(0.2f);
                        }
                        else
                        {
                            LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Ritual_s1", "SE_DUEL/SE_LAND_RITUAL_MIDDLE", landDelay);
                            CameraControl.ShakeCamera(0.2f, false);
                        }
                    }
                    else if((card.get_data().Type & (uint)CardType.Pendulum) > 0)
                    {
                        if (card.get_data().Level > 6)
                        {
                            LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Pendulum_s2", "SE_DUEL/SE_LAND_PENDULUM_HIGH", landDelay);
                            CameraControl.ShakeCamera(0.2f);
                        }
                        else
                        {
                            LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Pendulum_s1", "SE_DUEL/SE_LAND_PENDULUM_MIDDLE", landDelay);
                            CameraControl.ShakeCamera(0.2f, false);
                        }
                    }
                    else
                    {
                        if (CameraControl.NeedLanding(card.get_data().Type, card.get_data().Level))
                        {
                            LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Special_s2", "SE_DUEL/SE_LAND_NORMAL_HIGH", landDelay);
                            CameraControl.ShakeCamera(0.2f);
                        }
                        else
                        {
                            LoadSFX.DelayDecoration(card.UA_get_accurate_position(), gps.position, "effects/summon/fxp_somldg/Special_s1", "SE_DUEL/SE_LAND_NORMAL_MIDDLE", landDelay);
                            CameraControl.ShakeCamera(0.2f, false);
                        }
                    }
                    if((card.p.position & (uint)CardPosition.FaceUp) > 0)
                    {
                        var light = ABLoader.LoadABFromFile("effects/summon/fxp_somldg/hand/fxp_somldg_hand_001");
                        light.transform.position = card.gameObject.transform.position;
                        if ((gps.position & (uint)CardPosition.Attack) > 0)
                            Object.Destroy(light.transform.GetChild(1).gameObject);
                        else
                            Object.Destroy(light.transform.GetChild(0).gameObject);
                        Object.Destroy(light, 2f);
                    }

                    card.animation_show_off(true);
                    LoadSFX.spType = "SPECIAL";
                    LoadSFX.materials.Clear();
                }
                break;
            case GameMessage.SpSummoned:
                //Debug.Log("GameMessage.SpSummoned");
                break;
            case GameMessage.FlipSummoning:
                //Debug.Log("GameMessage.FlipSummoning");
                realize();
                removeSelectedAnimations();
                code = r.ReadInt32();
                gps = r.ReadGPS();
                card = GCS_cardGet(gps, false);
                if (card != null)
                {
                    card.set_code(code);
                    card.refreshFunctions.Add(card.card_verticle_drawing_handler);
                    card.card_verticle_drawing_handler();
                    card.LoadCard();
                    if (Program.I().setting.setting.spyer.value && (gps.position & (uint)CardPosition.FaceUp) > 0)
                        Program.I().cardDescription.setData(card.get_data(), gps.controller == 0 ? GameTextureManager.myBack : GameTextureManager.opBack, "", true);

                    UIHelper.playSound("summon", 1f);
                    if (Program.I().setting.setting.Vflip.value)
                    {
                        //var mod = Program.I().mod_ocgcore_ss_spsummon_normal;
                        //card.animationEffect(mod);
                    }

                    card.animation_show_off(true);
                }
                break;
            case GameMessage.FlipSummoned:
                //Debug.Log("GameMessage.FlipSummoned");
                break;
            case GameMessage.Chaining:
                //Debug.Log("GameMessage.Chaining");
                removeAttackHandler();
                code = r.ReadInt32();
                gps = r.ReadGPS();
                card = GCS_cardGet(gps, false);
                if (card != null)
                {
                    if(Program.I().setting.setting.spyer.value)
                        Program.I().cardDescription.setData(CardsManager.Get(code), GameTextureManager.myBack, "", true);
                    card.set_code(code);
                    //UIHelper.playSound("activate", 1);
                    if (Program.I().setting.setting.Voice.value)
                    {
                        var clip = VoiceHandler.GetClip("chants/activate/" + card.get_data().Id);
                        if (clip != null)
                        {
                            Sleep((int)(clip.length * 60));
                            if((card.get_data().Type & (uint)CardType.Monster) > 0)
                                VoiceHandler.PlayVoice(clip, card.p.controller == 0, "发动 " + card.get_data().Name + "的效果");
                            else if ((card.get_data().Type & (uint)CardType.Spell) > 0)
                                VoiceHandler.PlayVoice(clip, card.p.controller == 0, "发动魔法卡 " + card.get_data().Name);
                            else if ((card.get_data().Type & (uint)CardType.Trap) > 0)
                                VoiceHandler.PlayVoice(clip, card.p.controller == 0, "发动陷阱卡 " + card.get_data().Name);
                        }
                        else
                        {
                            int chain;
                            if (card.chains.Count == 0)
                                chain = 1;
                            else
                                chain = card.chains[0].i;

                            if((card.get_data().Type & (uint)CardType.Pendulum) > 0 && (gps.location & (uint)CardLocation.SpellZone) > 0 && card.firstOnField)
                                VoiceHandler.PlayFixedLine(card.p.controller, "SetPendulumScale");
                            else if ((card.get_data().Type & (uint)CardType.Pendulum) > 0 && (gps.location & (uint)CardLocation.SpellZone) > 0)
                            {
                                if (!VoiceHandler.PlayCardLine(gps.controller, "activate", card, true, card.firstOnField, gps.location, chain))
                                    VoiceHandler.PlayFixedLine(card.p.controller, "ActivatePendulum", true, card.get_data().Name);
                            }
                            else if((card.get_data().Type & (uint)CardType.Monster) > 0)
                            {
                                if (!VoiceHandler.PlayCardLine(gps.controller, "activateM", card, true, card.firstOnField, gps.location, chain))
                                    VoiceHandler.PlayFixedLine(card.p.controller, "ActivateMonsterEffect", true, card.get_data().Name);
                            }
                            else if ((card.get_data().Type & (uint)CardType.Spell) > 0)
                            {
                                if(!VoiceHandler.PlayCardLine(gps.controller, "activate", card, true, card.firstOnField, gps.location, chain))
                                {
                                    if ((card.get_data().Type & (uint)CardType.QuickPlay) > 0)
                                        VoiceHandler.PlayFixedLine(card.p.controller, "ActivateQuickPlayMagic", true, card.get_data().Name);
                                    else if ((card.get_data().Type & (uint)CardType.Continuous) > 0)
                                        VoiceHandler.PlayFixedLine(card.p.controller, "ActivateContinuousMagic", true, card.get_data().Name);
                                    else if ((card.get_data().Type & (uint)CardType.Equip) > 0)
                                        VoiceHandler.PlayFixedLine(card.p.controller, "ActivateEquipMagic", true, card.get_data().Name);
                                    else if ((card.get_data().Type & (uint)CardType.Ritual) > 0)
                                        VoiceHandler.PlayFixedLine(card.p.controller, "ActivateRitualMagic", true, card.get_data().Name);
                                    else if ((card.get_data().Type & (uint)CardType.Field) > 0)
                                        VoiceHandler.PlayFixedLine(card.p.controller, "ActivateFieldMagic", true, card.get_data().Name);
                                    else
                                        VoiceHandler.PlayFixedLine(card.p.controller, "ActivateMagic", true, card.get_data().Name);
                                }
                            }
                            else if ((card.get_data().Type & (uint)CardType.Trap) > 0)
                            {
                                    if (!VoiceHandler.PlayCardLine(gps.controller, "activate", card, true, card.firstOnField, gps.location, chain))
                                    {
                                        if ((card.get_data().Type & (uint)CardType.Continuous) > 0)
                                            VoiceHandler.PlayFixedLine(card.p.controller, "ActivateContinuousTrap", true, card.get_data().Name);
                                        else if ((card.get_data().Type & (uint)CardType.Counter) > 0)
                                            VoiceHandler.PlayFixedLine(card.p.controller, "ActivateCounterTrap", true, card.get_data().Name);
                                        else
                                            VoiceHandler.PlayFixedLine(card.p.controller, "ActivateTrap", true, card.get_data().Name);
                                    }
                                }
                        }
                    }
                    card.animation_show_off(false);
                    if ((card.get_data().Type & (int)CardType.Monster) > 0)
                        if (Program.I().setting.setting.Vactm.value)
                        {
                            //var mod = Program.I().mod_ocgcore_cs_mon_light;
                            //if ((card.get_data().Attribute & (int)CardAttribute.Earth) > 0)
                            //    mod = Program.I().mod_ocgcore_cs_mon_earth;
                            //if ((card.get_data().Attribute & (int)CardAttribute.Water) > 0)
                            //    mod = Program.I().mod_ocgcore_cs_mon_water;
                            //if ((card.get_data().Attribute & (int)CardAttribute.Fire) > 0)
                            //    mod = Program.I().mod_ocgcore_cs_mon_fire;
                            //if ((card.get_data().Attribute & (int)CardAttribute.Wind) > 0)
                            //    mod = Program.I().mod_ocgcore_cs_mon_wind;
                            //if ((card.get_data().Attribute & (int)CardAttribute.Light) > 0)
                            //    mod = Program.I().mod_ocgcore_cs_mon_light;
                            //if ((card.get_data().Attribute & (int)CardAttribute.Dark) > 0)
                            //    mod = Program.I().mod_ocgcore_cs_mon_dark;
                            //mod.GetComponent<partical_scaler>().scale =
                            //    2f + Mathf.Clamp(card.get_data().Attack, 0, 3500) / 3000f * 5f;
                            //card.fast_decoration(mod);
                        }

                    //if ((card.get_data().Type & (int)CardType.Spell) > 0)
                    //    if (Program.I().setting.setting.Vacts.value)
                    //        card.positionEffect(Program.I().mod_ocgcore_decoration_magic_activated);
                    //if ((card.get_data().Type & (int)CardType.Trap) > 0)
                    //    if (Program.I().setting.setting.Vactt.value)
                    //        card.positionShot(Program.I().mod_ocgcore_decoration_trap_activated);
                }
                card.firstOnField = false;
                realize(false, VoiceHandler.voiceTime);
                break;
            case GameMessage.Chained:
                //Debug.Log("GameMessage.Chained");
                Sleep(20);
                break;
            case GameMessage.ChainSolving:
                //Debug.Log("GameMessage.ChainSolving");
                var id = r.ReadByte() - 1;
                if (id < 0) id = 0;
                card = null;
                if (id < cardsInChain.Count)
                    card = cardsInChain[id];
                if (Program.I().setting.setting.Vcard.value == false)
                    break;
                if (card != null)
                {
                    ES_hint = InterString.Get("「[?]」的效果处理时", card.get_data().Name);
                    card.CS_chainCircleYellow();
                    if (card.disabled || card.negated)
                        break;
                    GameObject go = null;
                    if (card.get_data().Id == 12580477)
                    {
                        go = ABLoader.LoadABFromFolder("card/12580477", "CardAnimation");
                        for (int i = 0; i < 3; i++)
                        {
                            if(go.transform.GetChild(i).name.StartsWith("fxl_04343_tnd_emit_01"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("Ef04343_Far") && card.p.controller != 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                            if (go.transform.GetChild(i).name.StartsWith("Ef04343_Near") && card.p.controller == 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                        }
                    }
                    else if (card.get_data().Id == 72302403)
                    {
                        go = ABLoader.LoadABFromFolder("card/72302403", "CardAnimation");
                        for (int i = 0; i < 3; i++)
                        {
                            if (go.transform.GetChild(i).name.StartsWith("AREF_SRL_01"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name == "Ef04354(Clone)" && card.p.controller != 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                            if (go.transform.GetChild(i).name == "Ef04354Op(Clone)" && card.p.controller == 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                        }
                    }
                    else if (card.get_data().Id == 18144506 || card.get_data().Id == 18144507)
                    {
                        go = ABLoader.LoadABFromFolder("card/18144506", "CardAnimation");
                        for (int i = 0; i < 4; i++)
                        {
                            if (go.transform.GetChild(i).name.StartsWith("Ef04678_model"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("Ef04678_anim"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name == "Ef04678(Clone)" && card.p.controller != 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                            if (go.transform.GetChild(i).name == "Ef04678Op(Clone)" && card.p.controller == 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                        }
                        foreach(var child in go.transform.GetComponentsInChildren<Transform>(true))
                        {
                            if (child.name == "DistPlane" || child.name == "Plane")
                                Object.Destroy(child.gameObject);
                        }
                    }
                    else if (card.get_data().Id == 41420027)
                    {
                        go = ABLoader.LoadABFromFolder("card/41420027", "CardAnimation");
                        for (int i = 0; i < 3; i++)
                        {
                            if (go.transform.GetChild(i).name.StartsWith("Ef04861_anim"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("Ef04861_model"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                        }
                    }
                    else if (card.get_data().Id == 44095762)
                    {
                        if(card.p.controller == 0)
                            go = ABLoader.LoadABFromFile("card/44095762/37d5014e");
                        else
                            go = ABLoader.LoadABFromFile("card/44095762/7fc055c1");
                    }
                    else if (card.get_data().Id == 5318639)
                    {
                        gameCard targetCard = null;
                        foreach (var _card in cards)
                        {
                            if (_card.cardsTargetMe.Contains(card))
                            {
                                targetCard = _card;
                                break;
                            }
                        }
                        if ((targetCard.p.location & (uint)CardLocation.SpellZone) == 0)
                            break;
                        go = ABLoader.LoadABFromFolder("card/5318639", "CardAnimation");
                        for (int i = 0; i < 3; i++)
                        {
                            if (go.transform.GetChild(i).name.StartsWith("Efo4909Spiral01"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("TestTornado01"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                        }
                        Transform offset = go.transform.Find("Ef04909(Clone)/Offset");
                        if(targetCard.gameObject.transform.localPosition.z > 0)
                        {
                            offset.localPosition = new Vector3(targetCard.gameObject.transform.localPosition.x, 0, -7.4f + targetCard.gameObject.transform.localPosition.z);
                        }
                        else
                        {
                            offset.parent.localEulerAngles = new Vector3(0f, 180f, 0);
                            offset.localPosition = new Vector3(-targetCard.gameObject.transform.localPosition.x, 0, -7.4f - targetCard.gameObject.transform.localPosition.z);
                        }
                    }
                    else if (card.get_data().Id == 61740673)
                    {
                        go = ABLoader.LoadABFromFolder("card/61740673", "CardAnimation");
                        for (int i = 0; i < 6; i++)
                        {
                            if (go.transform.GetChild(i).name.StartsWith("Ef04960_anim"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("Ef04960_model"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("Ef04960_Spear01"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("Ef04960_Castle01"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("Ef04960_fxl_Circle_002"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                        }
                    }
                    else if (card.get_data().Id == 62279055)
                    {
                        gameCard targetCard = null;
                        foreach (var _card in cards)
                        {
                            if (_card.cardsTargetMe.Contains(card))
                            {
                                targetCard = _card;
                                break;
                            }
                        }
                        if ((targetCard.p.location & (uint)CardLocation.MonsterZone) == 0)
                            break;
                        go = ABLoader.LoadABFromFolder("card/62279055", "CardAnimation");
                        for (int i = 0; i < 3; i++)
                        {
                            if (go.transform.GetChild(i).name.StartsWith("fxl_04343_tnd_emit_01"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name == "Ef05124_near(Clone)" && card.p.controller != 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                            if (go.transform.GetChild(i).name == "Ef05124_far(Clone)" && card.p.controller == 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                        }
                        ABLoader.ChangeLayer(go, "Default", true);
                    }
                    else if (card.get_data().Id == 63391643)
                    {
                        gameCard targetCard = null;
                        foreach (var _card in cards)
                        {
                            if (_card.cardsTargetMe.Contains(card))
                            {
                                targetCard = _card;
                                break;
                            }
                        }
                        if ((targetCard.p.location & (uint)CardLocation.MonsterZone) == 0)
                            break;

                        go = ABLoader.LoadABFromFolder("card/63391643", "CardAnimation");
                        for (int i = 0; i < 2; i++)
                            if (go.transform.GetChild(i).name.StartsWith("EfThousandKnives01"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);

                        ABLoader.ChangeLayer(go, "Default", true);
                        go.transform.localPosition = targetCard.gameObject.transform.localPosition;
                    }
                    else if (card.get_data().Id == 75500286)
                    {
                        go = ABLoader.LoadABFromFolder("card/75500286", "CardAnimation");
                        for (int i = 0; i < 2; i++)
                            if (go.transform.GetChild(i).name.StartsWith("Ef06161_model"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                        foreach (var body in go.transform.GetComponentsInChildren<MeshRenderer>())
                            body.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                    else if (card.get_data().Id == 24224830)
                    {
                        gameCard targetCard = null;
                        foreach (var _card in cards)
                        {
                            if (_card.cardsTargetMe.Contains(card))
                            {
                                targetCard = _card;
                                break;
                            }
                        }
                        if ((targetCard.p.location & (uint)CardLocation.Grave) == 0)
                            break;

                        go = ABLoader.LoadABFromFolder("card/24224830", "CardAnimation");
                        for (int i = 0; i < 5; i++)
                        {
                            if (go.transform.GetChild(i).name.StartsWith("fxl_Mesh_E13619_BG_2"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("Ef13619_Anim01"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("Ef13619_Chunk01"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name == "Ef13619(Clone)" && card.p.controller != 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                            if (go.transform.GetChild(i).name == "Ef13619Op(Clone)" && card.p.controller == 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                        }
                    }
                    else if (card.get_data().Id == 23002292)
                    {
                        go = ABLoader.LoadABFromFolder("card/23002292", "CardAnimation");
                        for (int i = 0; i < 7; i++)
                            if (go.transform.GetChild(i).name.StartsWith("Ef13622") == false)
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                        ABLoader.ChangeLayer(go, "fx_2d", true);
                    }
                    else if (card.get_data().Id == 65681983)
                    {
                        go = ABLoader.LoadABFromFolder("card/65681983", "CardAnimation");
                        for (int i = 0; i < 3; i++)
                        {
                            if (go.transform.GetChild(i).name.StartsWith("Ef14627_ani"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name == "Ef14627_Far(Clone)" && card.p.controller != 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                            if (go.transform.GetChild(i).name == "Ef14627_Near(Clone)" && card.p.controller == 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                        }
                    }
                    else if (card.get_data().Id == 54693926)
                    {
                        go = ABLoader.LoadABFromFolder("card/54693926", "CardAnimation");
                        for (int i = 0; i < 9; i++)
                        {
                            if (go.transform.GetChild(i).name.StartsWith("Ef14742_Tamashi02_mesh"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("Ef14742_hand01_mesh"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("Ef14742_Tamashi02"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("fxl_04343_tnd_emit_01"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("Ef14742_Tamashi01_mesh"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("Ef14742_Tamashi01"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name.StartsWith("Ef14742_hand01"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            if (go.transform.GetChild(i).name == "Ef14742(Clone)" && card.p.controller != 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                            if (go.transform.GetChild(i).name == "Ef14742Op(Clone)" && card.p.controller == 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                        }
                        ABLoader.ChangeLayer(go, "Default", true);

                        Transform bgSet = null;
                        if(card.p.controller == 0)
                        {
                            go.transform.Find("Ef14742(Clone)/EffectParts").localPosition = new Vector3(0, 1, 0);
                            bgSet = go.transform.Find("Ef14742(Clone)/BGSet");
                        }
                        else
                        {
                            go.transform.Find("Ef14742Op(Clone)/EffectParts").localPosition = new Vector3(0, 1, 0);
                            bgSet = go.transform.Find("Ef14742Op(Clone)/BGSet");
                        }
                        float x = Screen.width / (float)(Screen.height / 9);
                        bgSet.localScale = new Vector3(bgSet.localScale.x * x / 16, bgSet.localScale.y, bgSet.localScale.z);
                    }
                    else if (card.get_data().Id == 25311006)
                    {
                        go = ABLoader.LoadABFromFolder("card/25311006", "CardAnimation");
                        for (int i = 0; i < 2; i++)
                        {
                            if (go.transform.GetChild(i).name.StartsWith("Ef15296Anim"))
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                        }
                        foreach (var body in go.transform.GetComponentsInChildren<SkinnedMeshRenderer>())
                            body.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                    else if (card.get_data().Id == 24299458)
                    {
                        go = ABLoader.LoadABFromFolder("card/24299458", "CardAnimation");
                        for (int i = 0; i < 2; i++)
                        {
                            if (go.transform.GetChild(i).name == "Ef15299_Far(Clone)" && card.p.controller != 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                            if (go.transform.GetChild(i).name == "Ef15299_Near(Clone)" && card.p.controller == 0)
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                        }
                    }
                    else if (card.get_data().Id == 2263869)
                    {
                        go = ABLoader.LoadABFromFolder("card/2263869", "CardAnimation");
                        for (int i = 0; i < 2; i++)
                        {
                            if (go.transform.GetChild(i).name.StartsWith("fxl_Ef17469Arrowheadset01"))
                            {
                                go.transform.GetChild(i).gameObject.SetActive(false);
                                Object.Destroy(go.transform.GetChild(i).gameObject);
                            }
                        }
                        var spine = go.transform.GetComponentInChildren<SkeletonAnimation>().state;
                        spine.SetAnimation(1, "animation", false);
                        ABLoader.ChangeLayer(go, "fx_2d", true);
                    }
                    else
                    {
                        if (Directory.Exists(Boot.root + "assetbundle/card/" + card.get_data().Id))
                        {
                            go = ABLoader.LoadABFromFolder("card/" + card.get_data().Id, "CardAnimation");
                            ABLoader.ChangeLayer(go, "fx_2d", true);
                        }
                    }

                    if (go == null) return;

                    go.transform.parent = GameObject.Find("fx_2d").transform;
                    Transform black = null;
                    foreach (var child in go.transform.GetComponentsInChildren<SpriteScaler>(true))
                    {
                        if ((child.name == "Black" || child.name == "BK" || child.name == "Back") 
                            && 
                            child.gameObject.activeInHierarchy
                            &&
                            child.transform.parent.name != "Ef04861(Clone)"
                            &&
                            child.transform.parent.name != "Ef04960(Clone)"
                            &&
                            child.transform.parent.name != "Ef17469VFX(Clone)"
                            )
                        {
                            black = child.transform;
                            break;
                        }
                    }
                    if (black != null)
                        black.localScale = new Vector3(black.localScale.x * 10, black.localScale.y, black.localScale.z);
                    float destroyTime = SEHandler.PlayCardSE(card.get_data().Id) - 1.5f;
                    if (destroyTime < 0) destroyTime = 0;
                    if (card.get_data().Id == 18144506 || card.get_data().Id == 18144507)
                    {
                        Object.Destroy(go, 1.8f);
                        Sleep(108);
                    }
                    else if (card.get_data().Id == 83764718 || card.get_data().Id == 83764719)
                    {
                        Object.Destroy(go, 2.5f);
                        Sleep(150);
                    }
                    else if (card.get_data().Id == 5318639)
                    {
                        Object.Destroy(go, 2f);
                        Sleep(120);
                    }
                    else if (card.get_data().Id == 61740673)
                    {
                        Object.Destroy(go, 1.75f);
                        Sleep(105);
                    }
                    else if (card.get_data().Id == 63391643)
                    {
                        Object.Destroy(go, 2.5f);
                        Sleep(150);
                    }
                    else if (card.get_data().Id == 75500286)
                    {
                        Object.Destroy(go, 1.67f);
                        Sleep(100);
                    }
                    else if (card.get_data().Id == 23002292)
                    {
                        Object.Destroy(go, 1.5f);
                        Sleep(90);
                    }
                    else if (card.get_data().Id == 65681983)
                    {
                        Object.Destroy(go, 1.8f);
                        Sleep(108);
                    }
                    else if (card.get_data().Id == 54693926)
                    {
                        Object.Destroy(go, 3.5f);
                        Sleep(210);
                    }
                    else if (card.get_data().Id == 25311006)
                    {
                        Object.Destroy(go, 2f);
                        Sleep(120);
                    }
                    else if (card.get_data().Id == 25311006)
                    {
                        Object.Destroy(go, 2.45f);
                        Sleep(147);
                    }
                    else if (card.get_data().Id == 2263869)
                    {
                        Object.Destroy(go, 2.167f);
                        Sleep(130);
                    }
                    else
                    {
                        Object.Destroy(go, destroyTime);
                        Sleep((int)(destroyTime * 60));
                    }
                }
                break;
            case GameMessage.ChainSolved:
                //Debug.Log("GameMessage.ChainSolved");
                id = r.ReadByte() - 1;
                if (id < 0) id = 0;
                card = null;
                if (id < cardsInChain.Count)
                {
                    card = cardsInChain[id];
                    if (id >= 1)
                        if (Program.I().setting.setting.Vchain.value)
                            Object.Destroy(
                                Object.Instantiate(Program.I().mod_ocgcore_cs_bomb, card.gameObject.transform.position,
                                    Quaternion.identity), 5f);
                }

                if (card != null)
                {
                    if (card.isShowed)
                    {
                        card.isShowed = false;
                        realize();
                        toNearest(true);
                    }
                }
                Sleep(17);
                LoadSFX.spType = "SPCIAL";//防止狞猛龙等装备墓地连接怪的Move reason带连接标签，但可能会导致连锁①的紧急同调等效果正规召唤无动画（未测试）；
                LoadSFX.materials.Clear();
                break;
            case GameMessage.ChainEnd:
                //Debug.Log("GameMessage.ChainEnd");
                clearChainEnd();
                foreach(var _card in cards)
                    _card.cardsTargetMe.Clear();
                break;
            case GameMessage.ChainNegated:
            case GameMessage.ChainDisabled:
                //Debug.Log("GameMessage.ChainDisabled");
                var id_ = r.ReadByte() - 1;
                if (id_ < 0) id_ = 0;
                card = null;
                if (id_ < cardsInChain.Count)
                {
                    card = cardsInChain[id_];
                    if (Program.I().setting.setting.Vchain.value)
                    {
                        card.cardAnimation.PlayDelayedAnimation("negate", 0);
                        card.negated = true;
                        Sleep(60);
                    }

                    card.animation_show_off(false, true);
                }

                if (card != null)
                    if (card.isShowed)
                    {
                        card.isShowed = false;
                        realize();
                        toNearest(true);
                    }

                break;
            case GameMessage.CardSelected:
                //Debug.Log("GameMessage.CardSelected");
                break;
            case GameMessage.RandomSelected:
                //Debug.Log("GameMessage.RandomSelected");
                pIN = false;
                psum = false;
                player = localPlayer(r.ReadByte());
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    gps = r.ReadGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        if (card.p.location == (uint) CardLocation.SpellZone)
                            if (card.p.sequence == 6 || card.p.sequence == 7)
                                pIN = true;
                        cardsInSelectAnimation.Add(card);
                        card.currentKuang = gameCard.kuangType.selected;
                        //if (Program.I().setting.setting.Vchain.value)
                        //{
                        //card.add_one_decoration(Program.I().mod_ocgcore_decoration_card_selected, 3, Vector3.zero,
                        //    "selected", false);
                        //}
                        card.gameObject.transform.Find("card/card").gameObject.SetActive(true);

                        if (Program.I().setting.setting.Vpedium.value)
                        {
                            var pvector = Vector3.zero;
                            if (cardsInChain.Count == 0)
                                if (cardsInSelectAnimation.Count == 2)
                                    if (cardsInSelectAnimation[0].p.location == (uint) CardLocation.SpellZone)
                                        if (cardsInSelectAnimation[1].p.location == (uint) CardLocation.SpellZone)
                                            if (cardsInSelectAnimation[1].p.sequence == 6 ||
                                                cardsInSelectAnimation[1].p.sequence == 7)
                                                if (cardsInSelectAnimation[0].p.sequence == 6 ||
                                                    cardsInSelectAnimation[0].p.sequence == 7)
                                                    if (cardsInSelectAnimation[0].p.controller ==
                                                        cardsInSelectAnimation[0].p.controller)
                                                    {
                                                        psum = true;
                                                        if (cardsInSelectAnimation[0].p.controller == 0)
                                                            pvector = new Vector3(0, 0, -9f);
                                                        else
                                                            pvector = new Vector3(0, 0, 9f);
                                                    }

                            if (psum)
                            {
                                var real = (Program.fieldSize - 1) * 0.9f + 1f;
                                Program.I().mod_ocgcore_ss_p_sum_effect.transform.Find("l").localPosition =
                                    new Vector3(-15.2f * real, 0, 0);
                                Program.I().mod_ocgcore_ss_p_sum_effect.transform.Find("r").localPosition =
                                    new Vector3(14.65f * real, 0, 0);
                                Object.Destroy(
                                    Object.Instantiate(Program.I().mod_ocgcore_ss_p_sum_effect, pvector,
                                        Quaternion.identity), 5f);
                            }
                        }
                    }
                }

                if (!pIN) Sleep(30);
                break;
            case GameMessage.BecomeTarget:
                //Debug.Log("GameMessage.BecomeTarget");
                var targetTime = 0;
                psum = false;
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    gps = r.ReadGPS();
                    card = GCS_cardGet(gps, false);
                    if (card != null)
                    {
                        if (card.p.location == (uint) CardLocation.SpellZone &&
                            (card.p.sequence == 6 || card.p.sequence == 7))
                            targetTime += 0;
                        else if ((card.p.location & (uint) CardLocation.Onfield) > 0)
                            targetTime += 30;
                        else
                            targetTime += 30;
                        cardsInSelectAnimation.Add(card);
                        card.currentKuang = gameCard.kuangType.selected;
                        GameObject select = null;
                        if ((card.p.location & (uint)CardLocation.Grave) >0 || (card.p.location & (uint)CardLocation.Removed) > 0)
                        {
                            card.cardAnimation.PlayDelayedAnimation("card_bff_target", 0);
                            select = LoadSFX.Decoration("effects/other/fxp_chain_cardlight_001", true, card.cardModel);
                            select.transform.localPosition = Vector3.zero;
                            select.transform.localScale = Vector3.one;
                            select.transform.localEulerAngles = new Vector3(90, 180, 0);
                        }
                        else
                        {
                            select = LoadSFX.Decoration("effects/other/fxp_card_decide_001", true, card.cardModel);
                            select.transform.localPosition = Vector3.zero;
                            if((card.p.position &(uint)CardPosition.FaceUp) >0)
                                select.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                            else
                                select.transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
                            select.transform.localScale = Vector3.one;
                        }

                        UIHelper.playSound("SE_DUEL/SE_CEMETERY_CARD", 1f);
                        Object.Destroy(select, 0.5f);

                        //card.gameObject.transform.Find("card/card").gameObject.SetActive(true);
                        var pvector = Vector3.zero;
                        if (MasterRule >= 4)
                            if (cardsInChain.Count == 0)
                                if (cardsInSelectAnimation.Count == 2)
                                    if (cardsInSelectAnimation[0].p.location == (uint)CardLocation.SpellZone)
                                        if (cardsInSelectAnimation[1].p.location == (uint)CardLocation.SpellZone)
                                            if (cardsInSelectAnimation[1].p.sequence == 0 ||
                                                cardsInSelectAnimation[1].p.sequence == 4)
                                                if (cardsInSelectAnimation[0].p.sequence == 0 ||
                                                    cardsInSelectAnimation[0].p.sequence == 4)
                                                    if (cardsInSelectAnimation[0].p.controller ==
                                                        cardsInSelectAnimation[0].p.controller)
                                                    {
                                                        psum = true;
                                                        if (cardsInSelectAnimation[0].p.controller == 0)
                                                            pvector = new Vector3(0, 0, -9f);
                                                        else
                                                            pvector = new Vector3(0, 0, 9f);
                                                    }
                        if (MasterRule <= 3)
                            if (cardsInChain.Count == 0)
                                if (cardsInSelectAnimation.Count == 2)
                                    if (cardsInSelectAnimation[0].p.location == (uint)CardLocation.SpellZone)
                                        if (cardsInSelectAnimation[1].p.location == (uint)CardLocation.SpellZone)
                                            if (cardsInSelectAnimation[1].p.sequence == 6 ||
                                                cardsInSelectAnimation[1].p.sequence == 7)
                                                if (cardsInSelectAnimation[0].p.sequence == 6 ||
                                                    cardsInSelectAnimation[0].p.sequence == 7)
                                                    if (cardsInSelectAnimation[0].p.controller ==
                                                        cardsInSelectAnimation[0].p.controller)
                                                    {
                                                        psum = true;
                                                        if (cardsInSelectAnimation[0].p.controller == 0)
                                                            pvector = new Vector3(0, 0, -9f);
                                                        else
                                                            pvector = new Vector3(0, 0, 9f);
                                                    }
                        if (psum && Program.I().setting.setting.Vpedium.value)
                        {
                            VoiceHandler.PlayFixedLine(cardsInSelectAnimation[0].p.controller, "PendulumSummon", false);
                        }
                        if(psum && !Program.I().setting.setting.Vpedium.value)
                        {
                            psum = false;
                            VoiceHandler.PlayFixedLine(cardsInSelectAnimation[0].p.controller, "PendulumSummon");
                        }

                        if (psum)
                        {
                            //var real = (Program.fieldSize - 1) * 0.9f + 1f;
                            //Program.I().mod_ocgcore_ss_p_sum_effect.transform.Find("l").localPosition =
                            //    new Vector3(-15.2f * real, 0, 0);
                            //Program.I().mod_ocgcore_ss_p_sum_effect.transform.Find("r").localPosition =
                            //    new Vector3(14.65f * real, 0, 0);
                            //Object.Destroy(
                            //    Object.Instantiate(Program.I().mod_ocgcore_ss_p_sum_effect, pvector,
                            //        Quaternion.identity), 5f);

                            targetTime = 220;
                            GameObject pendulum = ABLoader.LoadABFromFolder("timeline/summon/summonpendulum/summonpendulum01", "SummonAnimation");
                            Object.Destroy(pendulum.transform.GetChild(1).gameObject);
                            Object.Destroy(pendulum.transform.GetChild(2).gameObject);
                            pendulum.transform.parent = GameObject.Find("main_3d").transform;
                            ABLoader.ChangeLayer(pendulum, "main_3d");
                            UIHelper.playSound("SE_DUEL/SE_SMN_PENDULUM_01_01", 1f);
                            UIHelper.playSound("SE_DUEL/SE_SMN_PENDULUM_01_02", 1f);
                            UIHelper.playSound("SE_DUEL/SE_SMN_PENDULUM_01_03", 1f);
                            UIHelper.playSound("SE_DUEL/SE_SMN_PENDULUM_02", 1f);

                            Transform black = pendulum.transform.Find("SummonPendulum01(Clone)/Offset/Black");
                            Vector3 black_ = black.localScale;
                            black.localScale = new Vector3(black_.x * 10, black_.y, black_.z);

                            GameObject card01 = null;
                            GameObject card02 = null;
                            GameObject num1 = null;
                            GameObject num2 = null;
                            foreach (Transform child in pendulum.GetComponentsInChildren<Transform>(true))
                            {
                                if (child.name == "DummyCard01")
                                    card01 = child.gameObject;
                                if (child.name == "DummyCard02")
                                    card02 = child.gameObject;
                                if (child.name == "LPendulumNumSet")
                                    num1 = child.gameObject;
                                if (child.name == "RPendulumNumSet")
                                    num2 = child.gameObject;
                            }

                            card01.transform.GetChild(1).GetComponent<Renderer>().material.mainTexture = GameTextureManager.GetCardPictureNow(cardsInSelectAnimation[0].get_data().Id);
                            card02.transform.GetChild(1).GetComponent<Renderer>().material.mainTexture = GameTextureManager.GetCardPictureNow(cardsInSelectAnimation[1].get_data().Id);

                            LoadSFX ls = GameObject.Find("Program").GetComponent<LoadSFX>();
                            if (cardsInSelectAnimation[0].get_data().LScale < 10)
                            {
                                Object.Destroy(num1.transform.GetChild(1).gameObject);
                                Object.Destroy(num1.transform.GetChild(2).gameObject);
                                Object.Destroy(num1.transform.GetChild(3).gameObject);
                                Object.Destroy(num1.transform.GetChild(4).gameObject);
                                num1.transform.GetChild(0).GetComponent<Renderer>().material.mainTexture = ls.PendulumNum(cardsInSelectAnimation[0].get_data().LScale);
                                num1.transform.GetChild(5).GetComponent<Renderer>().material.mainTexture = ls.PendulumNum(cardsInSelectAnimation[0].get_data().LScale);
                            }
                            else
                            {
                                Object.Destroy(num1.transform.GetChild(0).gameObject);
                                Object.Destroy(num1.transform.GetChild(5).gameObject);
                                num1.transform.GetChild(2).GetComponent<Renderer>().material.mainTexture = ls.PendulumNum(1);
                                num1.transform.GetChild(3).GetComponent<Renderer>().material.mainTexture = ls.PendulumNum(1);
                                num1.transform.GetChild(1).GetComponent<Renderer>().material.mainTexture = ls.PendulumNum(cardsInSelectAnimation[0].get_data().LScale - 10);
                                num1.transform.GetChild(4).GetComponent<Renderer>().material.mainTexture = ls.PendulumNum(cardsInSelectAnimation[0].get_data().LScale - 10);
                            }
                            if (cardsInSelectAnimation[1].get_data().RScale < 10)
                            {
                                Object.Destroy(num2.transform.GetChild(1).gameObject);
                                Object.Destroy(num2.transform.GetChild(2).gameObject);
                                Object.Destroy(num2.transform.GetChild(3).gameObject);
                                Object.Destroy(num2.transform.GetChild(4).gameObject);
                                num2.transform.GetChild(0).GetComponent<Renderer>().material.mainTexture = ls.PendulumNum(cardsInSelectAnimation[1].get_data().RScale + 10);
                                num2.transform.GetChild(5).GetComponent<Renderer>().material.mainTexture = ls.PendulumNum(cardsInSelectAnimation[1].get_data().RScale + 10);
                            }
                            else
                            {
                                Object.Destroy(num2.transform.GetChild(0).gameObject);
                                Object.Destroy(num2.transform.GetChild(5).gameObject);
                                num2.transform.GetChild(2).GetComponent<Renderer>().material.mainTexture = ls.PendulumNum(11);
                                num2.transform.GetChild(3).GetComponent<Renderer>().material.mainTexture = ls.PendulumNum(11);
                                num2.transform.GetChild(1).GetComponent<Renderer>().material.mainTexture = ls.PendulumNum(cardsInSelectAnimation[1].get_data().RScale);
                                num2.transform.GetChild(4).GetComponent<Renderer>().material.mainTexture = ls.PendulumNum(cardsInSelectAnimation[1].get_data().RScale);
                            }
                            Object.Destroy(pendulum, 4f);
                            if(MasterRule >= 4)
                            {
                                UIHelper.playSound("SE_DUEL/SE_SUMMON_PENDULUM_SET", 1f);
                                GameObject pendulumSet = ABLoader.LoadABFromFolder("timeline/summon/summonpendulum/summonpendulumscaleset", "PendulumSet");
                                Object.Destroy(pendulumSet.transform.GetChild(1).gameObject);
                                pendulumSet.transform.parent = GameObject.Find("main_3d").transform;
                                //ABLoader.ChangeLayer(pendulumSet, "main_3d");

                                GameObject lScale = null;
                                GameObject rScale = null;
                                GameObject lineSet = null;
                                foreach(Transform child in pendulumSet.GetComponentsInChildren<Transform>(true))
                                {
                                    if (child.name == "LScale")
                                        lScale = child.gameObject;
                                    if (child.name == "RScale")
                                        rScale = child.gameObject;
                                    if (child.name == "PendulumScaleLineSet")
                                        lineSet = child.gameObject;
                                }
                                if(Program.I().setting.setting.Vpedium.value)
                                {
                                    if(card.p.controller == 0)
                                    {
                                        Vector3 lPosition = lScale.transform.localPosition;
                                        Vector3 rPosition = rScale.transform.localPosition;
                                        lPosition.y = 0.1f;
                                        rPosition.y = 0.1f;
                                        lScale.transform.localPosition = lPosition;
                                        rScale.transform.localPosition = rPosition;
                                    }
                                    else
                                    {
                                        Vector3 lPosition = lScale.transform.localPosition;
                                        Vector3 rPosition = rScale.transform.localPosition;
                                        lPosition.y = 0.1f;
                                        lPosition.z = 18;
                                        rPosition.y = 0.1f;
                                        rPosition.z = 18;
                                        lScale.transform.localPosition = lPosition;
                                        rScale.transform.localPosition = rPosition;
                                        lineSet.transform.localPosition = new Vector3(0, 0, 36f);
                                    }                                    
                                }
                                lScale.transform.localScale = new Vector3(1f, 1f, 1f);
                                rScale.transform.localScale = new Vector3(1f, 1f, 1f);
                                Object.Destroy(pendulumSet, 4f);
                            }
                        }
                    }
                }
                Sleep(targetTime);
                break;
            case GameMessage.Draw:
                //Debug.Log("GameMessage.Draw");
                player = localPlayer(r.ReadByte());
                int _phase = r.ReadUInt16();
                if (GameStringHelper.differ(_phase, (long)DuelPhase.Draw) && turns != 0)
                    VoiceHandler.PlayFixedLine((uint)player, "Draw");
                UIHelper.playSound("draw", 1);
                realize();
                Sleep(10);
                break;
            case GameMessage.PayLpCost:
                //Debug.Log("GameMessage.PayLpCost");
                gameInfo.realize();
                player = localPlayer(r.ReadByte());
                VoiceHandler.PlayFixedLine((uint)player, "PayCost");
                break;
            case GameMessage.Damage:
                //Debug.Log("GameMessage.Damage");
                gameInfo.realize();
                player = localPlayer(r.ReadByte());
                val = r.ReadInt32();

                if (hasPlayedDamageSE == false)
                    UIHelper.playSound("SE_DUEL/SE_COST_DAMAGE", 1f);
                else
                    hasPlayedDamageSE = false;

                if (val >= 3000)
                    VoiceHandler.PlayFixedLine((uint)player, "GetHugeDamage");
                else
                    VoiceHandler.PlayFixedLine((uint)player, "GetDamage");
                gameField.animation_show_lp_num(player, false, val);
                if (Program.I().setting.setting.Vdamage.value) gameField.animation_screen_blood(player, val);
                Sleep(60);
                break;
            case GameMessage.Recover:
                //Debug.Log("GameMessage.Recover");
                gameInfo.realize();
                player = localPlayer(r.ReadByte());
                val = r.ReadInt32();
                UIHelper.playSound("gainlp", 1f);
                gameField.animation_show_lp_num(player, true, val);
                Sleep(60);
                break;
            case GameMessage.CardTarget:
            case GameMessage.Equip:
                //Debug.Log("GameMessage.Equip");
                realize();
                from = r.ReadGPS();
                to = r.ReadGPS();
                var card_from = GCS_cardGet(from, false);
                var card_to = GCS_cardGet(to, false);
                if (card_from != null)
                {
                    UIHelper.playSound("equip", 1f);
                    if (Program.I().setting.setting.Veqquip.value)
                        card_from.fast_decoration(Program.I().mod_ocgcore_decoration_magic_zhuangbei);
                }

                break;
            case GameMessage.LpUpdate:
                //Debug.Log("GameMessage.LpUpdate");
                gameInfo.realize();
                break;
            case GameMessage.CancelTarget:
            case GameMessage.Unequip:
                //Debug.Log("GameMessage.Unequip");
                realize();
                break;
            case GameMessage.AddCounter:
                //Debug.Log("GameMessage.AddCounter");
                cctype = r.ReadUInt16();
                gps = r.ReadShortGPS();
                card = GCS_cardGet(gps, false);
                count = r.ReadUInt16();
                var name2 = GameStringManager.get("counter", cctype);

                if (card != null)
                    for (var i = 0; i < count; i++)
                    {
                        UIHelper.playSound("SE_DUEL/SE_CARD_COUNTER", 1);
                        //if (Program.YGOPro1 == false)
                        {
                            var pos = UIHelper.get_close(card.gameObject.transform.position, Program.I().main_camera,
                                5);
                            Object.Destroy(Object.Instantiate(Program.I().mod_ocgcore_cs_end, pos, Quaternion.identity),
                                5f);
                        }
                    }

                RMSshow_none(card.get_data().Name + "  " + InterString.Get("增加指示物：[?]", name2) + " *" + count);
                Sleep(10);
                break;
            case GameMessage.RemoveCounter:
                //Debug.Log("GameMessage.RemoveCounter");
                cctype = r.ReadUInt16();
                gps = r.ReadShortGPS();
                card = GCS_cardGet(gps, false);
                count = r.ReadUInt16();
                var name = GameStringManager.get("counter", cctype);
                if (card != null)
                    for (var i = 0; i < count; i++)
                    {
                        UIHelper.playSound("removecounter", 1);
                        //if (Program.YGOPro1 == false)
                        {
                            var pos = UIHelper.get_close(card.gameObject.transform.position, Program.I().main_camera,
                                5);
                            Object.Destroy(Object.Instantiate(Program.I().mod_ocgcore_cs_end, pos, Quaternion.identity),
                                5f);
                        }
                    }

                RMSshow_none(card.get_data().Name + "  " + InterString.Get("减少指示物：[?]", name) + " *" + count);
                Sleep(10);
                break;
            case GameMessage.Attack:
                //Debug.Log("GameMessage.Attack");
                UIHelper.playSound("attack", 1);
                var p1 = r.ReadGPS();
                var p2 = r.ReadGPS();
                VectorAttackCard = get_point_worldposition(p1);
                VectorAttackTarget = Vector3.zero;

                var attackcard = GCS_cardGet(p1, false);
                if (attackcard != null)
                {
                    if (Program.I().setting.setting.Voice.value)
                    {
                        int cardId = attackcard.get_data().Alias == 0 ? attackcard.get_data().Id : attackcard.get_data().Alias;
                        var clip = VoiceHandler.GetClip("chants/attack/" + cardId);
                        if (clip != null)
                        {
                            VoiceHandler.voiceTime = clip.length;
                            Sleep((int)(clip.length * 60));
                            VoiceHandler.PlayVoice(clip, attackcard.p.controller == 0, attackcard.get_data().Name + " 攻击");
                        }
                        else
                        {
                            if(!VoiceHandler.PlayCardLine(p1.controller, "attack", attackcard, true))
                                VoiceHandler.PlayFixedLine(p1.controller, "Attack", true, attackcard.get_data().Name);
                        }
                    }
                    else
                        VoiceHandler.voiceTime = 0f;

                }
                if (p2.location == 0)
                {
                    var attacker_bool_me = p1.controller == 0;
                    if (!attacker_bool_me)
                    {
                        VectorAttackTarget = new Vector3(0, 3, -5f -7f * Program.fieldSize);//new Vector3(0, 3, -5f - 15f * Program.fieldSize);
                    }
                    else
                    {
                        if (gameField.isLong)
                            VectorAttackTarget = new Vector3(0, 3, 2f + (19f + gameField.delat) * Program.fieldSize);
                        else
                            VectorAttackTarget = new Vector3(0, 3, 2f + 9f * Program.fieldSize);//new Vector3(0, 3, 2f + 19f * Program.fieldSize);
                    }
                }
                else
                {
                    VectorAttackTarget = get_point_worldposition(p2);
                }

                Arrow.speed = 10;
                Arrow.updateSpeed();
                Sleep(40);


                //shiftArrow(VectorAttackCard, VectorAttackTarget, true, 50);
                //Program.notGo(removeAttackHandler);
                //Program.go(666, removeAttackHandler);


                if (Program.I().setting.setting.Vbattle.value == false)
                {
                    shiftArrow(VectorAttackCard, VectorAttackTarget, true, 50);
                    Program.notGo(removeAttackHandler);
                    Program.go(666, removeAttackHandler);
                }
                else
                {
                    shiftArrow(VectorAttackCard, VectorAttackTarget, true, 200);
                    Program.notGo(removeAttackHandler);
                    Program.go(800, removeAttackHandler);
                }


                //if (Program.I().setting.setting.Vbattle.value == false)
                //{
                //    Arrow.speed = 10;
                //    Arrow.updateSpeed();
                //    Sleep(40);
                //    shiftArrow(VectorAttackCard, VectorAttackTarget, true,50);
                //    Program.notGo(removeAttackHandler);
                //    Program.go(666, removeAttackHandler);
                //}
                //else
                //{
                //    Arrow.speed = 5;
                //    Arrow.updateSpeed();
                //    shiftArrow(VectorAttackCard, VectorAttackTarget, true, 200);
                //    //Program.notGo(removeAttackHandler);
                //    //Program.go(1000, removeAttackHandler);
                //}
                break;
            case GameMessage.Battle:
                //Debug.Log("GameMessage.Battle");
                AnimationControl ac = GameObject.Find("new_gameField(Clone)/Assets_Loader").GetComponent<AnimationControl>();
                if (Program.I().setting.setting.Vbattle.value)
                {
                    removeAttackHandler();
                    var gpsAttacker = r.ReadShortGPS();
                    r.ReadByte();
                    var attackCard = GCS_cardGet(gpsAttacker, false);
                    if (attackCard != null)
                    {
                        var data2 = attackCard.get_data();
                        data2.Attack = r.ReadInt32();
                        data2.Defense = r.ReadInt32();
                        attackCard.set_data(data2);
                    }
                    else
                    {
                        r.ReadInt32();
                        r.ReadInt32();
                    }
                    //mark Battle
                    if (attackCard != null)
                    {
                        if (!Ocgcore.inSkiping)
                        {
                            if (attackCard.p.controller == 0) AnimationControl.PlayAnimation(LoadAssets.mate1_, "Attack");
                            else AnimationControl.PlayAnimation(LoadAssets.mate2_, "Attack");
                        }
                    }
                    
                    r.ReadByte();
                    var gpsAttacked = r.ReadShortGPS();
                    r.ReadByte();
                    var attackedCard = GCS_cardGet(gpsAttacked, false);
                    if (attackedCard != null && gpsAttacked.location != 0)
                    {
                        var data2 = attackedCard.get_data();
                        data2.Attack = r.ReadInt32();
                        data2.Defense = r.ReadInt32();
                        attackedCard.set_data(data2);
                    }
                    else
                    {
                        r.ReadInt32();
                        r.ReadInt32();
                    }

                    r.ReadByte();

                    VectorAttackCard = get_point_worldposition(gpsAttacker);
                    if (attackedCard == null || gpsAttacked.location == 0)
                    {
                        var attacker_bool_me = gpsAttacker.controller == 0;
                        if (attacker_bool_me)
                            VectorAttackTarget = new Vector3(0, 15, 25);//mark 玩家被攻击位置
                        else
                            VectorAttackTarget = new Vector3(0, 15, -25);
                    }
                    else
                    {
                        VectorAttackTarget = get_point_worldposition(gpsAttacked);
                    }

                    GameObject fx = null;
                    string hit = "无";
                    string sound1 = "无";
                    string sound2 = "无";
                    if (attackCard.get_data().Attack >= 2000)
                    {
                        if ((attackCard.get_data().Attribute & (uint)CardAttribute.Dark) > 0)
                        {
                            fx = ABLoader.LoadABFromFile("effects/attack/fxp_atkdak_s2_001");
                            hit = "effects/hit/fxp_hitdak_s2_001";
                            sound1 = "SE_DUEL/SE_ATTACK_A_DARK_SPECIAL_01";
                            sound2 = "SE_DUEL/SE_ATTACK_A_DARK_SPECIAL_02";
                        }
                        else if ((attackCard.get_data().Attribute & (uint)CardAttribute.Divine) > 0)
                        {
                            fx = ABLoader.LoadABFromFile("effects/attack/fxp_atklit_s2_001");
                            hit = "effects/hit/fxp_hitlit_s2_001";
                            sound1 = "SE_DUEL/SE_ATTACK_A_DIVINE_SPECIAL_01";
                            sound2 = "SE_DUEL/SE_ATTACK_A_DIVINE_SPECIAL_02";
                        }
                        else if ((attackCard.get_data().Attribute & (uint)CardAttribute.Earth) > 0)
                        {
                            fx = ABLoader.LoadABFromFile("effects/attack/fxp_atkeah_s2_001");
                            hit = "effects/hit/fxp_hiteah_s2_001";
                            sound1 = "SE_DUEL/SE_ATTACK_A_EARTH_SPECIAL_01";
                            sound2 = "SE_DUEL/SE_ATTACK_A_EARTH_SPECIAL_02";
                        }
                        else if ((attackCard.get_data().Attribute & (uint)CardAttribute.Fire) > 0)
                        {
                            fx = ABLoader.LoadABFromFile("effects/attack/fxp_atkfie_s2_001");
                            hit = "effects/hit/fxp_hitfie_s2_001";
                            sound1 = "SE_DUEL/SE_ATTACK_A_FIRE_SPECIAL_01";
                            sound2 = "SE_DUEL/SE_ATTACK_A_FIRE_SPECIAL_02";
                        }
                        else if ((attackCard.get_data().Attribute & (uint)CardAttribute.Light) > 0)
                        {
                            fx = ABLoader.LoadABFromFile("effects/attack/fxp_atklit_s2_001");
                            hit = "effects/hit/fxp_hitlit_s2_001";
                            sound1 = "SE_DUEL/SE_ATTACK_A_LIGHT_SPECIAL_01";
                            sound2 = "SE_DUEL/SE_ATTACK_A_LIGHT_SPECIAL_02";
                        }
                        else if ((attackCard.get_data().Attribute & (uint)CardAttribute.Water) > 0)
                        {
                            fx = ABLoader.LoadABFromFile("effects/attack/fxp_atkwtr_s2_001");
                            hit = "effects/hit/fxp_hitwtr_s2_001";
                            sound1 = "SE_DUEL/SE_ATTACK_A_WIND_SPECIAL_01";
                            sound2 = "SE_DUEL/SE_ATTACK_A_WIND_SPECIAL_02";
                        }
                        else if ((attackCard.get_data().Attribute & (uint)CardAttribute.Wind) > 0)
                        {
                            fx = ABLoader.LoadABFromFile("effects/attack/fxp_atkwid_s2_001");
                            hit = "effects/hit/fxp_hitwid_s2_001";
                            sound1 = "SE_DUEL/SE_ATTACK_A_DARK_SPECIAL_01";
                            sound2 = "SE_DUEL/SE_ATTACK_A_DARK_SPECIAL_02";
                        }
                    }
                    else
                    {
                        if ((attackCard.get_data().Attribute & (uint)CardAttribute.Dark) > 0)
                        {
                            fx = ABLoader.LoadABFromFile("effects/attack/fxp_atkdak_s1_001");
                            hit = "effects/hit/fxp_hitdak_s1_001";
                            sound1 = "SE_DUEL/SE_ATTACK_A_DARK_DEFAULT_01";
                            sound2 = "SE_DUEL/SE_ATTACK_A_DARK_DEFAULT_02";
                        }
                        else if ((attackCard.get_data().Attribute & (uint)CardAttribute.Divine) > 0)
                        {
                            fx = ABLoader.LoadABFromFile("effects/attack/fxp_atklit_s1_001");
                            hit = "effects/hit/fxp_hitlit_s1_001";
                            sound1 = "SE_DUEL/SE_ATTACK_A_DIVINE_DEFAULT_01";
                            sound2 = "SE_DUEL/SE_ATTACK_A_DIVINE_DEFAULT_02";
                        }
                        else if ((attackCard.get_data().Attribute & (uint)CardAttribute.Earth) > 0)
                        {
                            fx = ABLoader.LoadABFromFile("effects/attack/fxp_atkeah_s1_001");
                            hit = "effects/hit/fxp_hiteah_s1_001";
                            sound1 = "SE_DUEL/SE_ATTACK_A_EARTH_DEFAULT_01";
                            sound2 = "SE_DUEL/SE_ATTACK_A_EARTH_DEFAULT_02";
                        }
                        else if ((attackCard.get_data().Attribute & (uint)CardAttribute.Fire) > 0)
                        {
                            fx = ABLoader.LoadABFromFile("effects/attack/fxp_atkfie_s1_001");
                            hit = "effects/hit/fxp_hitfie_s1_001";
                            sound1 = "SE_DUEL/SE_ATTACK_A_FIRE_DEFAULT_01";
                            sound2 = "SE_DUEL/SE_ATTACK_A_FIRE_DEFAULT_02";
                        }
                        else if ((attackCard.get_data().Attribute & (uint)CardAttribute.Light) > 0)
                        {
                            fx = ABLoader.LoadABFromFile("effects/attack/fxp_atklit_s1_001");
                            hit = "effects/hit/fxp_hitlit_s1_001";
                            sound1 = "SE_DUEL/SE_ATTACK_A_LIGHT_DEFAULT_01";
                            sound2 = "SE_DUEL/SE_ATTACK_A_LIGHT_DEFAULT_02";
                        }
                        else if ((attackCard.get_data().Attribute & (uint)CardAttribute.Water) > 0)
                        {
                            fx = ABLoader.LoadABFromFile("effects/attack/fxp_atkwtr_s1_001");
                            hit = "effects/hit/fxp_hitwtr_s1_001";
                            sound1 = "SE_DUEL/SE_ATTACK_A_WIND_DEFAULT_01";
                            sound2 = "SE_DUEL/SE_ATTACK_A_WIND_DEFAULT_02";
                        }
                        else if ((attackCard.get_data().Attribute & (uint)CardAttribute.Wind) > 0)
                        {
                            fx = ABLoader.LoadABFromFile("effects/attack/fxp_atkwid_s1_001");
                            hit = "effects/hit/fxp_hitwid_s1_001";
                            sound1 = "SE_DUEL/SE_ATTACK_A_DARK_DEFAULT_01";
                            sound2 = "SE_DUEL/SE_ATTACK_A_DARK_DEFAULT_02";
                        }
                    }

                    Vector3 v = VectorAttackTarget - VectorAttackCard;
                    v.y = 0;
                    Vector3 originalAngle = attackCard.gameObject.transform.eulerAngles;
                    int directAttack = 0;
                    if(attackedCard != null)
                    {
                        attackCard.gameObject.transform.LookAt(attackedCard.gameObject.transform);
                        if ((attackedCard.p.position & (uint)CardPosition.FaceUpDefence) > 0)
                            if (attackedCard.get_data().Defense >= attackCard.get_data().Attack)
                            {
                                hit = "effects/hit/fxp_hit_guard_001";
                                sound2 = "SE_DUEL/SE_ATTACK_GUARD";
                            }
                        if ((attackedCard.p.position & (uint)CardPosition.FaceUpAttack) > 0)
                            if (attackedCard.get_data().Attack >= attackCard.get_data().Attack)
                            {
                                hit = "effects/hit/fxp_hit_guard_001";
                                sound2 = "SE_DUEL/SE_ATTACK_GUARD";
                            }
                    }
                    else
                    {
                        GameObject dummy = new GameObject();
                        if(attackCard.p.controller == 0)
                        {
                            dummy.transform.position = new Vector3(0f, 15f, 25f);
                            hit = "effects/hit/fxp_dithit_far_001";
                            sound2 = "SE_DUEL/SE_DIRECT_ATTACK_PLAYER";
                            directAttack = 1;
                        }
                        else
                        {
                            dummy.transform.position = new Vector3(0f, 15f, -25f);
                            hit = "effects/hit/fxp_dithit_near_001";
                            sound2 = "SE_DUEL/SE_DIRECT_ATTACK_RIVAL";
                            directAttack= 2;
                        }
                        attackCard.gameObject.transform.LookAt(dummy.transform);
                        Object.Destroy(dummy);
                    }

                    fx.transform.parent = attackCard.gameObject.transform;
                    fx.transform.localPosition = Vector3.zero;
                    fx.transform.localEulerAngles = Vector3.zero;
                    fx.SetActive(false);

                    Vector3 faceAngle = attackCard.gameObject.transform.eulerAngles;
                    attackCard.gameObject.transform.eulerAngles = originalAngle;

                    attackCard.inAnimation = true;
                    iTween.Pause(attackCard.gameObject);
                    Sequence quence = DOTween.Sequence();

                    if(attackCard.get_data().Attack < 2000)
                    {
                        quence.Append(attackCard.gameObject.transform.DOMove(VectorAttackCard + new Vector3(0f, 10f, 0f) - v * 0.3f, 0.3f).SetEase(Ease.InOutCubic).OnComplete(() => 
                            { fx.SetActive(true);
                                foreach (Transform t in fx.GetComponentsInChildren<Transform>(true))
                                    t.gameObject.SetActive(true);
                            }));
                        quence.Join(attackCard.gameObject.transform.DORotate(faceAngle, 0.3f).SetEase(Ease.InOutCubic));
                        quence.Append(attackCard.gameObject.transform.DOMove(VectorAttackCard + (VectorAttackTarget - VectorAttackCard) * 0.8f + new Vector3(0f, 5f, 0f), 0.1f).SetEase(Ease.InSine));
                        if (directAttack == 1)
                            quence.Join(Program.I().main_camera.transform.DOMoveZ(Program.I().main_camera.transform.position.z + 5, 0.1f));
                        else if (directAttack == 2)
                            quence.Join(Program.I().main_camera.transform.DOMoveZ(Program.I().main_camera.transform.position.z - 5, 0.1f));
                        quence.AppendCallback(() => CameraControl.ShakeCameraNowSmall());
                        quence.AppendInterval(0.3f);
                        quence.Append(attackCard.gameObject.transform.DOMove(VectorAttackCard, 0.3f).SetEase(Ease.InQuad).OnComplete(() => { attackCard.inAnimation = false; }));
                        quence.Insert(0.8f, attackCard.gameObject.transform.DORotate(originalAngle, 0.2f).SetEase(Ease.InQuad));

                        LoadSFX.DelayDecoration(VectorAttackTarget + new Vector3(0f, 5f, 0f), 0, hit, sound2, 0.4f, true);
                        if (attackCard.gameObject.transform.Find("mod_simple_quad(Clone)") != null)
                        {
                            CloseUpControl cc = attackCard.gameObject.transform.Find("mod_simple_quad(Clone)").GetChild(0).GetComponent<CloseUpControl>();
                            cc.FadeOutIn(0f, 0.3f, 0.5f, 0.2f);
                        }
                        if (attackedCard != null
                            &&
                            attackedCard.gameObject.transform.Find("mod_simple_quad(Clone)") != null)
                        {
                            CloseUpControl cc = attackedCard.gameObject.transform.Find("mod_simple_quad(Clone)").GetChild(0).GetComponent<CloseUpControl>();
                            cc.FadeOutIn(0.3f, 0.1f, 1f, 0.2f);
                        }
                        Sleep(24);
                    }
                    else
                    {
                        quence.Append(attackCard.gameObject.transform.DOMove(VectorAttackCard + new Vector3(0f, 10f, 0f) - v * 0.4f, 0.5f).SetEase(Ease.InOutCubic));
                        quence.Join(attackCard.gameObject.transform.DORotate(faceAngle + new Vector3(45f, 0f, 0f), 0.5f).SetEase(Ease.InOutCubic));
                        quence.InsertCallback(0.4f, () => 
                        {
                            fx.SetActive(true);
                            foreach (Transform t in fx.GetComponentsInChildren<Transform>(true))
                                t.gameObject.SetActive(true);
                        });
                        quence.Append(attackCard.gameObject.transform.DOMove(VectorAttackCard + (VectorAttackTarget - VectorAttackCard) * 0.8f + new Vector3(0f, 5f, 0f), 0.15f).SetEase(Ease.InSine));
                        quence.Join(attackCard.gameObject.transform.DORotate(faceAngle, 0.1f));
                        if (directAttack == 1)
                            quence.Join(Program.I().main_camera.transform.DOMoveZ(Program.I().main_camera.transform.position.z + 5, 0.15f));
                        else if (directAttack == 2)
                            quence.Join(Program.I().main_camera.transform.DOMoveZ(Program.I().main_camera.transform.position.z - 5, 0.15f));
                        quence.AppendCallback(() => CameraControl.ShakeCameraNowBig());
                        quence.AppendInterval(0.3f);
                        quence.Append(attackCard.gameObject.transform.DOMove(VectorAttackCard, 0.3f).SetEase(Ease.InQuad).OnComplete(() => { attackCard.inAnimation = false; }));
                        quence.Join(attackCard.gameObject.transform.DORotate(originalAngle, 0.3f).SetEase(Ease.InQuad));

                        LoadSFX.DelayDecoration(VectorAttackTarget + new Vector3(0f, 5f, 0f), 0, hit, sound2, 0.65f, true);
                        if (attackCard.gameObject.transform.Find("mod_simple_quad(Clone)") != null)
                        {
                            CloseUpControl cc = attackCard.gameObject.transform.Find("mod_simple_quad(Clone)").GetChild(0).GetComponent<CloseUpControl>();
                            cc.FadeOutIn(0f, 0.5f, 0.55f, 0.2f);
                        }
                        if (attackedCard != null
                            &&
                            attackedCard.gameObject.transform.Find("mod_simple_quad(Clone)") != null)
                        {
                            CloseUpControl cc = attackedCard.gameObject.transform.Find("mod_simple_quad(Clone)").GetChild(0).GetComponent<CloseUpControl>();
                            cc.FadeOutIn(0.5f, 0.1f, 1f, 0.2f);
                        }
                        Sleep(40);
                    }

                    SEHandler.Play(sound1);
                    if (sound2.Contains("SE_ATTACK_GUARD"))
                        hasPlayedDamageSE = false;
                    else
                        hasPlayedDamageSE = true;
                    Object.Destroy(fx, 5f);

                }

                break;
            case GameMessage.AttackDisabled:
                //Debug.Log("GameMessage.AttackDisabled");
                //removeAttackHandler();
                break;
            case GameMessage.DamageStepStart:
                //Debug.Log("GameMessage.DamageStepStart");
                break;
            case GameMessage.DamageStepEnd:
                //Debug.Log("GameMessage.DamageStepEnd");
                break;
            case GameMessage.BeChainTarget:
                //Debug.Log("GameMessage.BeChainTarget");
                break;
            case GameMessage.CreateRelation:
                //Debug.Log("GameMessage.CreateRelation");
                break;
            case GameMessage.ReleaseRelation:
                //Debug.Log("GameMessage.ReleaseRelation");
                break;
            case GameMessage.TossCoin:
                //Debug.Log("GameMessage.TossCoin");
                player = r.ReadByte();
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    data = r.ReadByte();
                    if (i == 0)
                    {
                        tempobj = create_s(Program.I().mod_ocgcore_coin);
                        tempobj.AddComponent<animation_screen_lock>().screen_point =
                            new Vector3(getScreenCenter(), Screen.height / 2, 1);
                        tempobj.GetComponent<coiner>().coin_app();
                        if (data == 0)
                            tempobj.GetComponent<coiner>().tocoin(false);
                        else
                            tempobj.GetComponent<coiner>().tocoin(true);
                        destroy(tempobj, 7);
                    }

                    if (data == 0)
                        RMSshow_none(InterString.Get("硬币反面"));
                    else
                        RMSshow_none(InterString.Get("硬币正面"));
                }

                Sleep(280);
                break;
            case GameMessage.TossDice:
                //Debug.Log("GameMessage.TossDice");
                player = r.ReadByte();
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    data = r.ReadByte();
                    if (i == 0)
                    {
                        //tempobj = create_s(Program.I().mod_ocgcore_dice);
                        //tempobj.AddComponent<animation_screen_lock>().screen_point =
                        //    new Vector3(getScreenCenter(), Screen.height / 2, 3);
                        //tempobj.GetComponent<coiner>().dice_app();
                        //tempobj.GetComponent<coiner>().todice(data);
                        tempobj = ABLoader.LoadABFromFile("effects/playdice");
                        ABLoader.ChangeLayer(tempobj, "fx_3d");
                        switch (data)
                        {
                            case 1: tempobj.transform.GetChild(0).GetChild(0).GetChild(0).localEulerAngles = new Vector3(90, 0, 0);
                                break;
                            case 2:
                                tempobj.transform.GetChild(0).GetChild(0).GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
                                break;
                            case 3:
                                tempobj.transform.GetChild(0).GetChild(0).GetChild(0).localEulerAngles = new Vector3(0, 90, 0);
                                break;
                            case 4:
                                tempobj.transform.GetChild(0).GetChild(0).GetChild(0).localEulerAngles = new Vector3(0, -90, 0);
                                break;
                            case 5:
                                tempobj.transform.GetChild(0).GetChild(0).GetChild(0).localEulerAngles = new Vector3(0, 180, 0);
                                break;
                            case 6:
                                tempobj.transform.GetChild(0).GetChild(0).GetChild(0).localEulerAngles = new Vector3(0, -90, 90);
                                break;
                        }
                        UIHelper.playSound("SE_DUEL/SE_DICE_ROLL", 1f);
                        destroy(tempobj, 3.4f);
                    }
                    RMSshow_none(InterString.Get("骰子结果：[?]", data.ToString()));
                }

                Sleep(200);
                break;
            case GameMessage.AnnounceRace:
                //Debug.Log("GameMessage.AnnounceRace");
                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(60);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                player = localPlayer(r.ReadByte());
                ES_min = r.ReadByte();
                available = r.ReadUInt32();
                values = new List<messageSystemValue>();
                for (var i = 0; i < 26; i++)
                    if ((available & (1 << i)) > 0)
                        values.Add(new messageSystemValue
                            {hint = GameStringManager.get_unsafe(1020 + i), value = (1 << i).ToString()});
                RMSshow_multipleChoice("returnMultiple", ES_min, values);
                break;
            case GameMessage.AnnounceAttrib:
                //Debug.Log("GameMessage.AnnounceAttrib");
                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(60);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                player = localPlayer(r.ReadByte());
                ES_min = r.ReadByte();
                available = r.ReadUInt32();
                values = new List<messageSystemValue>();
                for (var i = 0; i < 7; i++)
                    if ((available & (1 << i)) > 0)
                        values.Add(new messageSystemValue
                            {hint = GameStringManager.get_unsafe(1010 + i), value = (1 << i).ToString()});
                RMSshow_multipleChoice("returnMultiple", ES_min, values);
                break;
            case GameMessage.AnnounceCard:
                //Debug.Log("GameMessage.AnnounceCard");
                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(60);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                player = localPlayer(r.ReadByte());
                ES_searchCode.Clear();
                count = r.ReadByte();
                for (var i = 0; i < count; i++)
                {
                    var take = r.ReadInt32();
                    ES_searchCode.Add(take);
                }

                //values = new List<messageSystemValue>();
                //values.Add(new messageSystemValue { value = "", hint = "" });
                //ES_RMS("AnnounceCard", values);
                RMSshow_input("AnnounceCard", InterString.Get("请输入关键字："), "");
                break;
            case GameMessage.AnnounceNumber:
                //Debug.Log("GameMessage.AnnounceNumber");
                if (inIgnoranceReplay() || inTheWorld()) break;
                if (condition == Condition.record) Sleep(60);
                //destroy(waitObject, 0, false, true);
                PlayableGuide.state = 1;
                player = localPlayer(r.ReadByte());
                count = r.ReadByte();
                ES_min = 1;
                values = new List<messageSystemValue>();
                for (var i = 0; i < count; i++)
                    values.Add(new messageSystemValue {hint = r.ReadUInt32().ToString(), value = i.ToString()});
                RMSshow_multipleChoice("return", 1, values);
                break;
            case GameMessage.PlayerHint:
                //Debug.Log("GameMessage.PlayerHint");
                player = localPlayer(r.ReadByte());
                int ptype = r.ReadByte();
                var pvalue = r.ReadInt32();
                var valstring = GameStringManager.get(pvalue);
                if (pvalue == 38723936)
                {
                    valstring = InterString.Get("不能确认墓地里的卡");
                    if (player == 0)
                    {
                        if (ptype == 6)
                        {
                            clearAllShowed();
                            Program.I().cardDescription.setData(CardsManager.Get(38723936), GameTextureManager.opBack,
                                "", true);
                            cantCheckGrave = true;
                        }

                        if (ptype == 7)
                            cantCheckGrave = false;
                    }
                }

                if (ptype == 6)
                {
                    if (player == 0)
                        RMSshow_none(InterString.Get("我方状态：[?]", valstring));
                    else
                        RMSshow_none(InterString.Get("对方状态：[?]", valstring));
                }
                else if (ptype == 7)
                {
                    if (player == 0)
                        RMSshow_none(InterString.Get("我方取消状态：[?]", valstring));
                    else
                        RMSshow_none(InterString.Get("对方取消状态：[?]", valstring));
                }

                break;
            case GameMessage.CardHint:
                //Debug.Log("GameMessage.CardHint");
                var game_card = GCS_cardGet(r.ReadGPS(), false);
                int ctype = r.ReadByte();
                var value = r.ReadInt32();
                if (game_card != null)
                    if (ctype == 1)
                    {
                        animation_confirm(game_card);
                        var number = game_card.add_one_decoration(Program.I().mod_ocgcore_number, 3,
                            new Vector3(70, 0, 0), "number", false);
                        number.game_object.GetComponent<number_loader>().set_number(value, 3);
                        number.scale_change_ignored = true;
                        number.game_object.transform.localScale = new Vector3(1, 1, 1);
                        number.game_object.transform.eulerAngles = new Vector3(70, 0, 0);
                        destroy(number.game_object, 1f);
                        Sleep(42);
                    }

                break;
            case GameMessage.TagSwap:
                //Debug.Log("GameMessage.TagSwap");
                realize(true);
                arrangeCards();
                player = localPlayer(r.ReadByte());
                animation_suffleHand(player);
                Sleep(21);
                break;
            case GameMessage.AiName:
                //Debug.Log("GameMessage.AiName");
                break;
            case GameMessage.MatchKill:
                //Debug.Log("GameMessage.MatchKill");
                break;
            case GameMessage.CustomMsg:
                //Debug.Log("GameMessage.CustomMsg");
                break;
            case GameMessage.DuelWinner:
                //Debug.Log("GameMessage.DuelWinner");
                break;
        }

        r.BaseStream.Seek(0, 0);
    }

    private void createPlaceSelector(byte[] resp)
    {
        for (var i = 0; i < placeSelectors.Count; i++)
            if (placeSelectors[i].data[0] == resp[0])
                if (placeSelectors[i].data[1] == resp[1])
                    if (placeSelectors[i].data[2] == resp[2])
                        return;
        var player_m = (uint) localPlayer(resp[0]);
        uint location = resp[1];
        uint index = resp[2];
        var newP = new GPS();
        newP.controller = player_m;
        newP.location = location;
        newP.sequence = index;
        newP.position = 0;
        var worldVector = get_point_worldposition(newP);
        var placs = create(Program.I().New_ocgcore_placeSelector, worldVector, Vector3.zero, false, null, true,
            Vector3.one).GetComponent<placeSelector>();
        placs.data = new byte[3];
        placs.data[0] = resp[0];
        placs.data[1] = resp[1];
        placs.data[2] = resp[2];
        placeSelectors.Add(placs);
        if (location == (uint) CardLocation.MonsterZone && Program.I().setting.setting.hand.value == false)
            ES_placeSelected(placs);
        if (location == (uint) CardLocation.SpellZone && Program.I().setting.setting.handm.value == false)
            ES_placeSelected(placs);
    }

    private void animation_suffleHand(int player)
    {
        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if ((cards[i].p.location & (uint) CardLocation.Hand) > 0)
                    if (cards[i].p.controller == player)
                    {
                        Vector3 position;
                        if (cards[i].p.controller == 0)
                        {
                            position = new Vector3( 0f, 20f, -27.5f);//mark 洗牌
                            cards[i].animation_rush_to(position, new Vector3(-20, 0, 180));
                        }
                        else
                        {
                            position = new Vector3(0, 12, 20f);
                            cards[i].animation_rush_to(position, new Vector3(20, 180, 180));
                        }
                    }
    }

    private void clearChainEnd()
    {
        //removeAttackHandler();
        removeSelectedAnimations();        
        
    }

    private void logicalClearChain()
    {
        for (var i = 0; i < cardsInChain.Count; i++) cardsInChain[i].CS_clear();
        cardsInChain.Clear();
    }

    private void showWait()
    {
        PlayableGuide.state = 2;
        //if (waitObject == null)
        //{
        //    waitObject = create_s(Program.I().new_ocgcore_wait,
        //                                        Program.I().camera_main_2d.ScreenToWorldPoint(new Vector3(getScreenCenter(),
        //                                        Screen.height - 15f - 15f * (1.21f - Program.fieldSize) / 0.21f)), Vector3.zero, true,
        //                                        Program.I().ui_main_2d);
        //}
    }

    private void removeAttackHandler()
    {
        shiftArrow(Vector3.zero, Vector3.zero, false, 50);
    }

    private void removeSelectedAnimations()
    {
        for (var i = 0; i < cardsInSelectAnimation.Count; i++)
            try
            {
                cardsInSelectAnimation[i].del_all_decoration_by_string("selected");
                cardsInSelectAnimation[i].Highlight(false, false, false);
                cardsInSelectAnimation[i].currentKuang = gameCard.kuangType.none;
                foreach (var hiddenButton in gameField.gameHiddenButtons)
                    hiddenButton.CheckNotification();
                
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

        cardsInSelectAnimation.Clear();
    }


    private void confirm(gameCard card)
    {
        Program.go(cardsForConfirm.Count * 700, confirmGPS);
        cardsForConfirm.Add(card);
    }

    private void Nconfirm()
    {
        Program.notGo(confirmGPS);
        cardsForConfirm.Clear();
    }

    private void confirmGPS()
    {
        if (cardsForConfirm.Count > 0)
        {
            animation_confirm(cardsForConfirm[0]);
            cardsForConfirm.RemoveAt(0);
        }
    }

    private List<gameCard> MHS_getBundle(int controller, int location)
    {
        var cardsInLocation = new List<gameCard>();
        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if (cards[i].p.location == location)
                    if (cards[i].p.controller == controller)
                        cardsInLocation.Add(cards[i]);

        return cardsInLocation;
    }

    private void MHS_creatBundle(int count, int player, CardLocation location)
    {
        for (var i = 0; i < count; i++)
            GCS_cardCreate(new GPS
            {
                controller = (uint) player,
                location = (uint) location,
                position = (int) CardPosition.FaceDownAttack,
                sequence = (uint) i
            });
    }

    private List<gameCard> MHS_resizeBundle(int count, int player, CardLocation location)
    {
        var cardBow = new List<gameCard>();
        var waterOutOfBow = new List<gameCard>();
        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if ((cards[i].p.location & (uint) location) > 0)
                    if (cards[i].p.controller == player)
                    {
                        if (cardBow.Count < count)
                            cardBow.Add(cards[i]);
                        else
                            waterOutOfBow.Add(cards[i]);
                    }

        for (var i = 0; i < waterOutOfBow.Count; i++) waterOutOfBow[i].hide();
        while (cardBow.Count < count)
            cardBow.Add(GCS_cardCreate(new GPS
            {
                controller = (uint) player,
                location = (uint) location,
                position = (int) CardPosition.FaceDownAttack,
                sequence = (uint) cardBow.Count
            }));
        for (var i = 0; i < cardBow.Count; i++)
        {
            cardBow[i].erase_data();
            cardBow[i].p.position = (int) CardPosition.FaceDownAttack;
        }

        return cardBow;
    }

    public void realizeCardsForSelect(string hint = "")
    {
        for (var i = 0; i < allCardsInSelectMessage.Count; i++)
        {
            allCardsInSelectMessage[i].del_all_decoration();
            allCardsInSelectMessage[i].Highlight(false, false, false);

            allCardsInSelectMessage[i].isShowed = false;
            //allCardsInSelectMessage[i].show_number(0);
            allCardsInSelectMessage[i].currentFlash = gameCard.flashType.none;
        }
        foreach (var hiddenButton in gameField.gameHiddenButtons)
            hiddenButton.CheckNotification();

        cardsSelectable.Clear();

        getSelectableCards();

        if (cardsSelected.Count == 0)
            if (UIHelper.fromStringToBool(Config.Get("smartSelect_", "1")))
                switch (currentMessage)
                {
                    case GameMessage.SelectTribute:
                        if (cardsSelectable.Count == 1)
                        {
                            autoSendCards();
                            return;
                        }

                        var all = 0;
                        for (var i = 0; i < cardsSelectable.Count; i++) all += cardsSelectable[i].levelForSelect_1;
                        if (all == ES_min)
                        {
                            autoSendCards();
                            return;
                        }

                        break;
                    case GameMessage.SelectCard:
                        if (cardsSelectable.Count <= ES_min)
                        {
                            autoSendCards();
                            return;
                        }

                        if (ES_min == ES_max)
                            if (ifAllCardsInSameCode(cardsSelectable))
                                if (ifAllCardsInSameController(cardsSelectable))
                                    if (ifAllCardsInSameLocation(cardsSelectable))
                                    {
                                        autoSendCards();
                                        return;
                                    }

                        break;
                    case GameMessage.SelectSum:
                        Debug.Log("mark0");
                        if (cardsSelectable.Count <= ES_min)
                        {
                            autoSendCards();
                            return;
                        }
                        Debug.Log("mark1");

                        var allSame = true;
                        var selectableLevel = 0;
                        for (var x = 0; x < cardsMustBeSelected.Count; x++)
                            selectableLevel += cardsMustBeSelected[x].levelForSelect_1;
                        for (var x = 0; x < cardsSelectable.Count; x++)
                            selectableLevel += cardsSelectable[x].levelForSelect_1;
                        if (selectableLevel != ES_level) allSame = false;
                        selectableLevel = 0;
                        for (var x = 0; x < cardsMustBeSelected.Count; x++)
                            selectableLevel += cardsMustBeSelected[x].levelForSelect_2;
                        for (var x = 0; x < cardsSelectable.Count; x++)
                            selectableLevel += cardsSelectable[x].levelForSelect_2;
                        if (selectableLevel != ES_level) allSame = false;
                        if (allSame)
                        {
                            autoSendCards();
                            return;
                        }

                        break;
                }

        bool allOnFieldButNotOverlay = true;
        foreach (var card in cardsSelectable)
        {
            if ((card.p.location & (uint)CardLocation.Onfield) == 0)
            {
                allOnFieldButNotOverlay = false;
                break;
            }
            if ((card.p.location & (uint)CardLocation.Overlay) > 0)
            {
                allOnFieldButNotOverlay = false;
                break;
            }
        }
        if (!allOnFieldButNotOverlay)
            Program.I().cardSelection.show(cardsSelectable, false, ES_min, ES_max, hint);

        GameObject decoration = ABLoader.LoadABFromFile("effects/hitghlight/fxp_hl_select/fxp_hl_select_card_001");
        var main = decoration.GetComponent<ParticleSystem>().main;
        main.playOnAwake = true;
        for (var i = 0; i < cardsSelectable.Count; i++)
        {
            //cardsSelectable[i].isShowed = true;
            if ((cardsSelectable[i].p.location & (uint)CardLocation.Onfield) > 0 && (cardsSelectable[i].p.location & (uint)CardLocation.Overlay) == 0)
            {
                //cardsSelectable[i].add_one_decoration(Program.I().mod_ocgcore_decoration_card_selecting, 4, Vector3.zero, "card_selecting");
                if ((cardsSelectable[i].p.position & (uint)CardPosition.Attack) > 0)
                    decoration.transform.GetChild(0).localEulerAngles = new Vector3(0, 0, 0);
                else
                    decoration.transform.GetChild(0).localEulerAngles = new Vector3(0, 90, 0);
                if ((cardsSelectable[i].p.location & (uint)CardLocation.MonsterZone) > 0)
                    decoration.transform.GetChild(0).localScale = Vector3.one;
                else
                    decoration.transform.GetChild(0).localScale = new Vector3(0.8f, 1f, 0.8f);
                cardsSelectable[i].add_one_decoration(decoration, 4, Vector3.zero, "card_selecting");
            }
            cardsSelectable[i].currentFlash = gameCard.flashType.Select;
        }
        Object.Destroy(decoration);

        for (var x = 0; x < cardsMustBeSelected.Count; x++)
        {
            if (allOnFieldButNotOverlay)
            {
                if (currentMessage == GameMessage.SelectSum)
                    cardsMustBeSelected[x].show_number(cardsMustBeSelected[x].levelForSelect_2);
                else
                    cardsMustBeSelected[x].show_number(x + 1);
                cardsMustBeSelected[x].isShowed = true;
            }
        }

        for (var x = 0; x < cardsSelected.Count; x++)
        {
            if (allOnFieldButNotOverlay)
            {
                if (currentMessage == GameMessage.SelectSum)
                    cardsSelected[x].show_number(cardsSelected[x].levelForSelect_2);
                else
                    cardsSelected[x].show_number(x + 1);
                cardsSelected[x].isShowed = true;
            }
        }

        var sendable = false;
        var real_send = false;

        if (currentMessage == GameMessage.SelectSum)
        {
            if (cardsSelected.Count == ES_max) sendable = true;
            var selectedLevel = 0;
            for (var x = 0; x < cardsMustBeSelected.Count; x++)
                selectedLevel += cardsMustBeSelected[x].levelForSelect_1;
            for (var x = 0; x < cardsSelected.Count; x++) selectedLevel += cardsSelected[x].levelForSelect_1;
            if (ES_overFlow)
            {
                if (selectedLevel >= ES_level)
                {
                    sendable = true;
                    real_send = true;
                }
            }
            else
            {
                if (selectedLevel == ES_level) sendable = true;
            }

            selectedLevel = 0;
            for (var x = 0; x < cardsMustBeSelected.Count; x++)
                selectedLevel += cardsMustBeSelected[x].levelForSelect_2;
            for (var x = 0; x < cardsSelected.Count; x++) selectedLevel += cardsSelected[x].levelForSelect_2;
            if (ES_overFlow)
            {
                if (selectedLevel >= ES_level)
                {
                    sendable = true;
                    real_send = true;
                }
            }
            else
            {
                if (selectedLevel == ES_level) sendable = true;
            }

            if (cardsSelectable.Count == 0)
            {
                sendable = true;
                real_send = true;
            }
        }

        if (currentMessage == GameMessage.SelectCard)
        {
            if (cardsSelected.Count >= ES_min) sendable = true;
            if (cardsSelected.Count == ES_max || cardsSelected.Count == cardsSelectable.Count)
            {
                sendable = true;
                real_send = true;
            }
        }

        if (currentMessage == GameMessage.SelectTribute)
        {
            var all = 0;
            for (var i = 0; i < cardsSelected.Count; i++) all += cardsSelected[i].levelForSelect_1;
            if (all >= ES_min) sendable = true;
            if (all >= ES_max)
            {
                sendable = true;
                if (cardsSelectable.Count == 1) real_send = true;
            }

            if (cardsSelected.Count == cardsSelectable.Count)
            {
                sendable = true;
                real_send = true;
            }

            if (cardsSelected.Count == ES_max)
            {
                sendable = true;
                real_send = true;
            }
        }

        if (sendable)
        {
            if (real_send)
            {
                gameInfo.removeHashedButton("sendSelected");
                Program.I().ocgcore.gameField.btn_confirm.hide();
                Program.I().cardSelection.HideWithoutAction();
                sendSelectedCards();
            }
            else
            {
                if (gameInfo.queryHashedButton("sendSelected") == false)
                {
                    gameInfo.addHashedButton("sendSelected", 0, superButtonType.yes, InterString.Get("完成选择@ui"));
                    Program.I().ocgcore.gameField.ShowConfirmButton();
                }
            }
        }
        else if (currentMessage != GameMessage.SelectUnselect)
        {
            gameInfo.removeHashedButton("sendSelected");
            Program.I().ocgcore.gameField.btn_confirm.hide();
            Program.I().cardSelection.finishable = false;
        }


        realize();
        toNearest();
    }

    private void getSelectableCards()
    {
        if (currentMessage == GameMessage.SelectCard || currentMessage == GameMessage.SelectUnselect)
            for (var i = 0; i < allCardsInSelectMessage.Count; i++)
                cardsSelectable.Add(allCardsInSelectMessage[i]);
        if (currentMessage == GameMessage.SelectTribute)
            for (var i = 0; i < allCardsInSelectMessage.Count; i++)
                cardsSelectable.Add(allCardsInSelectMessage[i]);
        if (currentMessage == GameMessage.SelectSum)
        {
            var selectedLevel = 0;
            for (var x = 0; x < cardsMustBeSelected.Count; x++)
                selectedLevel += cardsMustBeSelected[x].levelForSelect_1;
            for (var x = 0; x < cardsSelected.Count; x++) selectedLevel += cardsSelected[x].levelForSelect_1;
            checkSum(selectedLevel);
            selectedLevel = 0;
            for (var x = 0; x < cardsMustBeSelected.Count; x++)
                selectedLevel += cardsMustBeSelected[x].levelForSelect_2;
            for (var x = 0; x < cardsSelected.Count; x++) selectedLevel += cardsSelected[x].levelForSelect_2;
            checkSum(selectedLevel);
        }
    }

    private static bool ifAllCardsInSameLocation(List<gameCard> cards)
    {
        var re = true;
        if (cards.Count > 0)
        {
            var loc = cards[0].p.location;
            if (loc != (uint) CardLocation.Deck) return false;
            for (var i = 0; i < cards.Count; i++)
                if (cards[i].p.location != loc)
                    re = false;
        }

        return re;
    }

    private static bool ifAllCardsInSameController(List<gameCard> cards)
    {
        var re = true;
        if (cards.Count > 0)
        {
            var con = cards[0].p.controller;
            for (var i = 0; i < cards.Count; i++)
                if (cards[i].p.controller != con)
                    re = false;
        }

        return re;
    }

    private static bool ifAllCardsInSameCode(List<gameCard> cards)
    {
        var re = true;
        if (cards.Count > 0)
        {
            var code = cards[0].get_data().Id;
            for (var i = 0; i < cards.Count; i++)
            {
                if (cards[i].get_data().Id != code) re = false;
                if (cards[i].get_data().Id == 0) re = false;
            }
        }

        return re;
    }

    public static List<List<gameCard>> GetCombination(List<gameCard> t, int n)
    //卡片全部放到t里面，n是小于selectMax的任意整数，返回卡片张数为n的卡片全组合
    {
        if (t.Count < n) return null;
        var temp = new int[n];
        var list = new List<List<gameCard>>();
        GetCombination(ref list, t, t.Count, n, temp, n);
        return list;
    }

    private static void GetCombination(ref List<List<gameCard>> list, List<gameCard> t, int n, int m, int[] b, int M)
    {
        for (var i = n; i >= m; i--)
        {
            b[m - 1] = i - 1;
            if (m > 1)
            {
                GetCombination(ref list, t, i - 1, m - 1, b, M);
            }
            else
            {
                if (list == null) list = new List<List<gameCard>>();
                var temp = new List<gameCard>();
                for (var j = 0; j < b.Length; j++) temp.Add(t[b[j]]);
                list.Add(temp);
            }
        }
    }

    private bool queryCorrectOverSumList(List<gameCard> temp, int sumlevel)
    {
        var illusionCount = temp.Count - cardsMustBeSelected.Count;
        if (illusionCount < ES_min) return false;
        if (illusionCount > ES_max) return false;
        var okCount = 0;
        for (var i = 1; i <= temp.Count; i++)
        {
            var totalCobination = GetCombination(temp, i);
            for (var i2 = 0; i2 < totalCobination.Count; i2++)
            {
                var re = false;
                var sumillustration = 0;
                for (var i3 = 0; i3 < totalCobination[i2].Count; i3++)
                    sumillustration += totalCobination[i2][i3].levelForSelect_1;
                if (sumillustration >= sumlevel) re = true;
                sumillustration = 0;
                for (var i3 = 0; i3 < totalCobination[i2].Count; i3++)
                    sumillustration += totalCobination[i2][i3].levelForSelect_2;
                if (sumillustration >= sumlevel) re = true;
                if (re) okCount++;
            }
        }

        return okCount == 1;
    }

    private void checkSum(int star)
    {
        var cards_remain_unselected = getUnselectedCards();
        if (ES_overFlow)
            for (var i = 1; i <= cards_remain_unselected.Count; i++)
            {
                var totalCobination = GetCombination(cards_remain_unselected, i);
                for (var i2 = 0; i2 < totalCobination.Count; i2++)
                {
                    var selectIllusion = new List<gameCard>();
                    for (var x = 0; x < totalCobination[i2].Count; x++) selectIllusion.Add(totalCobination[i2][x]);
                    for (var x = 0; x < cardsSelected.Count; x++) selectIllusion.Add(cardsSelected[x]);
                    for (var x = 0; x < cardsMustBeSelected.Count; x++) selectIllusion.Add(cardsMustBeSelected[x]);
                    if (queryCorrectOverSumList(selectIllusion, ES_level))
                        for (var i3 = 0; i3 < totalCobination[i2].Count; i3++)
                        {
                            cardsSelectable.Remove(totalCobination[i2][i3]);
                            cardsSelectable.Add(totalCobination[i2][i3]);
                        }
                }
            }
        else
            for (var i = 0; i < cards_remain_unselected.Count; i++)
            {
                var selectIllusion = new List<gameCard>();
                for (var x = 0; x < cards_remain_unselected.Count; x++)
                    if (x != i)
                        selectIllusion.Add(cards_remain_unselected[x]);
                var r = checkSum_process(selectIllusion, ES_level - star - cards_remain_unselected[i].levelForSelect_1,
                    cardsSelected.Count + 1);
                if (!r && cards_remain_unselected[i].levelForSelect_1 != cards_remain_unselected[i].levelForSelect_2)
                    r = checkSum_process(selectIllusion, ES_level - star - cards_remain_unselected[i].levelForSelect_2,
                        cardsSelected.Count + 1);
                if (r)
                {
                    cardsSelectable.Remove(cards_remain_unselected[i]);
                    cardsSelectable.Add(cards_remain_unselected[i]);
                }
            }
    }

    private List<gameCard> getUnselectedCards()
    {
        var cards_remain_unselected = new List<gameCard>();
        for (var x = 0; x < allCardsInSelectMessage.Count; x++) cards_remain_unselected.Add(allCardsInSelectMessage[x]);
        for (var x = 0; x < cardsSelected.Count; x++) cards_remain_unselected.Remove(cardsSelected[x]);
        for (var x = 0; x < cardsMustBeSelected.Count; x++) cards_remain_unselected.Remove(cardsMustBeSelected[x]);

        return cards_remain_unselected;
    }

    private bool checkSum_process(List<gameCard> cards_temp, int sum, int selectedCount)
    {
        if (sum == 0)
        {
            if (selectedCount < ES_min) return false;
            if (selectedCount > ES_max) return false;
            return true;
        }

        if (sum < 0) return false;

        for (var i = 0; i < cards_temp.Count; i++)
        {
            var new_cards = new List<gameCard>();
            for (var x = 0; x < cards_temp.Count; x++)
                if (x != i)
                    new_cards.Add(cards_temp[x]);
            var r = checkSum_process(new_cards, sum - cards_temp[i].levelForSelect_1, selectedCount + 1);
            if (!r && cards_temp[i].levelForSelect_1 != cards_temp[i].levelForSelect_2)
                r = checkSum_process(new_cards, sum - cards_temp[i].levelForSelect_2, selectedCount + 1);
            if (r) return r;
        }

        return false;
    }

    private void autoSendCards()
    {
        var m = new BinaryMaster();
        switch (currentMessage)
        {
            case GameMessage.SelectCard:
            case GameMessage.SelectUnselect:
            case GameMessage.SelectTribute:
                var c = ES_min;
                if (cardsSelectable.Count < c) c = cardsSelectable.Count;
                m.writer.Write((byte) c);
                for (var i = 0; i < c; i++)
                {
                    m.writer.Write((byte) cardsSelectable[i].selectPtr);
                    lastExcitedController = (int) cardsSelectable[i].p.controller;
                    lastExcitedLocation = (int) cardsSelectable[i].p.location;
                }

                sendReturn(m.get());
                break;
            case GameMessage.SelectSum:
                m = new BinaryMaster();
                m.writer.Write((byte) (cardsMustBeSelected.Count + cardsSelectable.Count));
                for (var i = 0; i < cardsMustBeSelected.Count; i++) m.writer.Write((byte) i);
                for (var i = 0; i < cardsSelectable.Count; i++)
                {
                    m.writer.Write((byte) cardsSelectable[i].selectPtr);
                    lastExcitedController = (int) cardsSelectable[i].p.controller;
                    lastExcitedLocation = (int) cardsSelectable[i].p.location;
                }
                sendReturn(m.get());
                break;
        }
    }

    public void sendSelectedCards()
    {
        BinaryMaster m;
        switch (currentMessage)
        {
            case GameMessage.SelectCard:
            case GameMessage.SelectUnselect:
            case GameMessage.SelectTribute:
            case GameMessage.SelectSum:
                m = new BinaryMaster();
                if (currentMessage == GameMessage.SelectUnselect && cardsSelected.Count == 0)
                {
                    m.writer.Write(-1);
                    sendReturn(m.get());
                    break;
                }

                m.writer.Write((byte) (cardsMustBeSelected.Count + cardsSelected.Count));
                for (var i = 0; i < cardsMustBeSelected.Count; i++) m.writer.Write((byte) i);
                for (var i = 0; i < cardsSelected.Count; i++)
                {
                    m.writer.Write((byte) cardsSelected[i].selectPtr);
                    lastExcitedController = (int) cardsSelected[i].p.controller;
                    lastExcitedLocation = (int) cardsSelected[i].p.location;
                }
                sendReturn(m.get());
                break;
        }
    }

    private void clearResponse()
    {
        flagForTimeConfirm = false;
        flagForCancleChain = false;
        //Package p = new Package();
        //p.Fuction = (int)GameMessage.sibyl_clear;
        //TcpHelper.AddRecordLine(p);
        if (clearTimeFlag)
        {
            clearTimeFlag = false;
            MessageBeginTime = 0;
        }

        ES_selectHint = "";
        cardsInSort.Clear();
        allCardsInSelectMessage.Clear();
        cardsSelected.Clear();
        cardsMustBeSelected.Clear();
        cardsSelectable.Clear();
        ES_sortResult.Clear();
        //cardsForConfirm.Clear();
        //Program.notGo(confirmGPS);
        gameField.Phase.colliderMp2.enabled = false;
        gameField.Phase.colliderBp.enabled = false;
        gameField.Phase.colliderEp.enabled = false;
        PhaseUIBehaviour.clickable = false;
        PhaseUIBehaviour.retOfBp = -1;
        PhaseUIBehaviour.retOfMp = -1;
        PhaseUIBehaviour.retOfEp = -1;

        toDefaultHint();

        clearAllSelectPlace();

        var myMaxDeck = countLocationSequence(0, CardLocation.Deck);

        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
            {
                cards[i].remove_all_cookie_button();
                cards[i].show_number(0);
                cards[i].del_all_decoration();
                cards[i].Highlight(false, false, false);
                cards[i].sortOptions.Clear();
                cards[i].currentFlash = gameCard.flashType.none;
                cards[i].prefered = false;
                if (cards[i].forSelect)
                {
                    cards[i].forSelect = false;
                    cards[i].isShowed = false;
                    if ((cards[i].p.location & (uint) CardLocation.Deck) > 0)
                        if (deckReserved == false || cards[i].p.controller != 0 || cards[i].p.sequence != myMaxDeck)
                            cards[i].erase_data();
                }

                cards[i].effects.Clear();
                if ((int) cards[i].p.location == lastExcitedLocation)
                    if ((int) cards[i].p.controller == lastExcitedController)
                        cards[i].isShowed = false;
                if (cards[i].p.location == (uint) CardLocation.Deck) cards[i].isShowed = false;
                if (clearAllShowedB) cards[i].isShowed = false;
            }
        foreach (var hiddenButton in gameField.gameHiddenButtons)
            hiddenButton.CheckNotification();
        clearAllShowedB = false;
        lastExcitedLocation = -1;
        lastExcitedController = -1;
        var to_clear = new List<gameCard>();
        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if (cards[i].p.location == (uint) CardLocation.Search)
                    to_clear.Add(cards[i]);

        for (var i = 0; i < to_clear.Count; i++)
        {
            to_clear[i].hide();
            to_clear[i].p.location = (uint) CardLocation.Unknown;
        }

        gameInfo.removeAll();
        gameField.btn_decide.hide();
        gameField.btn_confirm.hide();
        gameField.btn_cancel.hide();
        //Program.I().new_ui_cardSelection.hide();
        RMSshow_clear();
        realize();
        toNearest();
    }

    private void clearAllSelectPlace()
    {
        for (var i = 0; i < placeSelectors.Count; i++)
            if (placeSelectors[i] != null)
                if (placeSelectors[i].gameObject != null)
                    Object.DestroyImmediate(placeSelectors[i].gameObject);
        placeSelectors.Clear();
    }

    public void Sleep(int framsIn60)
    {
        var illustion = (int) (Program.TimePassed() + framsIn60 * 1000f / 60f);
        if (illustion > MessageBeginTime) MessageBeginTime = illustion;
    }

    public void StocMessage_TimeLimit(BinaryReader r)
    {
        int player = r.ReadByte();
        r.ReadByte();
        int time_limit = r.ReadInt16();
        TcpHelper.CtosMessage_TimeConfirm();
        gameInfo.setTime(unSwapPlayer(localPlayer(player)), time_limit);
        if (unSwapPlayer(localPlayer(player)) == 0)
        {
            //destroy(waitObject, 0, false, true);
            PlayableGuide.state = 1;
        }
    }

    public int localPlayer(int p, bool advanced = false)
    {
        if (p == 0 || p == 1)
        {
            if (advanced)
            {

            }
            else
            {
                if (isFirst)
                    return p;
                return 1 - p;
            }
        }

        return p;
    }

    public void realize(bool rush = false, float voiceTime = 0f)
    {
        someCardIsShowed = false;
        var real = (Program.fieldSize - 1) * 0.9f + 1f;
        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
            {
                cards[i].cookie_cared = false;
                cards[i].p_line_off();
                cards[i].sortButtons();
                cards[i].opMonsterWithBackGroundCard = false;
                cards[i].isMinBlockMode = false;
                cards[i].overFatherCount = 0;
            }

        var to_clear = new List<gameCard>();
        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if (cards[i].p.location == (uint) CardLocation.Unknown)
                    to_clear.Add(cards[i]);

        for (var i = 0; i < to_clear.Count; i++) to_clear[i].hide();

        //for (int i = 0; i < cards.Count; i++) if (cards[i].gameObject.activeInHierarchy)
        //        if (cards[i].cookie_cared == false)
        //        {
        //            if (winner == 2 || (winner != -1 && cards[i].p.controller != winner))
        //            {
        //                cards[i].cookie_cared = true;
        //                cards[i].UA_give_condition(gameCardCondition.still_unclickable);
        //                if (cards[i].p.controller == 0)
        //                {
        //                    cards[i].UA_give_position(new Vector3(UnityEngine.Random.Range(-15f, 15f), UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(-5f, -25f)));

        //                }
        //                else
        //                {
        //                    cards[i].UA_give_position(new Vector3(UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(0f, 5f), UnityEngine.Random.Range(5f, 22f)));

        //                }
        //                cards[i].UA_give_rotation(new Vector3(UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f)));
        //                cards[i].UA_flush_all_gived_witn_lock(rush);
        //            }
        //        }

        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if (cards[i].cookie_cared == false)
                {
                    if ((cards[i].p.location & (uint) CardLocation.Overlay) == 0)
                    {
                        if ((cards[i].p.location & (uint) CardLocation.SpellZone) > 0) cards[i].isShowed = false;
                        if ((cards[i].p.location & (uint) CardLocation.MonsterZone) > 0) cards[i].isShowed = false;
                    }

                    if ((cards[i].p.location & (uint) CardLocation.Hand) > 0 && cards[i].p.controller == 0 ||
                        (cards[i].p.location & (uint) CardLocation.Unknown) > 0)
                    {
                        cards[i].isShowed = true;
                    }
                    else
                    {
                        if (cards[i].isShowed && cards[i].forSelect == false) someCardIsShowed = true;
                    }
                }

        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if (cards[i].p.location == (uint)CardLocation.Search)
                {
                    //cards[i].isShowed = true;
                }

        var lines = new List<List<gameCard>>();
        uint preController = 9999;
        uint preLocation = 9999;
        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if (cards[i].cookie_cared == false)
                    if (cards[i].isShowed)
                    {
                        var lineMax = 8;
                        if (lines.Count <= 1) lineMax = 10;
                        if (
                            preController != cards[i].p.controller
                            ||
                            preLocation != cards[i].p.location
                            ||
                            lines[lines.Count - 1].Count == lineMax
                        )
                            lines.Add(new List<gameCard>());
                        lines[lines.Count - 1].Add(cards[i]);
                        preController = cards[i].p.controller;
                        preLocation = cards[i].p.location;
                    }

        if (lines.Count >= 2)
        {
            var lastLine = lines[lines.Count - 1];
            var preLine = lines[lines.Count - 2];
            if (lastLine.Count == 1)
                if (preLine.Count > 0)
                    if (lastLine[0].p.controller == preLine[0].p.controller)
                        if (lastLine[0].p.location == preLine[0].p.location)
                        {
                            preLine.Add(lastLine[0]);
                            lines.Remove(lastLine);
                        }
        }

        for (var line_index = 0; line_index < lines.Count; line_index++)
            for (var index = 0; index < lines[line_index].Count; index++)
            {
                var want_position = Vector3.zero;
                want_position.y = 12f;//mark 我方手卡默认高度

                if ((lines[line_index][index].p.location & (int)CardLocation.Hand) > 0)
                    want_position.z = -line_index * 8 + -27.5f;
                else
                    want_position.z = line_index * 8 + -27.5f;
                //else if (Shortcuts.lineShift)
                //    want_position.z = line_index * 8 + -27.5f;
                //else
                //    want_position.z = -line_index * 8 + -27.5f;
                if (line_index == 0 && (lines[line_index][index].p.location & (int)CardLocation.Hand) > 0)
                    want_position.x = UIHelper.get_left_right_indexEnhanced(-16, 16, index, lines[line_index].Count, 6); //(-10, 10, index, lines[line_index].Count, 5);
                else
                    want_position.x = UIHelper.get_left_right_indexEnhanced(-20, 20, index, lines[line_index].Count, 6);
                lines[line_index][index].cookie_cared = true;
                lines[line_index][index].UA_give_condition(gameCardCondition.floating_clickable);
                lines[line_index][index].UA_give_position(want_position);
                if ((lines[line_index][index].p.location & (int)CardLocation.Hand) > 0)
                    lines[line_index][index].UA_give_rotation(new Vector3(-20, 0, 3));//mark 我方手卡默认角度
                else
                    lines[line_index][index].UA_give_rotation(new Vector3(-20, 0, 0));
                lines[line_index][index].UA_flush_all_gived_witn_lock(rush);
            }

        gameField.isLong = false;

        var op_m = new List<gameCard>();

        var op_s = new List<gameCard>();

        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if (cards[i].p.controller == 1)
                    if ((cards[i].p.location & (uint) CardLocation.Overlay) == 0)
                    {
                        if ((cards[i].p.location & (uint) CardLocation.MonsterZone) > 0) op_m.Add(cards[i]);
                        if ((cards[i].p.location & (uint) CardLocation.SpellZone) > 0) op_s.Add(cards[i]);
                    }

        for (var m = 0; m < op_m.Count; m++)
            if ((op_m[m].p.position & (uint) CardPosition.FaceUp) > 0)
                for (var s = 0; s < op_s.Count; s++)
                    if (op_m[m].p.sequence == op_s[s].p.sequence)
                        if (op_m[m].p.sequence < 5)
                        {
                            op_m[m].opMonsterWithBackGroundCard = true;
                            //op_m[m].isMinBlockMode = true;
                            if (Program.getVerticalTransparency() >= 0.5f)
                                gameField.isLong = Program.longField; //这个设定恢复（？）了
                        }

        var opM = new gameCard[7];
        var meM = new gameCard[7];
        for (var i = 0; i < 7; i++)
        {
            opM[i] = null;
            meM[i] = null;
        }

        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if ((cards[i].p.location & (uint) CardLocation.Overlay) == 0)
                    if ((cards[i].p.location & (uint) CardLocation.MonsterZone) > 0)
                        if (cards[i].p.sequence >= 0 && cards[i].p.sequence <= 6)
                            if ((cards[i].p.position & (uint) CardPosition.FaceUp) > 0)
                            {
                                if (cards[i].p.controller == 1)
                                    opM[cards[i].p.sequence] = cards[i];
                                else
                                    meM[cards[i].p.sequence] = cards[i];
                            }

        if (opM[1] != null)
        {
            if (opM[5] != null) opM[5].isMinBlockMode = true;
            if (meM[6] != null) meM[6].isMinBlockMode = true;
        }

        if (opM[3] != null)
        {
            if (opM[6] != null) opM[6].isMinBlockMode = true;
            if (meM[5] != null) meM[5].isMinBlockMode = true;
        }

        if (opM[6] != null || meM[5] != null)
            if (meM[1] != null)
                meM[1].isMinBlockMode = true;

        if (opM[5] != null || meM[6] != null)
            if (meM[3] != null)
                meM[3].isMinBlockMode = true;


        var vvv = new gameCard[10, 10];

        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if ((cards[i].p.location & (uint) CardLocation.Overlay) == 0)
                    if ((cards[i].p.location & (uint) CardLocation.MonsterZone) > 0)
                        if (cards[i].p.sequence >= 0 && cards[i].p.sequence <= 6)
                            if ((cards[i].get_data().Type & (uint) CardType.Link) > 0)
                                if ((cards[i].p.position & (uint) CardPosition.FaceUp) > 0)
                                {
                                    if (cards[i].p.controller == 1)
                                    {
                                        if (cards[i].p.sequence >= 0 && cards[i].p.sequence <= 4)
                                            vvv[4, 4 - cards[i].p.sequence] = cards[i];
                                        if (cards[i].p.sequence == 5) vvv[3, 3] = cards[i];
                                        if (cards[i].p.sequence == 6) vvv[3, 1] = cards[i];
                                    }
                                    else
                                    {
                                        if (cards[i].p.sequence >= 0 && cards[i].p.sequence <= 4)
                                            vvv[2, cards[i].p.sequence] = cards[i];
                                        if (cards[i].p.sequence == 5) vvv[3, 1] = cards[i];
                                        if (cards[i].p.sequence == 6) vvv[3, 3] = cards[i];
                                    }
                                }


        var linkPs = new List<GPS>();


        for (var curHang = 2; curHang <= 4; curHang++)
        for (var curLie = 0; curLie <= 4; curLie++)
            //if (vvv[curHang, curLie] != null)
        {
            var currentGPS = new GPS();
            currentGPS.location = (int) CardLocation.MonsterZone;
            if (curHang == 4)
            {
                currentGPS.controller = 1;
                currentGPS.sequence = (uint) (4 - curLie);
            }

            if (curHang == 3)
            {
                currentGPS.controller = 0;
                if (currentGPS.sequence == 0) continue;
                if (currentGPS.sequence == 1) currentGPS.sequence = 5;
                if (currentGPS.sequence == 2) continue;
                if (currentGPS.sequence == 3) currentGPS.sequence = 6;
                if (currentGPS.sequence == 4) continue;
            }

            if (curHang == 2)
            {
                currentGPS.controller = 0;
                currentGPS.sequence = (uint) curLie;
            }

            var lighted = false;

            if (curHang - 1 >= 0)
                if (curLie - 1 >= 0)
                    if (vvv[curHang - 1, curLie - 1] != null)
                    {
                        var card = vvv[curHang - 1, curLie - 1];
                        if (card.p.controller == 0)
                            if (card.get_data().HasLinkMarker(CardLinkMarker.TopRight))
                                lighted = true;
                        if (card.p.controller == 1)
                            if (card.get_data().HasLinkMarker(CardLinkMarker.BottomLeft))
                                lighted = true;
                    }

            if (curLie - 1 >= 0)
                if (vvv[curHang, curLie - 1] != null)
                {
                    var card = vvv[curHang, curLie - 1];
                    if (card.p.controller == 0)
                        if (card.get_data().HasLinkMarker(CardLinkMarker.Right))
                            lighted = true;
                    if (card.p.controller == 1)
                        if (card.get_data().HasLinkMarker(CardLinkMarker.Left))
                            lighted = true;
                }

            if (curLie - 1 >= 0)
                if (vvv[curHang + 1, curLie - 1] != null)
                {
                    var card = vvv[curHang + 1, curLie - 1];
                    if (card.p.controller == 0)
                        if (card.get_data().HasLinkMarker(CardLinkMarker.BottomRight))
                            lighted = true;
                    if (card.p.controller == 1)
                        if (card.get_data().HasLinkMarker(CardLinkMarker.TopLeft))
                            lighted = true;
                }

            if (curHang - 1 >= 0)
                if (vvv[curHang - 1, curLie] != null)
                {
                    var card = vvv[curHang - 1, curLie];
                    if (card.p.controller == 0)
                        if (card.get_data().HasLinkMarker(CardLinkMarker.Top))
                            lighted = true;
                    if (card.p.controller == 1)
                        if (card.get_data().HasLinkMarker(CardLinkMarker.Bottom))
                            lighted = true;
                }

            if (vvv[curHang + 1, curLie] != null)
            {
                var card = vvv[curHang + 1, curLie];
                if (card.p.controller == 0)
                    if (card.get_data().HasLinkMarker(CardLinkMarker.Bottom))
                        lighted = true;
                if (card.p.controller == 1)
                    if (card.get_data().HasLinkMarker(CardLinkMarker.Top))
                        lighted = true;
            }

            if (curHang - 1 >= 0)
                if (vvv[curHang - 1, curLie + 1] != null)
                {
                    var card = vvv[curHang - 1, curLie + 1];
                    if (card.p.controller == 0)
                        if (card.get_data().HasLinkMarker(CardLinkMarker.TopLeft))
                            lighted = true;
                    if (card.p.controller == 1)
                        if (card.get_data().HasLinkMarker(CardLinkMarker.BottomRight))
                            lighted = true;
                }

            if (vvv[curHang, curLie + 1] != null)
            {
                var card = vvv[curHang, curLie + 1];
                if (card.p.controller == 0)
                    if (card.get_data().HasLinkMarker(CardLinkMarker.Left))
                        lighted = true;
                if (card.p.controller == 1)
                    if (card.get_data().HasLinkMarker(CardLinkMarker.Right))
                        lighted = true;
            }

            if (vvv[curHang + 1, curLie + 1] != null)
            {
                var card = vvv[curHang + 1, curLie + 1];
                if (card.p.controller == 0)
                    if (card.get_data().HasLinkMarker(CardLinkMarker.BottomLeft))
                        lighted = true;
                if (card.p.controller == 1)
                    if (card.get_data().HasLinkMarker(CardLinkMarker.TopRight))
                        lighted = true;
            }

            if (lighted) linkPs.Add(currentGPS);
        }

        for (var i = 0; i < linkPs.Count; i++)
        {
            var showed = false;
            for (var a = 0; a < linkMaskList.Count; a++)
                if (linkMaskList[a].p.controller == linkPs[i].controller &&
                    linkMaskList[a].p.sequence == linkPs[i].sequence)
                    showed = true;
            if (showed == false) linkMaskList.Add(makeLinkMask(linkPs[i]));
        }

        var removeList = new List<linkMask>();

        for (var i = 0; i < linkMaskList.Count; i++)
        {
            var deleted = true;
            for (var a = 0; a < linkPs.Count; a++)
                if (linkMaskList[i].p.controller == linkPs[a].controller &&
                    linkMaskList[i].p.sequence == linkPs[a].sequence)
                    deleted = false;
            if (deleted) removeList.Add(linkMaskList[i]);
        }

        for (var i = 0; i < removeList.Count; i++)
        {
            linkMaskList.Remove(removeList[i]);
            destroy(removeList[i].gameObject);
        }

        removeList.Clear();
        removeList = null;

        for (var i = 0; i < linkMaskList.Count; i++)
            shift_effect(linkMaskList[i], Program.I().setting.setting.Vlink.value);

        gameField.Update();
        //op hand
        var line = new List<gameCard>();
        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if (cards[i].cookie_cared == false)
                    if ((cards[i].p.location & (uint) CardLocation.Hand) > 0 && cards[i].p.controller == 1)
                        line.Add(cards[i]);
        for (var index = 0; index < line.Count; index++)
        {
            var want_position = Vector3.zero;
            want_position.y = 12f; //mark对方手卡高度
            if (gameField.isLong)
                want_position.z = 20f + index * 0.015f;//target 20
            else
                want_position.z = 20f + index * 0.015f;
            want_position.x = UIHelper.get_left_right_indexEnhanced(16, -16, index, line.Count, 6);
            line[index].cookie_cared = true;
            line[index].UA_give_position(want_position);
            if (line[index].get_data().Id > 0)
                line[index].UA_give_rotation(new Vector3(20, 180, 0));
            else
                line[index].UA_give_rotation(new Vector3(20, 180, 180));
            line[index].UA_give_condition(gameCardCondition.floating_clickable);
            line[index].UA_flush_all_gived_witn_lock(rush);
        }

        //effects
        for (var i = 0; i < gameField.thunders.Count; i++) gameField.thunders[i].needDestroy = true;

        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
            {
                var overlayed_cards = GCS_cardGetOverlayElements(cards[i]);
                var overC = 0;
                if (Program.getVerticalTransparency() > 0.5f)
                    if ((cards[i].p.position & (int) CardPosition.FaceUp) > 0 &&
                        (cards[i].p.location & (int) CardLocation.Onfield) > 0)
                        overC = overlayed_cards.Count;
                cards[i].set_overlay_light(overC);
                cards[i].set_overlay_see_button(overlayed_cards.Count > 0);
                for (var x = 0; x < overlayed_cards.Count; x++)
                {
                    overlayed_cards[x].overFatherCount = overlayed_cards.Count;
                    if (overlayed_cards[x].isShowed)
                        animation_thunder(overlayed_cards[x].gameObject, cards[i].gameObject);
                }

                foreach (var item in cards[i].target)
                    if ((item.p.location & (uint) CardLocation.SpellZone) > 0 ||
                        (item.p.location & (uint) CardLocation.MonsterZone) > 0)
                        animation_thunder(item.gameObject, cards[i].gameObject);
            }

        var needRemoveThunder = new List<thunder_locator>();
        for (var i = 0; i < gameField.thunders.Count; i++)
            if (gameField.thunders[i].needDestroy)
                needRemoveThunder.Add(gameField.thunders[i]);
        for (var i = 0; i < needRemoveThunder.Count; i++)
        {
            gameField.thunders.Remove(needRemoveThunder[i]);
            destroy(needRemoveThunder[i].gameObject);
        }

        needRemoveThunder.Clear();


        //p effect
        gameField.relocatePnums(Program.I().setting.setting.Vpedium.value);
        if (Program.I().setting.setting.Vpedium.value)
        {
            var my_p_cards = new List<gameCard>();

            var op_p_cards = new List<gameCard>();
            if (MasterRule >= 4)
                for (var i = 0; i < cards.Count; i++)
                    if (cards[i].gameObject.activeInHierarchy)
                        if (cards[i].cookie_cared == false)
                            if ((cards[i].p.location & (uint)CardLocation.SpellZone) > 0)
                                if (cards[i].p.sequence == 0 || cards[i].p.sequence == 4)
                                    if ((cards[i].get_data().Type & (int)CardType.Pendulum) > 0)
                                    {
                                        if (cards[i].p.controller == 0)
                                            my_p_cards.Add(cards[i]);
                                        else
                                            op_p_cards.Add(cards[i]);
                                    }
            if (MasterRule <= 3)
                for (var i = 0; i < cards.Count; i++)
                    if (cards[i].gameObject.activeInHierarchy)
                        if (cards[i].cookie_cared == false)
                            if ((cards[i].p.location & (uint)CardLocation.SpellZone) > 0)
                                if (cards[i].p.sequence == 6 || cards[i].p.sequence == 7)
                                    if ((cards[i].get_data().Type & (int)CardType.Pendulum) > 0)
                                    {
                                        if (cards[i].p.controller == 0)
                                            my_p_cards.Add(cards[i]);
                                        else
                                            op_p_cards.Add(cards[i]);
                                    }
            if (MasterRule >= 4)//mark 灵摆卡
            {
                if (my_p_cards.Count == 2)
                {
                    gameField.me_left_p_num.GetComponent<number_loader>()
                        .set_number(my_p_cards[0].get_data().LScale, 5);
                    gameField.me_right_p_num.GetComponent<number_loader>()
                        .set_number(my_p_cards[1].get_data().LScale, 0);
                    //gameField.mePHole = true;
                    //my_p_cards[0].cookie_cared = true;
                    //my_p_cards[0].UA_give_position(new Vector3(-17, 4, -18));
                    //my_p_cards[0].UA_give_rotation(new Vector3(-60, -45, 0));
                    my_p_cards[0].UA_give_condition(gameCardCondition.floating_clickable);
                    //my_p_cards[0].UA_flush_all_gived_witn_lock(rush);
                    //my_p_cards[1].cookie_cared = true;
                    //my_p_cards[1].UA_give_position(new Vector3(17, 4, -18));
                    //my_p_cards[1].UA_give_rotation(new Vector3(-60, 45, 0));
                    my_p_cards[1].UA_give_condition(gameCardCondition.floating_clickable);
                    //my_p_cards[1].UA_flush_all_gived_witn_lock(rush);
                    //my_p_cards[0].p_line_on();
                    //my_p_cards[1].p_line_on();

                    LoadSFX.me_pendulum = true;
                }
                else
                {
                    gameField.me_left_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.me_right_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.mePHole = false;

                    LoadSFX.me_pendulum = false;
                }

                if (op_p_cards.Count == 2)
                {
                    gameField.op_left_p_num.GetComponent<number_loader>()
                        .set_number(op_p_cards[1].get_data().LScale, 0);
                    gameField.op_right_p_num.GetComponent<number_loader>()
                        .set_number(op_p_cards[0].get_data().LScale, 5);
                    //gameField.opPHole = true;
                    //op_p_cards[0].cookie_cared = true;
                    //op_p_cards[0].UA_give_position(new Vector3(17.3f, 4, 17.6f));
                    //op_p_cards[0].UA_give_rotation(new Vector3(-70, 45, 0));
                    op_p_cards[0].UA_give_condition(gameCardCondition.floating_clickable);
                    //op_p_cards[0].UA_flush_all_gived_witn_lock(rush);
                    //op_p_cards[1].cookie_cared = true;
                    //op_p_cards[1].UA_give_position(new Vector3(-17.3f, 4, 17.6f));
                    //op_p_cards[1].UA_give_rotation(new Vector3(-70, -45, 0));
                    op_p_cards[1].UA_give_condition(gameCardCondition.floating_clickable);
                    //op_p_cards[1].UA_flush_all_gived_witn_lock(rush);
                    //op_p_cards[0].p_line_on();
                    //op_p_cards[1].p_line_on();

                    LoadSFX.op_pendulum = true;
                }
                else
                {
                    gameField.op_left_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.op_right_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.opPHole = false;

                    LoadSFX.op_pendulum = false;
                }
            }
            if (MasterRule <= 3)
            {
                if (my_p_cards.Count == 2)
                {
                    gameField.me_left_p_num.GetComponent<number_loader>()
                        .set_number(my_p_cards[0].get_data().LScale, 3);
                    gameField.me_right_p_num.GetComponent<number_loader>()
                        .set_number(my_p_cards[1].get_data().LScale, 3);
                    gameField.mePHole = true;
                    my_p_cards[0].cookie_cared = true;
                    my_p_cards[0].UA_give_position(new Vector3(-30f, 10f, -15f));
                    my_p_cards[0].UA_give_rotation(new Vector3(-60, -45, 0));
                    my_p_cards[0].UA_give_condition(gameCardCondition.floating_clickable);
                    my_p_cards[0].UA_flush_all_gived_witn_lock(rush);
                    my_p_cards[1].cookie_cared = true;
                    my_p_cards[1].UA_give_position(new Vector3(30f, 10f, -15f));
                    my_p_cards[1].UA_give_rotation(new Vector3(-60, 45, 0));
                    my_p_cards[1].UA_give_condition(gameCardCondition.floating_clickable);
                    my_p_cards[1].UA_flush_all_gived_witn_lock(rush);
                    my_p_cards[0].p_line_on();
                    my_p_cards[1].p_line_on();

                    LoadSFX.me_pendulum = true;
                }
                else
                {
                    gameField.me_left_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.me_right_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.mePHole = false;

                    LoadSFX.me_pendulum = false;
                }

                if (op_p_cards.Count == 2)
                {
                    gameField.op_left_p_num.GetComponent<number_loader>()
                        .set_number(op_p_cards[1].get_data().LScale, 3);
                    gameField.op_right_p_num.GetComponent<number_loader>()
                        .set_number(op_p_cards[0].get_data().LScale, 3);
                    gameField.opPHole = true;
                    op_p_cards[0].cookie_cared = true;
                    op_p_cards[0].UA_give_position(new Vector3(30f, 10f, 10f));
                    op_p_cards[0].UA_give_rotation(new Vector3(-70, 45, 0));
                    op_p_cards[0].UA_give_condition(gameCardCondition.floating_clickable);
                    op_p_cards[0].UA_flush_all_gived_witn_lock(rush);
                    op_p_cards[1].cookie_cared = true;
                    op_p_cards[1].UA_give_position(new Vector3(-30f, 10f, 10f));
                    op_p_cards[1].UA_give_rotation(new Vector3(-70, -45, 0));
                    op_p_cards[1].UA_give_condition(gameCardCondition.floating_clickable);
                    op_p_cards[1].UA_flush_all_gived_witn_lock(rush);
                    op_p_cards[0].p_line_on();
                    op_p_cards[1].p_line_on();

                    LoadSFX.op_pendulum = true;
                }
                else
                {
                    gameField.op_left_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.op_right_p_num.GetComponent<number_loader>().set_number(-1, 3);
                    gameField.opPHole = false;

                    LoadSFX.op_pendulum = false;
                }
            }
        }
        else
        {
            //p effect pain

            var my_p_cards = new List<gameCard>();

            var op_p_cards = new List<gameCard>();

            if(MasterRule <= 3)
                for (var i = 0; i < cards.Count; i++)
                    if (cards[i].gameObject.activeInHierarchy)
                        if (cards[i].cookie_cared == false)
                            if ((cards[i].p.location & (uint)CardLocation.SpellZone) > 0)
                                if (cards[i].p.sequence == 6 || cards[i].p.sequence == 7)
                                {
                                    if (cards[i].p.controller == 0)
                                        my_p_cards.Add(cards[i]);
                                    else
                                        op_p_cards.Add(cards[i]);
                                }
            if (MasterRule >= 4)
                for (var i = 0; i < cards.Count; i++)
                    if (cards[i].gameObject.activeInHierarchy)
                        if (cards[i].cookie_cared == false)
                            if ((cards[i].p.location & (uint)CardLocation.SpellZone) > 0)
                                if (cards[i].p.sequence == 0 || cards[i].p.sequence == 4)
                                {
                                    if (cards[i].p.controller == 0)
                                        my_p_cards.Add(cards[i]);
                                    else
                                        op_p_cards.Add(cards[i]);
                                }
            gameField.mePHole = false;
            gameField.opPHole = false;

            if (my_p_cards.Count == 2)
            {
                gameField.me_left_p_num.GetComponent<number_loader>().set_number(my_p_cards[0].get_data().LScale, 3);
                gameField.me_right_p_num.GetComponent<number_loader>().set_number(my_p_cards[1].get_data().LScale, 0);
            }
            else
            {
                gameField.me_left_p_num.GetComponent<number_loader>().set_number(-1, 3);
                gameField.me_right_p_num.GetComponent<number_loader>().set_number(-1, 3);
            }

            if (op_p_cards.Count == 2)
            {
                gameField.op_left_p_num.GetComponent<number_loader>().set_number(op_p_cards[1].get_data().LScale, 0);
                gameField.op_right_p_num.GetComponent<number_loader>().set_number(op_p_cards[0].get_data().LScale, 3);
            }
            else
            {
                gameField.op_left_p_num.GetComponent<number_loader>().set_number(-1, 3);
                gameField.op_right_p_num.GetComponent<number_loader>().set_number(-1, 3);
            }
        }

        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if (cards[i].cookie_cared == false)
                    if ((cards[i].p.location & (uint) CardLocation.Overlay) > 0)
                        if ((cards[i].p.location & (uint) CardLocation.Extra) > 0)
                        {
                            cards[i].cookie_cared = true;
                            cards[i].UA_give_condition(get_point_worldcondition(cards[i].p));
                            var temp = get_point_worldposition(cards[i].p_beforeOverLayed);
                            temp.y = cards[i].p.position * 0.05f;
                            cards[i].UA_give_position(temp);
                            cards[i].UA_give_rotation(get_world_rotation(cards[i]));
                            cards[i].UA_flush_all_gived_witn_lock(rush);
                        }

        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)//mark星星
            {
                if (cards[i].cookie_cared == false && cards[i].inAnimation == false)
                {
                    cards[i].UA_give_condition(get_point_worldcondition(cards[i].p));
                    cards[i].UA_give_position(get_point_worldposition(cards[i].p, cards[i]));
                    cards[i].UA_give_rotation(get_world_rotation(cards[i]));
                    cards[i].UA_flush_all_gived_witn_lock(rush);
                }
            }
        if (Program.I().setting.setting.Vfield.value)
        {
            var code = 0;

            for (var i = 0; i < cards.Count; i++)
                if (cards[i].gameObject.activeInHierarchy)
                    if ((cards[i].p.location & (uint) CardLocation.SpellZone) > 0 && cards[i].p.sequence == 5)
                        if (cards[i].p.controller == 0)
                            if ((cards[i].p.position & (int) CardPosition.FaceUp) > 0)
                                code = cards[i].get_data().Id;

            gameField.set(0, code);

            code = 0;

            for (var i = 0; i < cards.Count; i++)
                if (cards[i].gameObject.activeInHierarchy)
                    if ((cards[i].p.location & (uint) CardLocation.SpellZone) > 0 && cards[i].p.sequence == 5)
                        if (cards[i].p.controller == 1)
                            if ((cards[i].p.position & (int) CardPosition.FaceUp) > 0)
                                code = cards[i].get_data().Id;

            gameField.set(1, code);
        }
        else
        {
            gameField.set(0, 0);
            gameField.set(1, 0);
        }


        //mark camera
        float nearest_z = 0;
        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if (nearest_z > cards[i].UA_get_accurate_position().z)
                    nearest_z = cards[i].UA_get_accurate_position().z;
        camera_max = -37f;
        camera_min = nearest_z - 9.5f;//target -37
        if (camera_min > camera_max) camera_min = camera_max;

        if (InAI == false)
            if (condition != Condition.duel)
                toNearest();

        if (someCardIsShowed)
        {
            if (gameInfo.queryHashedButton("hide_all_card") == false)
                gameInfo.addHashedButton("hide_all_card", 0, superButtonType.see, InterString.Get("确认完毕@ui"));
        }
        else
        {
            gameInfo.removeHashedButton("hide_all_card");
        }

        if (InAI == false && condition != Condition.duel)
        {
            //if (gameInfo.queryHashedButton("swap") == false)
            //    gameInfo.addHashedButton("swap", 0, superButtonType.change, InterString.Get("转换视角@ui"));
        }
        else
        {
            gameInfo.removeHashedButton("swap");
        }


        //animation_count(gameField.LOCATION_DECK_0, CardLocation.Deck, 0);
        //animation_count(gameField.LOCATION_EXTRA_0, CardLocation.Extra, 0);
        //animation_count(gameField.LOCATION_GRAVE_0, CardLocation.Grave, 0);
        //animation_count(gameField.LOCATION_REMOVED_0, CardLocation.Removed, 0);
        //animation_count(gameField.LOCATION_DECK_1, CardLocation.Deck, 1);
        //animation_count(gameField.LOCATION_EXTRA_1, CardLocation.Extra, 1);
        //animation_count(gameField.LOCATION_GRAVE_1, CardLocation.Grave, 1);
        //animation_count(gameField.LOCATION_REMOVED_1, CardLocation.Removed, 1);
        gameField.realize();
        //Program.notGo(gameInfo.realize);
        //Program.go(50, gameInfo.realize);
        gameInfo.realize();
        Program.notGo(Program.I().book.realize);
        Program.go(50, Program.I().book.realize);
        Program.I().cardDescription.realizeMonitor();
    }

    private void animation_thunder(GameObject leftGameObject, GameObject rightGameObject)
    {
        thunder_locator thunder = null;
        for (var p = 0; p < gameField.thunders.Count; p++)
            if (gameField.thunders[p].leftobj == leftGameObject)
                if (gameField.thunders[p].rightobj == rightGameObject)
                    thunder = gameField.thunders[p];

        if (thunder == null)
        {
            thunder = create_s(Program.I().mod_ocgcore_decoration_thunder).GetComponent<thunder_locator>();
            thunder.set_objects(leftGameObject, rightGameObject);
            gameField.thunders.Add(thunder);
        }

        thunder.needDestroy = false;
    }

    public Vector3 get_world_rotation(gameCard card)
    {
        var r = cardRuleComdition.meUpAtk;
        if ((card.p.location & (uint) CardLocation.Deck) > 0)
        {
            if (card.get_data().Id > 0)
                r = cardRuleComdition.meUpDeck;
            else
                r = cardRuleComdition.meDownDeck;
        }

        if ((card.p.location & (uint) CardLocation.Grave) > 0) r = cardRuleComdition.meUpAtk;
        if ((card.p.location & (uint) CardLocation.Removed) > 0)
        {
            if ((card.p.position & (uint) CardPosition.FaceUp) > 0)
                r = cardRuleComdition.meUpAtk;
            else
                r = cardRuleComdition.meDownAtk;
        }

        if ((card.p.location & (uint) CardLocation.Extra) > 0)
        {
            if ((card.p.position & (uint) CardPosition.FaceUp) > 0)
                r = cardRuleComdition.meUpExDeck;
            else
                r = cardRuleComdition.meDownExDeck;
        }

        if ((card.p.location & (uint) CardLocation.MonsterZone) > 0)
        {
            if ((card.p.position & (uint) CardPosition.FaceDownDefence) > 0) r = cardRuleComdition.meDownDef;
            if ((card.p.position & (uint) CardPosition.FaceUpDefence) > 0) r = cardRuleComdition.meUpDef;
            if ((card.p.position & (uint) CardPosition.FaceDownAttack) > 0) r = cardRuleComdition.meDownAtk;
            if ((card.p.position & (uint) CardPosition.FaceUpAttack) > 0) r = cardRuleComdition.meUpAtk;
        }

        if ((card.p.location & (uint) CardLocation.SpellZone) > 0)
        {
            if ((card.p.position & (uint) CardPosition.FaceUp) > 0)
                r = cardRuleComdition.meUpAtk;
            else
                r = cardRuleComdition.meDownAtk;
        }

        if ((card.p.location & (uint) CardLocation.Overlay) > 0) r = cardRuleComdition.meUpAtk;
        if (card.p.controller == 1)
            switch (r)
            {
                case cardRuleComdition.meUpAtk:
                    r = cardRuleComdition.opUpAtk;
                    break;
                case cardRuleComdition.meUpDef:
                    r = cardRuleComdition.opUpDef;
                    break;
                case cardRuleComdition.meDownAtk:
                    r = cardRuleComdition.opDownAtk;
                    break;
                case cardRuleComdition.meDownDef:
                    r = cardRuleComdition.opDownDef;
                    break;
                case cardRuleComdition.meUpDeck:
                    r = cardRuleComdition.opUpDeck;
                    break;
                case cardRuleComdition.meDownDeck:
                    r = cardRuleComdition.opDownDeck;
                    break;
                case cardRuleComdition.meUpExDeck:
                    r = cardRuleComdition.opUpExDeck;
                    break;
                case cardRuleComdition.meDownExDeck:
                    r = cardRuleComdition.opDownExDeck;
                    break;
            }

        switch (r)
        {
            case cardRuleComdition.meUpAtk:
                return new Vector3(0, 0, 0);
            case cardRuleComdition.meUpDef:
                return new Vector3(0, -90, 0);
            case cardRuleComdition.meDownAtk:
                return new Vector3(0, 0, 180);
            case cardRuleComdition.meDownDef:
                return new Vector3(0, -90, 180);
            case cardRuleComdition.meUpDeck:
                return new Vector3(0, -20, 0);
            case cardRuleComdition.meDownDeck:
                return new Vector3(0, -20, 180);
            case cardRuleComdition.meUpExDeck:
                return new Vector3(0, 20, 0);
            case cardRuleComdition.meDownExDeck:
                return new Vector3(0, 20, 180);

                

            case cardRuleComdition.opUpAtk:
                return new Vector3(0, 180, 0);
            case cardRuleComdition.opUpDef:
                return new Vector3(0, 90, 0);
            case cardRuleComdition.opDownAtk:
                return new Vector3(0, 180, 180);
            case cardRuleComdition.opDownDef:
                return new Vector3(0, 90, 180);
            case cardRuleComdition.opUpDeck:
                return new Vector3(0, 160, 0);
            case cardRuleComdition.opDownDeck:
                return new Vector3(0, 160, 180);
            case cardRuleComdition.opUpExDeck:
                return new Vector3(0, 200, 0);
            case cardRuleComdition.opDownExDeck:
                return new Vector3(0, 200, 180);

            default:
                return Vector3.zero;
        }
    }

    //private Vector3 get_real_rotation(int i)
    //{
    //    Vector3 r = get_point_worldrotation(cards[i].p);
    //    if ((cards[i].p.location & (UInt32)CardLocation.Deck) > 0)
    //    {
    //        if (cards[i].get_data().Id > 0)
    //        {
    //            r = new Vector3(90, 0, 0);
    //        }
    //        else
    //        {
    //            r = new Vector3(-90, 0, 0);
    //        }
    //    }
    //    if ((cards[i].p.location & (UInt32)CardLocation.MonsterZone) > 0)
    //    {
    //        if ((cards[i].p.position & (UInt32)CardPosition.FaceDown_DEFENSE) > 0)
    //        {
    //            r = new Vector3(-90, 0, 90);
    //        }
    //        if ((cards[i].p.position & (UInt32)CardPosition.FaceUp_DEFENSE) > 0)
    //        {
    //            r = new Vector3(90, 0, 90);
    //        }
    //        if ((cards[i].p.position & (UInt32)CardPosition.FaceDownAttack) > 0)
    //        {
    //            r = new Vector3(-90, 0, 0);
    //        }
    //        if ((cards[i].p.position & (UInt32)CardPosition.FaceUpAttack) > 0)
    //        {
    //            r = new Vector3(90, 0, 0);
    //        }
    //    }
    //    if ((cards[i].p.location & (UInt32)CardLocation.SpellZone) > 0)
    //    {
    //        if ((cards[i].p.position & (UInt32)CardPosition.FaceDown_DEFENSE) > 0)
    //        {
    //            r = new Vector3(-90, 0, 90);
    //        }
    //        if ((cards[i].p.position & (UInt32)CardPosition.FaceUp_DEFENSE) > 0)
    //        {
    //            r = new Vector3(90, 0, 90);
    //        }
    //        if ((cards[i].p.position & (UInt32)CardPosition.FaceDownAttack) > 0)
    //        {
    //            r = new Vector3(-90, 0, 0);
    //        }
    //        if ((cards[i].p.position & (UInt32)CardPosition.FaceUpAttack) > 0)
    //        {
    //            r = new Vector3(90, 0, 0);
    //        }
    //    }
    //    if ((cards[i].p.location & (UInt32)CardLocation.Grave) > 0)
    //    {
    //        r = new Vector3(90, 0, 0);
    //    }
    //    if ((cards[i].p.location & (UInt32)CardLocation.Removed) > 0)
    //    {
    //        if ((cards[i].p.position & (UInt32)CardPosition.FaceUp) > 0)
    //        {
    //            r = new Vector3(90, 0, 0);
    //        }
    //        else
    //        {
    //            r = new Vector3(-90, 0, 0);
    //        }
    //    }
    //    if ((cards[i].p.location & (UInt32)CardLocation.Extra) > 0)
    //    {
    //        if ((cards[i].p.position & (UInt32)CardPosition.FaceUp) > 0)
    //        {
    //            r = new Vector3(90, 0, 0);
    //        }
    //        else
    //        {
    //            r = new Vector3(-90, 0, 0);
    //        }
    //    }
    //    if ((cards[i].p.location & (UInt32)CardLocation.Overlay) > 0)
    //    {
    //        r = new Vector3(90, 0, 0);
    //    }
    //    if (cards[i].p.controller == 1)
    //    {
    //        r.z += 179f;
    //    }

    //    return r;
    //}

    private void animation_count(TextMeshPro textmesh, CardLocation location, int player)
    {
        var count = 0;
        var countU = 0;
        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                if (cards[i].p.controller == player)
                    if ((cards[i].p.location & (uint) location) > 0)
                    {
                        count++;
                        if ((cards[i].p.position & (uint) CardPosition.FaceUp) > 0) countU++;
                    }

        if (count < 2)
        {
            textmesh.text = "";
        }
        else
        {
            if (location == CardLocation.Extra)
                textmesh.text = count + "(" + countU + ")";
            else
                textmesh.text = count.ToString();
        }
    }

    public void toNearest(bool fix = false)//mark 摄像机position
    {
        if (fix)
        {
            if (Program.cameraPosition.z < camera_min)
            {
                Program.cameraPosition.z = camera_min;
                Program.cameraPosition.x = 0;
                Program.cameraPosition.y = 95f;
            }
        }
        else
        {
            Program.cameraPosition.z = camera_min;
            Program.cameraPosition.x = 0;
            Program.cameraPosition.y = 95f;
        }
        Program.cameraRotation = new Vector3(70, 0, 0);
    }

    public gameCard GCS_cardCreate(GPS p)
    {
        gameCard c = null;
        for (var i = 0; i < cards.Count; i++)
            if (cards[i].md5 == md5Maker)
            {
                c = cards[i];
                c.p = p;
            }

        if (c == null)
        {
            c = new gameCard();
            c.md5 = md5Maker;
            c.p = p;
            cards.Add(c);
        }

        c.show();
        c.p = p;
        c.controllerBased = p.controller;
        md5Maker++;
        return c;
    }

    public gameCard GCS_cardGet(GPS p, bool create, int code = 0)
    {
        gameCard c = null;
        if(code == 0)
        {
            if ((p.location & (uint)CardLocation.Overlay) > 0)
            {
                for (var i = 0; i < cards.Count; i++)
                    if (cards[i].p.location == p.location)
                        if (cards[i].p.controller == p.controller)
                            if (cards[i].p.sequence == p.sequence)
                                if (cards[i].p.position == p.position)
                                    if (cards[i].gameObject.activeInHierarchy)
                                    {
                                        c = cards[i];
                                        break;
                                    }
            }
            else
            {
                for (var i = 0; i < cards.Count; i++)
                    if (cards[i].p.location == p.location)
                        if (cards[i].p.controller == p.controller)
                            if (cards[i].p.sequence == p.sequence)
                                if (cards[i].gameObject.activeInHierarchy)
                                {
                                    c = cards[i];
                                    break;
                                }
            }

            if (p.location == 0) c = null;
            if (create)
                if (c == null)
                    c = GCS_cardCreate(p);
        }
        else
        {
            for (var i = 0; i < cards.Count; i++)
                if (cards[i].get_data().Id == code)
                {
                    c = cards[i];
                    break;
                }                    
        }
        return c;
    }

    public List<gameCard> GCS_cardGetOverlayElements(gameCard c)
    {
        var cas = new List<gameCard>();
        if (c != null)
            if ((c.p.location & (uint) CardLocation.Overlay) == 0)
                for (var i = 0; i < cards.Count; i++)
                    if (cards[i].gameObject.activeInHierarchy)
                        if ((cards[i].p.location & (uint) CardLocation.Overlay) > 0)
                            if (cards[i].p.controller == c.p.controller)
                                if ((cards[i].p.location | (uint) CardLocation.Overlay) ==
                                    (c.p.location | (uint) CardLocation.Overlay))
                                    if (cards[i].p.sequence == c.p.sequence)
                                        cas.Add(cards[i]);
        return cas;
    }

    public gameCard GCS_cardMove(GPS p1, GPS p2, bool print = true, bool swap = false)
    {
        //from card
        var card_from = GCS_cardGet(p1, true);
        try
        {
            if (reportShowAll)
                if (print)
                    if (swap)
                    {
                        //printDuelLog(UIHelper.getGPSstringLocation(p1) + InterString.Get("交换") + UIHelper.getGPSstringLocation(p2) + UIHelper.getGPSstringName(card_from));
                    }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        //to card
        var card_to = GCS_cardGet(p2, false);

        card_from.isShowed = false;
        card_from.ChainUNlock();

        if (swap == false)
            if (p1.location != p2.location || (p2.position & (int) CardPosition.FaceDown) > 0)
            {
                card_from.target.Clear();
                for (var i = 0; i < cards.Count; i++)
                    if (cards[i].gameObject.activeInHierarchy)
                        cards[i].removeTarget(card_from);
                card_from.disabled = false;
                card_from.set_data_beforeMove(card_from.get_data().clone());
                card_from.refreshData();
            }

        if ((p2.location & (uint) CardLocation.Overlay) > 0) card_from.p_beforeOverLayed = p1;

        var overlayed_cards_of_cardFrom = GCS_cardGetOverlayElements(card_from);
        var overlayed_cards_of_cardTo = GCS_cardGetOverlayElements(card_to);

        //begin analyse
        if (swap)
        {
            if (card_from != null)
                card_from.p = p2;
            if (card_to != null)
                card_to.p = p1;
        }
        else
        {
            if (card_to == null)
            {
                if (card_from != null)
                {
                    card_from.p = p2;
                }
            }
            else
            {
                if (card_from == card_to)
                {
                    if (card_from != null)
                    {
                        card_from.p = p2;
                    }
                }
                else
                {
                    if ((card_to.p.location & (uint) CardLocation.Overlay) == 0)
                    {
                        if ((card_to.p.location & (uint) CardLocation.MonsterZone) > 0 ||
                            (card_to.p.location & (uint) CardLocation.SpellZone) > 0)
                        {
                            if (card_from != null)
                            {
                                card_from.p = p2;
                            }
                                
                            if (card_to != null)
                            {
                                card_to.p = p1;
                            }
                        }
                        else
                        {
                            if (card_from != null) GCS_cardRelocate(card_from, p2);
                        }
                    }
                    else
                    {
                        if (card_from != null)
                        {
                            card_from.p = p2;
                            card_from.p.position += 500;
                        }
                    }
                }
            }
        }

        //overlay 
        if (card_from != null)
            for (var i = 0; i < overlayed_cards_of_cardFrom.Count; i++)
            {
                overlayed_cards_of_cardFrom[i].p.controller = card_from.p.controller;
                overlayed_cards_of_cardFrom[i].p.location = card_from.p.location | (uint) CardLocation.Overlay;
                overlayed_cards_of_cardFrom[i].p.sequence = card_from.p.sequence;
                overlayed_cards_of_cardFrom[i].p.position += 1000;
            }

        if (card_to != null)
            for (var i = 0; i < overlayed_cards_of_cardTo.Count; i++)
            {
                overlayed_cards_of_cardTo[i].p.controller = card_to.p.controller;
                overlayed_cards_of_cardTo[i].p.location = card_to.p.location | (uint) CardLocation.Overlay;
                overlayed_cards_of_cardTo[i].p.sequence = card_to.p.sequence;
                overlayed_cards_of_cardTo[i].p.position += 1000;
            }

        arrangeCards();
        return card_from;
    }

    private void GCS_cardRelocate(gameCard card_from, GPS p2)
    {
        var cardsInLocation = MHS_getBundle((int) p2.controller, (int) p2.location);
        cardsInLocation.Remove(card_from);
        cardsInLocation.Sort((left, right) =>
        {
            var a = 0;
            if (left.p.sequence > right.p.sequence)
                a = 1;
            else if (left.p.sequence < right.p.sequence) a = -1;
            return a;
        });
        if ((int) p2.sequence < 0)
            cardsInLocation.Insert(0, card_from);
        else if ((int) p2.sequence > cardsInLocation.Count)
            cardsInLocation.Insert(cardsInLocation.Count, card_from);
        else
            cardsInLocation.Insert((int) p2.sequence, card_from);
        for (var i = 0; i < cardsInLocation.Count; i++)
        {
            cardsInLocation[i].p.sequence = (uint)i;
        }
        card_from.p = p2;
    }

    private void arrangeCards()
    {
        //sort 
        cards.Sort((left, right) =>
        {
            var a = 1;
            if (left.p.controller > right.p.controller)
            {
                a = 1;
            }
            else if (left.p.controller < right.p.controller)
            {
                a = -1;
            }
            else
            {
                if (left.p.location == (uint) CardLocation.Hand && right.p.location != (uint) CardLocation.Hand)
                {
                    a = -1;
                }
                else if (left.p.location != (uint) CardLocation.Hand && right.p.location == (uint) CardLocation.Hand)
                {
                    a = 1;
                }
                else
                {
                    if ((left.p.location | (uint) CardLocation.Overlay) >
                        (right.p.location | (uint) CardLocation.Overlay))
                    {
                        a = -1;
                    }
                    else if ((left.p.location | (uint) CardLocation.Overlay) <
                             (right.p.location | (uint) CardLocation.Overlay))
                    {
                        a = 1;
                    }
                    else
                    {
                        if (left.p.sequence > right.p.sequence)
                        {
                            a = 1;
                        }
                        else if (left.p.sequence < right.p.sequence)
                        {
                            a = -1;
                        }
                        else
                        {
                            if ((left.p.location & (uint) CardLocation.Overlay) >
                                (right.p.location & (uint) CardLocation.Overlay))
                            {
                                a = -1;
                            }
                            else if ((left.p.location & (uint) CardLocation.Overlay) <
                                     (right.p.location & (uint) CardLocation.Overlay))
                            {
                                a = 1;
                            }
                            else
                            {
                                if (left.p.position > right.p.position)
                                    a = 1;
                                else if (left.p.position < right.p.position) a = -1;
                            }
                        }
                    }
                }
            }
            return a;
        });

        /////rebuild
        uint preController = 9999;
        uint preLocation = 9999;
        uint preSequence = 9999;

        uint sequenceWriter = 0;
        var positionWriter = 0;

        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
            {
                if (preController != cards[i].p.controller) sequenceWriter = 0;
                if ((preLocation | (uint) CardLocation.Overlay) != (cards[i].p.location | (uint) CardLocation.Overlay))
                    sequenceWriter = 0;
                if (preSequence != cards[i].p.sequence) positionWriter = 0;

                if ((cards[i].p.location & (uint) CardLocation.MonsterZone) == 0)
                    if ((cards[i].p.location & (uint) CardLocation.SpellZone) == 0)
                    {
                        cards[i].p.sequence = sequenceWriter;
                    }
                        

                if ((cards[i].p.location & (uint) CardLocation.Overlay) > 0)
                {
                    cards[i].p.position = positionWriter;
                    positionWriter++;
                }
                else
                {
                    sequenceWriter++;
                }

                preController = cards[i].p.controller;
                preLocation = cards[i].p.location;
                preSequence = cards[i].p.sequence;
            }
    }

    private void toDefaultHint()
    {
        gameField.setHint(ES_turnString + ES_phaseString);
        Program.I().hint.gameObject.SetActive(false);
    }

    private void toDefaultHintLogical()
    {
        gameField.setHintLogical(ES_turnString + ES_phaseString);
        Program.I().hint.gameObject.SetActive(false);
    }

    private void returnFromDeckEdit()
    {
        TcpHelper.CtosMessage_UpdateDeck(Program.I().newDeckManager.FromObjectDeckToCodedDeckForSideChange());
        returnServant = Program.I().selectServer;
    }

    public override void show()
    {
        base.show();
        //Program.I().light.transform.eulerAngles = new Vector3(70, 0, 0);
        
        Program.I().main_camera.transform.eulerAngles = Program.cameraRotation;
        Program.reMoveCam(getScreenCenter());
        gameField = new GameField();
        Program.cameraPosition = new Vector3(0, 95f, -37f);
        Program.cameraRotation = new Vector3(70, 0, 0);
        Program.I().main_camera.transform.position = new Vector3(0f, 95f, -37f);
        Program.I().main_camera.transform.localEulerAngles = new Vector3(70, 0, 0);
        //关闭UI
        BackgroundControl.Hide();
        UIHandler.CloseHomeUI();
        if (paused)
            try
            {
                EventDelegate.Execute(UIHelper.getByName<UIButton>(toolBar, "go_").onClick);
            }
            catch (Exception e)
            {
                paused = false;
            }
        //
        //切换BGM
        BGMHandler.ChangeBGM("duel_normal");
        //
        deckReserved = false;
        cantCheckGrave = false;
        surrended = false;
        Program.I().room.duelEnded = false;
        gameInfo.swaped = false;
        keys.Clear();
        currentMessageIndex = -1;
        result = duelResult.disLink;
        theWorldIndex = 0;
        //gameInfo.setTimeStill(0);
        sideReference.Clear();
        confirmedCards.Clear();
    }

    public override void hide()
    {
        Program.I().cardDescription.shiftCardShower(true);
        InAI = false;
        MessageBeginTime = 0;
        currentMessage = GameMessage.Waiting;
        Packages_ALL.Clear();
        Packages.Clear();
        cardsForConfirm.Clear();
        logicalClearChain();
        deckReserved = false;
        cantCheckGrave = false;
        if (isShowed)
        {
            clearResponse();
            Program.I().book.clear();
            Program.I().book.hide();
        }

        for (var i = 0; i < cards.Count; i++) cards[i].hide();
        paused = false;
        condition = Condition.N;
        base.hide();
    }

    public void ES_gameButtonClicked(gameButton btn)
    {
        if (btn.cookieString == "see_overlay")
        {
            if (btn.cookieCard != null)
            {
                btn.cookieCard.ES_exit_excited(true);
                var cas = GCS_cardGetOverlayElements(btn.cookieCard);
                for (var i = 0; i < cas.Count; i++)
                {
                    cas[i].isShowed = !cas[i].isShowed;
                    cas[i].flash_line_off();
                    //if (cas[i].isShowed)
                    //{
                    //    cas[i].set_text(GameStringHelper.diefang);
                    //}
                    //else
                    //{
                    //    cas[i].set_text("");
                    //}
                }

                realize();
                toNearest();
            }

            return;
        }

        switch (currentMessage)
        {
            case GameMessage.SelectBattleCmd:
            case GameMessage.SelectIdleCmd:
                if (btn.hint == InterString.Get("发动效果@ui"))
                {
                    if (btn.cookieCard.effects.Count > 0)
                    {
                        if (btn.cookieCard.effects.Count == 1)
                        {
                            var binaryMaster = new BinaryMaster();
                            binaryMaster.writer.Write(btn.cookieCard.effects[0].ptr);
                            sendReturn(binaryMaster.get());
                        }
                        else
                        {
                            var values = new List<messageSystemValue>();
                            for (var i = 0; i < btn.cookieCard.effects.Count; i++)
                                values.Add(new messageSystemValue
                                {
                                    hint = btn.cookieCard.effects[i].desc,
                                    value = btn.cookieCard.effects[i].ptr.ToString()
                                });
                            values.Add(new messageSystemValue {hint = InterString.Get("取消"), value = "hide"});
                            RMSshow_singleChoice("return", values);
                        }
                    }

                    return;
                }
                lastExcitedController = (int) btn.cookieCard.p.controller;
                lastExcitedLocation = (int) btn.cookieCard.p.location;
                var p = new BinaryMaster();
                p.writer.Write(btn.response);
                sendReturn(p.get());
                break;
            case GameMessage.SelectEffectYn:
                break;
            case GameMessage.SelectYesNo:
                break;
            case GameMessage.SelectOption:
                break;
            case GameMessage.SelectCard:
                break;
            case GameMessage.SelectUnselect:
                break;
            case GameMessage.SelectChain:
                break;
            case GameMessage.SelectPlace:
                break;
            case GameMessage.SelectPosition:
                break;
            case GameMessage.SelectTribute:
                break;
            case GameMessage.SortChain:
                break;
            case GameMessage.SelectCounter:
                break;
            case GameMessage.SelectSum:
                break;
            case GameMessage.SelectDisfield:
                break;
            case GameMessage.AnnounceRace:
                break;
            case GameMessage.AnnounceAttrib:
                break;
            case GameMessage.AnnounceCard:
                break;
            case GameMessage.AnnounceNumber:
                break;
        }
    }

    public void ES_gameUIbuttonClicked(gameUIbutton btn)
    {
        if (btn.hashString == "clearCounter")
        {
            for (var i = 0; i < allCardsInSelectMessage.Count; i++)
            {
                allCardsInSelectMessage[i].counterSELcount = 0;
                allCardsInSelectMessage[i].show_number(allCardsInSelectMessage[i].counterSELcount);
            }

            return;
        }

        if (btn.hashString == "sendSelected")
        {
            sendSelectedCards();
            return;
        }

        if (btn.hashString == "hide_all_card")
        {
            if (flagForTimeConfirm)
            {
                flagForTimeConfirm = false;
                MessageBeginTime = Program.TimePassed();
            }

            clearAllShowed();
            return;
        }

        if (btn.hashString == "swap")
        {
            GCS_swapALL();
            return;
        }

        if (btn.hashString == "cancelPlace")
        {
            cancelSelectPlace();
            return;
        }

        switch (currentMessage)
        {
            case GameMessage.SelectBattleCmd:
            case GameMessage.SelectIdleCmd:
                var p = new BinaryMaster();
                p.writer.Write(btn.response);
                sendReturn(p.get());
                break;
            case GameMessage.SelectEffectYn:
            case GameMessage.SelectYesNo:
            case GameMessage.SelectCard:
            case GameMessage.SelectUnselect:
            case GameMessage.SelectTribute:
            case GameMessage.SelectChain:
                clearAllShowedB = true;
                var binaryMaster = new BinaryMaster();
                binaryMaster.writer.Write(btn.response);
                sendReturn(binaryMaster.get());
                break;
            case GameMessage.SelectPlace:
                break;
            case GameMessage.SelectPosition:
                break;
            case GameMessage.SortChain:
                break;
            case GameMessage.SelectCounter:
                break;
            case GameMessage.SelectSum:
                break;
            case GameMessage.SelectDisfield:
                break;
            case GameMessage.AnnounceRace:
                break;
            case GameMessage.AnnounceAttrib:
                break;
            case GameMessage.AnnounceCard:
                clearResponse();
                realize();
                toNearest();
                RMSshow_input("AnnounceCard", InterString.Get("请输入关键字："), "");
                break;
            case GameMessage.AnnounceNumber:
                break;
        }
    }

    private void GCS_swapALL(bool realized = true)
    {
        isFirst = !isFirst;
        for (var i = 0; i < cards.Count; i++)
        {
            cards[i].p.controller = 1 - cards[i].p.controller;
            cards[i].isShowed = false;
            cards[i].controllerBased = 1 - cards[i].controllerBased;
        }

        gameInfo.swaped = !gameInfo.swaped;
        if (realized) realize(true);
    }

    public void cancelSelectPlace()
    {
        clearAllSelectPlace();
        BinaryMaster binaryMaster = new BinaryMaster();
        byte[] resp = new byte[3];
        resp[0] = (byte)localPlayer(0);
        resp[1] = 0;
        resp[2] = 0;
        binaryMaster.writer.Write(resp);
        sendReturn(binaryMaster.get());
    }

    private void clearAllShowed()
    {
        for (var i = 0; i < cards.Count; i++)
            if (cards[i].gameObject.activeInHierarchy)
                cards[i].isShowed = false;
        realize();
        toNearest();
    }

    public bool inTheWorld()
    {
        return currentMessageIndex < theWorldIndex;
    }

    public void sendReturn(byte[] buffer)
    {
        if (paused) EventDelegate.Execute(UIHelper.getByName<UIButton>(toolBar, "go_").onClick);
        clearResponse();
        if (handler != null) handler(buffer);
    }

    public void ES_cardClicked(gameCard card)
    {
        card.showMeLeft();
        if ((card.p.location & (uint)CardLocation.Hand) > 0 && !card.forSelect)
            UIHelper.playSound("SE_DUEL/SE_CARD_MOVE_0" + UnityEngine.Random.Range(1, 5), 1);
        else
            SEHandler.PlayInternalAudio("se_sys/SE_MENU_SELECT_01");
        if (card != null)
        {
            lastExcitedController = (int) card.p.controller;
            lastExcitedLocation = (int) card.p.location;
        }
        if (card.forSelect)
        {
            var vector_of_begin = card.gameObject_face.transform.position;
            vector_of_begin = Program.I().main_camera.WorldToScreenPoint(vector_of_begin);
            if((card.p.location &(uint)CardLocation.Hand) >0 && card.p.controller == 1)
                gameField.btn_decide.show(vector_of_begin + new Vector3(0f, -150f * Screen.height / 1080f, 0f));
            else
                gameField.btn_decide.show(vector_of_begin + new Vector3(0f, 150f * Screen.height / 1080f, 0f));
            gameField.btn_decide.cookieCard = card;
        }
    }

    public void ES_cardClickedAndConfirmed(gameCard card)
    {
        if (card != null)
        {
            lastExcitedController = (int)card.p.controller;
            lastExcitedLocation = (int)card.p.location;
        }

        switch (currentMessage)
        {
            case GameMessage.SelectBattleCmd:
                break;
            case GameMessage.SelectIdleCmd:
                break;
            case GameMessage.SelectEffectYn:
                break;
            case GameMessage.SelectYesNo:
                break;
            case GameMessage.SelectOption:
                break;
            case GameMessage.SortChain:
            case GameMessage.SortCard:
                if (card.forSelect)
                {
                    for (var i = 0; i < cardsInSort.Count; i++) cardsInSort[i].show_number(0);
                    var avaliableSortOptions = new List<int>();
                    for (var i = 0; i < card.sortOptions.Count; i++) avaliableSortOptions.Add(card.sortOptions[i]);
                    for (var i = 0; i < ES_sortResult.Count; i++) avaliableSortOptions.Remove(ES_sortResult[i].option);
                    if (avaliableSortOptions.Count == 0)
                    {
                        var remove = new List<sortResult>();
                        for (var i = 0; i < ES_sortResult.Count; i++)
                            if (ES_sortResult[i].card == card)
                                remove.Add(ES_sortResult[i]);
                        for (var i = 0; i < remove.Count; i++) ES_sortResult.Remove(remove[i]);
                        remove.Clear();
                    }

                    if (avaliableSortOptions.Count == 1)
                        ES_sortResult.Add(new sortResult
                        {
                            card = card,
                            option = avaliableSortOptions[0]
                        });
                    if (avaliableSortOptions.Count > 1)
                    {
                        ES_sortCurrent.Clear();
                        for (var i = 0; i < avaliableSortOptions.Count; i++)
                            ES_sortCurrent.Add(new sortResult
                            {
                                card = card,
                                option = avaliableSortOptions[i]
                            });
                        var values = new List<messageSystemValue>();
                        values.Add(new messageSystemValue { hint = InterString.Get("顺发动顺序排序"), value = "shun" });
                        values.Add(new messageSystemValue { hint = InterString.Get("逆发动顺序排序"), value = "fan" });
                        values.Add(new messageSystemValue { hint = InterString.Get("确认其他场上的卡"), value = "hide" });
                        RMSshow_singleChoice("sort", values);
                    }

                    if (ES_sortResult.Count == ES_sortSum)
                        sendSorted();
                    else
                        for (var i = 0; i < ES_sortResult.Count; i++)
                            ES_sortResult[i].card.show_number(i + 1, true);
                }
                break;
            case GameMessage.SelectCard:
            case GameMessage.SelectTribute:
            case GameMessage.SelectSum:
                if (card.forSelect)
                {
                    var selectable = false;

                    for (var i = 0; i < cardsSelectable.Count; i++)
                        if (card == cardsSelectable[i])
                            selectable = true;

                    if (selectable)
                    {
                        var selected = false;
                        for (var i = 0; i < cardsSelected.Count; i++)
                            if (card == cardsSelected[i])
                                selected = true;
                        if (selected == false)
                            cardsSelected.Add(card);
                        else
                            cardsSelected.Remove(card);
                    }
                    else
                    {
                        cardsSelected.Remove(card);
                    }
                    realizeCardsForSelect("请选择卡片。");
                }
                break;
            case GameMessage.SelectUnselect:
                if (card.forSelect)
                {
                    cardsSelected.Add(card);
                    gameInfo.removeHashedButton("sendSelected");
                    Program.I().ocgcore.gameField.btn_confirm.hide();
                    Program.I().cardSelection.finishable = false;
                    sendSelectedCards();
                    realize();
                    toNearest();
                }
                break;
            case GameMessage.SelectChain:
                if (card.forSelect)
                    if (card.effects.Count > 0)
                    {
                        if (card.effects.Count == 1)
                        {
                            var binaryMaster = new BinaryMaster();
                            binaryMaster.writer.Write(card.effects[0].ptr);
                            sendReturn(binaryMaster.get());
                        }
                        else
                        {
                            var values = new List<messageSystemValue>();
                            for (var i = 0; i < card.effects.Count; i++)
                            {
                                if (card.effects[i].flag == 0)
                                {
                                    if (card.effects[i].desc.Length > 2)
                                        values.Add(new messageSystemValue
                                        { hint = card.effects[i].desc, value = card.effects[i].ptr.ToString() });
                                    else
                                        values.Add(new messageSystemValue
                                        {
                                            hint = InterString.Get("发动效果@ui"),
                                            value = card.effects[i].ptr.ToString()
                                        });
                                }

                                if (card.effects[i].flag == 1)
                                    values.Add(new messageSystemValue
                                    {
                                        hint = InterString.Get("适用「[?]」的效果", card.get_data().Name),
                                        value = card.effects[i].ptr.ToString()
                                    });
                                if (card.effects[i].flag == 2)
                                    values.Add(new messageSystemValue
                                    {
                                        hint = InterString.Get("重置「[?]」的控制权", card.get_data().Name),
                                        value = card.effects[i].ptr.ToString()
                                    });
                            }
                            values.Add(new messageSystemValue { hint = InterString.Get("取消"), value = "hide" });
                            RMSshow_singleChoice("return", values);
                        }
                    }
                break;
            case GameMessage.SelectPlace:
                break;
            case GameMessage.SelectPosition:
                break;
            case GameMessage.SelectCounter:
                if (card.forSelect)
                {
                    if (card.counterSELcount < card.counterCANcount) card.counterSELcount++;
                    var sum = 0;
                    for (var i = 0; i < allCardsInSelectMessage.Count; i++)
                        sum += allCardsInSelectMessage[i].counterSELcount;
                    if (sum == ES_min)
                    {
                        var binaryMaster = new BinaryMaster();
                        for (var i = 0; i < allCardsInSelectMessage.Count; i++)
                            binaryMaster.writer.Write((short)allCardsInSelectMessage[i].counterSELcount);
                        sendReturn(binaryMaster.get());
                    }
                    else
                    {
                        for (var i = 0; i < allCardsInSelectMessage.Count; i++)
                            allCardsInSelectMessage[i].show_number(allCardsInSelectMessage[i].counterSELcount);
                    }
                }
                break;
            case GameMessage.SelectDisfield:
                break;
            case GameMessage.AnnounceRace:
                break;
            case GameMessage.AnnounceAttrib:
                break;
            case GameMessage.AnnounceCard:
                if (card.forSelect)
                {
                    var binaryMaster = new BinaryMaster();
                    binaryMaster.writer.Write((uint)card.get_data().Id);
                    sendReturn(binaryMaster.get());
                }
                break;
            case GameMessage.AnnounceNumber:
                break;
        }
    }

    public void ES_placeSelected(placeSelector data)
    {
        Debug.Log("PlaceSelectorDataCount: " + data.data.Count());
        data.selected = !data.selected;
        switch (currentMessage)
        {
            case GameMessage.SelectPlace:
            case GameMessage.SelectDisfield:
                var all = 0;
                var binaryMaster = new BinaryMaster();
                for (var i = 0; i < placeSelectors.Count; i++)
                    if (placeSelectors[i].selected)
                    {
                        Debug.Log("selected: " + i + placeSelectors[i].data.Count());
                        binaryMaster.writer.Write(placeSelectors[i].data);
                        all++;
                    }

                if (all == ES_min)
                {
                    ES_min = -2;
                    sendReturn(binaryMaster.get());
                }

                if (ES_min == -2) clearAllSelectPlace();
                break;
            default:
                clearResponse();
                break;
        }
    }

    public List<gameCard> cardsWaitToSend = new List<gameCard>();

    public override void ES_RMS(string hashCode, List<messageSystemValue> result)
    {
        base.ES_RMS(hashCode, result);
        BinaryMaster binaryMaster;
        switch (hashCode)
        {
            case "return":
                if (result[0].value != "hide")
                    try
                    {
                        binaryMaster = new BinaryMaster();
                        binaryMaster.writer.Write(int.Parse(result[0].value));
                        sendReturn(binaryMaster.get());
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                else
                {
                    if(Program.I().cardSelection.hideByRMS)
                        Program.I().cardSelection.ShowWithoutAction();
                }
                break;
            case "autoForceChainHandler":
                if (result[0].value != "hide")
                {
                    if (result[0].value == "yes")
                    {
                        autoForceChainHandler = autoForceChainHandlerType.autoHandleAll;
                        try
                        {
                            int answer = -1;
                            foreach (var card in chainCards)
                            {
                                foreach (var effect in card.effects)
                                {
                                    if (effect.forced)
                                    {
                                        answer = effect.ptr;
                                        break;
                                    }
                                }
                                if (answer >= 0) break;
                            }
                            binaryMaster = new BinaryMaster();
                            binaryMaster.writer.Write(answer >= 0 ? answer : 0);
                            sendReturn(binaryMaster.get());
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e);
                        }
                    }

                    if (result[0].value == "no")
                    {
                        autoForceChainHandler = autoForceChainHandlerType.afterClickManDo;
                        Program.I().cardSelection.show(cardsWaitToSend, false, 1, 1, InterString.Get("[?]，@n请选择需要发动或处理的强制效果，或者选择快速效果发动。", ES_hint));
                    }
                }

                break;
            case "returnMultiple":
                binaryMaster = new BinaryMaster();
                uint res = 0;
                foreach (var item in result)
                    try
                    {
                        res |= uint.Parse(item.value);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }

                binaryMaster.writer.Write(res);
                sendReturn(binaryMaster.get());
                break;
            case "AnnounceCard":
                var datas = CardsManager.search(result[0].value, ES_searchCode);
                var max = datas.Count;
                if (max > 49) max = 49;
                List<gameCard> cards = new List<gameCard>();
                for (var i = 0; i < max; i++)
                {
                    var p = new GPS
                    {
                        controller = 0,
                        location = (uint) CardLocation.Search,
                        sequence = (uint) i,
                        position = 0
                    };
                    var card = GCS_cardCreate(p);
                    card.set_data(datas[i]);
                    cards.Add(card);
                    card.forSelect = true;
                    //card.add_one_decoration(Program.I().mod_ocgcore_decoration_card_selecting, 4, Vector3.zero, "card_selecting");
                }
                Program.I().cardSelection.show(cards, true, 1, 1, InterString.Get("请选择需要宣言的卡片。"));
                realize();
                gameInfo.addHashedButton("clear", 0, superButtonType.no, InterString.Get("重新输入@ui"));
                Program.I().ocgcore.gameField.ShowCancelButton("重新输入");
                toNearest();
                //gameField.setHint(InterString.Get("请选择需要宣言的卡片。"));
                break;
            case "sort":
                if (result[0].value != "hide")
                {
                    for (var i = 0; i < cardsInSort.Count; i++) cardsInSort[i].show_number(0);
                    if (result[0].value == "shun")
                        for (var i = 0; i < ES_sortCurrent.Count; i++)
                            ES_sortResult.Add(ES_sortCurrent[i]);
                    if (result[0].value == "fan")
                        for (var i = 0; i < ES_sortCurrent.Count; i++)
                            ES_sortResult.Add(ES_sortCurrent[ES_sortCurrent.Count - i - 1]);
                    if (ES_sortResult.Count == ES_sortSum)
                        sendSorted();
                    else
                        for (var i = 0; i < ES_sortResult.Count; i++)
                            ES_sortResult[i].card.show_number(i + 1, true);
                }

                break;
            case "RockPaperScissors":
            {
                try
                {
                    binaryMaster = new BinaryMaster();
                    binaryMaster.writer.Write(int.Parse(result[0].value));
                    sendReturn(binaryMaster.get());
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
                break;
        }
    }

    public override void ES_RMS_ForcedYesNo(messageSystemValue result)
    {
        base.ES_RMS_ForcedYesNo(result);
        if (result.value == "yes")
        {
            surrended = true;
            if (TcpHelper.tcpClient != null && TcpHelper.tcpClient.Connected)
            {
                if (paused) EventDelegate.Execute(UIHelper.getByName<UIButton>(toolBar, "go_").onClick);
                TcpHelper.CtosMessage_Surrender();
            }
            else
            {
                onExit();
            }
        }
    }

    public void onDuelResultConfirmed()
    {
        Program.I().room.joinWithReconnect = false;

        if (Program.I().room.duelEnded || surrended || TcpHelper.tcpClient == null ||
            TcpHelper.tcpClient.Connected == false)
        {
            surrended = false;
            Program.I().room.duelEnded = false;
            Program.I().room.needSide = false;
            Program.I().room.sideWaitingObserver = false;
            onExit();
            return;
        }

        if (Program.I().room.needSide)
        {
            Program.I().room.needSide = false;
            RMSshow_none(InterString.Get("右侧为您准备了对手上一局使用的卡。"));
            Program.I().newDeckManager.shiftCondition(NewDeckManager.Condition.changeSide);
            returnTo();
            Program.I().newDeckManager.deck = TcpHelper.deck;
            Program.I().newDeckManager.inDeckName = Config.Get("deckInUse", "miaowu");
            Program.I().newDeckManager.FromCodedDeckToObjectDeck();
            Program.I().newDeckManager.returnAction = returnFromDeckEdit;
            return;
        }

        if (condition != Condition.duel)
        {
            hideCaculator();
            return;
        }

        RMSshow_yesOrNoForce(InterString.Get("你确定要投降吗？"), new messageSystemValue {value = "yes", hint = "yes"},
            new messageSystemValue {value = "no", hint = "no"});
    }

    private void sendSorted()
    {
        var m = new BinaryMaster();
        var bytes = new byte[ES_sortResult.Count];
        for (var i = 0; i < ES_sortResult.Count; i++) bytes[ES_sortResult[i].option] = (byte) i;
        for (var i = 0; i < ES_sortResult.Count; i++) m.writer.Write(bytes);
        sendReturn(m.get());
    }

    public override void ES_mouseDownRight()
    {
        if (gameInfo.queryHashedButton("sendSelected")) return;
        if (flagForCancleChain) return;
        if (gameInfo.queryHashedButton("hide_all_card"))
            if (flagForTimeConfirm)
                return;
        if (gameInfo.queryHashedButton("cancleSelected")) return;
        if (gameInfo.queryHashedButton("cancelPlace")) return;
        rightExcited = true;
        //gameInfo.ignoreChain_set(true);
        base.ES_mouseDownRight();
    }

    public override void ES_mouseDownEmpty()
    {
        //if (Program.I().setting.setting.spyer.value == false)
        //    if (gameInfo.queryHashedButton("hide_all_card") == false)
        //        //gameInfo.keepChain_set(true);
        //        leftExcited = true;
        //base.ES_mouseDownEmpty();
    }

    public override void ES_mouseUpEmpty()
    {
        //if (Program.I().setting.setting.spyer.value)
        //{
        //    if (cantCheckGrave)
        //        RMSshow_none(InterString.Get("不能确认墓地里的卡，监控全局卡片功能暂停使用。"));
        //    else
        //        Program.I().cardDescription.shiftCardShower(false);
        //}
        Program.I().cardDescription.hide();
        Program.I().new_ui_cardList.hide();
        gameField.hideSpButtons();
        gameField.btn_decide.hide();

        if (gameInfo.queryHashedButton("hide_all_card"))
        {
            if (flagForTimeConfirm)
            {
                flagForTimeConfirm = false;
                MessageBeginTime = Program.TimePassed();
            }

            clearAllShowed();
        }
        else
        {
            //if (Program.I().setting.setting.spyer.value == false)
            //    if (leftExcited)
            //        if (Input.GetKey(KeyCode.A) == false)
            //            leftExcited = false;
            ////gameInfo.keepChain_set(false);
        }

        base.ES_mouseUpEmpty();
    }

    public override void ES_mouseUpGameObject(GameObject gameObject)
    {
        if (gameObject == gameInfo.instance_lab.gameObject)
        {
            ES_mouseUpEmpty();
            return;
        }

        if (leftExcited)
            if (Input.GetKey(KeyCode.A) == false)
                leftExcited = false;
        //gameInfo.keepChain_set(false);
        base.ES_mouseUpGameObject(gameObject);
        if(gameObject.name == "mod_simple_quad(Clone)" 
            &&
            gameObject.transform.parent != null
            && 
            gameObject.transform.parent.name == "mod_ocgcore_card(Clone)")
        {
            gameField.hideSpButtons(null, false);
            gameField.hideDecideButton(gameObject.transform.parent.gameObject);
        }
        else if(gameObject.name == "event" 
            &&
            gameObject.transform.parent != null
            &&
            gameObject.transform.parent.parent != null
            &&
            gameObject.transform.parent.parent.name == "mod_ocgcore_card(Clone)")
        {
            gameField.hideSpButtons(null, false);
            gameField.hideDecideButton(gameObject.transform.parent.parent.gameObject);
        }
        else if(gameObject.name == "small_")
        {
            gameField.hideSpButtons();
            gameField.btn_decide.hide();
        }
        else if(gameObject.name == "big_")
        {
            gameField.hideSpButtons();
            gameField.btn_decide.hide();
        }
        else if (gameObject.name == "base_cardDescription")
        {
            gameField.hideSpButtons();
            gameField.btn_decide.hide();
        }
        else if (gameObject.name == "description_")
        {
            gameField.hideSpButtons();
            gameField.btn_decide.hide();
        }
        else if (gameObject.name == "button_cardOnList")
        {
            gameField.hideSpButtons();
            gameField.btn_decide.hide();
        }
        else if (gameObject.name == "base_cardList")
        {
            gameField.hideSpButtons();
            gameField.btn_decide.hide();
        }
        else if (gameObject.name == "mod_ocgcore_hidden_collider(Clone)")
        {
            gameField.hideSpButtons(gameObject);
            gameField.btn_decide.hide();
        }
        else if (gameObject.name == "base_cardSelect")
        {
            gameField.hideSpButtons(gameObject);
            gameField.btn_decide.hide();
        }
        else if (gameObject.name == "id_card_description")
        {
            gameField.hideSpButtons(gameObject);
            gameField.btn_decide.hide();
        }
        else if (gameObject.name == "pic_button")
        {
            gameField.hideSpButtons(gameObject);
            gameField.btn_decide.hide();
        }
        else if (gameObject.name == "back_cardDetail")
        {
            gameField.hideSpButtons(gameObject);
            gameField.btn_decide.hide();
        }
        else if (gameObject.name == "duelLog_")
        {
            gameField.hideSpButtons(gameObject);
            gameField.btn_decide.hide();
            Program.I().new_ui_cardList.hide();
        }
        else
        {
            Program.I().cardDescription.hide();
            Program.I().new_ui_cardList.hide();
            gameField.hideSpButtons();
            gameField.btn_decide.hide();
        }
    }

    public override void ES_mouseUpRight()
    {
        base.ES_mouseUpRight();

        if (Program.I().character.isShowed)
        {
            Program.I().character.Hide();
            return;
        }
        else if (Program.I().appearance.isShowed)
        {
            Program.I().appearance.Hide();
            return;
        }
        else if (Program.I().setting.isShowed)
        {
            Program.I().setting.hide();
            return;
        }
        else if (Program.I().cardDetail.isShowed)
        {
            Program.I().cardDetail.Hide();
            return;
        }
        else if (PhaseUIBehaviour.isShowed)
        {
            PhaseUIBehaviour.hide();
            return;
        }
        else if(Program.I().cardSelection.isShowed && Program.I().cardSelection.exitable)
        {
            Program.I().cardSelection.hide();
            return;
        }

        if (rightExcited)
            if (Input.GetKey(KeyCode.S) == false)
                rightExcited = false;
        //gameInfo.ignoreChain_set(false);
        if (gameInfo.queryHashedButton("sendSelected"))
        {
            sendSelectedCards();
            Program.I().cardSelection.HideWithoutAction();
            return;
        }

        if (flagForCancleChain)
        {
            flagForCancleChain = false;
            clearAllShowedB = true;
            var binaryMaster = new BinaryMaster();
            binaryMaster.writer.Write(-1);
            sendReturn(binaryMaster.get());
            Program.I().cardSelection.HideWithoutAction();
            return;
        }

        if (gameInfo.queryHashedButton("hide_all_card"))
            if (flagForTimeConfirm)
            {
                flagForTimeConfirm = false;
                MessageBeginTime = Program.TimePassed();
                clearAllShowed();
                return;
            }

        if (gameInfo.queryHashedButton("cancleSelected"))
        {
            var binaryMaster = new BinaryMaster();
            binaryMaster.writer.Write(-1);
            sendReturn(binaryMaster.get());
            Program.I().cardSelection.HideWithoutAction();
            return;
        }

        if (gameInfo.queryHashedButton("cancelPlace"))
        {
            cancelSelectPlace();
            return;
        }
    }

    private void animation_confirm(gameCard target)//mark 动画确认
    {
        Program.I().cardDescription.setData(target.get_data(),
            target.p.controller == 0 ? GameTextureManager.myBack : GameTextureManager.opBack,
            target.tails.managedString);
        target.animation_confirm_screenCenter(new Vector3(-20, 0, 0), 0.2f, 0.5f);

        Transform child = target.gameObject_face.transform.parent;
        if((target.p.location & ((uint)CardLocation.Grave + (uint)CardLocation.Removed)) > 0)
        {
            Sequence quence = DOTween.Sequence();
            quence.Append(child.DOScale(new Vector3(1f, 1f, 1f), 0.2f));
            quence.Join(child.DORotate(new Vector3(90, 0, 0), 0.2f));
            quence.AppendInterval(0.5f);
            quence.Append(child.DOScale(new Vector3(0.25f, 0.25f, 0.25f), 0.2f));
            quence.Join(child.DORotate(new Vector3(0, 0, 180), 0.2f));
        }
    }

    public void animation_show_card_code(int code)
    {
        code_for_show = code;
        animation_show_card_code_handler();
        Sleep(30);
    }

    private async void animation_show_card_code_handler()
    {
        var shower =
            create(Program.I().Pro1_CardShower, Program.I().ocgcore.centre(), Vector3.zero, false,
                Program.I().ui_main_2d).GetComponent<pro1CardShower>();
        shower.card.mainTexture = await GameTextureManager.GetCardPicture(code_for_show);
        shower.mask.mainTexture = GameTextureManager.Mask;
        shower.disable.mainTexture = GameTextureManager.negated;
        shower.gameObject.transform.localScale = Utils.UIHeight() / 650f * Vector3.one;
        destroy(shower.gameObject, 0.5f);
    }

    private class linkMask
    {
        public bool eff;
        public GameObject gameObject;
        public GPS p;
    }

    //handle messages
    private enum autoForceChainHandlerType
    {
        autoHandleAll,
        manDoAll,
        afterClickManDo
    }


    private class sortResult
    {
        public gameCard card;
        public int option;
    }

    private enum cardRuleComdition
    {
        meUpAtk,
        meUpDef,
        meDownAtk,
        meDownDef,
        opUpAtk,
        opUpDef,
        opDownAtk,
        opDownDef,
        meUpDeck,
        meDownDeck,
        opUpDeck,
        opDownDeck,
        meUpExDeck,
        meDownExDeck,
        opUpExDeck,
        opDownExDeck
    }

    private enum duelResult
    {
        disLink,
        win,
        lose,
        draw
    }
}