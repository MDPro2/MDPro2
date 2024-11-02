using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public string bgName;
    static GameObject homeUI;
    static GameObject cameraUI;

    private void Awake()
    {
        homeUI = GameObject.Find("UI/HomeUI");
        cameraUI = GameObject.Find("UI/Camera_UI");
        LoadBgFront(Config.Get("Wallpaper0", "随机"));
    }

    static void LoadDIYBg(string name)
    {
        LoadBgFront("神影依·拿非利");

        Transform transform = GameObject.Find("UI/HomeUI/RootWallpaper/Wallpaper").transform;
        foreach(var t in transform.GetComponentsInChildren<Transform>(true))
            if(t.name == "Front0002_1")
                Destroy(t.gameObject);
        transform = GameObject.Find("UI/Camera_UI").transform;

        if (name == "迷宫城的白银姬")
        {
            GameObject frontLoader = ABLoader.LoadABFromFolder("spine/u81497285", "Front");
            frontLoader.transform.parent = transform;
            frontLoader.transform.localScale = Vector3.one;
            frontLoader.transform.localPosition = new Vector3(1.5f, 0, 0);
            var aniamation = frontLoader.GetComponentInChildren<SkeletonAnimation>();
            aniamation.loop = true;
            aniamation.AnimationName = "idel";
        }
        else if (name == "I：P伪装舞会莱娜")
        {
            GameObject frontLoader = ABLoader.LoadABFromFile("wallpaper/65741786");
            Vector3 size = frontLoader.transform.localScale;
            Vector3 position = frontLoader.transform.localPosition;
            frontLoader.transform.parent = transform;
            frontLoader.transform.localScale = size;
            frontLoader.transform.localPosition = position;
        }
        else if(name == "I：P伪装舞会莱娜（异画）")
        {
            GameObject frontLoader = ABLoader.LoadABFromFile("wallpaper/65741787");
            Vector3 size = frontLoader.transform.localScale;
            Vector3 position = frontLoader.transform.localPosition;
            frontLoader.transform.parent = transform;
            frontLoader.transform.localScale = size;
            frontLoader.transform.localPosition = position;
        }
        else if (name == "超魔导龙骑士-真红眼龙骑士")
        {
            GameObject frontLoader = ABLoader.LoadABFromFile("wallpaper/37818794");
            Vector3 size = frontLoader.transform.localScale;
            Vector3 position = frontLoader.transform.localPosition;
            frontLoader.transform.parent = transform;
            frontLoader.transform.localScale = size;
            frontLoader.transform.localPosition = position;
        }
    }

    public static void LoadBgFront(string name)
    {
        if (name == "迷宫城的白银姬"
            ||
            name == "超魔导龙骑士-真红眼龙骑士"
            ||
            name == "I：P伪装舞会莱娜"
            ||
            name == "I：P伪装舞会莱娜（异画）"
            )
        {
            LoadDIYBg(name);
            return;
        }

        name = BgMaping(name);
        Transform transform = GameObject.Find("UI/HomeUI/RootWallpaper/Wallpaper").transform;
        foreach(Transform t in transform.GetComponentsInChildren<Transform>(true))
        {
            if (t.name.StartsWith("Front"))
            {
                Destroy(t.gameObject);
            }
        }
        cameraUI.transform.DestroyChildren();

        GameObject frontLoader = ABLoader.LoadABFromFolder("wallpaper/front" + name, "front");
        RectTransform front = frontLoader.transform.GetChild(0).GetComponent<RectTransform>();
        front.parent = transform;
        Destroy(frontLoader);
        front.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0f, front.rect.width);
        front.localEulerAngles = Vector3.zero;
        front.localScale = Vector3.one;

        GameObject layer_1 = null;
        GameObject layer_2 = null;
        GameObject layer_3 = null;
        GameObject layer_1_1 = null;
        GameObject layer_1_2 = null;
        GameObject layer_1_3 = null;

        foreach(var child in front.GetComponentsInChildren<Transform>())
        {
            if (child.name.EndsWith("_1_1"))
                layer_1_1 = child.gameObject;
            else if (child.name.EndsWith("_1_2"))
                layer_1_2 = child.gameObject;
            else if (child.name.EndsWith("_1_3"))
                layer_1_3 = child.gameObject;
            else if (child.name.EndsWith("_1") && child.name.EndsWith("_1_1") == false)
                layer_1 = child.gameObject;
            else if (child.name.EndsWith("_2") && child.name.EndsWith("_1_2") == false)
                layer_2 = child.gameObject;
            else if (child.name.EndsWith("_3") && child.name.EndsWith("_1_3") == false)
                layer_3 = child.gameObject;
        }
        if(layer_1 != null && layer_1.name == "Front0016_1")
        {
            LoopMoveY move1 = layer_1.AddComponent<LoopMoveY>();
            move1.range = 10f;
            move1.time = 8f;
            LoopMoveY move2 = layer_2.AddComponent<LoopMoveY>();
            move2.range = 5f;
            move2.time = 8f;
        }
        else if(layer_1 == null)
        {
            LoopMoveY move11 = layer_1_1.AddComponent<LoopMoveY>();
            move11.range = 20f;
            move11.time = 8f;
            LoopMoveY move12 = layer_1_2.AddComponent<LoopMoveY>();
            move12.range = 25f;
            move12.time = 8f;
            LoopMoveY move13 = layer_1_3.AddComponent<LoopMoveY>();
            move13.range = 15f;
            move13.time = 8f;
            LoopMoveY move2 = layer_2.AddComponent<LoopMoveY>();
            move2.range = 10f;
            move2.time = 8f;
        }
        else if(layer_3 == null)
        {
            LoopMoveY move1 = layer_1.AddComponent<LoopMoveY>();
            move1.range = 20f;
            move1.time = 8f;
            LoopMoveY move2 = layer_2.AddComponent<LoopMoveY>();
            move2.range = 10f;
            move2.time = 8f;
        }
        else
        {
            LoopMoveY move1 = layer_1.AddComponent<LoopMoveY>();
            move1.range = 20f;
            move1.time = 8f;
            LoopMoveY move2 = layer_2.AddComponent<LoopMoveY>();
            move2.range = 25f;
            move2.time = 8f;
            LoopMoveY move3 = layer_3.AddComponent<LoopMoveY>();
            move3.range = 10f;
            move3.time = 8f;
        }

        foreach (ParticleSystem p in front.GetComponentsInChildren<ParticleSystem>(true))
            p.Play();
    }

    static string BgMaping(string name)
    {
        switch (name)
        {
            case "随机":
                {
                    string re = "";
                    int i = Random.Range(1, 25);
                    if (i == 22)
                        i = 41;
                    if (i > 9)
                        re = "00" + i;
                    else
                        re = "000" + i;
                    return re;
                }
            case "青眼亚白龙":
                return "0001";
            case "神影依·拿非利":
                return "0002";
            case "铁兽战线 凶鸟施莱格":
                return "0003";
            case "宵星之自鸣天琴 丁吉尔苏":
                return "0004";
            case "闪刀姬-篝":
                return "0005";
            case "黄金卿埃尔德里奇":
                return "0006";
            case "双穹之圣骑联盟·机界骑士":
                return "0007";
            case "辉龙机巧降星":
                return "0008";
            case "守护神官玛哈德":
                return "0009";
            case "元素-英雄 真诚新宇侠":
                return "0010";
            case "流天类星龙":
                return "0011";
            case "翼日编号0 未来龙皇霍普":
                return "0012";
            case "异色眼弧灵摆龙":
                return "0013";
            case "访问码语者":
                return "0014";
            case "鬼计节":
                return "0015";
            case "星遗物所引导的前路":
                return "0016";
            case "黑魔导":
                return "0017";
            case "丘与萌芽的春化精":
                return "0018";
            case "北斗天熊放射":
                return "0019";
            case "魔界剧团之谢幕":
                return "0020";
            case "救援王牌 爆流涡轮机":
                return "0021";
            case "传说的暗之魔导师":
                return "0023";
            case "传说的白之龙":
                return "0024";
            case "编号41 泥睡魔兽 貘熟梦":
                return "0041";
            default:
                return "0001";
        }
    }

    public static void ResetUI()
    {
        cameraUI.transform.DestroyChildren();
    }
    public static void CloseHomeUI()
    {
        if (homeUI != null)
        {
            homeUI.SetActive(false);
            cameraUI.SetActive(false);
        }
    }
    public static bool refresh = false;
    public static void OpenHomeUI()
    {
        if (homeUI != null)
        {
            if (refresh)
            {
                homeUI.SetActive(true);
                cameraUI.SetActive(true);
                LoadBgFront(Config.Get("Wallpaper0", "随机"));
                refresh = false;
            }
            else
            {
                homeUI.SetActive(true);
                cameraUI.SetActive(true);
            }
        }
    }
}
