using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YGOSharp;
using YGOSharp.OCGWrapper.Enums;

public class CardDetail : MonoBehaviour
{
    public UIPanel Panel;
    public UITexture pic;
    public UI2DSprite limit;
    public UILabel label_name;
    public UILabel label_property;
    public UI2DSprite icon_property;
    public UILabel label_description;
    public UILabel label_pendulum_description;
    public UILabel label_type;

    public UI2DSprite base_name;
    public UI2DSprite base_property;
    public UI2DSprite base_pendulum;
    public UI2DSprite base_description;

    public UI2DSprite back_name;
    public UI2DSprite back_property;
    public UI2DSprite back_pendulum;
    public UI2DSprite back_description;

    public UIWidget icons_;
    public UI2DSprite icon_lv;
    public UILabel label_lv;
    public UI2DSprite icon_pendulum;
    public UILabel label_pendulum;
    public UI2DSprite icon_race;
    public UILabel label_atk;
    public UI2DSprite icon_def;
    public UILabel label_def;

    public UI2DSprite icon_name;

    public bool isShowed;

    private void Start()
    {
        Panel.alpha = 0f;
        UIHelper.registEvent(gameObject, "exit_", Hide);

        Panel.alpha = 1f;
        gameObject.SetActive(false);
    }

    public void Show(Card card)
    {
        if (card.Id == 0)
            return;
        Card origin = CardsManager.Get(card.Id);
        isShowed = true;
        Panel.alpha = 1f;
        SEHandler.PlayInternalAudio("se_sys/SE_DECK_WINDOW_OPEN");

        limit.sprite2D = Program.I().cardDescription.li.GetComponent<UI2DSprite>().sprite2D;

        int limitNum = Program.I().newDeckManager.currentBanlist.GetQuantity(card.Id);
        if (limitNum == 3)
            limit.sprite2D = Program.I().newDeckManager.mono.limit_.GetComponent<LimitIcons>().null_;
        else if (limitNum == 2)
            limit.sprite2D = Program.I().newDeckManager.mono.limit_.GetComponent<LimitIcons>().limit2;
        else if (limitNum == 1)
            limit.sprite2D = Program.I().newDeckManager.mono.limit_.GetComponent<LimitIcons>().limit1;
        else if (limitNum == 0)
            limit.sprite2D = Program.I().newDeckManager.mono.limit_.GetComponent<LimitIcons>().ban;



        pic.mainTexture = GameTextureManager.GetCardPictureNow(origin.Id);
        label_name.text = origin.Name;
        label_property.text = GameStringHelper.GetType(origin);
        if (GameStringHelper.getSetName(origin.Setcode) != "")
            label_type.text = GameStringHelper.GetType(origin) + "¡¾" + GameStringHelper.getSetName(origin.Setcode) + "¡¿" + "¡¾" + (origin.Alias == 0 ? origin.Id.ToString() : origin.Alias.ToString()) + "¡¿";
        else
            label_type.text = GameStringHelper.GetType(origin) + "¡¾" + (origin.Alias == 0 ? origin.Id.ToString() : origin.Alias.ToString()) + "¡¿";

        if ((origin.Type & (uint)CardType.Monster) > 0)
        {
            icon_name.sprite2D = Program.I().cardDescription.ai.GetAttribute(GameStringHelper.attribute(origin.Attribute));

            icon_property.alpha = 0f;
            label_property.alpha = 0f;
            icons_.alpha = 1f;

            if ((origin.Type & (uint)CardType.Link) > 0)
            {
                icon_lv.sprite2D = Program.I().cardDescription.pi.link;
                icon_pendulum.leftAnchor.absolute = 0;
                icon_pendulum.rightAnchor.absolute = 0;
                icon_pendulum.alpha = 0f;
                label_pendulum.alpha = 0f;
                icon_def.alpha = 0f;
                label_def.alpha = 0f;
            }
            else if ((origin.Type & (uint)CardType.Pendulum) > 0)
            {
                if ((origin.Type & (uint)CardType.Xyz) > 0)
                    icon_lv.sprite2D = Program.I().cardDescription.pi.rank;
                else
                    icon_lv.sprite2D = Program.I().cardDescription.pi.lv;
                icon_pendulum.leftAnchor.absolute = 150;
                icon_pendulum.rightAnchor.absolute = 150;
                icon_pendulum.alpha = 1f;
                label_pendulum.alpha = 1f;
                label_pendulum.text = origin.LScale.ToString();
                icon_def.alpha = 1f;
                label_def.alpha = 1f;
            }
            else if ((origin.Type & (uint)CardType.Xyz) > 0)
            {
                icon_lv.sprite2D = Program.I().cardDescription.pi.rank;
                icon_pendulum.leftAnchor.absolute = 0;
                icon_pendulum.rightAnchor.absolute = 0;
                icon_pendulum.alpha = 0f;
                label_pendulum.alpha = 0f;
                icon_def.alpha = 1f;
                label_def.alpha = 1f;
            }
            else
            {
                icon_lv.sprite2D = Program.I().cardDescription.pi.lv;
                icon_pendulum.leftAnchor.absolute = 0;
                icon_pendulum.rightAnchor.absolute = 0;
                icon_pendulum.alpha = 0f;
                label_pendulum.alpha = 0f;
                icon_def.alpha = 1f;
                label_def.alpha = 1f;
            }

            label_lv.text = origin.Level.ToString();
            Program.I().cardDescription.SetRaceIcon(icon_race, GameStringHelper.race(origin.Race));
            if (origin.Attack == -2)
                label_atk.text = "?";
            else
                label_atk.text = origin.Attack.ToString();
            if (origin.Defense == -2)
                label_def.text = "?";
            else
                label_def.text = origin.Defense.ToString();
        }
        else if ((origin.Type & (uint)CardType.Spell) > 0)
        {
            icon_name.sprite2D = Program.I().cardDescription.ai.GetAttribute("Ä§·¨");

            icon_property.alpha = 1f;
            label_property.alpha = 1f;
            icons_.alpha = 0f;

            label_property.leftAnchor.absolute = 5;
            string type = "";
            if ((origin.Type & (uint)CardType.Continuous) > 0)
            {
                type += "ÓÀÐø";
                icon_property.sprite2D = Program.I().cardDescription.pi.continuous;
            }
            else if ((origin.Type & (uint)CardType.Equip) > 0)
            {
                type += "×°±¸";
                icon_property.sprite2D = Program.I().cardDescription.pi.equip;
            }
            else if ((origin.Type & (uint)CardType.Field) > 0)
            {
                type += "³¡µØ";
                icon_property.sprite2D = Program.I().cardDescription.pi.field;
            }
            else if ((origin.Type & (uint)CardType.QuickPlay) > 0)
            {
                type += "ËÙ¹¥";
                icon_property.sprite2D = Program.I().cardDescription.pi.quick_play;
            }
            else if ((origin.Type & (uint)CardType.Ritual) > 0)
            {
                type += "ÒÇÊ½";
                icon_property.sprite2D = Program.I().cardDescription.pi.ritual;
            }
            else
            {
                type += "Í¨³£";
                icon_property.alpha = 0f;
                label_property.leftAnchor.absolute = -40;
            }
            type += "Ä§·¨";

            label_property.text = type;

        }
        else if ((origin.Type & (uint)CardType.Trap) > 0)
        {
            icon_name.sprite2D = Program.I().cardDescription.ai.GetAttribute("ÏÝÚå");

            icon_property.alpha = 1f;
            label_property.alpha = 1f;
            icons_.alpha = 0f;

            label_property.leftAnchor.absolute = 5;

            string type = "";
            if ((origin.Type & (uint)CardType.Continuous) > 0)
            {
                type += "ÓÀÐø";
                icon_property.sprite2D = Program.I().cardDescription.pi.continuous;
            }
            else if ((origin.Type & (uint)CardType.Counter) > 0)
            {
                type += "·´»÷";
                icon_property.sprite2D = Program.I().cardDescription.pi.counter;
            }
            else
            {
                type += "Í¨³£";
                icon_property.alpha = 0f;
                label_property.leftAnchor.absolute = -40;
            }
            type += "ÏÝÚå";

            label_property.text = type;
        }

        if ((origin.Type & (uint)CardType.Monster) > 0)
        {
            base_property.bottomAnchor.absolute = -185;
        }
        else
        {
            base_property.bottomAnchor.absolute = -120;
        }
        try
        {
            if (origin.Desc.Contains("¡¾¹ÖÊÞÐ§¹û¡¿"))
            {
                string text_pendulum = "";
                string text_other = "";
                string temp = origin.Desc.Split("¡ú\r\n")[1];
                if (temp.Split("\r\n¡¾¹ÖÊÞÐ§¹û¡¿\r\n").Length > 1)
                {
                    text_pendulum = temp.Split("\r\n¡¾¹ÖÊÞÐ§¹û¡¿\r\n")[0];
                    text_other = temp.Split("\r\n¡¾¹ÖÊÞÐ§¹û¡¿\r\n")[1];
                }
                else
                    text_other = temp.Replace("¡¾¹ÖÊÞÐ§¹û¡¿\r\n", "");
                label_pendulum_description.text = text_pendulum;
                label_description.text = text_other;
                base_pendulum.alpha = 1f;
                base_pendulum.bottomAnchor.absolute = -250;
                base_pendulum.topAnchor.absolute = -20;
            }
            else if (origin.Desc.Contains("¡¾¹ÖÊÞÃèÊö¡¿"))
            {
                string text_pendulum = "";
                string text_other = "";
                string temp = origin.Desc.Split("¡ú\r\n")[1];
                if (temp.Split("\r\n¡¾¹ÖÊÞÃèÊö¡¿\r\n").Length > 1)
                {
                    text_pendulum = temp.Split("\r\n¡¾¹ÖÊÞÃèÊö¡¿\r\n")[0];
                    text_other = temp.Split("\r\n¡¾¹ÖÊÞÃèÊö¡¿\r\n")[1];
                }
                else
                    text_other = temp.Replace("¡¾¹ÖÊÞÃèÊö¡¿\r\n", "");
                label_pendulum_description.text = text_pendulum;
                label_description.text = text_other;
                base_pendulum.alpha = 1f;
                base_pendulum.bottomAnchor.absolute = -250;
                base_pendulum.topAnchor.absolute = -20;
            }
            else
            {
                base_pendulum.bottomAnchor.absolute = 0;
                base_pendulum.topAnchor.absolute = 50;
                base_pendulum.alpha = 0f;
                label_description.text = origin.Desc;
            }
        }
        catch
        {
            Debug.LogError("Áé°ÚÐ§¹ûÎÄ±¾·Ö¶ÎÊ§°Ü£¡£¡£¡");
        }


        Program.I().cardDescription.FrameColor(origin, false);
        back_name.color = Program.I().cardDescription.color1;
        back_property.color = Program.I().cardDescription.color1;
        back_description.color = Program.I().cardDescription.color1;
        back_pendulum.color = Program.I().cardDescription.color2;
    }

    public void Hide() 
    {
        isShowed = false;
        Panel.alpha=1f;

        DOTween.To(() => Panel.alpha, x => Panel.alpha = x, 0, 0.2f)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            }
            );
        
        SEHandler.PlayInternalAudio("se_sys/SE_MENU_CANCEL");
    }
}
