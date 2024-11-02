using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortToggle : MonoBehaviour
{
    public bool selected;

    public void Install()
    {
        EventDelegate.Add(GetComponent<UIButton>().onClick, OnClick);
    }

    void OnClick()
    {
        selected = true;
        GetComponent<UIButton>().normalSprite2D = Program.I().buttonToggleS_On;
        GetComponent<UIButton>().hoverSprite2D = null;
        Program.I().newDeckManager.mono.search_sort.transform.GetChild(0).GetComponent<UI2DSprite>().sprite2D = transform.GetChild(0).GetComponent<UI2DSprite>().sprite2D;
        gameObject.SetActive(false);
        gameObject.SetActive(true);
        Program.I().searchFliter.ResetSort(this);
        Program.I().searchFliter.HideSort();
        Program.I().newDeckManager.DoSearch();
    }

    public void Preselect()
    {
        selected = true;
        GetComponent<UIButton>().normalSprite2D = Program.I().buttonToggleS_On;
        GetComponent<UIButton>().hoverSprite2D = null;
    }

    public void Reset()
    {
        selected = false;
        GetComponent<UIButton>().normalSprite2D = Program.I().buttonToggleS;
        GetComponent<UIButton>().hoverSprite2D = Program.I().buttonToggleS_Over;
    }
}
