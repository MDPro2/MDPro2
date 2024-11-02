using System;
using System.Collections.Generic;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;

public class gameUIbutton
{
    public bool dead;
    public bool dying;
    public GameObject gameObject;
    public string hashString;
    public int response;
}

public class gameInfo : MonoBehaviour
{
    public enum chainCondition
    {
        standard,
        no,
        all,
        smart
    }

    public UITexture instance_btnPan;
    public UITextList instance_lab;
    public UIToggle toggle_ignore;
    public UIToggle toggle_all;
    public UIToggle toggle_smart;
    public GameObject line;

    public GameObject mod_healthBar;

    public GameObject mod_healthBar_me;
    public GameObject mod_healthBar_op;

    public float width;

    public bool swaped;

    private readonly List<gameUIbutton> HashedButtons = new List<gameUIbutton>();

    private readonly bool[] isTicking = new bool[2];

    private int lastTickTime;

    //public UITextList hinter;

    private barPngLoader me;

    private barPngLoader opponent;

    private readonly int[] time = new int[2];

    public UIButton btn;

    // Use this for initialization
    private void Start()
    {
        ini();
        SetFace();
    }

    private void Update()
    {
        if (me == null || opponent == null)
        {
            me = Instantiate(mod_healthBar_me).GetComponent<barPngLoader>();
            me.transform.SetParent(gameObject.transform);
            me.transform.localScale = Vector3.one;

            opponent = Instantiate(mod_healthBar_op).GetComponent<barPngLoader>();
            opponent.transform.SetParent(gameObject.transform);
            opponent.transform.localScale = Vector3.one;

            var Transforms = me.GetComponentsInChildren<Transform>();
            foreach (var child in Transforms) child.gameObject.layer = gameObject.layer;
            Transforms = opponent.GetComponentsInChildren<Transform>();
            foreach (var child in Transforms) child.gameObject.layer = gameObject.layer;
            Color c;
            ColorUtility.TryParseHtmlString(Config.Getui("gameChainCheckArea.color"), out c);
            UIHelper.getByName<UISprite>(UIHelper.getByName<UIToggle>(gameObject, "ignore_").gameObject, "Background")
                .color = c;
            UIHelper.getByName<UISprite>(UIHelper.getByName<UIToggle>(gameObject, "watch_").gameObject, "Background")
                .color = c;
            UIHelper.getByName<UISprite>(UIHelper.getByName<UIToggle>(gameObject, "use_").gameObject, "Background")
                .color = c;
        }
        me.transform.localPosition = new Vector3 (-Utils.UIWidth() / 2 + 80 * Utils.UIHeight() / 1080, -Utils.UIHeight() / 2 + 70 * Utils.UIHeight() / 1080, 0);
        opponent.transform.localPosition = new Vector3(Utils.UIWidth() / 2 - 80 * Utils.UIHeight() / 1080, Utils.UIHeight() / 2 - 70 * Utils.UIHeight() / 1080, 0);

        var k = Mathf.Clamp((Utils.UIWidth() - Program.I().cardDescription.width) / 1200f, 0.8f, 1.2f);
        var kb = Mathf.Clamp((Utils.UIWidth() - Program.I().cardDescription.width) / 1200f, 0.73f, 1.2f);
        var ksb = kb * Vector3.one;
        instance_btnPan.gameObject.transform.localScale = ksb;

        width = 150 * kb + 15f;
        var localPositionPanX = (Utils.UIWidth() - 150 * kb) / 2 - 15f;
        //instance_btnPan.transform.localPosition = new Vector3(localPositionPanX, 28, 0);
        //instance_lab.transform.localPosition = new Vector3(Utils.UIWidth() / 2 - 315, -Utils.UIHeight() / 2 + 90, 0);
        instance_btnPan.transform.localPosition = new Vector3(Utils.UIWidth(), 28, 0);
        instance_lab.transform.localPosition = new Vector3(Utils.UIWidth(), -Utils.UIHeight() / 2 + 90, 0);
        btn.transform.localPosition = new Vector3(Utils.UIWidth() / 2 - 60, -Utils.UIHeight() / 2 + 200, 0);
        var j = 0;
        foreach (var t in HashedButtons)
            if (t.gameObject != null)
            {
                if (t.dying)
                {
                    t.gameObject.transform.localPosition +=
                        (new Vector3(0, -120, 0) - t.gameObject.transform.localPosition) * Program.deltaTime * 20f;
                    if (Math.Abs(t.gameObject.transform.localPosition.y - -120) < 1) t.dead = true;
                }
                else
                {
                    t.gameObject.transform.localPosition +=
                        (new Vector3(0, -145 - j * 50, 0) - t.gameObject.transform.localPosition) * Program.deltaTime *
                        10f;
                    j++;
                }
            }
            else
            {
                t.dead = true;
            }

        for (var i = HashedButtons.Count - 1; i >= 0; i--)
            if (HashedButtons[i].dead)
                HashedButtons.RemoveAt(i);
        float height = 132 + 50 * j;
        if (j == 0) height = 116;
        instance_btnPan.height += (int) ((height - instance_btnPan.height) * 0.2f);
        if (Program.TimePassed() - lastTickTime > 1000)
        {
            lastTickTime = Program.TimePassed();
            tick();
        }
    }

