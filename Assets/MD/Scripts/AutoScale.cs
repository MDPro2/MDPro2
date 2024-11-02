using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScale : MonoBehaviour
{
    float width;
    float height;

    private void Start()
    {
        width = transform.localScale.x;
        height = transform.localScale.y;
        Program.onScreenChanged += Scale;
        Scale();
    }
    void Scale()
    {
        float screenAspect = (float)Screen.width / Screen.height;
        if (screenAspect > 16 / 9f)
            transform.localScale = new Vector3(width * screenAspect * 9 / 16, height * screenAspect * 9 / 16, transform.localScale.z);
        else
            transform.localScale = new Vector3(width, height, transform.localScale.z);
    }
}
