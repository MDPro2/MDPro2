using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    public UIPanel panel;
    public UILabel title;
    public UI2DSprite base_;

    public SerialButton _dm;
    public SerialButton _gx;
    public SerialButton _5ds;
    public SerialButton _zexal;
    public SerialButton _arcv; 
    public SerialButton _vrains;
    public SerialButton _dsod;

    List<SerialButton> serialButtons = new List<SerialButton>();

    public Sprite normal;
    public Sprite hover;
    public Sprite selected;

    public bool isShowed;

    int player;

    public string character;
    void Awake()
    {
        panel.alpha = 0f;
        serialButtons.Add(_dm);
        serialButtons.Add(_gx);
        serialButtons.Add(_5ds);
        serialButtons.Add(_zexal);
        serialButtons.Add(_arcv);
        serialButtons.Add(_vrains);
        serialButtons.Add(_dsod);
        EventDelegate.Add(UIHelper.getByName<UIButton>(gameObject, "exit_").onClick, Hide);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(int player = 0)
    {
        isShowed = true;

        Program.I().menu.hide();
        Program.I().setting.hide();

        if (Program.I().ocgcore.gameField != null && Program.I().ocgcore.gameField.gameObject != null)
            base_.alpha = 0.95f;
        else if (Program.I().deckManager.isShowed)
            base_.alpha = 0.95f;
        else
            base_.alpha = 0f;

        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_03");
        DOTween.To(() => panel.alpha, x => panel.alpha = x, 1f, 0.2f);

        this.player = player;
        if (player == 0)
        {
            character = Config.Get("Character0", "暗游戏");
            title.text = "选择我方角色";
        }
        else
        {
            character = Config.Get("Character1", "暗游戏");
            title.text = "选择对方角色";
        }
        foreach (var button in serialButtons)
            button.SetSelected(character);
    }
    public void Hide()
    {
        isShowed = false;

        Program.I().setting.show();
        if (Program.I().ocgcore.gameField == null || Program.I().ocgcore.gameField.gameObject == null)
            Program.I().menu.show();

        DOTween.To(() => panel.alpha, x => panel.alpha = x, 0f, 0.2f);
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_SLIDE_04");

        SaveChange();
        if(Program.I().ocgcore.gameField != null && Program.I().ocgcore.gameField.gameObject != null)
            Program.I().ocgcore.gameInfo.SetFace();
        gameObject.SetActive(false);
    }

    public void RefreshSelectedSerialButton(SerialButton btn)
    {
        foreach (var button in serialButtons)
        {
            if(btn.gameObject != button.gameObject)
            {
                button.UnselectThis();
            }
        }
    }
    public void UnselectAllOtherCharacterButton(CharacterButton btn)
    {
        foreach (var sbutton in serialButtons)
            foreach (var cbutton in sbutton.GetComponentsInChildren<CharacterButton>())
                if(cbutton.gameObject != btn.gameObject)
                    cbutton.UnselectThis();
    }

    public void SaveChange()
    {
        if (player == 0)
        {
            Config.Set("Character0", character);
            Program.I().setting.setting.character0.text = character;
            VoiceHandler.I().ABInit();
        }
        else
        {
            Config.Set("Character1", character);
            Program.I().setting.setting.character1.text = character;
            VoiceHandler.I().ABInit();
        }
    }
}