    public void on_toggle_ignore()
    {
        //toggle_all.value = false;
        //toggle_smart.value = false;
    }

    public void on_toggle_all()
    {
        //toggle_ignore.value = false;
        //toggle_smart.value = false;
    }

    public void on_toggle_smart()
    {
        //toggle_ignore.value = false;
        //toggle_all.value = false;
    }

    public void ini()
    {
        Update();
    }

    public void addHashedButton(string hashString_, int hashInt, superButtonType type, string hint)
    {
        var hashedButton = new gameUIbutton();
        var hashString = hashString_;
        if (hashString == "") hashString = hashInt.ToString();
        hashedButton.hashString = hashString;
        hashedButton.response = hashInt;
        hashedButton.gameObject = Program.I().create(Program.I().new_ui_superButtonTransparent);
        UIHelper.trySetLableText(hashedButton.gameObject, "hint_", hint);
        UIHelper.getRealEventGameObject(hashedButton.gameObject).name = hashString + "----" + hashInt;
        UIHelper.registUIEventTriggerForClick(hashedButton.gameObject, listenerForClicked);
        //hashedButton.gameObject.GetComponent<iconSetForButton>().setTexture(type);
        //hashedButton.gameObject.GetComponent<iconSetForButton>().setText(hint);
        var Transforms = hashedButton.gameObject.GetComponentsInChildren<Transform>();
        foreach (var child in Transforms) child.gameObject.layer = instance_btnPan.gameObject.layer;
        hashedButton.gameObject.transform.SetParent(instance_btnPan.transform, false);
        hashedButton.gameObject.transform.localScale = Vector3.zero;
        hashedButton.gameObject.transform.localPosition = new Vector3(0, -120, 0);
        hashedButton.gameObject.transform.localEulerAngles = Vector3.zero;
        iTween.ScaleTo(hashedButton.gameObject, new Vector3(0.9f, 0.9f, 0.9f), 0.3f);
        hashedButton.dying = false;
        HashedButtons.Add(hashedButton);
        refreshLine();
    }

    private void listenerForClicked(GameObject obj)
    {
        var mats = obj.name.Split("----");
        if (mats.Length == 2)
            for (var i = 0; i < HashedButtons.Count; i++)
                if (HashedButtons[i].hashString == mats[0])
                    if (HashedButtons[i].response.ToString() == mats[1])
                        Program.I().ocgcore.ES_gameUIbuttonClicked(HashedButtons[i]);
    }

    public bool queryHashedButton(string hashString)
    {
        for (var i = 0; i < HashedButtons.Count; i++)
            if (HashedButtons[i].hashString == hashString && !HashedButtons[i].dying)
                return true;
        return false;
    }

    public void removeHashedButton(string hashString)
    {
        gameUIbutton remove = null;
        for (var i = 0; i < HashedButtons.Count; i++)
            if (HashedButtons[i].hashString == hashString)
                remove = HashedButtons[i];
        if (remove != null)
        {
            if (remove.gameObject != null) Program.I().destroy(remove.gameObject, 0.3f, true);
            remove.dying = true;
        }

        refreshLine();
    }

    public void removeAll()
    {
        if (HashedButtons.Count == 1)
            if (HashedButtons[0].hashString == "swap")
                return;
        for (var i = 0; i < HashedButtons.Count; i++)
        {
            if (HashedButtons[i].gameObject != null) Program.I().destroy(HashedButtons[i].gameObject, 0.3f, true);
            HashedButtons[i].dying = true;
        }

        refreshLine();
    }

    private void refreshLine()
    {
        var j = 0;
        for (var i = 0; i < HashedButtons.Count; i++)
            if (!HashedButtons[i].dying)
                j++;
        line.SetActive(j > 0);
    }

