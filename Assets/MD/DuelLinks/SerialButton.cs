using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialButton : MonoBehaviour
{
    Collider collider;
    UI2DSprite sprite;
    UIPanel character;

    public bool selected;
    bool hover;
    private void Awake()
    {
        collider = GetComponent<Collider>();
        sprite = GetComponent<UI2DSprite>();
        character = transform.Find("charater").GetComponent<UIPanel>();
        character.alpha = 0f;
    }

    private void Update()
    {
        if(Program.pointedCollider == collider)
        {
            if (!hover)
            {
                hover = true;
                //SEHandler.PlayInternalAudio("se_sys/SE_MENU_SELECT_02");
            }

            if (Program.InputGetMouseButton_0)
            {
                sprite.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
            else if (Program.InputGetMouseButtonUp_0)
            {
                sprite.color = Color.white;
                if (!selected)
                {
                    selected = true;
                    sprite.sprite2D = Program.I().character.selected;
                    Program.I().character.RefreshSelectedSerialButton(this);
                    character.alpha = 1f;
                }
                SEHandler.PlayInternalAudio("se_sys/SE_MENU_SELECT_01");
            }
            else
            {
                sprite.color = Color.white;
                if (!selected)
                {
                    sprite.sprite2D = Program.I().character.hover;
                }
            }
        }
        else
        {
            if(hover)
            {
                hover = false;
                sprite.color = Color.white;
                if (selected)
                {
                    sprite.sprite2D = Program.I().character.selected;
                }
                else
                {
                    sprite.sprite2D = Program.I().character.normal;
                }
            }
        }
    }

    public bool SetSelected(string name)
    {
        foreach(var btn in character.GetComponentsInChildren<CharacterButton>())
        {
            btn.UnselectThis();
        }
        bool re = false;
        foreach (var btn in character.GetComponentsInChildren<CharacterButton>())
        {
            if(btn.character == name)
            {
                btn.SelectThis();
                re = true;
                selected = true;
                sprite.sprite2D = Program.I().character.selected;
                character.alpha = 1f;
                return re;
            }
        }
        if (!re)
            UnselectThis();
        return re;
    }

    public void UnselectThis()
    {
        selected = false;
        sprite.sprite2D = sprite.sprite2D = Program.I().character.normal;
        sprite.color = Color.white;
        character.alpha = 0f;
    }
}
