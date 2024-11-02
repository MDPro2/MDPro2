using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScaleWithX : MonoBehaviour
{
    float width;
    float height;
    bool localOneByOne;
    private void Start()
    {
        width = transform.localScale.x;
        height = transform.localScale.y;
        if(width / height == 1) localOneByOne = true;
    }
    void Update()
    {
        if (localOneByOne)
            gameObject.transform.localScale = new Vector3(width * ((Screen.width *9) / (float)(Screen.height * 16) ), height, 1);
    }
}