    public void setTime(int player, int t)
    {
        if (player < 2)
        {
            time[player] = t;
            setTimeAbsolutely(player, t);
            isTicking[player] = true;
            isTicking[1 - player] = false;
        }
    }

    public void setExcited(int player)
    {
        //if (player == 0)
        //{
        //    me.under.mainTexture = GameTextureManager.exBar;
        //    opponent.under.mainTexture = GameTextureManager.bar;
        //}
        //else
        //{
        //    opponent.under.mainTexture = GameTextureManager.exBar;
        //    me.under.mainTexture = GameTextureManager.bar;
        //}
    }


    public void setTimeStill(int player)
    {
        time[0] = Program.I().ocgcore.timeLimit;
        time[1] = Program.I().ocgcore.timeLimit;
        setTimeAbsolutely(0, Program.I().ocgcore.timeLimit);
        setTimeAbsolutely(1, Program.I().ocgcore.timeLimit);
        isTicking[0] = false;
        isTicking[1] = false;
        //if (player == 0)
        //{
        //    me.under.mainTexture = GameTextureManager.exBar;
        //    opponent.under.mainTexture = GameTextureManager.bar;
        //}
        //else
        //{
        //    opponent.under.mainTexture = GameTextureManager.exBar;
        //    me.under.mainTexture = GameTextureManager.bar;
        //}

        //if (Program.I().ocgcore.timeLimit == 0)
        //{
        //    me.api_timeHint.text = "infinite";
        //    opponent.api_timeHint.text = "infinite";
        //}
        //else
        //{
        //    me.api_timeHint.text = "paused";
        //    opponent.api_timeHint.text = "paused";
        //}
    }

    public bool amIdanger()
    {
        return time[0] < Program.I().ocgcore.timeLimit / 3;
    }

    private void setTimeAbsolutely(int player, int t)
    {
        if (Program.I().ocgcore.timeLimit == 0) return;
        if (player == 0)
        {
            //me.api_timeHint.text = t.ToString()/* + "/" + Program.I().ocgcore.timeLimit*/;
            TimerHandler.SetTime(0, t);
            //opponent.api_timeHint.text = "waiting";
            //UIHelper.clearITWeen(me.api_healthBar.gameObject);
            //iTween.MoveToLocal(me.api_timeBar.gameObject,
            //    new Vector3(me.api_timeBar.width - t / (float) Program.I().ocgcore.timeLimit * me.api_timeBar.width,
            //        me.api_healthBar.gameObject.transform.localPosition.y,
            //        me.api_healthBar.gameObject.transform.localPosition.z), 1f);
        }

        if (player == 1)
        {
            //opponent.api_timeHint.text = t.ToString()/* + "/" + Program.I().ocgcore.timeLimit*/;
            TimerHandler.SetTime(1, t);
            //me.api_timeHint.text = "waiting";
            //UIHelper.clearITWeen(opponent.api_healthBar.gameObject);
            //iTween.MoveToLocal(opponent.api_timeBar.gameObject,
            //    new Vector3(
            //        opponent.api_timeBar.width - t / (float) Program.I().ocgcore.timeLimit * opponent.api_timeBar.width,
            //        opponent.api_healthBar.gameObject.transform.localPosition.y,
            //        opponent.api_healthBar.gameObject.transform.localPosition.z), 1f);
        }
    }

    int chara0face = 0;
    int chara1face = 0;
    string chara0Name;
    string chara1Name;

    public GameObject SpineCharacter(string name)
    {
        GameObject re = null;
        if (name == "榊游矢")
            re = Program.I().yuya;
        else if(name == "BlueAngel")
            re = Program.I().blueangel;
        else if (name == "盗贼基斯")
            re = Program.I().geese;
        else if (name == "权现坂升")
            re = Program.I().gongenzaka;
        else if (name == "柊柚子")
            re = Program.I().yuzu;
        else if (name == "不动游星")
            re = Program.I().yusei;
        else if (name == "昆虫羽蛾")
            re = Program.I().haga;
        else if (name == "十六夜秋")
            re = Program.I().aki;
        else if (name == "杰克·阿特拉斯")
            re = Program.I().jack;
        else if (name == "城之内克也")
            re = Program.I().jyonochi;
        else if (name == "海马濑人")
            re = Program.I().kaiba;
        else if (name == "DS鬼柳京介")
            re = Program.I().kiryu;
        else if (name == "孔雀舞")
            re = Program.I().mai;
        else if (name == "万丈目准")
            re = Program.I().manjyome;
        else if (name == "丸藤翔")
            re = Program.I().syo;
        else if (name == "PlayerMaker&Ai")
            re = Program.I().playmaker;
        else if (name == "Revolver")
            re = Program.I().revolver;
        else if (name == "龙亚")
            re = Program.I().rua;
        else if (name == "神代凌牙")
            re = Program.I().ryoga;
        else if (name == "泽渡慎吾")
            re = Program.I().sawatari;
        else if (name == "天城快斗")
            re = Program.I().kaito;
        else if (name == "天上院明日香")
            re = Program.I().asuka;
        else if (name == "九十九游马/阿斯特拉尔")
            re = Program.I().yuma;
        else if (name == "暗游戏")
            re = Program.I().yugi;
        else if (name == "游城十代")
            re = Program.I().judai;
        else if (name == "IV")
            re = Program.I().four;
        else if (name == "导游小姐")
            re = Program.I().guide;

        return re;
    }

