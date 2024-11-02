using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerHandler : MonoBehaviour
{
    static Material material;
    static TextMeshPro tmp;
    public static int[] time = new int[2];
    void Start()
    {
        material = transform.Find("Timer").GetComponent<Renderer>().materials[1];
        tmp = transform.Find("Timer/TextMain").gameObject.AddComponent<TextMeshPro>();
        tmp.text = "999";
        tmp.fontSize = 22;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1, 1, 1, 0.75f);
        tmp.outlineColor = new Color(1, 1, 1, 0);
        tmp.GetComponent<TextContainer>().pivot = new Vector2(0.5f, 0.4f);
        material.SetFloat("_AddTime", 0);
        material.SetColor("_AddTimeColorBar01", Color.red);
        material.SetColor("_AddTimeColor02", Color.red);
    }

    public static void SetTime(int player , int time)
    {
        if (tmp == null)
            return;
        if(player == 0)
        {
            tmp.text = time.ToString();
            material.SetFloat("_MaxTime", time / (float)Program.I().ocgcore.timeLimit);
            material.SetFloat("_AddTime", 0f);
        }
        else
        {
            tmp.text = time.ToString();
            material.SetFloat("_MaxTime", 0f);
            material.SetFloat("_AddTime", time / (float)Program.I().ocgcore.timeLimit);
        }
    }

}
