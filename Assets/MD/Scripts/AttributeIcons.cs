using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributeIcons : MonoBehaviour
{
    public Sprite null_;
    public Sprite light_;
    public Sprite dark_;
    public Sprite water_;
    public Sprite fire_;
    public Sprite earth_;
    public Sprite wind_;
    public Sprite divine_;
    public Sprite spell_;
    public Sprite trap_;

    public void ChangeAttribute(string att)
    {
        GetComponent<UI2DSprite>().sprite2D = GetAttribute(att);
    }

    public Sprite GetAttribute(string att)
    {
        switch (att)
        {
            case "无":
                return null_;
            case "光":
                return light_;
            case "暗":
                return dark_;
            case "水":
                return water_;
            case "炎":
                return fire_;
            case "地":
                return earth_;
            case "风":
                return wind_;
            case "神":
                return divine_;
            case "魔法":
                return spell_;
            case "陷阱":
                return trap_;
            default:
                return null_;
        }
    }
}
