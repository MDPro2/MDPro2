using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearanceItem : MonoBehaviour
{
    UIButton btn;
    Collider collider;
    public UI2DSprite uiSprite;
    public UILabel labelName;
    public UILabel labelExplain;
    UI2DSprite mark;

    UILabel name_hover;
    UILabel setting_name;
    UI2DSprite setting_sprite;
    UILabel setting_explain;

    public bool selected;
    bool hover;

    void Awake()
    {
        btn = GetComponent<UIButton>();
        EventDelegate.Add(btn.onClick, OnSelect);
        collider = GetComponent<Collider>();

        uiSprite = transform.GetChild(1).GetComponent<UI2DSprite>();
        labelName = transform.GetChild(2).GetComponent<UILabel>();
        labelExplain = transform.GetChild(3).GetComponent<UILabel>();
        mark = transform.GetChild(4).GetComponent<UI2DSprite>();

        name_hover = transform.parent.parent.Find("name_hover").GetComponent<UILabel>();
        setting_name = transform.parent.parent.Find("setting_name").GetComponent<UILabel>();
        setting_sprite = transform.parent.parent.Find("setting_sprite").GetComponent<UI2DSprite>();
        setting_explain = transform.parent.parent.Find("setting_explain").GetComponent<UILabel>();
    }

    public void OnSelect()
    {
        selected = true;

        name_hover.text = labelName.text;
        setting_name.text = labelName.text;
        setting_sprite.sprite2D = uiSprite.sprite2D;
        setting_explain.text = labelExplain.text;
        mark.alpha = 1f;

        foreach (var item in transform.parent.GetComponentsInChildren<AppearanceItem>())
            if (item.gameObject != gameObject)
                item.Unselect();
    }

    public void Unselect()
    {
        selected = false;
        mark.alpha = 0f;
    }

    private void Update()
    {
        if(Program.pointedCollider == collider)
        {
            if (!hover)
            {
                hover = true;
                name_hover.text = labelName.text;
            }
        }
        else if (hover)
            hover = false;
    }
}
