using Spine.Unity;
using System;
using System.Globalization;
using System.IO;
using UnityEngine;
using YGOSharp;
using YGOSharp.OCGWrapper.Enums;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;

public  class CutinLoader : MonoBehaviour
{
    public static int id;
    public static int level;
    public static int attribute;
    public static int type;
    public static string cardName;
    public static int atk;
    public static int def;
    public static uint controller;
    public GameObject nameNear;
    public GameObject nameFar;
    Transform spine;
    static string path = "spine/";
    static string path2 = "effects/summonmonster_04backeff/";

    public bool test;
    public int testSpineID;

    public void Start_Delay_LoadCutin(float delayTime)
    {
        StartCoroutine(Delay_LoadCutin(delayTime));
    }

    IEnumerator Delay_LoadCutin(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        LoadCutin();
    }

    public void LoadCutin()
    {
        StopAllCoroutines();
        //Spine
        if (test) id = testSpineID;

        GameObject go;
        if (HasCutin(id) == 1)//官方Spine
        {
            go = ABLoader.LoadABFromFolder(path + id.ToString(), "Spine");
            ABLoader.ChangeLayer(go, "fx_2d");
            go.transform.SetParent(GameObject.Find("fx_2d").transform);
            if (id == 27204311)//陨石
            {
                go.transform.GetChild(0).localPosition = Vector3.zero;
            }
            else
            {
                if (id == 88307361 || id == 74889525)
                {
                    go.transform.GetChild(0).GetChild(1).SetParent(go.transform.GetChild(0).GetChild(0));
                    spine = go.transform.GetChild(0).GetChild(0).GetChild(0);
                }
                else spine = go.transform.GetChild(0).GetChild(0).GetChild(0);
                go.transform.GetChild(0).localPosition = Vector3.zero;
                spine.GetComponent<SkeletonAnimation>().state.SetAnimation(1, "animation", false);
                spine.GetComponent<MeshRenderer>().sortingOrder = 1;

                go.transform.localPosition = new Vector3(0, 0, -0.1f);
            }
            Program.I().destroy(go, 1.7f);
        }
        else if (HasCutin(id) == 2) //自制Spine
        {
            go = ABLoader.LoadABFromFolder(path + "u" + id.ToString(), "spine");
            //ABLoader.ChangeLayer(go, "fx_2d");
            go.transform.SetParent(GameObject.Find("fx_2d").transform);
            go.transform.localPosition = Vector3.zero;
            Program.I().destroy(go, 1.7f);
        }
        else
            return;

        //Sound + BackEffects
        string sound = "SE_DUEL/SE_MONSTER_CUTIN_EARTH";
        string pathBack = path2 + "summonmonster_bgeah_s2";

        if (GameStringHelper.differ(attribute, (long)CardAttribute.Water))
        {
            pathBack = path2 + "summonmonster_bgwtr_s2";
            sound = "SE_DUEL/SE_MONSTER_CUTIN_WATER";
        }
        if (GameStringHelper.differ(attribute, (long)CardAttribute.Fire))
        {
            pathBack = path2 + "summonmonster_bgfie_s2";
            sound = "SE_DUEL/SE_MONSTER_CUTIN_FIRE";
        }
        if (GameStringHelper.differ(attribute, (long)CardAttribute.Wind))
        {
            pathBack = path2 + "summonmonster_bgwid_s2";
            sound = "SE_DUEL/SE_MONSTER_CUTIN_WIND";
        }
        if (GameStringHelper.differ(attribute, (long)CardAttribute.Light))
        {
            pathBack = path2 + "summonmonster_bglit_s2";
            sound = "SE_DUEL/SE_MONSTER_CUTIN_LIGHT";
        }
        if (GameStringHelper.differ(attribute, (long)CardAttribute.Dark))
        {
            pathBack = path2 + "summonmonster_bgdak_s2";
            sound = "SE_DUEL/SE_MONSTER_CUTIN_DARK";
        }
        if (GameStringHelper.differ(attribute, (long)CardAttribute.Divine))
        {
            pathBack = path2 + "summonmonster_bgdve_s2";
            sound = "SE_DUEL/SE_MONSTER_CUTIN_DIVINE";
        }

        UIHelper.playSound(sound, 1f);

        GameObject back = ABLoader.LoadABFromFile(pathBack);
        ABLoader.ChangeLayer(back, "fx_3d", true);

        Transform eff_flame = back.transform.Find("Eff_Flame");
        Destroy(eff_flame.gameObject);

        Transform eff_bg00 = back.transform.Find("Eff_Bg00");
        eff_bg00.localScale = new Vector3(250f, 25f, 1f);
        Sequence quence = DOTween.Sequence();
        quence.Append(eff_bg00.GetComponent<SpriteRenderer>().DOFade(0.6784f, 0.3f));
        quence.AppendInterval(1);
        quence.Append(eff_bg00.GetComponent<SpriteRenderer>().DOFade(0f, 0.3f));

        Transform flame_re = back.transform.Find("flame_re");
        if (flame_re == null)
            flame_re = back.transform.Find("Eff_group/flame_re");
        if (flame_re == null)
            flame_re = back.transform.Find("Eff_Flame01_re");
        flame_re.gameObject.AddComponent<AutoScaleWithX>();
        Destroy(back, 1.7f);

        //name
        if (controller == 0) nameBarIns(nameNear);
        else nameBarIns(nameFar);
    }
    void nameBarIns(GameObject go)
    {
        var nameBar = Instantiate(go);
        ABLoader.ChangeLayer(nameBar, "fx_2d");
        nameBar.transform.SetParent(GameObject.Find("fx_2d").transform);
        nameBar.transform.localPosition = new Vector3(8.59f, 0f, 0f);
        TextBehaviour tb = nameBar.GetComponent<TextBehaviour>();
        if ((type & (int)CardType.Link) > 0) tb.type = "link";
        if ((type & (int)CardType.Xyz) > 0) tb.type = "rank";
        tb.cardName = cardName;
        tb.level = level;
        tb.atk = atk;
        tb.def = def;
        Destroy(nameBar, 1.63f);
        id = 0;
    }

    public static int HasCutin(int num)
    {
        if (Directory.Exists(Boot.root + "assetbundle/spine/" + num.ToString())) return 1;
        if(Directory.Exists(Boot.root + "assetbundle/spine/u" + num.ToString())) return 2;
        return 0;
    }
}
