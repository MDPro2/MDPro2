using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P14676_Handler : MonoBehaviour
{
    SkeletonAnimation sa;
    Collider collider;

    float waitTime;
    float time;
    float timeUse;
    bool playing;
    bool notUse;
    private void Start()
    {
        sa = GetComponent<SkeletonAnimation>();
        collider = GetComponent<Collider>();
        waitTime = 1;
        Idle();
    }

    private void Update()
    {
        time += Time.deltaTime;
        timeUse += Time.deltaTime;
        if (playing && time > 3.17f)
            playing = false;
        if (time > waitTime && playing == false)
        {
            time = 0;
            waitTime = Random.Range(3, 6);
            if(Random.Range(0, 6) < 4)
                sa.AnimationState.SetAnimation(2, "eye_blink", false);
            else
            {
                sa.AnimationState.SetAnimation(2, "eye_wink_start", false);
                sa.AnimationState.AddAnimation(2, "eye_wink_release", false, 0);
            }
        }
        if (Program.pointedCollider == collider && Program.InputGetMouseButtonDown_0 && playing == false)
        {
            sa.AnimationState.SetEmptyAnimation(2, 0.2f);
            sa.AnimationState.SetEmptyAnimation(5, 0.2f);
            sa.AnimationState.SetAnimation(0, "interaction", false);
            sa.AnimationState.AddAnimation(0, "idle", true, 3f);
            playing = true;
            notUse = false;
            time = 0;
            timeUse = 0;
        }
        //if(timeUse > 5.5f &&  playing == false && notUse == false)
        //{
        //    sa.AnimationState.SetAnimation(5, "idle_notuse", true);
        //    notUse = true;
        //}
    }

    void Idle()
    {
        sa.AnimationState.SetAnimation(0, "idle", true);
    }
}
