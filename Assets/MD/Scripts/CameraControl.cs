using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YGOSharp.OCGWrapper.Enums;

public class CameraControl : MonoBehaviour
{
    public static CameraControl Instance;
    private void Start()
    {
        Instance = this;
    }
    public static void ShakeCameraNowBig()
    {
        Instance.StatShakeCameraDelay(0, true);
    }
    public static void ShakeCameraNowSmall()
    {
        Instance.StatShakeCameraDelay(0, false);
    }
    public static void ShakeCamera(float t, bool big = true)
    {
        Instance.StatShakeCameraDelay(t, big);
    }
    void StatShakeCameraDelay(float t, bool big)
    {
        StartCoroutine(ShakeCameraDelay(t, big));
    }
    static IEnumerator ShakeCameraDelay(float t, bool big)
    {
        yield return new WaitForSeconds(t);
        if (big)
            iTween.ShakePosition(Program.I().main_camera.gameObject, iTween.Hash(
                            "x", 2,
                            "y", 2,
                            "z", 1,
                            "time", 0.4f
                        ));
        else
            iTween.ShakePosition(Program.I().main_camera.gameObject, iTween.Hash(
                            "x", 0.5,
                            "y", 0.5,
                            "z", 0.25,
                            "time", 0.2f
                        ));
    }
    public static bool NeedLanding(int type, int lv)
    {
        if (GameStringHelper.differ(type, (long)CardType.Fusion))
        {
            if(lv > 5) return true;
            else return false;
        }
        else if (GameStringHelper.differ(type, (long)CardType.Synchro))
        {
            if (lv > 4) return true;
            else return false;
        }
        else if (GameStringHelper.differ(type, (long)CardType.Xyz))
        {
            if (lv > 3) return true;
            else return false;
        }
        else if (GameStringHelper.differ(type, (long)CardType.Link))
        {
            if (lv > 1) return true;
            else return false;
        }
        else if (GameStringHelper.differ(type, (long)CardType.Link))
        {
            if (lv > 5) return true;
            else return false;
        }
        else
        {
            if (lv > 6) return true;
            else return false;
        }
    }
}
