using System;
using System.IO;
using UnityEngine;
using static UIBasicSprite;

public class SEHandler : MonoBehaviour
{
    public static void Play(string clip)
    {
        AudioSource audioSource = Instantiate(Program.I().mod_audio_effect).GetComponent<AudioSource>();
        audioSource.volume = Program.I().setting.seVol;
        audioSource.clip = GetClip(clip);
        Destroy(audioSource, 5f);
    }

    public static void PlayInternalAudio(string clip, float vol = 1)
    {
        AudioSource audioSource = Instantiate(Program.I().mod_audio_effect).GetComponent<AudioSource>();
        audioSource.volume = Program.I().setting.seVol * vol;
        audioSource.clip = Resources.Load<AudioClip>("Sound/" + clip);
        Destroy(audioSource.gameObject, 5f);
    }

    public static void Stop()
    {
        GameObject.Find("SE").GetComponent<AudioSource>().Stop();
    }

    public static void PlaySingle(string clip)
    {
        AudioSource audioSource = GameObject.Find("SE").GetComponent<AudioSource>();
        //if (audioSource.isPlaying)
        //    return;
        audioSource.volume = Program.I().setting.seVol;
        audioSource.clip = GetClip(clip);
        audioSource.Play();
    }

    static AudioClip GetClip(string p)
    {
        var path = "sound/" + p + ".mp3";
        if (File.Exists(path) == false) path = "sound/" + p + ".wav";
        if (File.Exists(path) == false) path = "sound/" + p + ".ogg";
        if (File.Exists(path) == false) return null;
        path = Environment.CurrentDirectory.Replace("\\", "/") + "/" + path;
        path = "file:///" + path;
        //Debug.Log(path);
        WWW www = new WWW(path);
        return www.GetAudioClip(true, true);
    }
    public static float PlayCardSE(int code)
    {
        AudioSource audioSource = Instantiate(Program.I().mod_audio_effect).GetComponent<AudioSource>();
        audioSource.volume = Program.I().setting.seVol;

        string path = "SE_DUEL/";
        float returnValue = 0;
        switch (code)
        {
            case 83764718:
            case 83764719:
                path += "SE_EV_MONSTER_REBORN";
                break;
            case 53129443:
                path += "SE_EV_BLACKHOLE";
                break;
            case 12580477:
                path += "SE_EV_RAIGEKI";
                break;
            case 72302403:
                path += "SE_EV_GOFUKEN";
                break;
            case 18144506:
            case 18144507:
                path += "SE_EV_HARPIESFEATHER_DUSTER_3D";
                break;
            case 41420027:
                path += "SE_EV_SOLEMNJUDGMENT";
                break;
            case 44095762:
                path += "SE_EV_MIRRORFORCE";
                break;
            case 5318639:
                path += "SE_EV_CYCLONE";
                break;
            case 61740673:
                path += "SE_EV_IMPERIAL_ORDER";
                break;
            case 62279055:
                path += "SE_EV_MAGIC_CYLINDER";
                break;
            case 63391643:
                path += "SE_EV_THOUSANDKNIVES";
                break;
            case 75500286:
                path += "SE_EV_GOLD_SARCOPHAGUS";
                break;
            case 58297729:
                path += "SE_EV_TRANSMISSION_GEAR";
                break;
            case 24224830:
                path += "SE_EV_CALLED_GRAVE";
                break;
            case 23002292:
                path += "SE_EV_REDREBOOT";
                break;
            case 65681983:
                path += "SE_EV_CROSSOUT_DESIGNATOR";
                break;
            case 54693926:
                path += "SE_EV_DARKRULER_NOMORE";
                break;
            case 25311006:
                path += "SE_EV_TRIPLETACTICS_TALENT";
                break;
            case 24299458:
                path += "SE_EV_FORBIDDEN_DROPLET";
                break;
            case 2263869:
                path += "SE_EV_ULTIMATE_SLAYER";
                break;
            default: path += "";
                break;
        }
        audioSource.clip = GetClip(path);
        if (audioSource.clip != null)
            returnValue = audioSource.clip.length;
        Destroy(audioSource, returnValue);
        return returnValue;
    }
}
