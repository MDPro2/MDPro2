using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class P3415_Handler : MonoBehaviour
{
    public SkeletonAnimation sa;
    public Collider colliderUp;
    public Collider colliderDown;

    bool engineOn;
    float waitTime;
    float time;
    bool playing;
    private void Start()
    {
        waitTime = 1;
        engineOn = false;
        Idle();
    }

    void Update()
    {
        time += Time.deltaTime;
        if (playing && time > 5f)
        {
            playing = false;
            Idle();
        }
        if (time > waitTime && playing == false)
        {
            time = 0;
            waitTime = Random.Range(2, 6);
            sa.AnimationState.SetAnimation(2, "eyes_blink", false);
        }

        if (Program.pointedCollider == colliderUp && Program.InputGetMouseButtonDown_0 && playing == false)
        {
            sa.AnimationState.SetEmptyAnimation(2, 0.2f);
            sa.AnimationState.SetAnimation(0, "interaction", false);
            playing = true;
            time = 0;
        }
        if (Program.pointedCollider == colliderDown && Program.InputGetMouseButtonDown_0)
        {
            if(engineOn == false)
            {
                sa.AnimationState.SetAnimation(3, "engine_on", true);
                engineOn = !engineOn;
            }
            else
            {
                sa.AnimationState.SetAnimation(3, "engine_on", false);
                engineOn = !engineOn;
            }
        }
    }

    void Idle()
    {
        sa.AnimationState.SetAnimation(0, "idle", true);
        sa.AnimationState.SetAnimation(4, "motor_bulinbulin", true);
    }
}