    public void SetCharacter0Face(int look = 1)
    {
        string currentChara = Config.Get("Character0", "暗游戏");
        bool changed = false;
        if(currentChara != chara0Name)
        {
            changed = true;

            chara0Name = currentChara;
            if (SpineCharacter(currentChara) != null)
            {
                me.api_face.gameObject.SetActive(false);
                if(me.spine != null && me.spine.gameObject != null)
                    Destroy(me.spine.gameObject);
                me.spine = Instantiate(SpineCharacter(currentChara)).GetComponent<SpineCharacterHandler>();
                me.spine.transform.parent = me.transform;
                me.spine.transform.localPosition = Vector3.zero;
                me.spine.transform.localScale = Vector3.one;
                ABLoader.ChangeLayer(me.spine.gameObject, "ui_back_ground_2d");
                me.dynamic = true;
            }
            else
            {
                if (me.spine != null && me.spine.gameObject != null)
                    Destroy(me.spine.gameObject);
                me.api_face.gameObject.SetActive(true);
                me.dynamic = false;
            }
        }

        if (look == chara0face && !changed) return;
        chara0face = look;

        if(me.dynamic)
        {
            if (look == 1)
                me.spine.Normal(false);
            else if (look == 2)
                me.spine.Laugh(true);
            else if (look == 3)
                me.spine.Angry(true);
            else if (look == 4)
                me.spine.Surp(true);
            else if (look == 5)
                me.spine.Sad(false);
            else
                me.spine.Normal(true);
        }
        else
        {
            string nameMe = VoiceHandler.NameMap(currentChara).Replace("V", "sn");
            Sprite spriteMe = Resources.Load<Sprite>("Texture/Character/" + nameMe + "_" + look);
            spriteMe = Sprite.Create(spriteMe.texture, new Rect(0, 0, spriteMe.texture.width, spriteMe.texture.height), new Vector2(0.5f, 0.5f), 50);
            me.api_face.sprite2D = spriteMe;
        }
    }

    public void SetCharacter1Face(int look = 1)
    {
        string currentChara = Config.Get("Character1", "暗游戏");
        bool changed = false;
        if (currentChara != chara1Name)
        {
            changed = true;

            chara1Name = currentChara;
            if (SpineCharacter(currentChara) != null)
            {
                opponent.api_face.gameObject.SetActive(false);
                if (opponent.spine != null && opponent.spine.gameObject != null)
                    Destroy(opponent.spine.gameObject);
                opponent.spine = Instantiate(SpineCharacter(currentChara)).GetComponent<SpineCharacterHandler>();
                opponent.spine.transform.parent = opponent.transform;
                opponent.spine.transform.localPosition = Vector3.zero;
                opponent.spine.transform.localScale = Vector3.one;
                ABLoader.ChangeLayer(opponent.spine.gameObject, "ui_back_ground_2d");
                opponent.dynamic = true;
            }
            else
            {
                if (opponent.spine != null && opponent.spine.gameObject != null)
                    Destroy(opponent.spine.gameObject);
                opponent.api_face.gameObject.SetActive(true);
                opponent.dynamic = false;
            }
        }

        if (look == chara1face && !changed) return;
        chara1face = look;

        if (opponent.dynamic)
        {
            if (look == 1)
                opponent.spine.Normal(false);
            else if (look == 2)
                opponent.spine.Laugh(true);
            else if (look == 3)
                opponent.spine.Angry(true);
            else if (look == 4)
                opponent.spine.Surp(true);
            else if (look == 5)
                opponent.spine.Sad(false);
            else
                opponent.spine.Normal(true);
        }
        else
        {
            string nameOp = VoiceHandler.NameMap(currentChara).Replace("V", "sn");
            Sprite spriteOp = Resources.Load<Sprite>("Texture/Character/" + nameOp + "_" + look);
            spriteOp = Sprite.Create(spriteOp.texture, new Rect(0, 0, spriteOp.texture.width, spriteOp.texture.height), new Vector2(0.5f, 0.5f), 50);
            opponent.api_face.sprite2D = spriteOp;
        }
    }

