using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterButton : MonoBehaviour
{
    UIButton button;
    UILabel label;
    UI2DSprite mark;
    public bool selected;
    public string character;
    void Awake()
    {
        button = GetComponent<UIButton>();
        EventDelegate.Add(button.onClick, SelectThis);
        label = transform.GetChild(0).GetComponent<UILabel>();
        character = label.text;
        mark = transform.GetChild(1).GetComponent<UI2DSprite>();
        mark.color = new Color(0.8f, 1f, 0f, 1f);
    }

    public void SelectThis()
    {
        selected = true;
        label.color = new Color(0.8f, 1f, 0f, 1f);
        mark.alpha = 1f;
        Program.I().character.UnselectAllOtherCharacterButton(this);
        Program.I().character.character = label.text;
    }

    public void UnselectThis()
    {
        selected = false;
        label.color = Color.white;
        mark.alpha = 0f;
    }
}
