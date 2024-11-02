using Percy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shortcuts : MonoBehaviour
{
    public static bool upmode;
    public static bool lockCamera;
    public static bool lineShift = true;
    bool hasChanged;
    Transform ui_back_ground_2d;
    Transform ui_main_2d;
    Transform new_toolBar_watchRecord;
    Transform new_toolBar_watchDuel;
    Transform chat_;
    Transform confirmed;
    public float timescale = 1;
    YGOSharp.OCGWrapper.Enums.GameMessage message = YGOSharp.OCGWrapper.Enums.GameMessage.Waiting;
    void Start()
    {
        upmode = false;
        hasChanged = false;
        ui_back_ground_2d = GameObject.Find("ui_back_ground_2d").transform;
        ui_main_2d = GameObject.Find("ui_main_2d").transform;
        chat_ = GameObject.Find("ui_back_ground_2d").transform.Find("new_cardDescriptionRemaster/chat_");
        confirmed = GameObject.Find("ui_back_ground_2d").transform.Find("new_gameInfoRemaster/confirmed");
    }


    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (Program.I().character.isShowed)
                Program.I().character.Hide();
            else if (Program.I().appearance.isShowed)
                Program.I().appearance.Hide();
            else if (Program.I().deckDecoration.isShowed)
                Program.I().deckDecoration.Hide();
            else if (Program.I().searchFliter.isShowed)
                Program.I().searchFliter.Hide();
            else if (Program.I().searchFliter.sortIsShowed)
                Program.I().searchFliter.HideSort();
            else if (Program.I().setting.isShowed)
                Program.I().setting.hide();
            else if (PhaseUIBehaviour.isShowed)
                PhaseUIBehaviour.hide();
            else if (Program.I().cardDetail.isShowed)
                Program.I().cardDetail.Hide();
            else if (Program.I().aiRoom.isShowed)
                Program.I().shiftToServant(Program.I().menu);
            else if (Program.I().animation.isPlaying)
                Program.I().animation.StopPlaying();
            else if (Program.I().animation.isShowed)
                Program.I().animation.hide();
            else if (Program.I().selectDeck.isShowed)
                Program.I().selectDeck.onClickExit();
            else if (Program.I().newDeckManager.isShowed)
            {

            }
                
            else
                Program.I().setting.show();
        }

        if(Program.I().ocgcore.gameField != null && Program.I().ocgcore.gameField.gameObject != null)
        {

        }
        else if (Program.I().deckManager.isShowed)
        {

        }
        else
        {
            if (Input.GetMouseButtonUp(1))
            {
                if (Program.I().character.isShowed)
                    Program.I().character.Hide();
                else if (Program.I().appearance.isShowed)
                    Program.I().appearance.Hide();
                else if (Program.I().deckDecoration.isShowed)
                    Program.I().deckDecoration.Hide();
                else if (Program.I().searchFliter.isShowed)
                    Program.I().searchFliter.Hide();
                else if (Program.I().searchFliter.sortIsShowed)
                    Program.I().searchFliter.HideSort();
                else if (Program.I().setting.isShowed)
                    Program.I().setting.hide();
                else if (Program.I().cardSelection.isShowed && Program.I().cardSelection.exitable)
                    Program.I().cardSelection.hide();
                else if (PhaseUIBehaviour.isShowed)
                    PhaseUIBehaviour.hide();
                else if (Program.I().cardDetail.isShowed)
                    Program.I().cardDetail.Hide();
                else if (Program.I().aiRoom.isShowed)
                    Program.I().shiftToServant(Program.I().menu);
                else if (Program.I().animation.isPlaying)
                    Program.I().animation.StopPlaying();
                else if (Program.I().animation.isShowed)
                    Program.I().animation.hide();
            }
        }
        //if (Input.GetKeyUp(KeyCode.F2))
        //{
        //    if (Program.I().setting.isShowed)
        //    {
        //        Program.I().setting.hide();
        //    }
        //    else
        //        Program.I().setting.show();
        //}

        if (Input.GetKeyUp(KeyCode.F4))
        {
            if(Program.I().cardDescription.isShowed)
            {
                Program.I().cardDescription.hide();
                Program.I().cardDescription.forcedClosed = true;
                chat_.GetComponent<UILabel>().fontSize = 0;
                confirmed.GetComponent<UILabel>().fontSize = 0;
            }
            else
            {
                Program.I().cardDescription.show();
                Program.I().cardDescription.forcedClosed = false;
                chat_.GetComponent<UILabel>().fontSize = 20;
                confirmed.GetComponent<UILabel>().fontSize = 20;
            }
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (upmode)
            {
                upmode = false;
                Program.PrintToChat(InterString.Get("开启UI"));
            }                
            else
            {
                upmode = true;
                Program.PrintToChat(InterString.Get("关闭UI"));
            }
                
            hasChanged = true;
        }
        //if (Input.GetKeyDown(KeyCode.F6))
        //{
        //    if (lockCamera)
        //    {
        //        lockCamera = false;
        //        Program.PrintToChat(InterString.Get("解锁镜头"));
        //    }
        //    else
        //    {
        //        lockCamera = true;
        //        Program.PrintToChat(InterString.Get("锁定镜头"));
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.F7))
        //{
        //    if (lineShift)
        //    {
        //        lineShift = false;
        //        Program.PrintToChat(InterString.Get("卡片下方展示"));
        //        Program.I().ocgcore.realize();
        //        Program.I().ocgcore.toNearest();
        //    }
        //    else
        //    {
        //        lineShift = true;
        //        Program.PrintToChat(InterString.Get("卡片上方展示"));
        //        Program.I().ocgcore.realize();
        //        Program.I().ocgcore.toNearest();
        //    }
        //}


        if (hasChanged)
        {
            if (upmode)
            {
                ui_back_ground_2d.gameObject.SetActive(false);
                if (new_toolBar_watchRecord != null) new_toolBar_watchRecord.gameObject.SetActive(false);
                else
                {
                    new_toolBar_watchRecord = ui_main_2d.Find("new_toolBar_watchRecord(Clone)");
                    if (new_toolBar_watchRecord != null) new_toolBar_watchRecord.gameObject.SetActive(false);
                }
                if (new_toolBar_watchDuel != null) new_toolBar_watchDuel.gameObject.SetActive(false);
                else
                {
                    new_toolBar_watchDuel = ui_main_2d.Find("new_toolBar_watchDuel(Clone)");
                    if (new_toolBar_watchDuel != null) new_toolBar_watchDuel.gameObject.SetActive(false);
                }
            }
            else
            {
                ui_back_ground_2d.gameObject.SetActive(true);
                if (new_toolBar_watchRecord != null) new_toolBar_watchRecord.gameObject.SetActive(true);
                else
                {
                    new_toolBar_watchRecord = ui_main_2d.Find("new_toolBar_watchRecord(Clone)");
                    if (new_toolBar_watchRecord != null) new_toolBar_watchRecord.gameObject.SetActive(true);
                }
                if (new_toolBar_watchDuel != null) new_toolBar_watchDuel.gameObject.SetActive(true);
                else
                {
                    new_toolBar_watchDuel = ui_main_2d.Find("new_toolBar_watchDuel(Clone)");
                    if (new_toolBar_watchDuel != null) new_toolBar_watchDuel.gameObject.SetActive(true);
                }
            }
            hasChanged = false;
        }

        //Time.timeScale = timescale;
        //if (message != Program.I().ocgcore.currentMessage)
        //{
        //    message = Program.I().ocgcore.currentMessage;
        //    Debug.Log(message);
        //}
    }
}
