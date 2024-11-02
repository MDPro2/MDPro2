using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseUIBehaviour : MonoBehaviour
{
    public static UIButton dp;
    public static UIButton sp;
    public static UIButton mp1;
    public static UIButton bp;
    public static UIButton mp2;
    public static UIButton ep;

    public static bool isShowed;
    public static bool clickable;
    public static PhaseUIBehaviour instance;

    public static int retOfBp = -1;
    public static int retOfEp = -1;
    public static int retOfMp = -1;
    private void onBP()
    {
        if (bp.GetComponent<BoxCollider>().size.x == 175)
        {
            var m = new BinaryMaster();
            m.writer.Write(retOfBp);
            Program.I().ocgcore.sendReturn(m.get());
        }
        hide();
    }
    private void onEP()
    {
        if (ep.GetComponent<BoxCollider>().size.x == 175)
        {
            var m = new BinaryMaster();
            m.writer.Write(retOfEp);
            Program.I().ocgcore.sendReturn(m.get());
        }
        hide();
    }
    private void onMP()
    {
        if (mp2.GetComponent<BoxCollider>().size.x == 175)
        {
            var m = new BinaryMaster();
            m.writer.Write(retOfMp);
            Program.I().ocgcore.sendReturn(m.get());
        }
        hide();
    }
    private void OnEnable()
    {
        instance = this;
        dp = instance.transform.GetChild(3).GetChild(0).GetComponent<UIButton>();
        sp = instance.transform.GetChild(3).GetChild(1).GetComponent<UIButton>();
        mp1 = instance.transform.GetChild(3).GetChild(2).GetComponent<UIButton>();
        bp = instance.transform.GetChild(3).GetChild(3).GetComponent<UIButton>();
        mp2 = instance.transform.GetChild(3).GetChild(4).GetComponent<UIButton>();
        ep = instance.transform.GetChild(3).GetChild(5).GetComponent<UIButton>();
    }

    void Start()
    {
        EventDelegate.Add(transform.GetChild(1).GetComponent<UIButton>().onClick, hide);
        EventDelegate.Add(transform.GetChild(3).GetChild(2).GetComponent<UIButton>().onClick, hide);
        EventDelegate.Add(transform.GetChild(3).GetChild(3).GetComponent<UIButton>().onClick, onBP);
        EventDelegate.Add(transform.GetChild(3).GetChild(4).GetComponent<UIButton>().onClick, onMP);
        EventDelegate.Add(transform.GetChild(3).GetChild(5).GetComponent<UIButton>().onClick, onEP);
    }

    void Update()
    {
        if (isShowed)
        {
            ButtonBehaviour(mp1);
            ButtonBehaviour(bp);
            ButtonBehaviour(mp2);
            ButtonBehaviour(ep);
        }
    }

    public static void SetButtonState(UIButton btn, int i)
    {
        if (btn == null)
            return;
        if (i == 0)//unselectable
        {
            btn.GetComponent<BoxCollider>().size = new Vector3(0, 0, 0);
            //UIHelper.getByName<UI2DSprite>(btn.gameObject, "background_").color = new Color(0.5f, 0.5f, 0.5f, 1f);
            //UIHelper.getByName<UI2DSprite>(btn.gameObject, "circle_main").color = new Color(0.5f, 0.5f, 0.5f, 1f);
            //UIHelper.getByName<UI2DSprite>(btn.gameObject, "icon_").color = new Color(0.5f, 0.5f, 0.5f, 1f);
            //UIHelper.getByName<UILabel>(btn.gameObject, "label_").color = new Color(0.5f, 0.5f, 0.5f, 1f);
            //UIHelper.getByName<UIWidget>(btn.gameObject, "active_").alpha = 0;
            btn.transform.GetChild(0).GetComponent<UI2DSprite>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
            btn.transform.GetChild(1).GetComponent<UI2DSprite>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
            btn.transform.GetChild(3).GetComponent<UI2DSprite>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
            btn.transform.GetChild(4).GetComponent<UILabel>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
            btn.transform.GetChild(5).GetComponent<UIWidget>().alpha = 0;
        }
        else if (i == 1)//selectable
        {
            btn.GetComponent<BoxCollider>().size = new Vector3(175, 175, 0);
            //UIHelper.getByName<UI2DSprite>(btn.gameObject, "background_").color = Color.white;
            //UIHelper.getByName<UI2DSprite>(btn.gameObject, "circle_main").color = Color.white;
            //UIHelper.getByName<UI2DSprite>(btn.gameObject, "icon_").color = Color.white;
            //UIHelper.getByName<UILabel>(btn.gameObject, "label_").color = Color.white;
            //UIHelper.getByName<UIWidget>(btn.gameObject, "active_").alpha = 0;
            btn.transform.GetChild(0).GetComponent<UI2DSprite>().color = Color.white;
            btn.transform.GetChild(1).GetComponent<UI2DSprite>().color = Color.white;
            btn.transform.GetChild(3).GetComponent<UI2DSprite>().color = Color.white;
            btn.transform.GetChild(4).GetComponent<UILabel>().color = Color.white;
            btn.transform.GetChild(5).GetComponent<UIWidget>().alpha = 0;
        }
        else if (i == 2)//selected
        {
            btn.GetComponent<BoxCollider>().size = new Vector3(174, 174, 0);
            //UIHelper.getByName<UI2DSprite>(btn.gameObject, "background_").color = Color.black;
            //UIHelper.getByName<UI2DSprite>(btn.gameObject, "circle_main").color = new Color(0.8f, 1f, 0f, 1f);
            //UIHelper.getByName<UI2DSprite>(btn.gameObject, "icon_").color = new Color(0.8f, 1f, 0f, 1f);
            //UIHelper.getByName<UILabel>(btn.gameObject, "label_").color = new Color(0.8f, 1f, 0f, 1f);
            //UIHelper.getByName<UIWidget>(btn.gameObject, "active_").alpha = 0;
            btn.transform.GetChild(0).GetComponent<UI2DSprite>().color = Color.black;
            btn.transform.GetChild(1).GetComponent<UI2DSprite>().color = new Color(0.8f, 1f, 0f, 1f);
            btn.transform.GetChild(3).GetComponent<UI2DSprite>().color = new Color(0.8f, 1f, 0f, 1f);
            btn.transform.GetChild(4).GetComponent<UILabel>().color = new Color(0.8f, 1f, 0f, 1f);
            btn.transform.GetChild(5).GetComponent<UIWidget>().alpha = 0;
        }
    }

    void ButtonBehaviour(UIButton btn)
    {
        if (btn == null)
            return;
        BoxCollider collider = btn.GetComponent<BoxCollider>();

        int state = 0;
        if (collider.size.x == 0) 
            return;
        else if(collider.size.x == 175)
            state = 1;
        else
            state = 2;
        if(Program.pointedCollider == collider && Input.GetMouseButton(0))
        {
            //UIHelper.getByName<UIWidget>(btn.gameObject, "active_").alpha = 1;
            btn.transform.GetChild(5).GetComponent<UIWidget>().alpha = 1;
            if (state == 1)
            {
                //UIHelper.getByName<UI2DSprite>(btn.gameObject, "icon_").color = Color.white;
                //UIHelper.getByName<UILabel>(btn.gameObject, "label_").color = Color.white;
                btn.transform.GetChild(3).GetComponent<UI2DSprite>().color = Color.white;
                btn.transform.GetChild(4).GetComponent<UILabel>().color = Color.white;
            }
            else
            {
                //UIHelper.getByName<UI2DSprite>(btn.gameObject, "icon_").color = new Color(0.8f, 1f, 0f, 1f);
                //UIHelper.getByName<UILabel>(btn.gameObject, "label_").color = new Color(0.8f, 1f, 0f, 1f);
                btn.transform.GetChild(3).GetComponent<UI2DSprite>().color = new Color(0.8f, 1f, 0f, 1f);
                btn.transform.GetChild(4).GetComponent<UILabel>().color = new Color(0.8f, 1f, 0f, 1f);
            }
            
        }
        else if (Program.pointedCollider == collider && !Input.GetMouseButton(0))
        {
            //UIHelper.getByName<UI2DSprite>(btn.gameObject, "icon_").color = Color.black;
            //UIHelper.getByName<UILabel>(btn.gameObject, "label_").color = Color.black;
            //UIHelper.getByName<UIWidget>(btn.gameObject, "active_").alpha = 1;
            btn.transform.GetChild(3).GetComponent<UI2DSprite>().color = Color.black;
            btn.transform.GetChild(4).GetComponent<UILabel>().color = Color.black;
            btn.transform.GetChild(5).GetComponent<UIWidget>().alpha = 1;
        }
        else
        {
            SetButtonState(btn, state);
        }
    }

    public static void show()
    {
        if(isShowed) return;
        isShowed = true;
        instance.transform.DOLocalMoveY(0, 0.3f);
        DOTween.To(() => instance.GetComponent<UIWidget>().alpha, x => instance.GetComponent<UIWidget>().alpha = x, 1, 0.3f);
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_03");

        SetButtonState(dp, 0);
        SetButtonState(sp, 0);
        SetButtonState(mp1, 0);
        SetButtonState(bp, 0);
        SetButtonState(mp2, 0);
        SetButtonState(ep, 0);

        if (retOfBp != -1)
            SetButtonState(bp, 1);
        if (retOfMp != -1)
            SetButtonState(mp2, 1);
        if (retOfEp != -1)
            SetButtonState(ep, 1);

        if (Program.I().ocgcore.MD_phaseString == "Main1")
            SetButtonState(mp1, 2);
        else if (Program.I().ocgcore.MD_phaseString == "Battle")
            SetButtonState(bp, 2);
        else if (Program.I().ocgcore.MD_phaseString == "Main2")
            SetButtonState(mp2, 2);
    }
    public static void hide()
    {
        if (!isShowed) return;
        isShowed = false;
        instance.transform.DOLocalMoveY(-600, 0.3f);
        DOTween.To(() => instance.GetComponent<UIWidget>().alpha, x => instance.GetComponent<UIWidget>().alpha = x, 0, 0.3f);
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_04");
    }
}
