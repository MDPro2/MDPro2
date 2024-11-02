using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SpineCharacterHandler : MonoBehaviour
{
    public SkeletonAnimation animation;

    public void Angry(bool speak)
    {
        animation.AnimationState.SetAnimation(1, "emo_angry", true);
        if (speak)
            animation.AnimationState.SetAnimation(2, "lip_angry", true);
        else
            animation.AnimationState.SetAnimation(2, "lip_stop_angry", true);
    }
    public void Laugh(bool speak)
    {
        animation.AnimationState.SetAnimation(1, "emo_laugh", true);
        if(speak)
            animation.AnimationState.SetAnimation(2, "lip_laugh", true);
        else
            animation.AnimationState.SetAnimation(2, "lip_stop_laugh", true);
    }
    public void Normal(bool speak)
    {
        animation.AnimationState.SetAnimation(1, "emo_normal", true);
        if(speak)
            animation.AnimationState.SetAnimation(2, "lip_normal", true);
        else
            animation.AnimationState.SetAnimation(2, "lip_stop_normal", true);
    }
    public void Sad(bool speak)
    {
        animation.AnimationState.SetAnimation(1, "emo_sad", true);
        if (speak)
            animation.AnimationState.SetAnimation(2, "lip_sad", true);
        else
            animation.AnimationState.SetAnimation(2, "lip_stop_sad", true);
    }
    public void Shy(bool speak)
    {
        animation.AnimationState.SetAnimation(1, "emo_shy", true);
        if (speak)
            animation.AnimationState.SetAnimation(2, "lip_shy", true);
        else
            animation.AnimationState.SetAnimation(2, "lip_stop_shy", true);
    }
    public void Surp(bool speak)
    {
        animation.AnimationState.SetAnimation(1, "emo_surp", true);
        if (speak)
            animation.AnimationState.SetAnimation(2, "lip_surp", true);
        else
            animation.AnimationState.SetAnimation(2, "lip_stop_surp", true);
    }
    public void Wait2()
    {
        animation.AnimationState.SetAnimation(0, "0_wait_2", false);
        animation.AnimationState.AddAnimation(0, "0_wait", true, 0);
    }
    public void Wait3()
    {
        animation.AnimationState.SetAnimation(0, "0_wait_3", false);
        animation.AnimationState.AddAnimation(0, "0_wait", true, 0);
    }
}
