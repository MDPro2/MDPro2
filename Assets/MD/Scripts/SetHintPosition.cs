using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetHintPosition : MonoBehaviour
{
    public Collider collider;
    bool up;
    bool hover;
    void OnEnable()
    {
        gameObject.transform.localPosition = new Vector3(0, 280, 0);
        up = true;
        hover = false;
    }

    void Update()
    {
        if (Program.pointedCollider == collider)
        {
            if (!hover)
            {
                hover = true;
                if (up)
                {
                    up = false;
                    gameObject.transform.localPosition = new Vector3(0, -260, 0);
                }
                else
                {
                    up = true;
                    gameObject.transform.localPosition = new Vector3(0, 280, 0);
                }
            }
        }
        else
            hover = false;

    }
}
