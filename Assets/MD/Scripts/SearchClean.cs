using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchClean : MonoBehaviour
{
    public UIInput input_;

    void Update()
    {
        if(input_.text == "")
            GetComponent<UI2DSprite>().alpha = 0;
        else
            GetComponent<UI2DSprite>().alpha = 1;
    }
}
