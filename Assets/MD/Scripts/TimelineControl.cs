using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineControl : MonoBehaviour
{
    PlayableDirector director;
    private void Start()
    {
        director = GetComponent<PlayableDirector>();
    }
    private void OnGUI()
    {
        if (Input.GetMouseButtonUp(0) && director.time < 1.82f)
        {
            director.time = 1.82f;
        }
    }
}