    public void SetFace()
    {
        chara0face = 0;
        chara1face = 0;

        if (Program.I().setting.setting.Voice.value)
        {
            SetCharacter0Face();
            SetCharacter1Face();
        }
        else
        {
            if (Config.Get("Face0", "随机") == "自定义")
            {
                Texture2D texture_me = UIHelper.getFace(Program.I().ocgcore.name_0_c);
                me.api_face.sprite2D = Sprite.Create(texture_me, new Rect(0, 0, texture_me.width, texture_me.height), new Vector2(0.5f, 0.5f));
            }
            else
                me.api_face.sprite2D = Program.I().appearance.GetSprite("Face", Config.Get("Face0", "随机"));

            if (Config.Get("Face1", "随机") == "自定义")
            {
                Texture2D texture_op = UIHelper.getFace(Program.I().ocgcore.name_1_c);
                opponent.api_face.sprite2D = Sprite.Create(texture_op, new Rect(0, 0, texture_op.width, texture_op.height), new Vector2(0.5f, 0.5f));
            }
            else
                opponent.api_face.sprite2D = Program.I().appearance.GetSprite("Face", Config.Get("Face1", "随机"));
        }
        //MyCard.LoadAvatar(Program.I().ocgcore.name_0_c, texture => me.api_face.mainTexture = texture);
        //MyCard.LoadAvatar(Program.I().ocgcore.name_1_c, texture => opponent.api_face.mainTexture = texture);

        me.api_frame.sprite2D = Program.I().appearance.GetSprite("FaceFrame", Config.Get("FaceFrame0", "随机"));
        opponent.api_frame.sprite2D = Program.I().appearance.GetSprite("FaceFrame", Config.Get("FaceFrame1", "随机"));
    }


    public void realize()
    {
        me.SetLP(Program.I().ocgcore.life_0 > 0 ? Program.I().ocgcore.life_0 : 0);
        opponent.SetLP(Program.I().ocgcore.life_1 > 0 ? Program.I().ocgcore.life_1 : 0);
        me.api_name.text = Program.I().ocgcore.name_0_c;
        opponent.api_name.text = Program.I().ocgcore.name_1_c;

        instance_lab.Clear();
        if (Program.I().ocgcore.confirmedCards.Count > 0) instance_lab.Add(GameStringHelper.yijingqueren);
        foreach (var item in Program.I().ocgcore.confirmedCards) instance_lab.Add(item);
    }

    private static float getRealLife(float in_)
    {
        if (in_ < 0) return 0;
        if (in_ > Program.I().ocgcore.lpLimit) return Program.I().ocgcore.lpLimit;
        return in_;
    }

    private void tick()
    {
        //Debug.Log("tick: " + isTicking[0] + isTicking[1]);
        if (isTicking[0])
        {
            if (time[0] > 0) time[0]--;
            if (amIdanger())
                if (Program.I().ocgcore != null)
                    Program.I().ocgcore.dangerTicking();
            setTimeAbsolutely(0, time[0]);
        }

        if (isTicking[1])
        {
            if (time[1] > 0) time[1]--;
            setTimeAbsolutely(1, time[1]);
        }
    }

    public void set_condition(chainCondition c)
    {
        switch (c)
        {
            case chainCondition.standard:
                toggle_all.value = false;
                toggle_smart.value = false;
                toggle_ignore.value = false;
                break;
            case chainCondition.no:
                toggle_all.value = false;
                toggle_smart.value = false;
                toggle_ignore.value = true;
                break;
            case chainCondition.all:
                toggle_all.value = true;
                toggle_smart.value = false;
                toggle_ignore.value = false;
                break;
            case chainCondition.smart:
                toggle_all.value = false;
                toggle_smart.value = true;
                toggle_ignore.value = false;
                break;
        }
    }

    public chainCondition get_condition()
    {
        var res = chainCondition.standard;
        if (toggle_ignore.value) res = chainCondition.no;
        if (toggle_smart.value) res = chainCondition.smart;
        if (toggle_all.value) res = chainCondition.all;
        return res;
    }
}