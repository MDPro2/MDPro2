using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterToggle : MonoBehaviour
{
    Collider collider_;
    UIButton button_;
    public UILabel label_;

    public bool selected;

    private void Start()
    {
        
    }

    public void Install()
    {
        collider_ = GetComponent<Collider>();
        button_ = GetComponent<UIButton>();
        label_ = transform.GetChild(0).GetComponent<UILabel>();
        EventDelegate.Add(button_.onClick, Switch);
    }

    public void Switch()
    {
        selected = !selected;
        if(selected)
        {
            button_.normalSprite2D = Program.I().newDeckManager.mono.toggleOn;
            button_.hoverSprite2D = null;
            label_.color = Color.black;
        }
        else 
        {
            button_.normalSprite2D = Program.I().newDeckManager.mono.toggleOff;
            button_.hoverSprite2D = Program.I().newDeckManager.mono.toggleOffOver;
            label_.color = Color.white;
        }
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

}
