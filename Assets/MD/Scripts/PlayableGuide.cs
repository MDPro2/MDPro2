using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YGOSharp.OCGWrapper.Enums;

public class PlayableGuide : MonoBehaviour
{
    public Animator animator1;
    public Animator animator2;
    bool me;
    int currentCondition = 0;
    int updatedCondition = 0;
    float time;
    bool changed;
    public static int state = 0;
    void Start()
    {
        time = 0f;
        changed = false;
        animator1 = GetComponent<Animator>();
        Guide(0);
    }
    void Update()
    {
        time += Time.deltaTime;

        //if (Program.I().ocgcore.myTurn)
        //{
        //    if (Program.I().ocgcore.currentMessage == GameMessage.Waiting)
        //        updatedCondition = 2;
        //    else
        //        updatedCondition = 1;
        //}
        //else
        //{
        //    if (Program.I().ocgcore.currentMessage == GameMessage.SelectChain)
        //        updatedCondition = 1;
        //    else
        //        updatedCondition = 2;
        //}
        updatedCondition = state;

        if (currentCondition != updatedCondition)
        {
            if (changed == false)
            {
                changed = true;
                time = 0f;
                animator1.SetTrigger("Notice");
                animator2.SetTrigger("Notice");
            }
            if (time > 0.5f)
            {
                changed = false;
                currentCondition = updatedCondition;
                Guide(currentCondition);
            }
        }
    }

    void Guide(int i)
    {
        switch (i)
        {
            case 0:
                animator1.SetTrigger("Out");
                animator2.SetTrigger("Out");
                break;
            case 1:
                animator1.SetTrigger("Change");
                animator2.SetTrigger("Out");
                break;
            case 2:
                animator1.SetTrigger("Out");
                animator2.SetTrigger("Change");
                break;
        }
    }
}
