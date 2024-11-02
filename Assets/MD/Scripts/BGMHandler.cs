using DefaultNamespace;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BGMHandler : MonoBehaviour
{
    public static float vol;
    AudioSource audioSource;
    AudioSource audioSource2;
    static bool climax;
    static bool keycard;
    public static string fieldID = "001";

    public static int card;
    public static bool cardBGM;

    static string bgmName;
    static float startTime;
    static float endTime;

    public struct BGMLoop
    {
        public string name;
        public float startTime;
        public float endTime;
    }

    readonly static List<BGMLoop> loops = new List<BGMLoop>
    {
        new BGMLoop{name = "BGM_MENU_01", startTime = 8.374f, endTime = 120 + 27.040f },
        new BGMLoop{name = "BGM_MENU_02", startTime = 15.684f, endTime = 120 + 2.355f },
        new BGMLoop{name = "BGM_DUEL_NORMAL_01", startTime = 9.608f, endTime = 60 + 55.209f },
        new BGMLoop{name = "BGM_DUEL_NORMAL_02", startTime = 16.226f, endTime = 60 + 48.218f },
        new BGMLoop{name = "BGM_DUEL_NORMAL_03", startTime = 5.717f, endTime = 120 + 11.445f },
        new BGMLoop{name = "BGM_DUEL_NORMAL_04", startTime = 13.514f, endTime = 60 + 57.283f },
        new BGMLoop{name = "BGM_DUEL_NORMAL_05", startTime = 11.207f, endTime = 120 + 22.902f },
        new BGMLoop{name = "BGM_DUEL_NORMAL_06", startTime = 14.280f, endTime = 60 + 46.642f },
        new BGMLoop{name = "BGM_DUEL_NORMAL_07", startTime = 5.182f, endTime = 60 +56.722f },
        new BGMLoop{name = "BGM_DUEL_NORMAL_08", startTime = 18.400f, endTime = 120 + 12.400f },
        new BGMLoop{name = "BGM_DUEL_NORMAL_09", startTime = 6.247f, endTime = 60 +51.454f },
        new BGMLoop{name = "BGM_DUEL_NORMAL_10", startTime = 10.006f, endTime = 60 + 51.636f },
        new BGMLoop{name = "BGM_DUEL_NORMAL_11", startTime = 2.378f, endTime = 60 +29.653f },
        new BGMLoop{name = "BGM_DUEL_NORMAL_12", startTime = 7.805f, endTime = 60 +48.122f },
        new BGMLoop{name = "BGM_DUEL_NORMAL_13", startTime = 7.431f, endTime = 60 + 54.743f },
        new BGMLoop{name = "BGM_DUEL_KEYCARD_01", startTime = 12.218f, endTime = 60 + 49.848f },
        new BGMLoop{name = "BGM_DUEL_KEYCARD_02", startTime = 10.604f, endTime = 60 + 46.605f },
        new BGMLoop{name = "BGM_DUEL_KEYCARD_03", startTime = 13.697f, endTime = 60 + 38.150f },
        new BGMLoop{name = "BGM_DUEL_KEYCARD_04", startTime = 7.124f, endTime = 60 + 49.977f },
        new BGMLoop{name = "BGM_DUEL_KEYCARD_05", startTime = 14.444f, endTime = 60 +12.048f },
        new BGMLoop{name = "BGM_DUEL_KEYCARD_06", startTime = 11.348f, endTime = 60 + 38.350f },
        new BGMLoop{name = "BGM_DUEL_KEYCARD_07", startTime = 6.501f, endTime = 60 + 24.925f },
        new BGMLoop{name = "BGM_DUEL_KEYCARD_08", startTime = 13.788f, endTime = 60 + 57.740f },
        new BGMLoop{name = "BGM_DUEL_KEYCARD_09", startTime = 3.849f, endTime = 60 + 20.352f },
        new BGMLoop{name = "BGM_DUEL_KEYCARD_10", startTime = 17.584f, endTime = 60 + 40.506f },
        new BGMLoop{name = "BGM_DUEL_KEYCARD_11", startTime = 11.725f, endTime = 60 + 57.112f },
        new BGMLoop{name = "BGM_DUEL_KEYCARD_12", startTime = 13.554f, endTime = 60 + 45.649f },
        new BGMLoop{name = "BGM_DUEL_KEYCARD_13", startTime = 18.490f, endTime = 60 + 55.729f },
        new BGMLoop{name = "BGM_DUEL_CLIMAX_01", startTime = 6.298f, endTime = 60 + 37.781f },
        new BGMLoop{name = "BGM_DUEL_CLIMAX_02", startTime = 12.852f, endTime = 60 + 53.977f },
        new BGMLoop{name = "BGM_DUEL_CLIMAX_03", startTime = 12.579f, endTime = 120 + 7.434f },
        new BGMLoop{name = "BGM_DUEL_CLIMAX_04", startTime = 3.325f, endTime = 60 + 31.047f },
        new BGMLoop{name = "BGM_DUEL_CLIMAX_05", startTime = 5.412f, endTime = 60 + 37.170f },
        new BGMLoop{name = "BGM_DUEL_CLIMAX_06", startTime = 5.877f, endTime = 60 + 26.159f },
        new BGMLoop{name = "BGM_DUEL_CLIMAX_07", startTime = 11.517f, endTime = 60 + 31.527f },
        new BGMLoop{name = "BGM_DUEL_CLIMAX_08", startTime = 15.801f, endTime = 60 + 48.759f },
        new BGMLoop{name = "BGM_DUEL_CLIMAX_09", startTime = 6.254f, endTime = 60 + 28.773f },
        new BGMLoop{name = "BGM_DUEL_CLIMAX_10", startTime = 2.7f, endTime = 60 + 34.724f },
        new BGMLoop{name = "BGM_DUEL_CLIMAX_11", startTime = 13.203f, endTime = 60 + 43.952f },
        new BGMLoop{name = "BGM_DUEL_CLIMAX_12", startTime = 6.444f, endTime = 60 + 34.242f },
        new BGMLoop{name = "BGM_DUEL_CLIMAX_13", startTime = 5.637f, endTime = 60 + 50.429f }
    };


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource2 = GameObject.Find("BGM2").GetComponent<AudioSource>();
        vol = int.Parse(Config.Get("bgmVol_", "500")) / 1000f;
        PlayBGM("menu");
    }


    static bool changing;
    readonly float fadeTime = 0.5f;
    float timeStamp;
    private void Update()
    {
        //if(audioSource.time - timeStamp > 1)
        //{
        //    timeStamp = Mathf.Floor(audioSource.time);
        //    Debug.Log(audioSource.time);
        //}

        if (cardBGM == false && audioSource.clip != null && audioSource.time > endTime && changing == false)
        {
            changing = true;

            float[] copiedData = new float[audioSource.clip.samples * audioSource.clip.channels];
            audioSource.clip.GetData(copiedData, 0);
            AudioClip copiedClip = AudioClip.Create(bgmName, copiedData.Length, audioSource.clip.channels, audioSource.clip.frequency, false);
            copiedClip.SetData(copiedData, 0);
            audioSource2.clip = copiedClip;
            audioSource2.time = startTime;
            audioSource2.volume = 0;
            audioSource2.Play();

            DOTween.To(() => audioSource2.volume, x => audioSource2.volume = x, vol, fadeTime).OnComplete(() =>
            {
                DOTween.To(() => audioSource2.volume, x => audioSource2.volume = x, 0, fadeTime).OnComplete(() =>
                {
                    audioSource2.Stop();
                });
            });

            DOTween.To(() => audioSource.volume, x => audioSource.volume = x, 0, fadeTime).OnComplete(() =>
            {
                audioSource.time = audioSource2.time;
                DOTween.To(() => audioSource.volume, x => audioSource.volume = x, vol, fadeTime).OnComplete(() =>
                {
                    changing = false;
                });
            });
        }
    }

    public static void PlayBGM(string type)
    {
        AudioSource audioSource = GameObject.Find("BGM").GetComponent<AudioSource>();
        AudioClip clip = GetClip(GetBgmByType(type));
        RefreshLoop();
        audioSource.clip = clip;
        audioSource.time = 0f;
        audioSource.Play();
        changing = false;
    }

    static void RefreshLoop()
    {
        foreach (var loop in loops)
        {
            if(loop.name == bgmName)
            {
                startTime = loop.startTime;
                endTime = loop.endTime;
            }
        }
    }

    public static void PlayCardBGM(AudioClip clip)
    {
        AudioSource audioSource = GameObject.Find("BGM").GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();
    }
    public void LoopBGM()
    {
        var clip = audioSource.clip;
        audioSource.PlayScheduled(AudioSettings.dspTime);
        audioSource.SetScheduledEndTime(AudioSettings.dspTime + audioSource.clip.length - 3f);
    }

    public static void ChangeCardBGM(int id)
    {
        BGMHandler handler = GameObject.Find("BGM").GetComponent<BGMHandler>();
        AudioClip clip = GetClip("card/" + id);
        if(clip != null && card != id)
        {
            card = id;
            cardBGM = true;
            handler.StartCardBgmChange(clip);
            startTime = 0f;
            endTime = clip.length;
        }
    }
    public static void ChangeBGM(string type)
    {
        BGMHandler handler = GameObject.Find("BGM").GetComponent<BGMHandler>();
        if ((type == "duel_normal" || type == "duel_keycard" || type == "duel_climax") && cardBGM)
            return;
        if (type == "menu" || type == "deck")
        {
            cardBGM = false;
            card = 0;
        }
            
        if (type == "duel_climax" && !climax)
        {
            climax = true;
            handler.StartBgmChange(type);
        }
        else if (type == "duel_climax" && climax) 
        {
        }
        else if (type == "duel_keycard" && !keycard && !climax)
        {
            keycard = true;
            handler.StartBgmChange(type);
        }
        else if (type == "duel_keycard" && (keycard || climax))
        {
        }
        else
        {
            keycard = false;
            climax = false;
            handler.StartBgmChange(type);
        }   
    }

    void StartBgmChange(string type)
    {
        StartCoroutine(ChangeBGMFade(type, vol, vol));
    }
    void StartCardBgmChange(AudioClip clip)
    {
        StartCoroutine(ChangeCardBGMFade(clip, vol, vol));
    }
    public static void ChangeBgmVol(float _vol)
    {
        AudioSource audioSource = GameObject.Find("BGM").GetComponent<AudioSource>();
        audioSource.volume = _vol;
        vol = _vol;
    }

    static AudioClip GetClip(string p)
    {
        if (p.Contains("card/") == false)
        {
            return Resources.Load<AudioClip>("Sound/bgm/" + p);
        }
        else
        {
            var path = "sound/BGM/" + p + ".mp3";
            if (File.Exists(path) == false) path = "sound/BGM/" + p + ".wav";
            if (File.Exists(path) == false) path = "sound/BGM/" + p + ".ogg";
            if (File.Exists(path) == false) return null;
            path = Environment.CurrentDirectory.Replace("\\", "/") + "/" + path;
            path = "file:///" + path;
            WWW www = new WWW(path);
            return www.GetAudioClip(true, true);
        }
    }

    IEnumerator ChangeBGMFade(string type, float vol, float currentVol, bool changed = false)
    {
        if(currentVol > 0 && !changed)
        {
            currentVol = currentVol - 0.01f;
            audioSource.volume = currentVol;
            yield return new WaitForSeconds(0.01f);
            yield return ChangeBGMFade(type, vol, currentVol);
            yield break;
        }
        if(currentVol <= 0 || changed)
        {
            if (!changed)
            {
                PlayBGM(type);
                changed = true;
            }
            currentVol = currentVol + 0.01f;
            audioSource.volume = currentVol;
            if (currentVol < vol)
            {
                yield return new WaitForSeconds(0.01f);
                yield return ChangeBGMFade(type, vol, currentVol, changed);
            }
            else
                audioSource.volume = vol;
        }
    }
    IEnumerator ChangeCardBGMFade(AudioClip clip, float vol, float currentVol, bool changed = false)
    {
        if (currentVol > 0 && !changed)
        {
            currentVol = currentVol - 0.01f;
            audioSource.volume = currentVol;
            yield return new WaitForSeconds(0.001f);
            yield return ChangeCardBGMFade(clip, vol, currentVol);
            yield break;
        }
        if (currentVol <= 0 || changed)
        {
            if (!changed)
            {
                PlayCardBGM(clip);
                changed = true;
            }
            currentVol = currentVol + 0.01f;
            audioSource.volume = currentVol;
            if (currentVol < vol)
            {
                yield return new WaitForSeconds(0.001f);
                yield return ChangeCardBGMFade(clip, vol, currentVol, changed);
            }
            else
                audioSource.volume = vol;
        }
    }
    static string GetBgmByType(string type)
    {
        switch (type)
        {
            case "menu":
                return bgmName = "BGM_MENU_01";
            case "deck":
                return bgmName = "BGM_MENU_02";
            case "duel_normal":
                return bgmName = "BGM_DUEL_NORMAL_" + GetBgmID();
            case "duel_keycard":
                return bgmName = "BGM_DUEL_KEYCARD_" + GetBgmID();
            case "duel_climax":
                return bgmName = "BGM_DUEL_CLIMAX_" + GetBgmID();
            case "climax":
                int i = UnityEngine.Random.Range(1, int.Parse(bgms[bgms.Count-1]));
                return i > 9 ? bgmName = "BGM_DUEL_CLIMAX_" + i : bgmName = "BGM_DUEL_CLIMAX_0" + i;
            default:
                return bgmName = "BGM_MENU_01";
        }
    }
    static string GetBgmID()
    {
        switch (fieldID)
        {
            case "007":
                return "05";
            case "016":
                return "09";
            case "017":
                return "09";

            default:
                return RandomBGM();
        }
    }

    static List<string> bgms = new List<string>()
    {
        "01",
        "02",
        "03",
        "04",
        //"05",
        "06",
        "07",
        "08",
        //"09",
        "10",
        "11",
        "12",
        "13",
    };
    static string RandomBGM()
    {
        return bgms[UnityEngine.Random.Range(0, bgms.Count)];
    }
}
